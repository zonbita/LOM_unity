using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace com.wao.core.Utility
{
    [DisallowMultipleComponent]
    public class AssetManager : MonobehaviourSingleton<AssetManager>
    {
        [SerializeField]
        private float _checkTime = 10;
        [SerializeField]
        private float _timeOut = 6000;
        private Dictionary<Type, Func<byte[], System.Object>> _loadAssetMethods = new Dictionary<Type, Func<byte[], System.Object>>();
        private Dictionary<string, UnityEngine.Object> _cache = new Dictionary<string, UnityEngine.Object>();
        private Dictionary<string, Action> _onLoadSamethingCache = new Dictionary<string, Action>();
        private void Awake()
        {
            Debug.Log(UnityEngine.Application.persistentDataPath);
            _loadAssetMethods.Add(typeof(Texture2D), com.wao.Utility.Utility.LoadTexture2D);
            _loadAssetMethods.Add(typeof(AssetBundle), AssetBundle.LoadFromMemory);
            _loadAssetMethods.Add(typeof(string), BitConverter.ToString);
        }

        public CallBack<T> Load<T>(string path, AssetFrom assetFrom = AssetFrom.Remote, bool cache = true, bool isData = false) where T : class
        {
            var callBack = new CallBack<T>();
            switch (assetFrom)
            {
                case AssetFrom.Remote:
                    string localPath = UnityEngine.Application.persistentDataPath + Path.AltDirectorySeparatorChar + typeof(T).Name + Path.AltDirectorySeparatorChar + Path.GetFileName(path);
                    if (!NeedToDownload(path, localPath))
                    {
                        path = localPath;
                        cache = false;
                    }
                    UnityWebRequest unityWebRequest = UnityWebRequest.Get(path);
                    unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
                    callBack.SetExcuteAction(Download(unityWebRequest, callBack, cache));
                    break;
                case AssetFrom.Resources:
                    callBack.SetExcuteAction(LoadAsyncFromResource<T>(path, isData, cache, callBack));
                    break;
            }
            return callBack;
        }

        private bool NeedToDownload(string path, string localPath)
        {
            if (File.Exists(localPath) && PlayerPrefs.GetString(path) == com.wao.Utility.Utility.CalculateMD5(localPath))
            {
                return false;
            }
            return true;
        }

        private IEnumerator LoadAsyncFromResource<T>(string path, bool isData, bool cache, CallBack<T> callBack) where T : class
        {
            var type = typeof(T);
            UnityEngine.Object res = _cache.ContainsKey(path) ? _cache[path] : null;
            Action onLoadComplete = () =>
             {
                 if (!_loadAssetMethods.ContainsKey(type))
                 {
                     if (!isData && res != null)
                     {

                         if (type != typeof(GameObject) && type != typeof(AudioClip))
                         {
                             callBack?.onComplete?.Invoke((res as GameObject).GetComponent<T>());
                         }
                         else
                         {
                             callBack?.onComplete?.Invoke(res as T);
                         }
                     }
                     else
                     {
                         var textAsset = res as TextAsset;
                         if (textAsset != null)
                         {
                             callBack?.onComplete?.Invoke(JsonConvert.DeserializeObject<T>(textAsset.text));
                         }
                     }
                 }
                 else
                 {
                     callBack?.onComplete?.Invoke(res as T);
                 }
             };

            if (res != null)
            {
                onLoadComplete.Invoke();
                yield break;
            }
            ResourceRequest loadasyn = null;

            if (!_loadAssetMethods.ContainsKey(type))
            {
                if (!isData)
                {
                    loadasyn = Resources.LoadAsync(path);
                }
                else
                {
                    loadasyn = Resources.LoadAsync<TextAsset>(path);
                }
            }
            else
            {
                loadasyn = Resources.LoadAsync(path);
            }

            yield return new WaitUntil(() => loadasyn.isDone);
            res = loadasyn.asset;
            if (loadasyn.asset != null && cache && !_cache.ContainsKey(path))
            {
                _cache.Add(path, loadasyn.asset);
            }
            onLoadComplete.Invoke();
        }


        private IEnumerator Download<T>(UnityWebRequest request, CallBack<T> callBack, bool cache = true) where T : class
        {
            if (callBack == null)
            {
                yield break;
            }

            var wfs = new WaitForSeconds(_checkTime);

            string localPath = UnityEngine.Application.persistentDataPath + Path.AltDirectorySeparatorChar + typeof(T).Name + Path.AltDirectorySeparatorChar + Path.GetFileName(request.url);

            DateTime beginRequest = DateTime.Now;

            request.SendWebRequest();

            while (!request.isDone)
            {
                callBack?.progress?.Invoke(request.downloadProgress);
                if ((DateTime.Now - beginRequest).Milliseconds > _timeOut)
                {
                    request.Abort();
                    callBack?.Retry();
                    yield break;
                }
                yield return wfs;
            }

            if (request.result != UnityWebRequest.Result.ProtocolError && request.result != UnityWebRequest.Result.DataProcessingError && request.result != UnityWebRequest.Result.ConnectionError)
            {
                var bytes = request.downloadHandler?.data;
                if (cache)
                {
                    _ = SaveFile(localPath, bytes);
                }
                yield return FromBinaryToObject<T>(bytes);
                callBack?.onCompleteWithOutResource?.Invoke();
            }
            else
            {
                if (request.result == UnityWebRequest.Result.ProtocolError)
                {
                    callBack?.onNetworkError?.Invoke(request.responseCode);
                }
                else
                {
                    callBack?.onErrorCallBack?.Invoke(request.error);
                }
            }
            request?.Dispose();
            callBack?.Dispose();
        }

        private T FromBinaryToObject<T>(byte[] data) where T : class
        {
            T res = default(T);
            var type = typeof(T);
            if (_loadAssetMethods.ContainsKey(type))
            {
                res = _loadAssetMethods[type]?.Invoke(data) as T;
            }
            else
            {
                try
                {
                    res = data.DeserializeObject<T>();
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.ToString());
                }
            }
            return res;
        }
        private async System.Threading.Tasks.Task SaveFile(string path, byte[] data)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
                File.WriteAllBytes(path, data);
            });
        }
    }
    public enum AssetFrom
    {
        Resources,
        Remote,
    }
}
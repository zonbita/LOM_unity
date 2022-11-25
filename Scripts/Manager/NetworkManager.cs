namespace com.wao.core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.Networking;
    using Newtonsoft.Json;
    using System.Runtime.InteropServices;
    using Microsoft.Win32.SafeHandles;
    using System.Threading.Tasks;
    using System.Text;
    using Newtonsoft.Json.Linq;

    [DisallowMultipleComponent]
    public class NetworkManager : MonobehaviourSingleton<NetworkManager>
    {
        [SerializeField]
        private EEnviroment _enviroment;
        [SerializeField]
        private int _timeOut;
        [SerializeField]
        private float _timeCheck;
        [SerializeField]
        private EviromentAPIEviromentDictionary _enviroments = new EviromentAPIEviromentDictionary();
        [SerializeField]
        private APIConfigData _data;
        [SerializeField]
        private Dictionary<string, APIDefine> _apis = new Dictionary<string, APIDefine>();
        public APIEnviroment enviromentInUse { get; private set; }

        public EEnviroment enviroment
        {
            get { return _enviroment; }
            set
            {
                _enviroment = value;
                enviromentInUse = _enviroments[_enviroment];
            }
        }

        public delegate void ServerCallBackStatus(long code, ServerMessage<JObject> serverMessage);
        public event ServerCallBackStatus onErrorCB;
#if UNITY_EDITOR // use for build script
        public void EditorSetEnvironment(EEnviroment env)
        {
            _enviroment = env;
        }
        public EEnviroment EditorGetEnvironment()
        {
            return _enviroment;
        }
#endif
        private WaitForSeconds waitForSeconds;
        protected void Awake()
        {
            waitForSeconds = new WaitForSeconds(_timeCheck);
            if (_enviroments.ContainsKey(_enviroment))
            {
                enviromentInUse = _enviroments[_enviroment];
                for (int i = 0; i < _data.aPIDefines?.Count; i++)
                {
                    if (!_apis.ContainsKey(_data.aPIDefines[i].name))
                    {
                        _apis.Add(_data.aPIDefines[i].name, _data.aPIDefines[i]);
                    }
                }
            }
        }
        public CallBack<T> Request<T>(string apiName, string query = null, Dictionary<string, string> form = null, Dictionary<string, string> headers = null, string domain = null, object raw = null)
        {
            CallBack<T> cb = null;
            if (_apis.ContainsKey(apiName))
            {
                cb = Request<T>(_apis[apiName], query, form, headers, domain, raw);
            }
            else
            {
                Debug.LogError("API " + apiName + "is not exist");
            }
            return cb;
        }
        public CallBack<T> Request<T>(APIDefine api, string query = null, Dictionary<string, string> form = null, Dictionary<string, string> headers = null, string domain = null, object raw = null)
        {

            CallBack<T> apiCallBack = new CallBack<T>();
            if (api == null)
                return apiCallBack;
            UnityWebRequest unityWebRequest = null;
            domain = !string.IsNullOrWhiteSpace(domain) ? domain : string.Format("{0}{1}", enviromentInUse.prefix, enviromentInUse.domain);
            string url = domain;

            if (!string.IsNullOrWhiteSpace(api.controller))
            {
                url += "/" + api.controller;
            }

            if (!string.IsNullOrWhiteSpace(api.action))
            {
                url += "/" + api.action;
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                url = string.Format("{0}{1}", url, query);
            }

            string logRequest = api.ToString();

            switch (api.method)
            {
                case APIMethod.GET:
                    unityWebRequest = UnityWebRequest.Get(url);
                    break;
                case APIMethod.POST:

                    WWWForm wform = new WWWForm();
                    if (form != null)
                    {
                        foreach (string key in form.Keys)
                        {
                            wform.AddField(key, form[key]);
                            logRequest += " " + key.ToString() + ":" + form[key].ToString();
                        }
                    }

                    unityWebRequest = UnityWebRequest.Post(url, wform);
                    if (raw != null)
                    {
                        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(raw));
                        unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                        unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
                    }

                    break;
            }

            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    unityWebRequest.SetRequestHeader(key, headers[key]);
                    logRequest += " " + key.ToString() + ":" + headers[key].ToString();
                }
            }

            string token = string.Empty;

            if (api.isAuthenticated)
            {
                switch (api.authenticatedMethod)
                {
                    case AuthenticatedMethod.JWTToken:
                        token = PlayerPrefs.GetString("JWT Token", string.Empty);
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            unityWebRequest.SetRequestHeader("Authorization", "Bearer " + token);
                        }
                        break;
                    case AuthenticatedMethod.EditorToken:
                        token = PlayerPrefs.GetString("editor-token", string.Empty);
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            unityWebRequest.SetRequestHeader("editor-token", token);
                        }
                        break;
                    case AuthenticatedMethod.PlayerToken:
                        token = PlayerPrefs.GetString("player-token", string.Empty);
                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            unityWebRequest.SetRequestHeader("player-token", token);
                        }
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(api.contentType))
            {
                unityWebRequest.SetRequestHeader("Content-type", api.contentType);
            }
            apiCallBack.SetExcuteAction(IRequest<T>(unityWebRequest, apiCallBack));
            return apiCallBack;
        }

        public string GetToken(AuthenticatedMethod authenticatedMethod)
        {
            switch (authenticatedMethod)
            {
                case AuthenticatedMethod.JWTToken:
                    return PlayerPrefs.GetString("JWT Token");
                case AuthenticatedMethod.EditorToken:
                    return PlayerPrefs.GetString("editor-token");
                case AuthenticatedMethod.PlayerToken:
                    return PlayerPrefs.GetString("player-token");
            }
            return string.Empty;
        }

        public void SetToken(AuthenticatedMethod authenticatedMethod, string token)
        {
            switch (authenticatedMethod)
            {
                case AuthenticatedMethod.JWTToken:
                    PlayerPrefs.SetString("JWT Token", token);
                    break;
                case AuthenticatedMethod.EditorToken:
                    PlayerPrefs.SetString("editor-token", token);
                    break;
                case AuthenticatedMethod.PlayerToken:
                    PlayerPrefs.SetString("player-token", token);
                    break;
            }
        }
        private IEnumerator IRequest<T>(UnityWebRequest unityWebRequest, CallBack<T> callBack = null)
        {
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    callBack.onErrorCallBack?.Invoke("INTERNET_NOT_REACHABLE");
            //    unityWebRequest.Abort();
            //    yield break;
            //}

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            Debug.Log("<color=yellow>" + unityWebRequest.url + "</color>");

            DateTime beginRequest = DateTime.Now;
            if (unityWebRequest != null)
            {
                unityWebRequest.SendWebRequest();
            }
            else
            {
                yield break;
            }
            while (!unityWebRequest.isDone)
            {
                if ((DateTime.Now - beginRequest).Milliseconds > _timeOut)
                {
                    unityWebRequest.Abort();
                    callBack?.Retry();
                    yield break;
                }
                yield return waitForSeconds;
            }

            sw.Stop();
            TimeSpan timeSpan = sw.Elapsed;
            Debug.Log(unityWebRequest.url + " complete in " + timeSpan.TotalMilliseconds);
#if UNITY_EDITOR
            Debug.Log(unityWebRequest.downloadHandler.text);
#endif
            if (unityWebRequest != null)
            {
                if (unityWebRequest.result == UnityWebRequest.Result.Success)
                {
                    callBack?.onCompleteWithOutResource?.Invoke();
                    if (callBack.onComplete != null)
                    {
                        try
                        {
                            _ = unityWebRequest.downloadHandler.text.DeserializeObject<ServerMessage<T>>((res) =>
                            {
                                if (res != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(res.error))
                                    {
                                        Debug.LogError(res.error);
                                        callBack?.onErrorCallBack?.Invoke(res.error);
                                    }
                                    else
                                    {
                                        callBack?.onComplete?.Invoke(res.res);
                                    }
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex);
                            Debug.LogError(unityWebRequest.downloadHandler.text);
                            callBack?.onExceptionCallBack?.Invoke(ex);
                        }
                    }
                }
                else
                {
                    Debug.LogError(unityWebRequest.responseCode + ":" + unityWebRequest.url);
                    callBack.onNetworkError?.Invoke(unityWebRequest.responseCode);
                    onErrorCB?.Invoke(unityWebRequest.responseCode, JsonConvert.DeserializeObject<ServerMessage<JObject>>(unityWebRequest.downloadHandler.text));
                }
            }
            else
            {
                if (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
                {
                    Debug.LogError("Internet unreachable");
                }
                else
                {
                }

            }
            unityWebRequest?.Dispose();
            callBack?.Dispose();
        }

    }
    public enum EEnviroment
    {
        Local,
        Staging,
        Production,
    }
    [Serializable]
    public class APIEnviroment
    {
        public string prefix;
        public string websocket;
        public string domain;
    }

    public class CallBack<T> : IDisposable
    {
        public Action onCompleteWithOutResource
        {
            get { return _onCompleteWithoutResource; }
        }
        private Action _onCompleteWithoutResource;
        public Action<T> onComplete
        {
            get { return _onComplete; }
        }
        private Action<T> _onComplete;
        public Action<Exception> onExceptionCallBack
        {
            get { return _onExceptionCallBack; }
        }
        private Action<Exception> _onExceptionCallBack;
        public Action<string> onErrorCallBack
        {
            get { return _onErrorCallBack; }
        }
        private Action<string> _onErrorCallBack;
        public Action<long> onNetworkError
        {
            get { return _onNetworkError; }
        }
        public DateTime createdTime
        {
            get { return _createdTime; }
        }
        private Action<float> _progress;
        public Action<float> progress
        {
            get { return _progress; }
        }
        private DateTime _createdTime = DateTime.Now;
        private Action<long> _onNetworkError;
        private int _retryTime;
        private int _times;
        private object _excuteAction;
        private bool _disposed = false;
        // Instantiate a SafeHandle instance.
        private SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);

        public CallBack<T> SetOnComplete(Action<T> onComplete)
        {
            _onComplete = onComplete;
            return this;
        }
        public CallBack<T> SetOnComplete(Action onComplete)
        {
            _onCompleteWithoutResource = onComplete;
            return this;
        }
        public CallBack<T> SetOnExceptionCallBack(Action<Exception> onExceptionCallBack)
        {
            _onExceptionCallBack = onExceptionCallBack;
            return this;
        }

        public CallBack<T> SetOnErrorCallBack(Action<string> onErrorCallBack)
        {
            _onErrorCallBack = onErrorCallBack;
            return this;
        }
        public CallBack<T> SetOnProgressCB(Action<float> progressCb)
        {
            _progress = progressCb;
            return this;
        }
        public CallBack<T> SetOnNetworkError(Action<long> onNetworkError)
        {
            _onNetworkError = onNetworkError;
            return this;
        }
        private IEnumerator ExcuteCoroutine(IEnumerator enumerator)
        {
            yield return enumerator;
            if (enumerator != null && enumerator.Current?.GetType() == typeof(T))
            {
                onComplete?.Invoke((T)enumerator.Current);
            }

        }
        public void Excute()
        {
            try
            {
                if (_excuteAction != null)
                {
                    if (_excuteAction is IEnumerator)
                    {
                        CorountineManager.Instance.StartCoroutine(ExcuteCoroutine(_excuteAction as IEnumerator));
                    }
                    else
                    {
                        if (_excuteAction is Func<T>)
                        {
                            T value = (_excuteAction as Func<T>).Invoke();
                            if (onComplete != null)
                            {
                                onComplete(value);
                            }
                        }
                        else
                        {
                            if (_excuteAction is Func<Task<T>>)
                            {
                                T value = (_excuteAction as Func<Task<T>>).Invoke().Result;
                                if (onComplete != null)
                                {
                                    onComplete(value);
                                }
                            }
                            else
                            {
                                if (_excuteAction is Action)
                                {
                                    (_excuteAction as Action)?.Invoke();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (onExceptionCallBack != null)
                {
                    onExceptionCallBack(ex);
                }
            }
        }

        public CallBack<T> SetExcuteAction(object api)
        {
            _excuteAction = api;
            return this;
        }
        public void Retry()
        {
            if (_retryTime > 0 && _times < _retryTime)
            {
                _times++;
                Excute();
            }
            else
            {
                if (_retryTime == -1)
                {
                    Excute();
                }
            }
        }
        internal int GetRetryTimes()
        {
            return _retryTime;
        }
        public CallBack<T> SetRetryTimes(int retryTimes)
        {
            _retryTime = retryTimes;
            return this;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _handle.Dispose();
            }
            _disposed = true;
        }


    }
    public class ServerMessage<T>
    {
        public T res;
        public string error;
    }
    [Serializable]
    public class APIDefine
    {
        public string name;
        public string controller;
        public string action;
        public APIMethod method;
        public bool allowMultiRequest;
        public bool isAuthenticated;
        public string contentType;
        public AuthenticatedMethod authenticatedMethod;
    }
    public enum APIMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
    }
    public enum AuthenticatedMethod
    {
        JWTToken,
        EditorToken,
        PlayerToken
    }

}
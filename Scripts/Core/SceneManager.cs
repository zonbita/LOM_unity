namespace com.wao.core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class SceneManager : MonobehaviourSingleton<SceneManager>
    {
        [SerializeField]
        private Transform _content;
        private SceneBase _current;
        public SceneBase Current
        {
            get { return _current; }
        }
#if UNITY_EDITOR
        [ListToPopupAttribute(typeof(SceneManager), "myList")]
#endif
        public string scene;

        public static List<string> myList;

        private void OnEnable()
        {
            myList = typeof(SceneBase).GetAllClass<SceneBase>().Select(x => x.Name).ToList();
            DestroyLastScene();
            GetScene();
        }
        public void OnChangeResolution()
        {
            _content.GetComponent<AspectRatioFitter>().aspectRatio = (float)Screen.currentResolution.width / Screen.currentResolution.height;
        }
        public void LoadScene<T>(Action<T> cb = null) where T : SceneBase
        {
            if (Current?.GetType() == typeof(T))
            {
                var res = Current;
                if (_current == null)
                {
                    _current = res;
                    res.Show();
                }
                else
                {
                    _current.Hide();
                    _current = res;
                }

                cb?.Invoke(_current as T);
            }
            Utility.AssetManager.Instance.Load<T>("Scene/" + typeof(T).Name, Utility.AssetFrom.Resources).SetOnComplete((res) =>
            {
                if (res != null)
                {
                    res = Instantiate<T>(res, _content);
                    if (_current == null)
                    {
                        _current = res;
                        res.Show();
                    }
                    else
                    {
                        _current.Hide();
                        _current = res;
                    }

                    cb?.Invoke(_current as T);
                }

             }).Excute();
        }

        public void GetScene()
        {
            LoadSceneWithType(typeof(SceneBase).GetClassWithName(scene));
        }
        private void LoadSceneWithType(Type type)
        {
            if (Current?.GetType() == type)
            {
                var res = Current;
                if (_current == null)
                {
                    _current = res;
                    res.Show();
                }
                else
                {
                    _current.Hide();
                    _current = res;
                }
        
            }
            Utility.AssetManager.Instance.Load<SceneBase>("Scene/" + type.Name, Utility.AssetFrom.Resources).SetOnComplete((res) =>
            {
                if (res != null)
                {
                    res = Instantiate<SceneBase>(res, _content);
                    if (_current == null)
                    {
                        _current = res;
                        res.Show();
                    }
                    else
                    {
                        _current.Hide();
                        _current = res;
                    }
                }

            }).Excute();
        }
        private void OnApplicationQuit()
        {
#if UNITY_EDITOR
            DestroyLastScene();
#endif
        }
        public void DestroyLastScene()
        {
            Debug.Log("DestroyLastScene");
            if (_content != null)
            {
                foreach (Transform transform in _content)
                {
                    DestroyImmediate(transform.gameObject);
                }
            }
        }

        public void HideFinish()
        {
            if (_current != null)
            {
                _current.Show();
            }
        }
    }
    public class SceneBase : MonoBehaviour
    {
        private bool _isDestroy;
        public virtual void Show()
        {
            if (!_isDestroy)
            {
                if (this != null)
                {
                    gameObject.SetActive(true);
                }
            }
        }
        public virtual void Capture()
        {

        }
        public virtual void Hide()
        {
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            _isDestroy = true;
            if (SceneManager.Instance != null)
                SceneManager.Instance.HideFinish();
        }
    }
}
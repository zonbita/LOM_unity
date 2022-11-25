namespace com.wao.core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    [ExecuteInEditMode]
    public class DialogManager : MonobehaviourSingleton<DialogManager>
    {
        [SerializeField]
        private RectTransform _content;
        private List<DialogBase> _dialogLoaded = new List<DialogBase>();
        private List<DialogBase> _singletonList = new List<DialogBase>();

#if UNITY_EDITOR
        [ListToPopupAttribute(typeof(DialogManager), "myList")]
#endif
        public string dialog;

        public static List<string> myList;

        public DialogBase CurrentDialog
        {
            get { return _currentDialog; }
        }
        private DialogBase _currentDialog;

        private void OnEnable()
        {
            myList = typeof(DialogBase).GetAllClass<DialogBase>().Select(x => x.Name).ToList();
        }
        public DialogBase LoadDialogWithType(Type type, bool isSingleton = false)
        {
            if (!_isDestroy)
            {
                if (!_dialogLoaded.Any(x => x != null && x.GetType() == type))
                {
                    if (isSingleton)
                    {
                        var singleton = _singletonList.FirstOrDefault(x => x.GetType() == type) as DialogBase;
                        if (singleton != null)
                        {
                            return singleton;
                        }
                    }

                    DialogBase res = Instantiate<DialogBase>(Resources.Load<DialogBase>("Dialog/" + type.Name), _content);
                    _currentDialog = res;
                    if (isSingleton)
                    {
                        _singletonList.Add(res);
                    }
                    else
                    {
                        _dialogLoaded.Add(res);
                    }
                    return res;
                }
                else
                {
                    return _dialogLoaded.FirstOrDefault(x => x.GetType() == type) as DialogBase;
                }
            }
            return null;
        }
        public T LoadDialog<T>(bool isSingleton = false) where T : DialogBase
        {
            if (!_isDestroy)
            {
                if (!_dialogLoaded.Any(x => x != null && x.GetType() == typeof(T)))
                {
                    if (isSingleton)
                    {
                        var singleton = _singletonList.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
                        if (singleton != null)
                        {
                            return singleton;
                        }
                    }

                    T res = Instantiate<T>(Resources.Load<T>("Dialog/" + typeof(T).Name), _content);
                    _currentDialog = res;
                    if (isSingleton)
                    {
                        _singletonList.Add(res);
                    }
                    else
                    {
                        _dialogLoaded.Add(res);
                    }
                    return res;
                }
                else
                {
                    return _dialogLoaded.FirstOrDefault(x => x.GetType() == typeof(T)) as T;
                }
            }
            return default(T);
        }

        public void Register(DialogBase dialogBase)
        {
            if (!_isDestroy)
            {
                _dialogLoaded.Add(dialogBase);
            }
        }

        public void GetDialog()
        {
            LoadDialogWithType(typeof(DialogBase).GetClassWithName(dialog));
        }

        public void HideLastDialog()
        {
            if (_content.childCount > 0)
            {
                DestroyImmediate(_content.GetChild(_content.childCount - 1)?.gameObject);
            }
        }
        public void HideDialog<T>(Action hide = null)
        {
            if (!_isDestroy)
            {
                var type = typeof(T);
                _dialogLoaded.LastOrDefault(x => x.GetType() == type)?.Hide(hide);
                _singletonList.FirstOrDefault(x => x.GetType() == type)?.Hide(hide);
            }
        }
        public void OnDialogDestroy(DialogBase dialog)
        {
            if (!_isDestroy)
            {
                _dialogLoaded.RemoveAll(x => x == dialog);
                _singletonList.RemoveAll(x => x == dialog);
            }
            _singletonList.RemoveAll(x => x == null);
            _dialogLoaded.RemoveAll(x => x == null);
            _currentDialog = _dialogLoaded.LastOrDefault();
        }
    }
    public class DialogBase : MonoBehaviour
    {
        protected Action _onHideFinish;
        private void Awake()
        {
            DialogManager.Instance.Register(this);
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide(Action onHideFinish = null)
        {
            _onHideFinish = onHideFinish != null ? onHideFinish : _onHideFinish;
            Destroy(gameObject);
            _onHideFinish?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.OnDialogDestroy(this);
            }
        }

        public void Back()
        {
            AudioManager.Instance.PlaySfx("Digital Click 01");
            Destroy(gameObject);
        }
    }

}
namespace com.wao.rpgs
{
    using com.wao.core;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    public class LogoScene : SceneBase
    {
        [SerializeField]
        private RawImage _logo;

        private void Awake()
        {
            _logo.color = new Color(1, 1, 1, 0);
            _logo.DOFade(1, 0.5f).onComplete = ()=>
            {
                _logo.DOFade(0, 0.5f).onComplete = () =>
                {
                    SceneManager.Instance.LoadScene<StartScene>();
                };
            };
        }
    }
}
using com.wao.core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : Button
{
    [SerializeField]
    private string _clickSound;
    [SerializeField]
    private string _errorSound;
    public bool isError;
    public Action onError;
    public Action onNoError;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        onClick.AddListener(OnClick);
        
    }

    private void OnClick()
    {
        if(isError)
        {
            onError?.Invoke();
            AudioManager.Instance.PlaySfx(_errorSound);
        }
        else
        {
            onNoError?.Invoke();
            AudioManager.Instance.PlaySfx(_clickSound);
        }
    }

    #region additional color change for text
    [Space]
    [SerializeField]
    private Text _targetText;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (_targetText != null && interactable)
        {
            _targetText.color = colors.highlightedColor;
        }
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (_targetText != null && interactable)
        {
            _targetText.color = colors.normalColor;
        }
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (_targetText != null && interactable)
        {
            _targetText.color = colors.pressedColor;
        }
    }
    #endregion
}

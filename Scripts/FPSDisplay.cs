using System;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonobehaviourSingleton<FPSDisplay>
{
    float deltaTime = 0.0f;
    [SerializeField]
    private Color _color;
    [SerializeField]
    private Text _fpsTxt;
    [SerializeField]
    private GameObject _root;
    public bool IsDisplay
    {
        set
        {
            _root.SetActive(value);
        }
        get
        {
            return _root.activeSelf;
        }
    }
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        _fpsTxt.color = _color;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        if (fps >= Application.targetFrameRate - 10)
        {
            if(UnityEngine.Random.Range(0,5)==0)
            {
                GC.Collect();
            }
        }
        _fpsTxt.text = text;
    }
}
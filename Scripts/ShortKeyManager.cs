using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ShortKeyManager : MonobehaviourSingleton<ShortKeyManager>
{
    private List<ShortKey> _shortKeys = new List<ShortKey>();
    private List<KeyCode> _listKeyCode = new List<KeyCode>();
    private List<KeyCode> keydown = new List<KeyCode>();
    private List<ShortKey> _alreadyInvoke = new List<ShortKey>();

    public void RegisterKeyCode(string receiver, string method, object val, params KeyCode[] keys)
    {
        var shortKey = new ShortKey();
        shortKey.receiver = receiver;
        shortKey.method = method;
        shortKey.keyCode = new List<KeyCode>();
        shortKey.val = val;
        _shortKeys.Add(shortKey);
        for (int i = 0; i < keys.Length; i++)
        {
            shortKey.keyCode.Add(keys[i]);
        }
        for (int i = 0; i < shortKey.keyCode?.Count; i++)
        {
            if (!_listKeyCode.Contains(shortKey.keyCode[i]))
            {
                _listKeyCode.Add(shortKey.keyCode[i]);
            }
        }
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            for (int i = 0; i < _listKeyCode.Count; i++)
            {
                if (Input.GetKeyDown(_listKeyCode[i]))
                {
                    if (!keydown.Contains(_listKeyCode[i]))
                        keydown.Add(_listKeyCode[i]);
                }
                if (Input.GetKeyUp(_listKeyCode[i]))
                {
                    keydown.Remove(_listKeyCode[i]);
                t:
                    for (int j = 0; j < _alreadyInvoke.Count; j++)
                    {
                        if (_alreadyInvoke[j].keyCode.Contains(_listKeyCode[i]))
                        {
                            _alreadyInvoke.Remove(_alreadyInvoke[j]);
                            goto t;
                        }
                    }
                }
            }
            for (int i = 0; i < _shortKeys.Count; i++)
            {
                if (!_alreadyInvoke.Contains(_shortKeys[i]) && !_shortKeys[i].keyCode.Any(x => !keydown.Contains(x)))
                {
                    if (_shortKeys[i].keyCode.Count == keydown.Count)
                    {
                        GameObject.Find(_shortKeys[i].receiver)?.SendMessage(_shortKeys[i].method, _shortKeys[i].val);
                        _alreadyInvoke.Add(_shortKeys[i]);
                    }
                }
            }

        }
        for (int i = 0; i < _listKeyCode.Count; i++)
        {
            if (Input.GetKeyDown(_listKeyCode[i]))
            {
                if (!keydown.Contains(_listKeyCode[i]))
                    keydown.Add(_listKeyCode[i]);
            }
            if (Input.GetKeyUp(_listKeyCode[i]))
            {
                keydown.Remove(_listKeyCode[i]);
            t:
                for (int j = 0; j < _alreadyInvoke.Count; j++)
                {
                    if (_alreadyInvoke[j].keyCode.Contains(_listKeyCode[i]))
                    {
                        _alreadyInvoke.Remove(_alreadyInvoke[j]);
                        goto t;
                    }
                }
            }
        }
    }


}
public struct ShortKey
{
    public List<KeyCode> keyCode;
    public string receiver;
    public string method;
    public object val;
}
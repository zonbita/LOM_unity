using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonobehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    private static object m_Lock = new object();
    private static T m_Instance;
    protected static bool _isDestroy;
    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T Instance
    {
        get
        {

            lock (m_Lock)
            {
                if (m_Instance == null && !_isDestroy)
                {
                    m_Instance = FindObjectOfType<T>(true);
                    if (m_Instance == null)
                    {
                        var obj = new GameObject();
                        obj.name = typeof(T).Name;
                        m_Instance = obj.AddComponent<T>();
                    }
                }

                return m_Instance;
            }
        }
    }
    private void OnApplicationQuit()
    {
        _isDestroy = true;
    }
}
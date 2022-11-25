namespace com.wao.core
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    [CustomEditor(typeof(SceneManager))]
    public class SceneManagerEditor : Editor
    {
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            SceneManager mp = (SceneManager)target;

            DrawDefaultInspector();
            if (GUILayout.Button("Load Scene"))
            {
                mp.GetScene();
            }
            if (GUILayout.Button("Destroy Scene"))
            {
                mp.DestroyLastScene();
            }
        }
    }
}
namespace com.wao.core
{
    using UnityEditor;
    using UnityEngine;
    [CustomEditor(typeof(DialogManager))]
    public class DialogManagerEditor : Editor
    {
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            DialogManager mp = (DialogManager)target;
          
            DrawDefaultInspector();
            if (GUILayout.Button("Get Dialog"))
            {
                mp.GetDialog();
            }
            if (GUILayout.Button("Hide Dialog"))
            {
                mp.HideLastDialog();
            }
        }
    }
}
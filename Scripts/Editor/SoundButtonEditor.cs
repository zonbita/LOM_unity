using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SoundButton))]
public class SoundButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundButton mp = (SoundButton)target;

        DrawDefaultInspector();
    

    }
}

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class MyBuildPostprocessor : MonoBehaviour
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        return;
        var path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ConfigData";
        com.wao.Utility.Utility.CopyDirectory(path, Path.GetDirectoryName(pathToBuiltProject) + Path.DirectorySeparatorChar + "ConfigData");
    }
}

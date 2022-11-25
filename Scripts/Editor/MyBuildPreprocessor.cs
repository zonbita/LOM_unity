using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class MyBuildPreprocessor : IPreprocessBuildWithReport
{
	public int callbackOrder => 0;

	public void OnPreprocessBuild(BuildReport report)
	{
		UnityEngine.Debug.Log("[OnPreprocessBuild]");
		return;
#if UNITY_ANDROID || UNITY_IOS
		isPhone = true;
#endif
		var path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ConfigData";
		com.wao.Utility.Utility.CopyDirectory(path, Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Assets/StreamingAssets/ConfigData");
	}
}

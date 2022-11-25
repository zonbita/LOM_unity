using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace com.wao.Utility
{
	public static class Utility
	{
		public static Vector2 GetPositionFromAngle(float angle)
		{
			var radians = angle * Mathf.Deg2Rad;
			var x = Mathf.Cos(radians);
			var y = Mathf.Sin(radians);
			var pos = new Vector2(x, y);
			return pos;
		}
		public static bool OneIn(int number)
		{
			return UnityEngine.Random.Range(0, number) == 0;
		}
		public static byte[] Combine(params byte[][] arrays)
		{
			byte[] rv = new byte[arrays.Sum(a => a.Length)];
			int offset = 0;
			foreach (byte[] array in arrays)
			{
				System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
				offset += array.Length;
			}
			return rv;
		}
		public static void CopyDirectory(string strSource, string strDestination)
		{
			if (Directory.Exists(strDestination))
			{
				//Directory.Delete(strDestination, true);
			}
			Directory.CreateDirectory(strDestination);
			DirectoryInfo dirInfo = new DirectoryInfo(strSource);
			FileInfo[] files = dirInfo.GetFiles();
			foreach (FileInfo tempfile in files)
			{
				tempfile.CopyTo(Path.Combine(strDestination, tempfile.Name), true);
			}

			DirectoryInfo[] directories = dirInfo.GetDirectories();
			foreach (DirectoryInfo tempdir in directories)
			{
				CopyDirectory(Path.Combine(strSource, tempdir.Name), Path.Combine(strDestination, tempdir.Name));
			}

		}
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();

			// If the destination directory doesn't exist, create it.       
			Directory.CreateDirectory(destDirName);

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string tempPath = Path.Combine(destDirName, file.Name);
				file.CopyTo(tempPath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string tempPath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
				}
			}
		}
		public static string GetStorageDirectory()
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar;
#else
			return UnityEngine.Application.persistentDataPath + Path.DirectorySeparatorChar;
#endif
		}
		public static string GetExportDirectory(bool isPhone)
		{
			string exportDir = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ConfigData";
			if (isPhone)
			{
				exportDir = Path.Combine(Application.streamingAssetsPath, "ConfigData");
			}
			if (!Directory.Exists(exportDir))
			{
				Directory.CreateDirectory(exportDir);
			}
			exportDir += Path.DirectorySeparatorChar;
			return exportDir;
		}
		public static string GetMapDirectory()
		{
			return GetStorageDirectory() + "Map";
		}
		public static T LoadConfig<T>(string fileName)
		{
			T res = default;
			try
			{

#if UNITY_EDITOR || UNITY_STANDALONE
				string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ConfigData" + Path.DirectorySeparatorChar + fileName;
				if(File.Exists(path))
				{
					res = File.ReadAllText(path).DeserializeObject<T>();
				}
#else
				string path = "jar:file://" + Application.dataPath + "!/assets/ConfigData/" + fileName;
				string text = LoadInternalFile(path);
				if (!string.IsNullOrEmpty(text))
				{
					res = text.DeserializeObject<T>();
				}
#endif
			}
			catch (Exception e)
			{
				Debug.Log("[LoadConfig] exception:\n" + e);
			}
			return res;
		}

		public static string LoadInternalFile(string path)
		{
			UnityWebRequest www = UnityWebRequest.Get(path);
			www.SendWebRequest();
			while (!www.isDone) { }
			return www.downloadHandler.text;
		}

		public static Vector2 WorldToAnchorPosition(RectTransform canvas, Vector3 worldPosition, Camera camera)
		{
			Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPosition);
			Vector2 WorldObject_ScreenPosition = new Vector2(
			((ViewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
			((ViewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));
			return WorldObject_ScreenPosition;
		}
		public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b)
		{
			return Mathf.Atan2(a.x - b.x, a.z - b.z) * Mathf.Rad2Deg;
		}
		public static float AngleBetweenTwoPoints(Vector2 a, Vector2 b)
		{
			return Mathf.Atan2(a.x - b.x, a.y - b.y) * Mathf.Rad2Deg;
		}
        public static float AngleBetweenTwoPoints(float x1, float y1, float x2, float y2)
        {
            return Mathf.Atan2(x1 - x2, y1 - y2) * Mathf.Rad2Deg;
        }
		public static float GetAngleBetween(Vector2 a, Vector2 b)
		{
			Vector2 normalized = (a - b).normalized;
			float angle = Mathf.Atan2(normalized.y, normalized.x) * Mathf.Rad2Deg;
			if (angle < 0)
				angle = 360 + angle;
			angle = 360 - angle;
			return angle;
		}
		public static string CalculateMD5(string filename)
		{
			using (var md5 = MD5.Create())
			{
				using (var stream = new BufferedStream(File.OpenRead(filename), 1200000))
				{
					var hash = md5.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
			}
		}
		public static string GetMd5FromString(string content)
		{
			using (var md5 = MD5.Create())
			{

				var hash = md5.ComputeHash(Encoding.Default.GetBytes(content));
				return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

			}
		}
		public static List<Type> GetEnumerableOfType<T>() where T : class
		{
			List<Type> objects = new List<Type>();
			foreach (Type type in
				Assembly.GetAssembly(typeof(T)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
			{
				objects.Add(type);
			}
			return objects;
		}


		public static Texture2D LoadTexture2D(byte[] data)
		{
			var tex = new Texture2D(1, 1);
			tex.LoadImage(data);
			return tex;
		}
		public static string GetPlatFormName()
		{
			string platForm = string.Empty;
#if UNITY_STANDALONE_WIN
			platForm = "windows";
#elif UNITY_STANDALONE_OSX
                platForm = "Mac";
#elif UNITY_STANDALONE_LINUX
                platForm = "Linux";
#endif
			return platForm;
		}



		public static async Task<AssetBundle> LoadFromBinary(byte[] data)
		{
			var ie = AssetBundle.LoadFromMemoryAsync(data);
			while (!ie.isDone)
			{
				await Task.Delay(100);
			}
			return ie.assetBundle;
		}
		public static void AnimatedNode(Vector2 currentPos, RectTransform animatedNode, int distanceChild, float scaleMin = 0.35f, float scaleMax = 0.7f)
		{
			var pos = Utility.GetPositionFromAngle(UnityEngine.Random.Range(0, 360));
			pos *= distanceChild;
			pos += currentPos;
			animatedNode.DOScale(Vector3.one * UnityEngine.Random.Range(scaleMin, scaleMax), 10f).SetEase(Ease.OutQuad);
			animatedNode.DOAnchorPos(pos, 4f).SetEase(Ease.OutQuad).onComplete = () =>
			{
				AnimatedNode(currentPos, animatedNode, distanceChild, scaleMin, scaleMax);
			};
		}
		public static List<T> GetListEnum<T>() where T : Enum
		{
			return Enum.GetValues(typeof(T)).Cast<T>().ToList();
		}
		public static void CloneDirectory(string root, string dest)
		{

			if (Directory.Exists(root))
			{
				if (!Directory.Exists(dest))
				{
					Directory.CreateDirectory(dest);
				}
				foreach (var directory in Directory.GetDirectories(root))
				{
					string dirName = Path.GetFileName(directory);
					if (!Directory.Exists(Path.Combine(dest, dirName)))
					{
						Directory.CreateDirectory(Path.Combine(dest, dirName));
					}
					CloneDirectory(directory, Path.Combine(dest, dirName));
				}

				foreach (var file in Directory.GetFiles(root))
				{
					File.Copy(file, Path.Combine(dest, Path.GetFileName(file)), true);
				}
			}
		}

		/// <summary>
		/// check if there is Wall/Gate block sight.
		/// </summary>
		/// <param name="from">From Possition.</param>
		/// <param name="to">To Possition.</param>
		/// <param name="offset">Offset so if To is at the wall, we exclude it.</param>
		/// <returns></returns>
		public static bool IsBlockedByWall(Vector3 from, Vector3 to, float offset = 0.5f)
		{
			Vector3 dir = to - from;
			var ray = new Ray(from, dir);
			var hits = Physics.RaycastAll(ray, dir.magnitude - offset);
			for (int i = 0; i < hits.Length; i++)
			{
				if (hits[i].collider.name == "Wall" || hits[i].collider.name == "Gate")
				{
					return true;
				}
			}
			return false;
		}

        /// <summary>
        /// Random with a specified rate
        /// </summary>
        /// <param name="randItemRate">rate corresponding with index in array</param>
        /// <param name="itemCount">how many time need to random</param>
        /// <returns>Array of indexes</returns>
        public static int[] RandomWithRate(int[] randItemRate, int itemCount)
        {
            int maxRate = 0;
            int kindCount = randItemRate.Length;
            int[] lowerThreshold = new int[kindCount];
            int[] upperThreshold = new int[kindCount];
            for (int i = 0; i < kindCount; i++)
            {
                lowerThreshold[i] = maxRate;
                maxRate += randItemRate[i];
                upperThreshold[i] = maxRate;
            }
            int[] res = new int[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                int randValue = UnityEngine.Random.Range(0, maxRate);
                bool found = false;
                for (int j = 0; j < kindCount; j++)
                {
                    if (lowerThreshold[j] <= randValue && upperThreshold[j] > randValue)
                    {
                        res[i] = j;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Debug.LogError("[Random] invalid " + randValue + " max: " + maxRate);
                    res[i] = 0;
                }
            }
            return res;
        }

        /// <summary>
        /// Random with a specified rate
        /// </summary>
        /// <param name="randItemRate">rate corresponding with index in array</param>
        /// <param name="itemCount">how many time need to random</param>
        /// <returns>List of indexes</returns>
        public static List<int> RandomItemWithRate(int[] randItemRate, int itemCount)
		{
			List<int> res = new List<int>();
			if (itemCount > 0)
			{
				int maxRate = 0;
				Dictionary<int, KeyValuePair<int, int>> randThresholds = new Dictionary<int, KeyValuePair<int, int>>();

				for (int i = 0; i < randItemRate.Length; i++)
				{
					if (!randThresholds.ContainsKey(i) && randItemRate[i] > 0)
					{
						randThresholds.Add(i, new KeyValuePair<int, int>(maxRate, maxRate + randItemRate[i]));
						maxRate += randItemRate[i] + 1;
					}
				}
				if (randThresholds.Count > 0)
				{
					for (int i = 0; i < itemCount; i++)
					{
						int randValue = UnityEngine.Random.Range(0, maxRate);
						bool found = false;
						foreach (var randThreshold in randThresholds)
						{
							KeyValuePair<int, int> lim = randThreshold.Value;
							if (lim.Key <= randValue && randValue <= lim.Value)
							{
								res.Add(randThreshold.Key);
								found = true;
								break;
							}
						}
						if (!found)
						{
							Debug.LogError("[Random] invalid " + randValue + " max: " + maxRate);
						}
					}
				}
			}
			return res;
		}
        
        /// <summary>
        /// Clame angle to between 0 and 360
        /// </summary>
        public static float ClampEulerAngle(this float angle)
        {
            float result = angle;
            if (result < 0)
            {
                result += 360;
            }
            if (result >= 360)
            {
                result -= 360;
            }
            return result;
        }
	
		public static float GetNearestValue(float input, params float[] values)
		{
			if(values == null || values.Length == 0)
			{
				return input;
			}
			else
			{
				float minDif = float.MaxValue;
				float res = input;
				for (int i = 0; i < values.Length; i++)
				{
					float dif = Mathf.Abs(values[i] - input);
					if(dif < minDif)
					{
						minDif = dif;
						res = values[i];
					}
				}
				return res;
			}
		}
	}
}
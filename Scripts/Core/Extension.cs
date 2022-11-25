using com.wao.core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class Extension
{
    public static async Task<T> DeserializeObjectAsync<T>(this string json, bool inherit = false)
    {
        T res = default(T);
        try
        {
            res = await Task.Run<T>(() => json.DeserializeObject<T>(inherit));
        }
        catch
        {
            Debug.LogError(json);
        }
        return res;
    }
    public static float Size(this Vector3 vector3)
    {
        return (vector3.x + vector3.y + vector3.z) / 3f;
    }
    public static List<Type> GetAllClass<T>(this Type baseClass) where T : class
    {
        List<Type> objects = new List<Type>();
        objects = AppDomain.CurrentDomain.GetAssemblies()
                               .SelectMany(assembly => assembly.GetTypes())
                               .Where(type => type.IsSubclassOf(baseClass)).ToList();
        return objects;
    }
    public static Type GetClassWithName(this Type baseClass,string name)
    {
       var  res = AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(baseClass)).SingleOrDefault(x=>x.Name == name);
        return res;
    }
    public static object GetDefaultValue(this Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    public static Vector3 ToVector3(this GVector3 vector3)
    {
        return new Vector3(vector3.x, vector3.y, vector3.z);
    }
    public static Vector3Int ToVector3Int(this GVector3 vector3)
    {
        return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
    }

    public static Vector3Int ToVector3Int(this Vector3 vector3)
    {
        return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
    }


    public static double GetAngle(Vector2 me, Vector2 target)
    {
        return Math.Atan2(target.y - me.y, target.x - me.x) * (180 / Math.PI);
    }
    public static void ScrollToTop(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }
    public static void ScrollToBottom(this ScrollRect scrollRect)
    {
        scrollRect.normalizedPosition = new Vector2(0, 0);
    }

    public static void RotateTo(this RectTransform a, Vector2 targetPosition)
    {
        a.transform.localEulerAngles = new Vector3(0, 0, (float)GetAngle(a.anchoredPosition, targetPosition));
    }
    public static IPEndPoint GetLocalEndPoint(this EndPoint endPoint, AddressFamily addressFamily = AddressFamily.InterNetwork)
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        IPAddress ipAddress = null;
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == addressFamily)
            {
                ipAddress = ip;
                break;
            }
        }

        return new IPEndPoint(ipAddress, (endPoint as IPEndPoint).Port);
    }
    public static async void SendData(this TcpClient client, object data)
    {
        try
        {
            if (client != null)
            {
                string json = null;
                if (data != null)
                {
                    json = data.SerializeObject();
                }
                json = string.Format("{0}#", json);
                byte[] sendBuff = Encoding.UTF8.GetBytes(json);
                await client.SendData(sendBuff);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public static async Task SendData(this TcpClient client, byte[] data)
    {
        try
        {
            if (client.Connected && client.GetStream().CanWrite)
            {
                await client.GetStream().WriteAsync(data, 0, data.Length);
            }

        }
        catch (SocketException ex)
        {
            throw (ex);
        }
    }
    public static Transform FindRecursive(this Transform a, string childName)
    {
        var res = a.Find(childName);
        if (res != null)
            return res;
        else
        {
            foreach (Transform child in a)
            {
                res = child.FindRecursive(childName);
                if (res != null)
                {
                    return res;
                }
            }
        }
        return null;

    }
    public static List<Transform> FindRecursives(this Transform a, string childName)
    {
        var res = new List<Transform>();

        foreach (Transform child in a)
        {
            if (child.name == childName)
            {
                res.Add(child);
            }
            var list = child.FindRecursives(childName);
            if (list.Count > 0)
            {
                res.AddRange(list);
            }
        }

        return res;

    }
    public static bool Smaller(this Vector3 a, Vector3 b, float distanceAllow)
    {
        var d = Vector3.Distance(a, b);
        return d < distanceAllow;
    }
    public static bool CompareVector3IgnoreY(this Vector3 a, Vector3 b, float distanceAllow)
    {
        var d = Vector3.Distance(new Vector3(a.x, 0, a.z), new Vector3(b.x, 0, b.z));
        return d > distanceAllow;
    }
    public static bool ListEqual<T>(this List<T> a, List<T> b)
    {
        if (a.Count == b.Count)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (!b.Contains(a[i]))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public static IPEndPoint CreateIPEndPoint(this string endPoint)
    {
        string[] ep = endPoint.Split(':');
        if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
        IPAddress ip;
        if (ep.Length > 2)
        {
            if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
        }
        else
        {
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
        }
        int port;
        if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
        {
            throw new FormatException("Invalid port");
        }
        return new IPEndPoint(ip, port);
    }
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }
    public static T[] CopyAddElements<T>(this T[] array, params T[] add)
    {
        for (int i = 0; i < add.Length; i++)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = add[i];
        }
        return array;
    }

    public static T[] RemoveElements<T>(this T[] array, params T[] remove)
    {
        var list = array.ToList();
        for (int i = 0; i < remove.Length; i++)
        {
            list.Remove(remove[i]);
        }
        return list.ToArray();
    }
    public static T Clone<T>(this T obj)
    {
        return obj.SerializeObject().DeserializeObject<T>();
    }

    public static bool IsNeigbor(this RectInt a, RectInt b)
    {
        if (a.x + a.width == b.x && ((a.y >= b.y && a.y < b.y + b.height) || (b.y >= a.y && b.y < a.y + a.height)))
            return true;
        if (a.y + a.height == b.y && ((a.x >= b.x && a.x < b.x + b.width) || (b.x >= a.x && b.x < a.x + a.width)))
            return true;
        return false;
    }

    public static List<T> GetRandomItems<T>(this List<T> list, int numberOfItem, T except)
    {
        var copy = new List<T>(list);
        copy.Remove(except);
        if (numberOfItem < list.Count)
        {
            var res = new List<T>();
            for (int i = 0; i < numberOfItem; i++)
            {
                T item = copy[UnityEngine.Random.Range(0, copy.Count)];
                res.Add(item);
                copy.Remove(item);
            }
            return res;
        }
        return copy;
    }


    public static async Task<T> DeserializeObjectAsync<T>(this string json, Action<T> callBackWhenDone, bool inherit)
    {
        var res = await Task.Run<T>(() => JsonConvert.DeserializeObject<T>(json));
        callBackWhenDone?.Invoke(res);
        return res;

    }
    public static object Deserialize(this string text, Type type)
    {
        return JsonConvert.DeserializeObject(text, type);
    }

    public static IEnumerator LoadAssetEX<T>(this AssetBundle assetBundle, string name, UnityAction<T> cb, UnityAction<float> onProgress = null) where T : UnityEngine.Object
    {
        if (assetBundle == null)
        {
            onProgress?.Invoke(1f);
            yield break;
        }

        var asset = assetBundle.LoadAssetAsync<T>(name);

        int i = 0;
        while (!asset.isDone)
        {
            if (i < 9)
            {
                i++;
            }
            onProgress?.Invoke(i * 0.1f);
            yield return new WaitForEndOfFrame();
        }
        cb?.Invoke(asset.asset as T);
    }
    public static T DeserializeObject<T>(this string json, bool inherit = false)
    {
        if (json == null)
            return default(T);
        try
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = inherit ? TypeNameHandling.All : TypeNameHandling.None
            });
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError(json);
            return default(T);
        }
    }
    public static async Task<string> SerializeObjectAsync(this object data, bool inherit = false)
    {
        return await Task.Run<string>(() => data.SerializeObject(inherit));
    }
    public static string SerializeObject(this object obj, bool inherit = false, bool beautify = false)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = inherit ? TypeNameHandling.All : TypeNameHandling.None,
            Formatting = beautify ? Formatting.Indented : Formatting.None,
        });
    }
    public static string ToFormattedText<T>(this T value) where T : Enum
    {
        var stringVal = value.ToString();
        var bld = new StringBuilder();

        for (var i = 0; i < stringVal.Length; i++)
        {
            if (char.IsUpper(stringVal[i]))
            {
                bld.Append(" ");
            }

            bld.Append(stringVal[i]);
        }

        return bld.ToString();
    }
    public static bool NearlyEqual(this float a, float b, float epsilon)
    {
        float absA = Math.Abs(a);
        float absB = Math.Abs(b);
        float diff = Math.Abs(a - b);

        if (a == b)
        { // shortcut, handles infinities
            return true;
        }
        else if (a == 0 || b == 0 || absA + absB < float.MinValue)
        {
            // a or b is zero or both are extremely close to it
            // relative error is less meaningful here
            return diff < (epsilon * float.MinValue);
        }
        else
        { // use relative error
            return diff / (absA + absB) < epsilon;
        }
    }
    public static T DeserializeObject<T>(this byte[] data) where T : class
    {
        using (var stream = new MemoryStream(data))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
            return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
    }
    public static async Task DeserializeObject<T>(this string json, Action<T> oncomplete)
    {
        oncomplete?.Invoke(await Task.Run<T>(() => JsonConvert.DeserializeObject<T>(json)));
    }

    public static string UppercaseFirstLetter(this string s)
    {
        return s[0].ToString().ToUpper() + s.Remove(0, 1);
    }

    public static void StartThreadFromPool(this object obj)
    {
        ThreadPool.QueueUserWorkItem(new WaitCallback(WCB), obj);
    }
    private static void WCB(System.Object obj)
    {
        Action ac = obj as Action;
        ac?.Invoke();
    }
}

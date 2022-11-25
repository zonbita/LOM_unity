namespace com.wao.rpgs
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "World Database", menuName = "Data/World", order = 1)]
    [System.Serializable]
    public class MapData : ScriptableObject
    {
        public Vector3IntAreaDataDictionary areaDatas;
     
    }
}
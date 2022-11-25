namespace com.wao.rpgs
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "World Database", menuName = "Data/World", order = 1)]
    public class WorldDatabase : ScriptableObject
    {
        public MapData worldData;
    }
}
namespace com.wao.rpgs
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "Creature Database", menuName = "Data/Creature", order = 1)]
    public class CreatureDatabase : ScriptableObject
    {
        public StringSoulDictionary creatures;
    }
}
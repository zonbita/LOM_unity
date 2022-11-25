namespace com.wao.rpgs
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "Game Event Database", menuName = "Data/Game Event", order = 1)]
    [System.Serializable]
    public class GameEventData : ScriptableObject
    {
        public StringGameEventDictionary gameEvents;
    }
}
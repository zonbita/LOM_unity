namespace com.wao.rpgs
{
    using UnityEngine;
    [CreateAssetMenu(fileName = "Quest Database", menuName = "Data/Quest", order = 1)]
    [System.Serializable]
    public class QuestData : ScriptableObject
    {
        public StringQuestDictionary quests;
    }
}
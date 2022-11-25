namespace com.wao.rpgs
{
    [System.Serializable]
    public struct GameEvent 
    {
        public string id;
        public string name;
        public GameEventTriggerType triggerType;
        public string actionName;
    }

    public enum GameEventTriggerType
    {
        NPCDie,
        Area,
        OpenItem,
        CompleteQuest,
    }
}
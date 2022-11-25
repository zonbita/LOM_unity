namespace com.wao.rpgs
{
    [System.Serializable]
    public struct Quest
    {
        public string id;
        public string giver;
        public string name;
        public QuestReward questReward;
        public QuestType type;
        public string area;
        public string targetArea;
        public string target;
        public int amount;
        public string receiver;
    }
    [System.Serializable]
    public struct QuestReward
    {
        public string rewardId;
        public int amount;
        public QuetRewardType type;
    }

    public enum QuetRewardType
    {
        EXP,
        Gold,
        Item,
        Skill
    }

    public enum QuestType
    {
        Kill,
        Gather,
        DoAction,
    }
}
namespace com.wao.rpgs
{
    [System.Serializable]
    public class Item
    {
        public string id;
        public bool storeAble
        {
            get
            {
                return (type == ItemType.Equipment || type == ItemType.ConsumeAble || type == ItemType.Trigger);
            }
        }
        public bool canCombine;
        public ItemType type;
        public string icon;
        public string assetOutSide;
    }
    [System.Serializable]
    public class ItemConsumeAble : Item
    {
        public int numberOfUsage;
    }
    [System.Serializable]
    public class ItemTrigger: Item
    {
        public string objectTrigger;
        public string triggerAction;
    }
    [System.Serializable]
    public class ItemEquipment : Item
    {
        public int durable;
        public string equiptTo;
    }
   
    public enum ItemType
    {
        Object,
        ConsumeAble,
        Equipment,
        Trigger,
    }
}
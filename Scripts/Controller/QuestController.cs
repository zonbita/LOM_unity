namespace com.wao.rpgs
{
    using com.wao.core;
    using com.wao.rpgs.service;
    using UnityEngine;
    using System.Linq;
    public class QuestController : GameController
    {
        private QuestData _questData;
        private StringQuestDictionary _currentActiveQuest = new StringQuestDictionary();
        public QuestController(IGameDatabaseService gameDatabaseService) : base(gameDatabaseService)
        {
            _questData = Resources.Load<QuestData>("Data/Quest/Quest Database").Clone();
        }

        public bool IsQuestGiver(Soul soul)
        {
            if(_currentActiveQuest.Any(x=>x.Value.giver == soul.id))
            {
                return true;
            }
            return false;
        }
    }
}
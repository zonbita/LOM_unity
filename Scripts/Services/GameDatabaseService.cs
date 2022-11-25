namespace com.wao.rpgs.service
{
    using com.wao.core;
    using UnityEngine;
    public class GameDatabaseService : IGameDatabaseService, IService
    {
        private MapData _current;
        private CreatureDatabase _database;
        public MapData GetWorldDatabase(string area)
        {
            if (_current == null || _current.name != area)
            {
                _current = Resources.Load<MapData>("Data/Map/" + area);
            }
            return _current;
        }
        public AreaData GetAreaData(Vector3Int id)
        {
            return _current.areaDatas.ContainsKey(id) ? _current.areaDatas[id].Clone() : default(AreaData);
        }


        public void Init()
        {
            _database = Resources.Load<CreatureDatabase>("Data/Creature/Creature Database");
        }

        public Soul GetSoulInDatabase(string id)
        {
            return _database != null && _database.creatures.ContainsKey(id) ? _database.creatures[id].Clone() : default(Soul);
        }

    }

    public interface IGameDatabaseService
    {
        MapData GetWorldDatabase(string area);
        Soul GetSoulInDatabase(string id);
        AreaData GetAreaData(Vector3Int position);
    }
}
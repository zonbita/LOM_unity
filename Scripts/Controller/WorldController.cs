
namespace com.wao.rpgs
{
    using com.wao.core;
    using com.wao.rpgs.service;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine;
    using Unity.Burst;
    using System.Collections.Generic;

    public class WorldController : Controller
    {
        private IGameDatabaseService _gameDatabaseService;
        private Dictionary<AreaData, List<Soul>> _soulsInTheMap = new Dictionary<AreaData, List<Soul>>();
        private WorldView WorldView
        {
            get
            {
                return (view as WorldView);
            }
        }
        public void LoadMap(string worldData)
        {
            data = _gameDatabaseService.GetWorldDatabase(worldData);
        }

        public WorldController(IGameDatabaseService gameDatabaseService)
        {
            _gameDatabaseService = gameDatabaseService;
            view = SceneManager.Instance.Current.GetComponentInChildren<WorldView>(true);
            LoadMap("Begin");
            view.Init(this, data);

            LoadAreasSurroundPosition(Vector3Int.zero);
        }
        public void UnLoadArea(AreaData areaData)
        {
            if (_soulsInTheMap.ContainsKey(areaData))
            {
                WorldView worldView = (view as WorldView);
                var souls = _soulsInTheMap[areaData];
                _soulsInTheMap.Remove(areaData);
                for (int i = 0; i < souls.Count; i++)
                {
                    worldView.UnloadBody(souls[i]);
                }
                worldView.UnloadArea(areaData);
            }
        }
        public void LoadAreasSurroundPosition(Vector3Int position)
        {
            foreach (AreaData areaData in _soulsInTheMap.Keys)
            {
                if (areaData.position.x > position.x + 1 ||
                    areaData.position.x < position.x - 1 ||
                    areaData.position.y > position.y + 1 ||
                    areaData.position.y < position.y - 1)
                {
                    //ouside
                    UnLoadArea(areaData);
                }
            }

            var area = _gameDatabaseService.GetAreaData(position);
            AudioManager.Instance.PlayBgm(area.bgm);
            for (int x = position.x - 1; x < position.x + 1; x++)
            {
                for (int y = position.y - 1; y < position.y + 1; y++)
                {
                    area = _gameDatabaseService.GetAreaData(new Vector3Int(x, 0, y));
                    if (!_soulsInTheMap.ContainsKey(area))
                    {
                        _soulsInTheMap.Add(area, new List<Soul>());
                        var souls = _soulsInTheMap[area];
                        WorldView.SetupArea(area);
                        if (area.monsters != null)
                        {
                            for (int m = 0; m < area.monsters.Count; m++)
                            {
                                foreach (string monsterId in area.monsters[m].monsters.Keys)
                                {
                                    var soulPrototype = _gameDatabaseService.GetSoulInDatabase(monsterId);
                                    souls.Add(soulPrototype);
                                    WorldView.CreateBodyForSoul(soulPrototype, "Enemy");
                                }
                            }
                            for (int n = 0; n < area.npcs.Count; n++)
                            {
                                var soul = area.npcs[n];
                                souls.Add(soul);
                                WorldView.CreateBodyForSoul(soul, "NPC");
                            }
                        }
                    }
                   
                }
            }
        }

        public void SetTargetObjectPosition(Vector3 position)
        {
            WorldView.SetTargetPosition(position);
        }

        public override void UpdateFrame()
        {
            base.UpdateFrame();
        }
    }

    [BurstCompile]
    public struct UpdateAIJob : IJobParallelFor
    {
        public NativeArray<float3> positions;
        public void Execute(int index)
        {

        }
    }
}
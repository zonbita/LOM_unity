namespace com.wao.rpgs
{
    using com.wao.core;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct AreaData
    {
        public string bgm;
        public List<MonsterSpawnArea> monsters;
        public List<Soul> npcs;
        public string asset;
        public Vector3Int position;
    }

    [System.Serializable]
    public struct MonsterSpawnArea
    {
        public string name;
        public Vector3 position;
        public StringFloatDictionary monsters;
    }

    public enum Direction
    {
        Center,
        Top,
        TopRight,
        TopLeft,
        Left,
        Right,
        Bot,
        BotRight,
        BotLeft
    }
}
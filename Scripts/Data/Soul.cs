

namespace com.wao.core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Unity.Mathematics;
    using UnityEngine;
    [System.Serializable]
    public struct Soul 
    {
        public string id;
        public string name;
        public string bodyAssetPath;
        public string controller;
        public long maxHp;
        public int level;
        public long hp;
        public long sp;
        public long maxSp;
        public List<string> interactWith;
        public List<string> status;
        public GVector3 position;
        [HideInInspector]
        [JsonIgnore]
        public GameController gcontroller;
        [HideInInspector]
        [JsonIgnore]
        public GameView view;
    }
    public struct GVector3
    {
        public float x, y, z;
    }
}
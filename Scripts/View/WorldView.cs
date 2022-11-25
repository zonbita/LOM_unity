namespace com.wao.rpgs
{
    using com.wao.core;
    using com.wao.core.Utility;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;

    public class WorldView : GameView
    {
        private GameObject _targetObject;
        [SerializeField]
        private Soul _playerPrototype;
        private Dictionary<AreaData, GameObject> _areaObject = new Dictionary<AreaData, GameObject>();
        public override void Init(Controller controller, object data)
        {
            base.Init(controller, data);
            CreateBodyForSoul(_playerPrototype.Clone(), "Player",(playerBody)=>
            {
                CameraFollow.Instance.target = playerBody.transform;
            });
        }
        public void LoadAll(string worldData)
        {
            (controller as WorldController).LoadMap(worldData);
        }
        public void CreateBodyForSoul(Soul soul, string tag, System.Action<Body> onComplete = null)
        {
            FactoryManager.Instance.GetObjectFromPool<Body>(soul.bodyAssetPath, AssetFrom.Resources, (creature) =>
            {
                creature.transform.SetParent(transform);
                creature.tag = tag;
                creature.transform.position = soul.position.ToVector3();
                creature.status = soul.status;
                if (creature.GetComponent<NavMeshAgent>() == null)
                {
                    creature.gameObject.AddComponent<NavMeshAgent>();
                }
                creature.soul = soul;
                onComplete?.Invoke(creature);
            });
        }

        public void UnloadBody(Soul soul)
        {
            if(soul.view!=null)
            {
                soul.view.gameObject.SetActive(false);
            }
        }

        public void SetupArea(AreaData data)
        {
            if (!string.IsNullOrEmpty(data.asset))
            {
                AssetManager.Instance.Load<GameObject>("Map/Area/" + data.asset, AssetFrom.Resources).SetOnComplete((terrain) =>
                {
                    var area = GameObject.Instantiate(terrain, transform);
                    _areaObject.Add(data,area);
                    area.transform.position = data.position * 50;
                }).Excute();
            }
        }

        internal void UnloadArea(AreaData area)
        {
            var areaObject = _areaObject[area];
            _areaObject.Remove(area);
            if(areaObject!=null)
            {
                Destroy(areaObject);
            }
        }

        public void SetTargetPosition(Vector3 position)
        {
            if (_targetObject == null)
            {
                _targetObject = Instantiate(Resources.Load<GameObject>("Etc/TargetObject"));
                _targetObject.transform.SetParent(transform);
            }
            _targetObject.transform.position = position;
        }

        protected override void Update()
        {
            base.Update();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            FactoryManager.Instance.CleanAllCreatedObjects();
        }
    }
}
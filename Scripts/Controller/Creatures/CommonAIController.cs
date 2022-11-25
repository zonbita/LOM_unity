namespace com.wao.rpgs
{
    using com.wao.core;
    using com.wao.rpgs.service;
    using System;
    using UnityEngine.AI;

    public class CommonAIController : CreatureController
    {
        protected WorldController _worldController;
        private DateTime _lastUpdate;
        protected float _aiIntelligent = 1;
        public CommonAIController(IGameDatabaseService gameDatabaseService) : base(gameDatabaseService)
        {
            _worldController = ControllerManager.Instance.GetController<WorldController>();
        }

        protected virtual void UpdateAI()
        {

            if (view.GetComponent<FieldOfView>().canSeeTarget)
            {
                view.GetComponent<NavMeshAgent>().destination = view.GetComponent<FieldOfView>().playerRef.transform.position;
            }

        }


        public override void UpdateFrame()
        {
            base.UpdateFrame();
            if (data != null)
            {
                if (view != null)
                {
                    if ((DateTime.Now - _lastUpdate).TotalSeconds > _aiIntelligent)
                    {
                        UpdateAI();
                        _lastUpdate = DateTime.Now;
                    }
                }
            }

        }
    }
}
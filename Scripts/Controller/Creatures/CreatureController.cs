namespace com.wao.rpgs
{
    using com.wao.core;
    using com.wao.rpgs.service;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class CreatureController : GameController
    {
        protected Soul soul
        {
            get
            {
                return (Soul)data;
            }
        }
        public CreatureController(IGameDatabaseService gameDatabaseService) : base(gameDatabaseService)
        {

        }

        public override void UpdateFrame()
        {
            base.UpdateFrame();
           
        }
    
        public bool IsVisible(Camera c)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(c);
            var position = soul.position;
            foreach (var plane in planes)
            {
                if (plane.GetDistanceToPoint(position.ToVector3()) < 0)
                    return false;
            }
            return true;
        }
   
    }
}
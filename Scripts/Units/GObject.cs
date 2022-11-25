namespace com.wao.rpgs
{
    using com.wao.core;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using System.Linq;
    public class GObject : GameView, IInteractionAble
    {
        public float size;
        public string objectId
        {
            set;
            get;
        }
        public virtual void Interaction(Soul soul)
        {
            if (soul.status.Contains("Shop"))
            {
                Debug.Log("Open Shop");
            }
        }
        public List<string> status;

        public virtual void DoAction(string action, params object[] values)
        {
            MethodInfo methodInfo = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == action);
            if (methodInfo != null)
            {
                if (methodInfo.GetParameters().Length == values.Length)
                    methodInfo.Invoke(this, values);
            }
        }
        public virtual void GetHit(int damage)
        {
          
        }

        public override void Init(Controller controller, object data)
        {

        }
    }

}
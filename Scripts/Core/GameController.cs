
namespace com.wao.core
{

    using com.wao.rpgs.service;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class GameController : Controller
    {
        protected string _lastCommand;
        public GameController(IGameDatabaseService gameDatabaseService)
        {

        }
 
        public override void ControllerDoAction(string method, params object[] datas)
        {
            MethodInfo methodInfo = this.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.Name == method);
            if (methodInfo != null)
            {
                if (methodInfo.GetParameters().Length == datas.Length)
                    methodInfo.Invoke(this, datas);
            }
            _lastCommand = method;

        }
        public virtual void OnTriggerEnter(Collider collider)
        {

        }
    }
}
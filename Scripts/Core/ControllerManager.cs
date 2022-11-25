

namespace com.wao.core
{
    using System.Collections.Generic;
    using System;

    public class ControllerManager : MonobehaviourSingleton<ControllerManager>
    {
        private Dictionary<System.Type, Controller> _singletonControllers = new Dictionary<System.Type, Controller>();
        public T GetController<T>() where T : Controller
        {
            var type = typeof(T);
            if (_singletonControllers.ContainsKey(type))
            {
                if (_singletonControllers[type] == null)
                {
                    _singletonControllers[type] = CreateController<T>();
                }
                return _singletonControllers[type] as T;
            }
            var instance = CreateController<T>();
            return instance;
        }
        public void RegisterSingletonController<T>() where T : Controller
        {
            Type type = typeof(T);
            if (!_singletonControllers.ContainsKey(type))
            {
                _singletonControllers.Add(type, null);
            }
        }
        public Controller GetControllerFromName(string controllerName)
        {
            Type type = Type.GetType(controllerName);
            return GetControllerFromType(type);
        }
        public Controller GetControllerFromType(Type type)
        {
            if (type == null)
                return null;
            if (_singletonControllers.ContainsKey(type))
            {
                if(_singletonControllers[type]==null)
                {
                    _singletonControllers[type] = CreateControllerFromType(type);
                }
             
                return _singletonControllers[type];
            }
            return CreateControllerFromType(type);
        }
        private Controller CreateControllerFromType(Type type)
        {
            Controller controler = null;
            var contructor = type.GetConstructors()[0];
            var services = new List<object>();
            foreach (var param in contructor.GetParameters())
            {
                var service = ServiceManager.Instance.GetService(param.ParameterType);
                services.Add(service);
            }
            if (services.Count > 0)
            {
                controler = Activator.CreateInstance(type, services.ToArray()) as Controller;
            }
            else
            {
                controler = Activator.CreateInstance(type) as Controller;
            }
            return controler;
        }
        private T CreateController<T>() where T : Controller
        {
            var type = typeof(T);
            return GetControllerFromType(type) as T;
        }
    }

    [Serializable]
    public abstract class Controller
    {
        protected object data;
        protected GameView view;
        public void Init(object data,GameView view)
        {
            this.data = data;
            this.view = view;
        }
        public virtual void UpdateFrame()
        {

        }
        public virtual void ControllerDoAction(string method, params object[] datas)
        {

        }
    }

}
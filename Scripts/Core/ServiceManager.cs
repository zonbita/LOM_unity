
namespace com.wao.core
{
    using System.Collections.Generic;
    using System;
    using System.Linq;
    public class ServiceManager : MonobehaviourSingleton<ServiceManager>
    {
        private Dictionary<Type, Type> _registerInterface = new Dictionary<Type, Type>();
        private Dictionary<Type, IService> _singleton = new Dictionary<Type, IService>();
        public void RegisterSingleton<T, U>() where T : class where U : IService
        {
            var typeT = typeof(T);
            var typeU = typeof(U);
            if (!_registerInterface.ContainsKey(typeT))
            {
                _registerInterface.Add(typeT, typeU);
            }
        }
        public Type GetServiceType(Type type)
        {
            if (_registerInterface.ContainsKey(type))
            {

                return _registerInterface[type];
            }
            return null;
        }
        public IService GetService(Type type)
        {
            if (_registerInterface.ContainsKey(type))
            {
                if (_singleton.ContainsKey(type))
                {
                    return _singleton[type];
                }

                IService instance = AddService(_registerInterface[type]);
                (instance as IService).Init();
                _singleton.Add(type, instance);
                return instance;
            }
            return (IService)type.GetDefaultValue();
        }
        private IService AddService(Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length > 0)
            {
                var contructor = type.GetConstructors().FirstOrDefault(x => x.IsPublic);
                var services = new List<object>();
                foreach (var param in contructor.GetParameters())
                {
                    var service = GetService(param.ParameterType);
                    services.Add(service);
                }
                if (services.Count > 0)
                {
                    IService instance = (IService)Activator.CreateInstance(type, services.ToArray());
                    return instance;
                }
                else
                {
                    return (IService)Activator.CreateInstance(type);
                }
            }
            else
            {
                return (IService)Activator.CreateInstance(type);
            }

        }
        public T GetService<T>() where T : IService
        {
            var type = typeof(T);
            if (_singleton.ContainsKey(type))
            {
                return (T)_singleton[type];
            }
            return default(T);
        }
    }

}
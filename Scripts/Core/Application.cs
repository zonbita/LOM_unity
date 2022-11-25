
namespace com.wao.core
{
    [System.Serializable]
    public class Application
    {
        public string version;
        public string name;
        public Application RegisterSingletonService<T, U>() where T : class where U : IService
        {
            ServiceManager.Instance.RegisterSingleton<T, U>();
            return this;
        }
    }
}
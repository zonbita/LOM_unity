
namespace com.wao.rpgs.manager
{
    using com.wao.core;
    using com.wao.rpgs.service;
    public class GameManager : MonobehaviourSingleton<GameManager>
    {
        [UnityEngine.SerializeField]
        private com.wao.core.Application _application;
        private void AddService()
        {
            _application.RegisterSingletonService<IGameDatabaseService, GameDatabaseService>();
            _application.RegisterSingletonService<ILocalizeService, LocalizeService>();
        }
        private void InitController()
        {

            ControllerManager.Instance.RegisterSingletonController<PlayerController>();
            ControllerManager.Instance.RegisterSingletonController<WorldController>();
            ControllerManager.Instance.RegisterSingletonController<StoryController>();
            ControllerManager.Instance.RegisterSingletonController<CraftSystemController>();
            ControllerManager.Instance.RegisterSingletonController<SkillSystemController>();

        }
        void Start()
        {
            AddService();
            InitController();
        }
    }
}
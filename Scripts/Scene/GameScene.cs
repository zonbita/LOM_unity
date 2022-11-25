namespace com.wao.rpgs
{
    using com.wao.core;
    public class GameScene : SceneBase
    {
        private WorldController _worldController;
        public override void Show()
        {
            base.Show();
            _worldController = ControllerManager.Instance.GetController<WorldController>();
        }

    }
}
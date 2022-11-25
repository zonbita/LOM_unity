
namespace com.wao.core
{
    public class GameView : MonoProduct
    {
        protected object data;
        protected Controller controller;
        public Controller Controller
        {
            get
            {
                return controller;
            }
        }
     
        public virtual void Init(Controller controller, object data)
        {
            this.data = data;
            this.controller = controller;
        }
        protected virtual void Update()
        {
            controller?.UpdateFrame();
        }
    }
}
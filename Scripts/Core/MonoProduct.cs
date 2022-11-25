using UnityEngine;
namespace com.wao.core
{
    public class MonoProduct : MonoBehaviour
    {
        [SerializeField]
        private string _productTd;

        public string productId
        {
            get { return _productTd; }
        }
        public bool ignore;
        protected virtual void Awake()
        {

        }
        protected virtual void OnDisable()
        {
            if (!ignore && FactoryManager.Instance != null)
            {
                FactoryManager.Instance.ReturnProduct(this, productId);
            }
        }

        protected virtual void OnDestroy()
        {
            if (!ignore && FactoryManager.Instance != null)
            {
                FactoryManager.Instance.RemoveProduct(this, productId);
            }
        }
    }
}
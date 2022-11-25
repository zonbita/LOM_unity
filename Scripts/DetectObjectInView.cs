namespace com.wao.rpgs
{
    using UnityEngine;

    public class DetectObjectInView : MonoBehaviour
    {
        [SerializeField]
        private bool _showGizmos;
        [SerializeField]
        private float _radius;
        [SerializeField]
        private Color _grizmoColor;
        [SerializeField]
        private Body _body;
        void Update()
        {
            RaycastHit info;
            if (Physics.SphereCast(transform.position, _radius, transform.forward, out info,1000))
            {
                Debug.Log(info.collider);
                _body.DetectPhysicsObject(info.collider);
            }
        }

        private void OnDrawGizmos()
        {
            if (_showGizmos)
            {
                Gizmos.color = _grizmoColor;
                Gizmos.DrawSphere(transform.position, _radius);
            }
        }
    }
}
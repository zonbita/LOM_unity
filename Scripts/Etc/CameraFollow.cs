namespace com.wao.rpgs
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraFollow : MonobehaviourSingleton<CameraFollow>
    {
        public Vector3 range;
        public Transform target;
        public float smooth;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                transform.position = Vector3.Lerp(transform.position, target.position + range, Time.deltaTime * smooth);
            }
        }
    }
}
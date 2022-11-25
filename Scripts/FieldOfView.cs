namespace com.wao.rpgs
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    using com.wao.rpgs;

    public class FieldOfView : MonoBehaviour
    {
        public float radius;
        [Range(0, 360)]
        public float angle;

        public GameObject playerRef;
        public List<GObject> bodyInRange;
        public List<GObject> bodyNotInRange;
        public LayerMask targetMask;
        public LayerMask obstructionMask;
        public List<string> lookingFor;
        public float checkTime;
        public bool canSeeTarget
        {
            get;
            private set;
        }

        private void OnEnable()
        {
            StartCoroutine(FOVRoutine());
        }

        private IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(checkTime);

            while (true)
            {
                yield return wait;
                FieldOfViewCheck();
            }
        }

        private void FieldOfViewCheck()
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask).Where(x => lookingFor != null && lookingFor.Contains(x.tag)).ToArray();
            bodyInRange.Clear();
            bodyNotInRange.Clear();
            canSeeTarget = false;
            if (rangeChecks.Length != 0)
            {
                for (int i = 0; i < rangeChecks.Length; i++)
                {
                    Transform target = rangeChecks[i].transform;
                    Vector3 directionToTarget = (target.position - transform.position).normalized;
                    var gobject = target.GetComponent<GObject>();
                    if (gobject != null)
                    {
                        if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, target.position);

                            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                            {
                                canSeeTarget = true;
                                bodyInRange.Add(gobject);

                                playerRef = target.gameObject;
                            }


                        }
                        else
                        {
                            bodyNotInRange.Add(gobject);
                        }
                    }
                }
            }
        }
    }
}
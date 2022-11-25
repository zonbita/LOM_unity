
namespace com.wao.rpgs
{
    using UnityEngine;
    using com.wao.core;
    using System.Collections.Generic;
    using UnityEngine.Events;
    using UnityEngine.AI;
    using System;

    public abstract class Body : GObject
    {
        [SerializeField]
        private FieldOfView _detectView, _attackRange;
        private Vector3 _previousPosition;
        protected NavMeshAgent agent;
        private DateTime _checkTime;
        protected Soul _soul;
        protected AnimatorOverrideController animatorOverrideController
        {
            get;
            private set;
        }
        public bool isStopMoving
        {
            get;
            protected set;
        }
        public override void Interaction(Soul soul)
        {
            base.Interaction(soul);
            transform.LookAt(soul.view?.transform);
        }
        protected void ActiveSkill(params object[] values)
        {

        }
        protected void Move(Vector3 pos, GObject target, string animation, float speed)
        {

            agent.stoppingDistance = 0;
            if (target != null)
            {
                agent.stoppingDistance = transform.localScale.Size() * this.size + target.transform.localScale.Size() * target.size;
            }
            PlayAnimation(animation, speed);

            isStopMoving = false;
            agent.enabled = true;
            agent.destination = pos;
            agent.isStopped = false;
            _checkTime = DateTime.Now.AddSeconds(0.6f);


        }
        protected void PlayAnimation(string animationName, float speed)
        {
            var clip = Resources.Load<AnimationClip>("Animation/Humanoid/" + animationName); ;
            animatorOverrideController["Idle"] = clip;

            GetComponent<Animator>().speed = speed;

        }

        public Soul soul
        {
            set
            {
                _soul = value;
                controller = ControllerManager.Instance.GetControllerFromName(value.controller) as GameController;
                value.gcontroller = controller as GameController;
                controller.Init(_soul, this);
                agent = GetComponent<NavMeshAgent>();
                if (_detectView != null)
                    _detectView.lookingFor = value.interactWith;
                if (_attackRange != null)
                    _attackRange.lookingFor = value.interactWith;
                value.view = this;
            }
            get
            {
                return _soul;
            }
        }
        public List<GObject> bodyInDetectRange
        {
            get
            {
                return _detectView.bodyInRange;
            }
        }
        public List<GObject> objectNotInAttackRange
        {
            get
            {
                return _attackRange.bodyNotInRange;
            }
        }
        public List<GObject> objectInAttackRange
        {
            get
            {
                return _attackRange.bodyInRange;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            var animator = GetComponent<Animator>();
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        public override void GetHit(int damage)
        {
            base.GetHit(damage);
            _soul.GetDamage(damage);
            controller.ControllerDoAction("GetHit", damage);
        }

        public void Hit()
        {
            controller.ControllerDoAction("Hit");
        }
        public void FinishAttack()
        {
            controller.ControllerDoAction("FinishAttack");
        }

        private void OnTriggerEnter(Collider other)
        {
            (controller as CreatureController)?.OnTriggerEnter(other);

        }
        protected override void Update()
        {
            base.Update();
            if (agent != null)
            {

                if (_checkTime < DateTime.Now)
                {
                    if (!isStopMoving && transform.localPosition.Smaller(_previousPosition, 0.01f))
                    {
                        isStopMoving = true;
                        agent.isStopped = true;
                        agent.enabled = false;
                    }
                    else
                    {
                        _previousPosition = transform.localPosition;
                        _checkTime = DateTime.UtcNow.AddSeconds(0.6f);
                    }
                }
            }
            else
            {
                isStopMoving = false;
            }

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            controller = null;
            status.Clear();
        }

        public virtual void DetectPhysicsObject(Collider collider)
        {

        }
    }

}
namespace com.wao.rpgs
{
    using com.wao.core;
    using com.wao.rpgs.service;
    using UnityEngine;

    public class PlayerController : CreatureController
    {
        private WorldController _worldController;
        private bool _isAttacking;
        private GObject _target;
        private Vector3Int _currentPositionOnMap;
        public PlayerController(IGameDatabaseService gameDatabaseService) : base(gameDatabaseService)
        {
            PlayerInputManager.Instance.onMouseRightClick += OnMouseRightClick;
            PlayerInputManager.Instance.onMouseLeftClick += OnMouseLeftClick;
            PlayerInputManager.Instance.qKey += OnQKeyClick;
            PlayerInputManager.Instance.wkey += OnWKeyClick;
            PlayerInputManager.Instance.ekey += OnEKeyClick;
            PlayerInputManager.Instance.rkey += OnRkeyClick;
            _worldController = ControllerManager.Instance.GetController<WorldController>();
         
        }

        private void OnRkeyClick()
        {
            (view as GObject).DoAction("ActiveSkill", "4");
        }

        private void OnEKeyClick()
        {

            (view as GObject).DoAction("ActiveSkill", "3");
        }

        private void OnWKeyClick()
        {
            (view as GObject).DoAction("ActiveSkill", "2");
        }

        private void OnQKeyClick()
        {
            (view as GObject).DoAction("ActiveSkill", "1");
        }

        private void OnMouseLeftClick()
        {
            if (_target != null)
            {
                (view as GObject).DoAction("CheckInfo", _target);
            }
        }

        private void FinishAttack(object[] arg0)
        {
            _isAttacking = false;
            if ((view as Body).objectInAttackRange != null && (view as Body).objectInAttackRange.Count == 0)
                (view as Body).DoAction("PlayAnimation", "Idle", 1f);

        }

        private void OnHit(object[] arg0)
        {

            if ((view as Body).objectInAttackRange != null && (view as Body).objectInAttackRange.Count > 0)
            {
                for (int i = 0; i < (view as Body).objectInAttackRange.Count; i++)
                {
                    if ((view as Human).objectInAttackRange[i].gameObject.activeInHierarchy)
                        (view as Human).objectInAttackRange[i].GetHit(1);
                }
            }

        }

        public override void OnTriggerEnter(Collider collider)
        {

        }
        private void MoveToTarget()
        {
            var position = PlayerInputManager.Instance.RaycastPosition;
            if (_lastCommand == "Attack")
            {
                position = PlayerInputManager.Instance.tagHit.position;
            }
            (view as Body).DoAction("Move", position, _target, "RUN", 1f);
            _worldController.SetTargetObjectPosition(position);
            _isAttacking = false;
        }
        public override void UpdateFrame()
        {
            base.UpdateFrame();
            UpdateLastCommand();
            CheckPosition();
        }
        private void UpdateLastCommand()
        {
            if ((view as Human).isStopMoving)
            {
                switch (_lastCommand)
                {
                    case "":
                    case "MoveToTarget":
                        (view as Body).DoAction("PlayAnimation", "Idle", 1f);
                        break;
                    case "Attack":
                    case "FinishAttack":
                        Attack();
                        break;
                    case "InteractionAble":
                        if ((view as Body).objectInAttackRange.Contains(_target) || (view as Body).objectNotInAttackRange.Contains(_target))
                        {
                            view.transform.LookAt(_target.transform);
                            _target.GetComponent<IInteractionAble>()?.Interaction(soul);
                            _lastCommand = "";
                        }
                    (view as Body).DoAction("PlayAnimation", "Idle", 1f);
                        break;
                }
            }
            else
            {
                switch (_lastCommand)
                {
                    case "Attack":
                        if ((view as Human).objectInAttackRange.Contains(_target))
                        {
                            Attack();
                        }
                        break;
                }
            }
        }
        private void CheckPosition()
        {
            var positionNow = (view.transform.position / 50).ToVector3Int();
            if (_currentPositionOnMap != positionNow)
            {
                _worldController.LoadAreasSurroundPosition(positionNow);
                _currentPositionOnMap = positionNow;
            }
        }
        private void Attack()
        {
            if (_target == null)
            {
                _target = PlayerInputManager.Instance.tagHit.GetComponent<GObject>();
            }

            if (_target != null && _target.gameObject.activeInHierarchy)
            {
                if ((view as Human).objectInAttackRange.Contains(_target) || (view as Body).objectNotInAttackRange.Contains(_target))
                {
                    view.transform.LookAt(_target.transform);
                    (view as Human).DoAction("PlayAnimation", "Unarmed-Attack-L1", 1f);
                }
                else
                {
                    ControllerDoAction("MoveToTarget");
                }
                _lastCommand = "Attack";
            }
            else
            {
                _lastCommand = "";
            }
        }
        private void OnMouseRightClick()
        {
            _target = PlayerInputManager.Instance.tagHit?.GetComponent<GObject>();
            if (_target != null)
            {
                if (_target.status != null)
                {
                    for (int i = 0; i < _target.status.Count; i++)
                    {
                        switch (_target.status[i])
                        {
                            case "InteractionAble":
                                _lastCommand = "InteractionAble";
                                if ((view as Body).objectInAttackRange.Contains(_target) || (view as Body).objectNotInAttackRange.Contains(_target))
                                {
                                    view.transform.LookAt(_target.transform);
                                    _target.GetComponent<IInteractionAble>()?.Interaction(soul);
                                    _lastCommand = "";
                                }
                                else
                                {
                                    MoveToTarget();

                                }
                                break;
                            case "Enemy":
                                Attack();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            else
            {
                ControllerDoAction("MoveToTarget");
            }
        }

    }
}
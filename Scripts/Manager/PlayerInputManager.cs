namespace com.wao.rpgs
{
    using UnityEngine;

    public class PlayerInputManager : MonobehaviourSingleton<PlayerInputManager>
    {
        public delegate void OnPlayerInputTrigger();
        public event OnPlayerInputTrigger onMouseLeftClick;
        public event OnPlayerInputTrigger onMouseRightClick;

        public event OnPlayerInputTrigger qKey,wkey,ekey,rkey;

        public Transform tagHit;
        private void Update()
        {
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasReleasedThisFrame)
            {
                onMouseLeftClick?.Invoke();
            }
            if (UnityEngine.InputSystem.Mouse.current.rightButton.wasReleasedThisFrame)
            {
                onMouseRightClick?.Invoke();
            }

            if (UnityEngine.InputSystem.Keyboard.current.qKey.wasReleasedThisFrame)
            {
                qKey?.Invoke();
            }

            if (UnityEngine.InputSystem.Keyboard.current.wKey.wasReleasedThisFrame)
            {
                wkey?.Invoke();
            }
            if (UnityEngine.InputSystem.Keyboard.current.eKey.wasReleasedThisFrame)
            {
                ekey?.Invoke();
            }
            if (UnityEngine.InputSystem.Keyboard.current.rKey.wasReleasedThisFrame)
            {
                rkey?.Invoke();
            }
            lastRaycastHit = RaycastPosition;
        }
        private Vector3 lastRaycastHit;
        public Vector3 RaycastPosition
        {
            get
            {
                RaycastHit hit;
                if (Camera.main != null)
                {
                    var ray = Camera.main.ScreenPointToRay(UnityEngine.InputSystem.Mouse.current.position.ReadValue());

                    if (Physics.Raycast(ray, out hit))
                    {
                        tagHit = hit.transform;
                        return hit.point;
                    }
                }
                return lastRaycastHit;
            }
        }
    }

}
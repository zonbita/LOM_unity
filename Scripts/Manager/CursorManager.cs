namespace com.wao.rpgs
{
    using UnityEngine;
    public class CursorManager : MonobehaviourSingleton<CursorManager>
    {

        [SerializeField]
        private RectTransform _cursor;
        [SerializeField]
        private RectTransform _parent;
        [SerializeField]
        private Canvas _canvas;

        public Vector2 ScreenToRectPos(Vector2 screen_pos)
        {
            if (_canvas.renderMode != RenderMode.ScreenSpaceOverlay && _canvas.worldCamera != null)
            {
                //Canvas is in Camera mode
                Vector2 anchorPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_cursor, screen_pos, _canvas.worldCamera, out anchorPos);
                return anchorPos;
            }
            else
            {
                Vector2 anchorPos = screen_pos - new Vector2(_parent.position.x, _parent.position.y);
                anchorPos = new Vector2(anchorPos.x / _parent.lossyScale.x, anchorPos.y / _parent.lossyScale.y);
                return anchorPos;
            }
        }
        public Vector3 GetWorldPosition()
        {
            if (Camera.main != null)
            {
                return Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            return Vector3.zero;
        }
        public float GetDistanceOfMouse()
        {
            if (Camera.main != null)
            {
                var center = Camera.main.ScreenToWorldPoint(new Vector2(0.5f * Screen.width, 0.5f * Screen.height));
                return Vector3.Distance(center, GetWorldPosition());
            }
            return 0;
        }
#if UNITY_STANDALONE
        private void Update()
        {
            if (PlayerInputManager.Instance.tagHit != null)
                switch (PlayerInputManager.Instance.tagHit.tag)
                {
                    case "Enemy":
                        _cursor.gameObject.SetActive(true);
                        Cursor.visible = false;
                        break;
                    default:
                        _cursor.gameObject.SetActive(false);
                        Cursor.visible = true;
                        break;
                }
            if (Camera.main != null)
            {
                SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
#endif
        public void SetPosition(Vector3 position)
        {
            _cursor.anchoredPosition = com.wao.Utility.Utility.WorldToAnchorPosition(_canvas.GetComponent<RectTransform>(), position, Camera.main);
        }
    }
}

namespace com.wao.rpgs
{
    using com.wao.core;
    using UnityEngine;

    public class Chest : GObject
    {
        [SerializeField]
        private GameObject _open, _close, _coin;
        public int hp;
        public override void Interaction(Soul soul)
        {
            base.Interaction(soul);
            if (_close.activeInHierarchy)
            {
                _close.SetActive(false);
                _open.SetActive(true);
            }
            else
            {
                _coin.SetActive(false);
            }
        }
        public override void GetHit(int damage)
        {
            base.GetHit(damage);
            hp -= damage;
            if (hp <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
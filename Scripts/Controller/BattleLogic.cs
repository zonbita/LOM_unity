namespace com.wao.rpgs
{
    using com.wao.core;
    using UnityEngine;

    public static class BattleLogic
    {
        public static void GetDamage(ref this Soul soul, int damagePoint)
        {
            soul.hp = (int)Mathf.Clamp(soul.hp - damagePoint, 0, soul.maxHp);
        }
    }
}
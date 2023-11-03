using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HitBoxWithDamage : HitBox
    {
        [SerializeField] protected float _damage = 1f;

        public override void DeliverHit(Collider collider)
        {
            HurtBox hurtBox = collider.GetComponent<HurtBox>();
            hurtBox?.NotifyHit(_damage, transform);
            OnDeliveredHit?.Invoke();
        }
    }
}
using HackingOps.CombatSystem.HitHurtBox;
using UnityEngine;

namespace HackingOps.Weapons.Projectiles
{
    public class Projectile : HitBoxWithDamage
    {
        [SerializeField] private bool _destroyOnCollision = true;

        private Transform _origin;

        private void OnEnable()
        {
            _origin = transform;
        }

        public override void DeliverHit(Collider collider)
        {
            if (collider.TryGetComponent(out HurtBox hurtBox))
                hurtBox?.NotifyHit(_damage, _origin);

            if (_destroyOnCollision)
                Destroy(gameObject);
        }

        public void SetOriginTransform(Transform origin)
        {
            _origin = origin;
        }
    }
}
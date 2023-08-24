using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HurtBoxWithLife : HurtBox
    {
        public UnityEvent<float, float> OnNotifyHitWithLife;

        [SerializeField] private float _maxLife = 1f;

        private float _currentLife;

        private void Start()
        {
            _currentLife = _maxLife;
        }

        public override void NotifyHit(float damage = 1f)
        {
            _currentLife -= damage;
            base.NotifyHit(damage);

            OnNotifyHitWithLife?.Invoke(_currentLife, _maxLife);
        }
    }
}
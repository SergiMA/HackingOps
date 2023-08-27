using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HurtBoxWithLife : HurtBox
    {
        public UnityEvent<float, float> OnNotifyHitWithLife;

        [SerializeField] private float _maxLife = 1f;
        [SerializeField] private float _healthRegeneration = 1f;
        [SerializeField] private float _healthRegenerationCooldown = 5f;

        private float _currentLife;
        private float _currentHealthRegenerationCooldown;

        private bool _isDead = false;

        private void Start()
        {
            _currentLife = _maxLife;
        }

        private void Update()
        {
            if (_isDead) return;

            UpdateHealthRegenerationCooldown();

            if (_currentHealthRegenerationCooldown <= 0)
                RecoverHealthOverTime();
        }

        private void UpdateHealthRegenerationCooldown()
        {
            if (_currentHealthRegenerationCooldown > 0f)
            {
                _currentHealthRegenerationCooldown -= Time.deltaTime;

                if (_currentHealthRegenerationCooldown < 0f)
                    _currentHealthRegenerationCooldown = 0f;
            }
        }

        private void RecoverHealthOverTime()
        {
            _currentLife += _healthRegeneration * Time.deltaTime;
            _currentLife = Mathf.Min(_currentLife, _maxLife);
        }

        public override void NotifyHit(float damage = 1f)
        {
            _currentHealthRegenerationCooldown = _healthRegenerationCooldown;

            _currentLife -= damage;
            base.NotifyHit(damage);

            if (_currentLife <= 0)
                _isDead = true;

            OnNotifyHitWithLife?.Invoke(_currentLife, _maxLife);
        }
    }
}
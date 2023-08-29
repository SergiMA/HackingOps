using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HurtBoxWithLife : HurtBox
    {
        public UnityEvent<float, float> OnNotifyHitWithLife;
        public UnityEvent<float, float, Vector3> OnNotifyHitWithLifeAndDirection;

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

        public override void NotifyHit(float damage = 1f, Transform byWhom = null)
        {
            _currentHealthRegenerationCooldown = _healthRegenerationCooldown;

            _currentLife -= damage;
            base.NotifyHit(damage, byWhom);

            if (byWhom != null)
            {
                OnNotifyHitWithLifeAndDirection.Invoke(_currentLife, _maxLife, (transform.position - byWhom.position).normalized);
            }
            else
            {
                OnNotifyHitWithLifeAndDirection.Invoke(_currentLife, _maxLife, Vector3.zero);
            }

            if (_currentLife <= 0)
                _isDead = true;

            OnNotifyHitWithLife?.Invoke(_currentLife, _maxLife);
        }
    }
}
using HackingOps.Characters.Common;
using HackingOps.Characters.Common.CombatSystem;
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

        [Tooltip("Optional. Used to notify about a hit to the Block Controller if it's not null")]
        [SerializeField] BlockController _blockController;

        bool _isBlocking;

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

        public override void NotifyHit(float damage = 1f, Transform damageDealerTransform = null, CharacterCombat damageDealer = null)
        {
            if (damageDealerTransform != null)
            {
                Vector3 directionToDamageDealer = Vector3.Normalize(damageDealerTransform.transform.position - transform.position);
                float dot = Vector3.Dot(transform.forward, directionToDamageDealer);

                if (_isBlocking && dot > 0f && _blockController != null)
                {
                    _blockController.OnHitReceived(damageDealerTransform, damageDealer);
                    return;
                }
            }
            _currentHealthRegenerationCooldown = _healthRegenerationCooldown;

            _currentLife -= damage;
            base.NotifyHit(damage, damageDealerTransform);

            if (damageDealerTransform != null)
            {
                OnNotifyHitWithLifeAndDirection.Invoke(_currentLife, _maxLife, (transform.position - damageDealerTransform.position).normalized);
            }
            else
            {
                OnNotifyHitWithLifeAndDirection.Invoke(_currentLife, _maxLife, Vector3.zero);
            }

            if (_currentLife <= 0)
                _isDead = true;

            OnNotifyHitWithLife?.Invoke(_currentLife, _maxLife);
        }

        public void OnBlockStarted() => _isBlocking = true;
        public void OnBlockFinished() => _isBlocking = false;
    }
}
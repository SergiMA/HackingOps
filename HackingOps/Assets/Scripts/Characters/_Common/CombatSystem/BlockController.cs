using DG.Tweening;
using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Characters.Common.CombatSystem
{
    public class BlockController : MonoBehaviour
    {
        public UnityEvent OnHitBlocked;
        public UnityEvent OnPerfectParryPerformed;
        public UnityEvent<CharacterCombat> OnPerfectParryPerformedWithTarget;

        [SerializeField] private Collider[] _blockColliders;
        [SerializeField] private float _perfectParryDuration = 0.5f;

        private float _currentPerfectParryDuration;

        private void EnableColliders()
        {
            foreach (Collider c in _blockColliders)
            {
                c.enabled = true;
            }
        }

        private void DisableColliders()
        {
            foreach (Collider c in _blockColliders)
            {
                c.enabled = false;
            }
        }

        private void StartPerfectParryCountdown()
        {
            _currentPerfectParryDuration = _perfectParryDuration;

            float from = _perfectParryDuration;
            float to = 0f;
            float duration = _perfectParryDuration;
            DOVirtual.Float(from, to, duration, timer =>
            {
                _currentPerfectParryDuration = timer;
            });
        }

        public void OnStartBlocking()
        {
            StartPerfectParryCountdown();
            EnableColliders();
        }

        public void OnStopBlocking()
        {
            DisableColliders();
        }

        public void OnHitReceived(Transform damageDealerTransform, CharacterCombat damageDealer = null)
        {
            if (IsPerfectParryAvailable())
            {
                OnPerfectParryPerformed.Invoke();
                damageDealer?.OnPerfectParryReceived();
            }
            else
                OnHitBlocked.Invoke();
        }

        /// <summary>
        /// Get the current state of the perfect parry.
        /// </summary>
        /// <returns>Returns true if the perfect parry is still active. Returns false if the perfect parry has expired</returns>
        public bool IsPerfectParryAvailable() => _currentPerfectParryDuration > 0f;
    }
}
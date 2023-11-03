using System.Collections;
using UnityEngine;

namespace HackingOps.Weapons.Barrels.BarrelsForProjectiles
{
    public class BarrelByBurstInstantiation : BarrelByInstantiation
    {
        [Header("Burst settings")]

        [Tooltip("Rounds (bullets) to fire in a burst")]
        [SerializeField] private int _roundsAmount = 3;

        [Tooltip("Delay between one burst (succession of fired rounds) and the next one")]
        [SerializeField] private float _delayBetweenBursts = 1f;

        bool _isShooting;
        bool _isBlocked;

        IEnumerator FireBurst(int roundsAmount,  float cadence)
        {
            _isShooting = true;
            float delayBetweenRounds = 1f / cadence;

            for(int i = 0; i < roundsAmount; i++)
            {
                base.InternalShot();
                yield return new WaitForSeconds(delayBetweenRounds);
            }

            yield return new WaitForSeconds(_delayBetweenBursts);
            _isShooting = false;

            if (_isUsedByAI && !_isBlocked) InternalStartShooting();
        }

        #region Barrel implementation
        protected override void InternalStartShooting()
        {
            _isBlocked = false;
            if (!_isShooting)
                StartCoroutine(FireBurst(_roundsAmount, Cadence));
        }

        protected override void InternalStopShooting()
        {
            _isBlocked = false;
        }

        public override void ResetBarrel() => _isShooting = false;
        #endregion
    }
}
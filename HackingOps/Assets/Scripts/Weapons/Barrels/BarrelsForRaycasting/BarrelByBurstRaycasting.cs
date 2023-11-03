using System.Collections;
using UnityEngine;

namespace HackingOps.Weapons.Barrels
{
    public class BarrelByBurstRaycasting : BarrelByRaycasting
    {
        [Header("Burst settings")]

        [Tooltip("Rounds (bullets) to fire in a burst")]
        [SerializeField] int _roundsAmount = 3;

        [Tooltip("Delay between one burst (succession of fired rounds) and the next one")]
        [SerializeField] float _delayBetweenBursts = 1f;

        bool _isShooting;
        bool _isBlocked;

        IEnumerator FireBurst(int roundsAmount, float cadence)
        {
            _isShooting = true;
            float delayBetweenRounds = 1f / cadence;

            for (int i = 0; i < roundsAmount; i++)
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
            {
                StartCoroutine(FireBurst(_roundsAmount, Cadence));
            }
        }

        protected override void InternalStopShooting()
        {
            _isBlocked = true;
            //_isShooting = false;
        }

        public override void ResetBarrel() => _isShooting = false;
        #endregion
    }
}
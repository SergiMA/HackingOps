using UnityEngine;

namespace HackingOps.Weapons.Barrels
{
    public class BarrelByHoldRaycasting : BarrelByRaycasting
    {
        private bool _isShooting;
        private float _shootingTimer;

        private void Update()
        {
            if (_isShooting && Time.time > _shootingTimer)
            {
                ShootContinously();
                _shootingTimer = Time.time + 1f / Cadence;
            }
        }

        void ShootContinously()
        {
            base.InternalShot();
        }

        #region Barrel implementation
        protected override void InternalShot() { }

        protected override void InternalStartShooting() => _isShooting = true;

        protected override void InternalStopShooting() => _isShooting = false;
        #endregion
    }
}
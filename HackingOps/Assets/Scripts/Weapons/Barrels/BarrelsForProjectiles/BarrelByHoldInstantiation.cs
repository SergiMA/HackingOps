using UnityEngine;

namespace HackingOps.Weapons.Barrels.BarrelsForProjectiles
{
    public class BarrelByHoldInstantiation : BarrelByInstantiation
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
        protected override void InternalStopShooting()
        {
            _isShooting = false;
        }
        public override void ResetBarrel()
        {
            base.ResetBarrel();
            _isShooting = false;
        }
        #endregion
    }
}
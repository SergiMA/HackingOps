using System.Collections;
using UnityEngine;

namespace HackingOps.Weapons.Barrels
{
    public class BarrelByBurstRaycasting : BarrelByRaycasting
    {
        [SerializeField] int _burstSize = 3;

        bool _isShooting;

        IEnumerator FireBurst(int burstSize, float cadence)
        {
            _isShooting = true;
            float bulletDelay = 1f / Cadence;

            for (int i = 0; i < burstSize; i++)
            {
                base.InternalShot();
                yield return new WaitForSeconds(bulletDelay);
            }
            _isShooting = false;
        }


        #region Barrel implementation
        protected override void InternalStartShooting()
        {
            if (!_isShooting)
            {
                StartCoroutine(FireBurst(_burstSize, Cadence));
            }
        }
        #endregion
    }
}
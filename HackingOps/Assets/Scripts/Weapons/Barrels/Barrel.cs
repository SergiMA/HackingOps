using HackingOps.Weapons.Common;
using UnityEngine;

namespace HackingOps.Weapons.Barrels
{
    public abstract class Barrel : MonoBehaviour
    {
        [Header("Barrel bindings")]
        [SerializeField] public AmmunitionTypeBase AmmoType;

        [Header("Barrel firing settings")]
        [SerializeField] public float Cadence = 3f;
        [SerializeField] public int MaxAmmo = 12;
        [SerializeField] public int CurrentAmmo = 12;
        [SerializeField] public float RechargeTime = 2.5f;
        [SerializeField] public float EffectiveRange = 15f;

        protected bool _isUsedByAI;

        private float _remainingRechargeTime = 0f;
        private double _lastShotTime = 0f;

        private void Update()
        {
            if (_remainingRechargeTime > 0f)
            {
                _remainingRechargeTime -= Time.deltaTime;
                if (_remainingRechargeTime < 0f )
                {
                    _remainingRechargeTime = 0f;
                    CurrentAmmo = MaxAmmo;
                }
            }
        }

        public void Shot()
        {
            if (IsReady())
            {
                ConsumeAmmo();
                InternalShot();
            }
        }

        public void StartShooting()
        {
            InternalStartShooting();
        }

        public void StopShooting()
        {
            InternalStopShooting();
        }

        protected abstract void InternalShot();
        protected abstract void InternalStartShooting();
        protected abstract void InternalStopShooting();
        public virtual void ResetBarrel() { }

        protected void ConsumeAmmo()
        {
            CurrentAmmo--;
            if (CurrentAmmo == 0)
            {
                _remainingRechargeTime = RechargeTime;
            }
            _lastShotTime = Time.time;
        }

        public bool IsReady()
        {
            return
                (CurrentAmmo > 0) &&
                (Time.time - _lastShotTime) > (1f / Cadence) &&
                ((Time.time - _remainingRechargeTime) > RechargeTime);
        }

        public void IsControlledByAI(bool isUsedByAI)
        {
            _isUsedByAI = isUsedByAI;
        }
    }
}
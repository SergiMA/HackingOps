using HackingOps.Weapons.Barrels;
using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public abstract class FireWeapon : Weapon
    {
        [Header("Aiming")]
        [SerializeField] Transform _rotationPoint;

        [Header("Grab points")]
        [SerializeField] Transform _leftArmHint;
        [SerializeField] Transform _leftArmTarget;
        [SerializeField] Transform _rightArmHint;
        [SerializeField] Transform _rightArmTarget;

        Barrel[] _barrels;

        protected override void InternalAwake()
        {
            _barrels = GetComponentsInChildren<Barrel>();
            {
                foreach (Barrel barrel in _barrels)
                {
                    barrel.IsControlledByAI(IsUsedByAI);
                }
            }
        }

        public override void Use()
        {
            foreach (Barrel barrel in _barrels)
            {
                barrel.Shot();
            }
        }

        public override void StartUsing()
        {
            foreach (Barrel barrel in _barrels)
            {
                barrel.StartShooting();
            }
        }

        public override void StopUsing()
        {
            foreach (Barrel barrel in _barrels)
            {
                barrel.StopShooting();
            }
        }

        public override void ResetWeapon()
        {
            foreach (Barrel barrel in _barrels)
            {
                barrel.ResetBarrel();
            }
        }

        public bool IsShotReady()
        {
            bool anyBarrelIsReady = false;
            for (int i = 0; (i < _barrels.Length) && (!anyBarrelIsReady); i++)
            {
                anyBarrelIsReady = _barrels[i].IsReady();
            }

            return anyBarrelIsReady;
        }

        public override float GetEffectiveRange()
        {
            float effectiveRange = 0f;
            foreach (Barrel b in _barrels)
            {
                effectiveRange = Mathf.Max(effectiveRange, b.EffectiveRange);
            }

            return effectiveRange;
        }

        public override bool HasGrabPoints() => true;
        public override Transform GetLeftArmHint() => _leftArmHint;
        public override Transform GetLeftArmTarget() => _leftArmTarget;
        public override Transform GetRightArmHint() => _rightArmHint;
        public override Transform GetRightArmTarget() => _rightArmTarget;

        public override void AdaptToVerticalAngle(Vector3 aimForward)
        {
            Vector3 aimForwardXZ = aimForward;
            aimForwardXZ.y = 0f;

            Quaternion rotateToAimForward = Quaternion.FromToRotation(transform.forward, aimForwardXZ);
        }

        public override void NotifyAimingAngle(float currentAimingAngle)
        {
            _rotationPoint.localRotation = Quaternion.Euler(currentAimingAngle, 0f, 0f);
        }

        public override Vector3 GetRotationPointPosition() => _rotationPoint.position;
        public Transform GetRotationPoint() => _rotationPoint;

        public override void ResetRotation()
        {
            _rotationPoint.localRotation = Quaternion.identity;
        }
    }
}
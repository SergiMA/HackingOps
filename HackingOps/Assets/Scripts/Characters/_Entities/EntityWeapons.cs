using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace HackingOps.Characters.Entities
{
    public class EntityWeapons : MonoBehaviour
    {
        [Header("Bindings - Weapons")]
        [SerializeField] private Transform _weaponsParent;

        [Header("Bindings - Rig")]
        [SerializeField] private Rig _armsRig;

        [Space]
        [SerializeField] private Transform _leftArmHint;
        [SerializeField] private Transform _leftArmTarget;
        [SerializeField] private Transform _rightArmHint;
        [SerializeField] private Transform _rightArmTarget;

        private Weapon[] _weapons;
        private int _currentWeaponIndex = 0;
        private bool _mustShoot;

        [Header("Debug properties")]
        public bool DebugMustShoot;
        private void OnValidate()
        {
            _mustShoot = DebugMustShoot;
        }

        private void Awake()
        {
            _weapons = _weaponsParent.GetComponentsInChildren<Weapon>();
        }

        private bool _oldMustShoot = false;
        private void Update()
        {
            Weapon currentWeapon = _weapons[_currentWeaponIndex];
            if (currentWeapon is FireWeapon)
            {
                FireWeapon fireWeapon = currentWeapon as FireWeapon;

                _armsRig.weight = fireWeapon.HasGrabPoints() ? 1f : 0f;

                if (fireWeapon.CanUse())
                {
                    if (_mustShoot && fireWeapon.IsShotReady())
                    {
                        fireWeapon.Use();
                    }
                }
                else if (fireWeapon.CanContinuouslyUse())
                {
                    if (_oldMustShoot != _mustShoot)
                    {
                        if (_mustShoot) { fireWeapon.StartUsing(); }
                        else { fireWeapon.StopUsing(); }
                    }
                }
            }

            _oldMustShoot = _mustShoot;
        }

        private void LateUpdate()
        {
            if (_currentWeaponIndex == -1)
                return;

            Weapon weapon = _weapons[_currentWeaponIndex];
            if (weapon.HasGrabPoints())
            {
                _leftArmHint.transform.position = weapon.GetLeftArmHint().position;
                _leftArmHint.transform.rotation = weapon.GetLeftArmHint().rotation;

                _leftArmTarget.transform.position = weapon.GetLeftArmTarget().position;
                _leftArmTarget.transform.rotation = weapon.GetLeftArmTarget().rotation;

                _rightArmHint.transform.position = weapon.GetRightArmHint().position;
                _rightArmHint.transform.rotation = weapon.GetRightArmHint().rotation;

                _rightArmTarget.transform.position = weapon.GetRightArmTarget().position;
                _rightArmTarget.transform.rotation = weapon.GetRightArmTarget().rotation;
            }
        }

        public void MustShoot() => _mustShoot = true;
        public void MustNotShoot() => _mustShoot = false;

        public float GetEffectiveRange()
        {
            float effectiveRange = 0f;
            Weapon currentWeapon = _weapons[_currentWeaponIndex];

            if (currentWeapon)
            {
                effectiveRange = currentWeapon.GetEffectiveRange();
            }

            return effectiveRange;
        }
    }
}

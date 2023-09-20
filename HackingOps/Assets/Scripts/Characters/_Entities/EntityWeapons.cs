using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

namespace HackingOps.Characters.Entities
{
    [DefaultExecutionOrder(-1)]
    public class EntityWeapons : MonoBehaviour
    {
        [Header("Bindings - Weapons")]
        [SerializeField] private Transform _weaponsParent;
        [SerializeField] private Inventory _inventory;

        [Header("Bindings - Rig")]
        [SerializeField] private Rig _armsRig;

        [Space]
        [SerializeField] private Transform _leftArmHint;
        [SerializeField] private Transform _leftArmTarget;
        [SerializeField] private Transform _rightArmHint;
        [SerializeField] private Transform _rightArmTarget;

        [Space]
        [SerializeField] private Transform _rightHandBone;
        [SerializeField] private Transform _firearmHolder;
        [SerializeField] private Transform _largeFirearmHolster;
        [SerializeField] private Transform _mediumFirearmHolster;
        [SerializeField] private Transform _smallFirearmHolster;
        [SerializeField] private Transform _meleeWeaponHolster;

        [Header("Settings")]
        [SerializeField] private bool _isUsedByAI = false;

        // Weapon properties
        private Weapon[] _weapons;
        private int _currentWeaponIndex = 0;
        private Weapon _currentWeapon;

        private bool _mustShoot;

        // Rigging properties (holder and holster)
        private ConstraintSource _currentHolderConstraint = new();
        private ConstraintSource _currentHolsterConstraint = new();

        [Header("Debug properties")]
        public bool DebugMustShoot;
        private void OnValidate()
        {
            _mustShoot = DebugMustShoot;
        }

        private void Awake()
        {
            _weapons = _weaponsParent.GetComponentsInChildren<Weapon>();

            foreach (Weapon weapon in _weapons)
            {
                weapon.IsUsedByAI = _isUsedByAI;
            }
        }

        private void OnEnable()
        {
            _inventory.OnWeaponAdded += OnWeaponAdded;
            _inventory.OnWeaponSwitched += OnWeaponSwitched;
            _inventory.OnWeaponDropped += OnWeaponDropped;
        }

        private void OnDisable()
        {
            _inventory.OnWeaponSwitched -= OnWeaponSwitched;
            _inventory.OnWeaponSwitched -= OnWeaponSwitched;
            _inventory.OnWeaponDropped -= OnWeaponDropped;
        }

        private bool _oldMustShoot = false;
        private void Update()
        {
            if (_currentWeapon is FireWeapon)
            {
                FireWeapon fireWeapon = _currentWeapon as FireWeapon;

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
            else
            {
                _armsRig.weight = 0f;
                MeleeWeaponOneUse meleeWeapon = _currentWeapon as MeleeWeaponOneUse;

                if (meleeWeapon.CanUse())
                {
                    if (_mustShoot)
                    {
                        meleeWeapon.Use();
                    }
                }
            }

            _oldMustShoot = _mustShoot;
        }

        private void LateUpdate()
        {
            if (_currentWeapon == null)
                return;

            if (_currentWeapon.HasGrabPoints())
            {
                _leftArmHint.transform.position = _currentWeapon.GetLeftArmHint().position;
                _leftArmHint.transform.rotation = _currentWeapon.GetLeftArmHint().rotation;

                _leftArmTarget.transform.position = _currentWeapon.GetLeftArmTarget().position;
                _leftArmTarget.transform.rotation = _currentWeapon.GetLeftArmTarget().rotation;

                _rightArmHint.transform.position = _currentWeapon.GetRightArmHint().position;
                _rightArmHint.transform.rotation = _currentWeapon.GetRightArmHint().rotation;

                _rightArmTarget.transform.position = _currentWeapon.GetRightArmTarget().position;
                _rightArmTarget.transform.rotation = _currentWeapon.GetRightArmTarget().rotation;
            }
        }

        private void OnWeaponAdded(Weapon weapon)
        {
            if (weapon.TryGetComponent(out ParentConstraint parentConstraint))
            {
                if (weapon.Slot != WeaponSlot.MeleeWeapon)
                {
                    _currentHolderConstraint.sourceTransform = _firearmHolder;
                }
                else
                {
                    _currentHolderConstraint.sourceTransform = _rightHandBone;
                }
                _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(weapon);

                if (_inventory.GetCurrentSlot().Slot == weapon.Slot ||
                    _inventory.GetCurrentSlot().Slot == WeaponSlot.Unarmed)
                {
                    _currentHolderConstraint.weight = 1f;
                    _currentHolsterConstraint.weight = 0f;
                }
                else
                {
                    _currentHolderConstraint.weight = 0f;
                    _currentHolsterConstraint.weight = 1f;
                }

                parentConstraint.SetTranslationOffset(0, Vector3.zero);
                parentConstraint.SetRotationOffset(0, Vector3.zero);

                parentConstraint.SetTranslationOffset(1, Vector3.zero);
                parentConstraint.SetRotationOffset(1, Vector3.zero);

                parentConstraint.SetSource(0, _currentHolderConstraint);
                parentConstraint.constraintActive = true;

                parentConstraint.SetSource(1, _currentHolsterConstraint);
                parentConstraint.constraintActive = true;
            }
        }

        private void OnWeaponSwitched(Weapon oldWeapon, Weapon newWeapon)
        {
            if (oldWeapon != null)
            {
                if (oldWeapon.TryGetComponent(out ParentConstraint parentConstraint))
                {
                    SetOldWeaponConstraints(oldWeapon, parentConstraint);
                }
            }

            if (newWeapon != null)
            {
                if (newWeapon.TryGetComponent(out ParentConstraint parentConstraint))
                {
                    SetNewWeaponConstraints(newWeapon, parentConstraint);

                    if (newWeapon.Slot != WeaponSlot.Unarmed)
                        _armsRig.weight = newWeapon.HasGrabPoints() ? 1f : 0f;
                    else
                        _armsRig.weight = 0f;
                }
            }
            else
                _armsRig.weight = 0f;

            _currentWeapon = newWeapon;
        }

        private void OnWeaponDropped(Weapon weapon)
        {
            if (weapon.TryGetComponent(out ParentConstraint parentConstraint))
            {
                _currentHolderConstraint.sourceTransform = null;
                _currentHolderConstraint.weight = 0f;

                _currentHolsterConstraint.sourceTransform = null;
                _currentHolsterConstraint.weight = 0f;

                parentConstraint.SetSource(0, _currentHolderConstraint);
                parentConstraint.SetSource(1, _currentHolsterConstraint);

                parentConstraint.constraintActive = false;
            }
        }

        private void SetOldWeaponConstraints(Weapon weapon, ParentConstraint parentConstraint)
        {
            if (weapon.Slot != WeaponSlot.MeleeWeapon)
                _currentHolderConstraint.sourceTransform = _firearmHolder;
            else
                _currentHolderConstraint.sourceTransform = _rightHandBone;

            _currentHolderConstraint.weight = 0f;

            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(weapon);
            _currentHolsterConstraint.weight = 1f;

            parentConstraint.SetSource(0, _currentHolderConstraint);
            parentConstraint.SetSource(1, _currentHolsterConstraint);
        }
        
        private void SetNewWeaponConstraints(Weapon weapon, ParentConstraint parentConstraint)
        {
            if (weapon.Slot != WeaponSlot.MeleeWeapon)
                _currentHolderConstraint.sourceTransform = _firearmHolder;
            else
                _currentHolderConstraint.sourceTransform = _rightHandBone;

            _currentHolderConstraint.weight = 1f;

            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(weapon);
            _currentHolsterConstraint.weight = 0f;

            parentConstraint.SetSource(0, _currentHolderConstraint);
            parentConstraint.SetSource(1, _currentHolsterConstraint);
        }

        private Transform GetHolsterSourceTransform(Weapon weapon)
        {
            switch (weapon.Slot)
            {
                case WeaponSlot.LargeFirearm: return _largeFirearmHolster;
                case WeaponSlot.MediumFirearm: return _mediumFirearmHolster;
                case WeaponSlot.SmallFirearm: return _smallFirearmHolster;
                case WeaponSlot.MeleeWeapon: return _meleeWeaponHolster;
                default: return _meleeWeaponHolster;
            }
        }

        public void MustShoot() => _mustShoot = true;
        public void MustNotShoot() => _mustShoot = false;

        public float GetEffectiveRange()
        {
            float effectiveRange = 0f;

            if (_currentWeapon)
            {
                effectiveRange = _currentWeapon.GetEffectiveRange();
            }

            return effectiveRange;
        }
    }
}

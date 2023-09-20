using HackingOps.Characters.NPC.Senses;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HackingOps.Characters.Player
{
    public class PlayerWeapons : MonoBehaviour
    {
        public UnityEvent<Weapon> OnWeaponSelected;

        [Header("Bindings - Weapon")]
        [SerializeField] private Transform _weaponsParent;
        [SerializeField] private Inventory _inventory;

        [Header("Bindings - Rig")]
        [SerializeField] private Rig armsRig;

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

        [Header("Bindings - Aiming")]
        [SerializeField] private Transform _sightPoint;

        [Header("Settings - Aiming")]
        [SerializeField] private LayerMask _targetLayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private float _aimingAngularVelocity = 360f;

        // Weapons properties
        public Weapon[] _weapons { get; private set; }
        public int _currentWeaponIndex { get; private set; } = -1;

        private Weapon _currentWeapon;

        // Aiming properties
        private Transform _aimingTarget;
        private IVisible _aimingTargetVisible;
        private float _currentAimingAngle;
        private float _desiredAimingAngle;

        // Rigging properties (holder and holster)
        private ConstraintSource _currentHolderConstraint = new();
        private ConstraintSource _currentHolsterConstraint = new();

        private void Awake()
        {
            _weapons = _weaponsParent.GetComponentsInChildren<Weapon>();
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

        private void Update()
        {
            Vector3 aimPosition;
            bool hasAimingPosition = CalcBestAimingPosition(_currentWeapon, out aimPosition);

            if (hasAimingPosition)
            {
                Vector3 rotationPosition = _currentWeapon.GetRotationPointPosition();
                Vector3 direction = aimPosition - rotationPosition;
                _desiredAimingAngle = CalcAimingAngle(direction);
            }
            else
            {
                _desiredAimingAngle = CalcAimingAngle(Camera.main.transform.forward);
            }

            float angleDifference = _desiredAimingAngle - _currentAimingAngle;
            float angleToApply =
                Mathf.Sign(angleDifference) *
                Mathf.Min(
                    Mathf.Abs(angleDifference),
                    _aimingAngularVelocity * Time.deltaTime
                    );
            _currentAimingAngle += angleToApply;

            if (_currentWeapon != null && _currentWeapon.Slot != WeaponSlot.Unarmed)
                _currentWeapon.NotifyAimingAngle(_currentAimingAngle);
        }

        private void LateUpdate()
        {
            if (_currentWeapon == null)
            {
                return;
            }

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

        private float CalcAimingAngle(Vector3 direction)
        {
            Vector3 directionXZ = direction;
            directionXZ.y = 0f;

            return Mathf.Atan2(-direction.y, directionXZ.magnitude) * Mathf.Rad2Deg;
        }

        private bool CalcBestAimingPosition(Weapon currentWeapon, out Vector3 bestPosition)
        {
            bool hasLineOfSight = false;

            bestPosition = Vector3.zero;

            if (currentWeapon && currentWeapon is FireWeapon)
            {
                FireWeapon fireWeapon = currentWeapon as FireWeapon;

                Vector3[] checkpoints;

                if (_aimingTargetVisible != null) checkpoints = _aimingTargetVisible.GetCheckpoints();
                else if (_aimingTarget) checkpoints = new Vector3[1] { _aimingTarget.position };
                else checkpoints = new Vector3[0];

                for (int i = 0; !hasLineOfSight && (i < checkpoints.Length); i++)
                {
                    if (Physics.Linecast(
                        _sightPoint.position,
                        checkpoints[i],
                        out RaycastHit hit,
                        _targetLayerMask))
                    {
                        if (hit.collider.GetComponent<IVisible>() == _aimingTargetVisible)
                        {
                            hasLineOfSight = true;
                            bestPosition = checkpoints[i];
                        }
                    }
                }

            }

            return hasLineOfSight;
        }

        private void SelectWeapon(int newWeaponIndex)
        {
            if (newWeaponIndex < _weapons.Length)
            {
                _currentWeaponIndex = newWeaponIndex;

                if (_currentWeaponIndex != -1)
                {
                    armsRig.weight = _weapons[_currentWeaponIndex].HasGrabPoints() ? 1f : 0f;
                }
                else { armsRig.weight = 0f; }
            }

            OnWeaponSelected.Invoke(_currentWeaponIndex == -1 ? null : _weapons[_currentWeaponIndex]);
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
                        armsRig.weight = newWeapon.HasGrabPoints() ? 1f : 0f;
                    else
                        armsRig.weight = 0f;
                }
            }
            else
            {
                armsRig.weight = 0f;
            }

            _currentWeapon = newWeapon;
        }

        private void SetOldWeaponConstraints(Weapon oldWeapon, ParentConstraint parentConstraint)
        {
            if (oldWeapon.Slot != WeaponSlot.MeleeWeapon)
            {
                _currentHolderConstraint.sourceTransform = _firearmHolder;
            }
            else
            {
                _currentHolderConstraint.sourceTransform = _rightHandBone;
            }
            _currentHolderConstraint.weight = 0f;

            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(oldWeapon);
            _currentHolsterConstraint.weight = 1f;

            parentConstraint.SetSource(0, _currentHolderConstraint);
            parentConstraint.SetSource(1, _currentHolsterConstraint);
        }

        private void SetNewWeaponConstraints(Weapon newWeapon, ParentConstraint parentConstraint)
        {
            if (newWeapon.Slot != WeaponSlot.MeleeWeapon)
            {
                _currentHolderConstraint.sourceTransform = _firearmHolder;
            }
            else
            {
                _currentHolderConstraint.sourceTransform = _rightHandBone;
            }
            _currentHolderConstraint.weight = 1f;

            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(newWeapon);
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

        float _oldAnalogValue = 0f;
        public void OnShoot(float analogValue)
        {
            if (_currentWeapon && _currentWeapon is FireWeapon)
            {
                float newAnalogValue = analogValue;
                if (_currentWeapon.CanUse())
                {
                    if ((_oldAnalogValue <= 0f) && (newAnalogValue > 0f)) { _currentWeapon?.Use(); }
                }
                else if (_currentWeapon.CanContinuouslyUse())
                {
                    if ((_oldAnalogValue <= 0f) && (newAnalogValue > 0f)) { _currentWeapon?.StartUsing(); }
                    else if ((_oldAnalogValue > 0f) && (newAnalogValue <= 0f)) { _currentWeapon?.StopUsing(); }
                }
                _oldAnalogValue = newAnalogValue;
            }
        }
        public void ChangeToPreviousWeapon()
        {
            _inventory.ChangeToPreviousWeapon();
        }

        public void ChangeToNextWeapon()
        {
            _inventory.ChangeToNextWeapon();
        }

        public void OnSelectWeaponReceived(int weaponSelectedIndex) => SelectWeapon(weaponSelectedIndex);

        public void SetAimingTarget(Transform target)
        {
            _aimingTarget = target;
            if (_aimingTarget != null)
                _aimingTargetVisible = _aimingTarget.GetComponent<IVisible>();
            else
                _aimingTargetVisible = null;
        }
    }
}
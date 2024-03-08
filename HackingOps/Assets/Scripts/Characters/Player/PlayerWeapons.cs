using HackingOps.Characters.NPC.Senses.SightSense;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

namespace HackingOps.Characters.Player
{
    public class PlayerWeapons : MonoBehaviour, IEventObserver
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
        private float _currentAimingAngleY = 0f;
        private float _currentAimingAngleX = 0f;
        private float _desiredAimingAngleY = 0f;
        private float _desiredAimingAngleX = 0f;
        Vector3 _desiredAimingPosition = Vector3.one * Mathf.Infinity;

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

            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CutsceneStarted, this);
        }

        private void OnDisable()
        {
            _inventory.OnWeaponSwitched -= OnWeaponSwitched;
            _inventory.OnWeaponSwitched -= OnWeaponSwitched;
            _inventory.OnWeaponDropped -= OnWeaponDropped;

            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.CutsceneStarted, this);
        }

        private void Update()
        {
            bool hasAimingPosition = CalcBestAimingPosition(_currentWeapon, out Vector3 aimPosition);
            Vector3 rotationPosition = _currentWeapon ? _currentWeapon.GetRotationPointPosition() : Vector3.zero;

            if (hasAimingPosition)
            {
                CalcDesiredAimingAngles(aimPosition, rotationPosition, out _desiredAimingAngleY, out _desiredAimingAngleX);
            }
            else if (_desiredAimingPosition.x == Mathf.Infinity)
            {
                _desiredAimingAngleY = CalcAimingAngleVertical(Camera.main.transform.forward);
                _desiredAimingAngleX = 0;
            }
            else
            {
                CalcDesiredAimingAngles(_desiredAimingPosition, rotationPosition, out _desiredAimingAngleY, out _desiredAimingAngleX);
            }

            RotateAngleTo(ref _currentAimingAngleY, _desiredAimingAngleY, _aimingAngularVelocity);
            RotateAngleTo(ref _currentAimingAngleX, _desiredAimingAngleX, _aimingAngularVelocity);

            if (_currentWeapon != null)
                _currentWeapon.NotifyAimingAngles(_currentAimingAngleY, _currentAimingAngleX);
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

        internal void SetAimingPosition(Vector3 position) => _desiredAimingPosition = position;
        internal void UnsetAimingPosition() => _desiredAimingPosition = Vector3.one * Mathf.Infinity;

        private bool CalcBestAimingPosition(Weapon currentWeapon, out Vector3 bestPosition)
        {
            bool hasLineOfSight = false;

            bestPosition = Vector3.zero;

            if (currentWeapon && currentWeapon is FireWeapon)
            {
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

        private void CalcDesiredAimingAngles(Vector3 aimPosition, Vector3 rotationPosition, out float desiredAimingAngleV, out float desiredAimingAngleH)
        {
            Vector3 direction = aimPosition - rotationPosition;
            desiredAimingAngleV = CalcAimingAngleVertical(direction);
            desiredAimingAngleH = CalcAimingAngleHorizontal(direction);
        }

        private float CalcAimingAngleHorizontal(Vector3 direction)
        {
            Vector3 directionXZ = direction;
            directionXZ.y = 0f;
            Vector3 forwardXZ = transform.forward;
            forwardXZ.y = 0f;

            return Vector3.SignedAngle(forwardXZ, directionXZ, Vector3.up);
        }

        private float CalcAimingAngleVertical(Vector3 direction)
        {
            Vector3 directionXZ = direction;
            directionXZ.y = 0f;

            return Mathf.Atan2(-direction.y, directionXZ.magnitude) * Mathf.Rad2Deg;
        }

        private void RotateAngleTo(ref float current, float desired, float angularVelocity)
        {
            float angleDifference = desired - current;
            float angleToApply = Mathf.Sign(angleDifference) * Mathf.Min(Mathf.Abs(angleDifference), angularVelocity * Time.deltaTime);
            current += angleToApply;
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
                ConfigureWeaponConstraintsPivots(weapon);
                ConfigureWeaponParentConstraint(parentConstraint, weapon.GetHolderOffset());
            }
        }

        private void ConfigureWeaponConstraintsPivots(Weapon weapon)
        {
            if (weapon.Slot != WeaponSlot.MeleeWeapon)
                _currentHolderConstraint.sourceTransform = _firearmHolder;
            else
                _currentHolderConstraint.sourceTransform = _rightHandBone;

            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(weapon);

            if (_inventory.GetCurrentSlot().Slot == weapon.Slot)
                SetWeightToCurrentHolderConstraint();
            else
                SetWeightToCurrentHolsterConstraint();
        }

        private void ConfigureWeaponParentConstraint(ParentConstraint parentConstraint, Vector3 holderOffset)
        {
            parentConstraint.SetTranslationOffset(0, holderOffset);
            parentConstraint.SetRotationOffset(0, Vector3.zero);

            parentConstraint.SetTranslationOffset(1, Vector3.zero);
            parentConstraint.SetRotationOffset(1, Vector3.zero);

            parentConstraint.SetSource(0, _currentHolderConstraint);
            parentConstraint.constraintActive = true;

            parentConstraint.SetSource(1, _currentHolsterConstraint);
            parentConstraint.constraintActive = true;
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

                    armsRig.weight = newWeapon.HasGrabPoints() ? 1f : 0f;
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

            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(oldWeapon);
            SetWeightToCurrentHolsterConstraint();


            parentConstraint.SetSource(0, _currentHolderConstraint);
            parentConstraint.SetSource(1, _currentHolsterConstraint);
        }

        private void SetNewWeaponConstraints(Weapon newWeapon, ParentConstraint parentConstraint)
        {
            _currentHolsterConstraint.sourceTransform = GetHolsterSourceTransform(newWeapon);

            if (newWeapon.Slot != WeaponSlot.MeleeWeapon)
            {
                _currentHolderConstraint.sourceTransform = _firearmHolder;
                SetWeightToCurrentHolderConstraint();
            }
            else
            {
                _currentHolderConstraint.sourceTransform = _rightHandBone;
                SetWeightToCurrentHolsterConstraint();
            }

            parentConstraint.SetSource(0, _currentHolderConstraint);
            parentConstraint.SetSource(1, _currentHolsterConstraint);

            ConfigureWeaponParentConstraint(parentConstraint, newWeapon.GetHolderOffset());
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

        private void SetWeightToCurrentHolsterConstraint()
        {
            _currentHolsterConstraint.weight = 1;
            _currentHolderConstraint.weight = 0;
        }

        private void SetWeightToCurrentHolderConstraint()
        {
            _currentHolsterConstraint.weight = 0;
            _currentHolderConstraint.weight = 1;
        }

        public void Unsheath()
        {
            if (_currentWeapon && _currentWeapon.TryGetComponent(out ParentConstraint parentConstraint))
            {
                ConfigureWeaponConstraintsPivots(_currentWeapon);
                SetWeightToCurrentHolderConstraint();
                ConfigureWeaponParentConstraint(parentConstraint, _currentWeapon.GetHolderOffset());
                
                ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.WeaponUnsheathed));
            }
        }

        public void Sheathe()
        {
            if (_currentWeapon && _currentWeapon.TryGetComponent(out ParentConstraint parentConstraint))
            {
                ConfigureWeaponConstraintsPivots(_currentWeapon);
                SetWeightToCurrentHolsterConstraint();
                ConfigureWeaponParentConstraint(parentConstraint, _currentWeapon.GetHolderOffset());

                ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.WeaponSheathed));
            }
        }

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.CutsceneStarted)
            {
                _inventory.ChangeToSlot(WeaponSlot.MeleeWeapon);
            }
        }
        #endregion
    }
}
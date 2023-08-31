using HackingOps.Characters.NPC.Senses;
using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HackingOps.Characters.Player
{
    public class CharacterWeaponManager : MonoBehaviour
    {
        public UnityEvent<Weapon> OnWeaponSelected;

        [Header("Bindings - Input")]
        [SerializeField] private Input.PlayerInputManager _inputManager;

        [Header("Bindings - Weapon")]
        [SerializeField] private Transform _weaponsParent;

        [Header("Bindings - Rig")]
        [SerializeField] private Rig armsRig;

        [Space]
        [SerializeField] private Transform _leftArmHint;
        [SerializeField] private Transform _leftArmTarget;
        [SerializeField] private Transform _rightArmHint;
        [SerializeField] private Transform _rightArmTarget;

        [Header("Bindings - Aiming")]
        [SerializeField] private Transform _sightPoint;

        [Header("Settings - Aiming")]
        [SerializeField] private LayerMask _targetLayerMask = Physics.DefaultRaycastLayers;
        //[SerializeField] private float _aimingAngularVelocity = 360f;

        // Weapons properties
        private Weapon[] _weapons;
        private int _currentWeaponIndex = -1;

        // Aiming properties
        private Transform _aimingTarget;
        private IVisible _aimingTargetVisible;
        //private float _currentAimingAngle;
        //private float _desiredAimingAngle;

        private void Awake()
        {
            _weapons = _weaponsParent.GetComponentsInChildren<Weapon>();
        }

        private void OnEnable()
        {
            _inputManager.OnChangeWeaponDeltaUpdated += OnChangeWeapon;
            _inputManager.OnSelectWeapon += OnSelectWeaponReceived;
            _inputManager.OnShoot += OnShoot;
        }

        private void OnDisable()
        {
            _inputManager.OnChangeWeaponDeltaUpdated -= OnChangeWeapon;
            _inputManager.OnSelectWeapon -= OnSelectWeaponReceived;
            _inputManager.OnShoot -= OnShoot;
        }

        private void Start()
        {
            foreach (Weapon weapon in _weapons) { weapon.gameObject.SetActive(false); }

            SelectWeapon(-1);
        }

        private void Update()
        {
            Weapon currentWeapon = _currentWeaponIndex != -1 ? _weapons[_currentWeaponIndex] : null;

            //Vector3 aimPosition;
            //bool hasAimingPosition = CalcBestAimingPosition(currentWeapon, out aimPosition);

            //if (hasAimingPosition)
            //{
            //    Vector3 rotationPosition = currentWeapon.GetRotationPointPosition();
            //    Vector3 direction = aimPosition - rotationPosition;
            //    _desiredAimingAngle = CalcAimingAngle(direction);
            //}
            //else
            //{
            //    _desiredAimingAngle = CalcAimingAngle(Camera.main.transform.forward);
            //}

            //float angleDifference = _desiredAimingAngle - _currentAimingAngle;
            //float angleToApply =
            //    Mathf.Sign(angleDifference) *
            //    Mathf.Min(
            //        Mathf.Abs(angleDifference),
            //        _aimingAngularVelocity * Time.deltaTime
            //        );
            //_currentAimingAngle += angleToApply;

            //if (_currentWeaponIndex != -1)
            //    _weapons[_currentWeaponIndex].NotifyAimingAngle(_currentAimingAngle);
        }

        private void LateUpdate()
        {
            if (_currentWeaponIndex != -1)
            {
                Weapon weapon = _weapons[_currentWeaponIndex];
                if (_weapons[_currentWeaponIndex].HasGrabPoints())
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
                if (_currentWeaponIndex != -1)
                {
                    _weapons[_currentWeaponIndex].ResetWeapon();
                    _weapons[_currentWeaponIndex].gameObject.SetActive(false);
                }

                _currentWeaponIndex = newWeaponIndex;

                if (_currentWeaponIndex != -1)
                {
                    _weapons[_currentWeaponIndex].gameObject.SetActive(true);
                    armsRig.weight = _weapons[_currentWeaponIndex].HasGrabPoints() ? 1f : 0f;
                }
                else { armsRig.weight = 0f; }
            }

            OnWeaponSelected.Invoke(_currentWeaponIndex == -1 ? null : _weapons[_currentWeaponIndex]);
        }

        float _oldAnalogValue = 0f;

        private void OnShoot(float analogValue)
        {
            if ((_currentWeaponIndex != -1) && (_weapons[_currentWeaponIndex] is FireWeapon))
            {
                float newAnalogValue = analogValue;
                if (_weapons[_currentWeaponIndex].CanUse())
                {
                    if ((_oldAnalogValue <= 0f) && (newAnalogValue > 0f)) { _weapons[_currentWeaponIndex]?.Use(); }
                }
                else if (_weapons[_currentWeaponIndex].CanContinuouslyUse())
                {
                    if ((_oldAnalogValue <= 0f) && (newAnalogValue > 0f)) { _weapons[_currentWeaponIndex]?.StartUsing(); }
                    else if ((_oldAnalogValue > 0f) && (newAnalogValue <= 0f)) { _weapons[_currentWeaponIndex]?.StopUsing(); }
                }
                _oldAnalogValue = newAnalogValue;
            }
        }

        private void OnChangeWeapon(Vector2 delta)
        {
            int newWeaponIndex = _currentWeaponIndex;

            if (delta.y < 0f)
            {
                newWeaponIndex--;
                if (newWeaponIndex < -1) { newWeaponIndex = _weapons.Length - 1; }
            }
            else if (delta.y > 0f)
            {
                newWeaponIndex++;
                if (newWeaponIndex >= _weapons.Length) { newWeaponIndex = -1; }
            }

            SelectWeapon(newWeaponIndex);
        }

        private void OnSelectWeaponReceived(int weaponSelectedIndex) => SelectWeapon(weaponSelectedIndex);

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
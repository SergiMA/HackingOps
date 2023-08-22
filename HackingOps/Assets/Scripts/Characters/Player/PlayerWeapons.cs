using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace HackingOps.Characters.Player
{
    public class PlayerWeapons : MonoBehaviour
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

        private Weapon[] _weapons;
        private int _currentWeaponIndex = -1;

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

        private void SelectWeapon(int newWeaponIndex)
        {
            if (newWeaponIndex < _weapons.Length)
            {
                if (_currentWeaponIndex != -1) { _weapons[_currentWeaponIndex].gameObject.SetActive(false); }

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
    }
}
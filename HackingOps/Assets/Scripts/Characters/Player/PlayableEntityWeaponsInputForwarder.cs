using HackingOps.Input;
using UnityEngine;

namespace HackingOps.Characters.Player
{
    public class PlayableEntityWeaponsInputForwarder : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager _inputManager;

        private PlayerWeapons _characterWeapons;

        private void Awake()
        {
            _characterWeapons = GetComponent<PlayerWeapons>();
        }

        private void OnEnable()
        {
            _inputManager.OnChangeWeaponDeltaUpdated += OnChangeWeapon;
            _inputManager.OnSelectWeapon += OnSelectWeaponReceived;
            _inputManager.OnShoot += OnAttack;
        }

        private void OnDisable()
        {
            _inputManager.OnChangeWeaponDeltaUpdated -= OnChangeWeapon;
            _inputManager.OnSelectWeapon -= OnSelectWeaponReceived;
            _inputManager.OnShoot -= OnAttack;
        }

        private void OnChangeWeapon(Vector2 delta)
        {
            if (delta.y < 0f)
                _characterWeapons.ChangeToPreviousWeapon();
            else if (delta.y > 0f)
                _characterWeapons.ChangeToNextWeapon();
        }

        private void OnSelectWeaponReceived(int weaponSelectedIndex)
        {
            _characterWeapons.OnSelectWeaponReceived(weaponSelectedIndex);
        }

        private void OnAttack(float analogValue)
        {
            _characterWeapons.OnShoot(analogValue);
        }
    }
}
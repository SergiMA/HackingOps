using Cinemachine;
using HackingOps.Characters.Player;
using HackingOps.Input;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CharacterLock : MonoBehaviour
    {
        [Header("Bindings")]

        [SerializeField] PlayerInputManager _inputManager;

        [field: Space]
        [SerializeField] public Transform Target { get; private set; }

        [Space]
        [SerializeField] private CinemachineVirtualCameraBase _lockOnCamera;
        [SerializeField] private PlayerBehaviourProfileSO _lockOnBehaviourProfile;
        [SerializeField] private PlayerBehaviourProfileSO _blockingBehaviourProfile;

        [SerializeField] private Transform _aimTarget;

        [Header("Settings")]
        [SerializeField] private float _maxAimDistance = 99999f;
        [SerializeField] private LayerMask _aimColliderLayerMask = Physics.DefaultRaycastLayers;

        private PlayerController _playerController;
        private PlayerWeapons _playerWeapons;
        private Inventory _inventory;

        private bool _isLocking;
        private bool _isBlocking;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerWeapons = GetComponent<PlayerWeapons>();
            _inventory = GetComponent<Inventory>();
        }

        private void Update()
        {
            ProjectRaycastAtScreenCenter();

            if (_isLocking)
            {
                _lockOnCamera.gameObject.SetActive(true);
                _playerController.UseBehaviourProfileOverride(_lockOnBehaviourProfile);
            }
            else
            {
                _lockOnCamera.gameObject.SetActive(false);
                _playerController.RecoverOriginalProfileOverride();
            }

            if (_isBlocking)
            {
                _playerController.UseBehaviourProfileOverride(_blockingBehaviourProfile);
            }
            else
            {
                _playerController.RecoverOriginalProfileOverride();
            }
        }

        private void ProjectRaycastAtScreenCenter()
        {
            if (_aimTarget == null) return;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxAimDistance, _aimColliderLayerMask))
            {
                _aimTarget.position = hit.point;
            }
        }

        private void ProjectRaycastAtWeaponForward()
        {
            if (_aimTarget == null) return;
            if (_playerWeapons._currentWeaponIndex == -1) return;

            Weapon weapon = _playerWeapons._weapons[_playerWeapons._currentWeaponIndex];

            if (weapon is not FireWeapon) return;

            FireWeapon fireWeapon = weapon as FireWeapon;

            Transform rotationPoint = fireWeapon.GetRotationPoint();
            Ray ray = new Ray(rotationPoint.position, rotationPoint.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxAimDistance, _aimColliderLayerMask))
            {
                _aimTarget.position = hit.point;
            }
        }

        public void OnStartLocking() => _isLocking = true;

        public void OnStopLocking() => _isLocking = false;

        public void OnStartBlocking() => _isBlocking = true;

        public void OnStopBlocking() => _isBlocking = false;
    }
}
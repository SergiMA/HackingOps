using Cinemachine;
using HackingOps.Characters.Player;
using HackingOps.Input;
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

        [SerializeField] private Transform _aimTarget;

        [Header("Settings")]
        [SerializeField] private float _maxAimDistance = 99999f;
        [SerializeField] private LayerMask _aimColliderLayerMask = Physics.DefaultRaycastLayers;

        private PlayerController _playerController;
        private PlayerWeapons _playerWeapons;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerWeapons = GetComponent<PlayerWeapons>();
        }

        private void Update()
        {
            //ProjectRaycastAtScreenCenter();
            ProjectRaycastAtWeaponForward();

            if (_inputManager.IsLocking)
            {
                _lockOnCamera.gameObject.SetActive(true);
                _playerController.UseBehaviourProfileOverride(_lockOnBehaviourProfile);
            }
            else
            {
                _lockOnCamera.gameObject.SetActive(false);
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
    }
}
using Cinemachine;
using HackingOps.Characters.Player;
using HackingOps.Input;
using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CharacterLock : MonoBehaviour
    {
        [field: Header("Bindings")]

        [field: Space]
        [field: SerializeField] public Transform Target { get; private set; }

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
        private Camera _mainCamera;

        enum AimState
        {
            Free,
            Locking,
            Blocking,
        }

        private AimState _aimState;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerWeapons = GetComponent<PlayerWeapons>();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            ProjectRaycastAtScreenCenter();
            UpdateBehaviourProfileOverrides();
        }

        private void ProjectRaycastAtScreenCenter()
        {
            if (_aimTarget == null) return;

            _playerWeapons?.UnsetAimingPosition();

            if (_aimState == AimState.Locking)
            {
                Debug.DrawRay(_mainCamera.transform.position, _mainCamera.transform.forward, Color.magenta, 0.5f);
                float camToPlayerDistance = (transform.position - _mainCamera.transform.position).magnitude;
                Vector3 rayStartPosition = _mainCamera.transform.position + (_mainCamera.transform.forward * camToPlayerDistance);

                if (Physics.Raycast(rayStartPosition, _mainCamera.transform.forward, out RaycastHit hit, Mathf.Infinity, _aimColliderLayerMask))
                {
                    Debug.DrawRay(rayStartPosition, hit.point - rayStartPosition, Color.cyan, 0.5f);
                    _playerWeapons?.SetAimingPosition(hit.point);
                    _aimTarget.position = hit.point;
                }
            }
        }

        private void UpdateBehaviourProfileOverrides()
        {
            switch (_aimState)
            {
                case AimState.Free:
                    _lockOnCamera.gameObject.SetActive(false);
                    _playerController.RecoverOriginalProfileOverride();
                    break;
                case AimState.Locking:
                    _lockOnCamera.gameObject.SetActive(true);
                    _playerController.UseBehaviourProfileOverride(_lockOnBehaviourProfile);
                    break;
                case AimState.Blocking:
                    _playerController.UseBehaviourProfileOverride(_blockingBehaviourProfile);
                    break;
            }
        }

        public void OnStartLocking() => _aimState = AimState.Locking;

        public void OnStopLocking() => _aimState = AimState.Free;

        public void OnStartBlocking() => _aimState = AimState.Blocking;

        public void OnStopBlocking() => _aimState = AimState.Free;
    }
}
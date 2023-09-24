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
        }

        private void Update()
        {
            ProjectRaycastAtScreenCenter();
            UpdateBehaviourProfileOverrides();
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
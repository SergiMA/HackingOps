using Cinemachine;
using HackingOps.Characters.Player;
using HackingOps.Input;
using System.Collections;
using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class AimController : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private PlayerInputManager _inputManager;
        [SerializeField] private CinemachineVirtualCamera _aimCamera;
        [SerializeField] private Transform _aimCameraTarget;
        [SerializeField] private ThirdPersonController _thirdPersonController;
        [SerializeField] private Transform _aimCameraVictim;

        [Space]

        [SerializeField] private PlayerBehaviourProfileSO _lockOnBehaviourProfile;

        [Header("Settings")]
        [SerializeField] private float _aimSensitivity = 1f;
        //[SerializeField] private float _bottomAngleClamp = -80f;
        //[SerializeField] private float _topAngleClamp = 80f;
        [SerializeField] private float _maxAimDistance = 99999f;
        [SerializeField] private LayerMask _aimColliderLayerMask = Physics.DefaultRaycastLayers;

        private float _aimCameraYaw;
        private float _aimCameraPitch;
        private Quaternion _initialLocalRotation;

        bool _isAiming;

        private void OnEnable()
        {
            _inputManager.OnStartAiming += OnStartAiming;
            _inputManager.OnStopAiming += OnStopAiming;
        }

        private void OnDisable()
        {
            _inputManager.OnStartAiming -= OnStartAiming;
            _inputManager.OnStopAiming -= OnStopAiming;
        }

        private void Start()
        {
            _initialLocalRotation = _aimCamera.transform.localRotation;
        }

        private void Update()
        {
            ProjectRaycastAtScreenCenter();

            if (_isAiming)
            {
                _aimCamera.gameObject.SetActive(true);
                _thirdPersonController.UseBehaviourProfileOverride(_lockOnBehaviourProfile);
                RotateCamera();
                //RotateCharacter();
            }
            else
            {
                _aimCamera.transform.rotation = _initialLocalRotation;
                _aimCamera.gameObject.SetActive(false);
                _thirdPersonController.RecoverOriginalProfileOverride();
            }
        }

        private void ProjectRaycastAtScreenCenter()
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxAimDistance, _aimColliderLayerMask))
            {
                _aimCameraVictim.position = hit.point;
            }
        }

        private void RotateCamera()
        {
            _aimCameraYaw += _inputManager.MouseInput.x * _aimSensitivity * Time.deltaTime;
            _aimCameraPitch -= _inputManager.MouseInput.y * _aimSensitivity * Time.deltaTime;
            //_aimCameraPitch = ClampAngle(_aimCameraPitch, _bottomAngleClamp, _topAngleClamp);
            //_aimCameraTarget.localRotation = Quaternion.Euler(_aimCameraPitch, _aimCameraYaw, 0f);
        }
        
        private void RotateCharacter()
        {
            Vector3 desiredLocalRotationEuler = _aimCameraTarget.localRotation.eulerAngles;
            desiredLocalRotationEuler.x = 0f;
            desiredLocalRotationEuler.z = 0f;

            Quaternion desiredLocalRotation = Quaternion.identity;
            desiredLocalRotation.eulerAngles = desiredLocalRotationEuler;

            _thirdPersonController.transform.localRotation = desiredLocalRotation;
        }

        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            else if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }

        public void OnStartAiming()
        {
            Debug.Log($"AimController --- {name} started aiming");
            _isAiming = true;
        }

        public void OnStopAiming()
        {
            Debug.Log($"AimController --- {name} finished aiming");
            _isAiming = false;
        }
    }
}
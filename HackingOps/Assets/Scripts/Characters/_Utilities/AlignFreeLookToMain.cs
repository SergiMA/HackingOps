using UnityEngine;
using Cinemachine;

namespace HackingOps.Characters.Utilities
{
    public class AlignFreeLookToMain : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private UpdateMode _updateMode;
        [SerializeField] private float _angularSpeed = 360f;

        public enum UpdateMode
        { 
            Always,
            OnlyOnActivation
        }

        private CinemachineFreeLook _freeLook;
        private Transform _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main.transform;
            _freeLook = GetComponent<CinemachineFreeLook>();
        }

        private void OnEnable()
        {
            if (_updateMode == UpdateMode.OnlyOnActivation)
            {
                if (_updateMode == UpdateMode.Always)
                    PerformRotation(Mathf.Infinity);
            }
        }

        // https://forum.unity.com/threads/set-rotation-of-cinemachine-freelook-camera.914744/
        private void Update()
        {
            if (_updateMode == UpdateMode.Always)
            { PerformRotation(_angularSpeed); }
        }

        private void PerformRotation(float angularSpeed)
        {
            Vector3 mainCameraForwardOnPlane = Vector3.ProjectOnPlane(_mainCamera.forward, Vector3.up);
            Vector3 freelookCameraForward = _freeLook.LookAt.position - _freeLook.transform.position;
            Vector3 freelookCameraForwardOnPlane = Vector3.ProjectOnPlane(freelookCameraForward, Vector3.up);

            float angularDistance = Vector3.SignedAngle(mainCameraForwardOnPlane, freelookCameraForwardOnPlane, Vector3.up);
            _freeLook.m_XAxis.Value = -Mathf.Sign(angularDistance) * Mathf.Min(Mathf.Abs(angularDistance), angularSpeed * Time.deltaTime);
        }
    }
}
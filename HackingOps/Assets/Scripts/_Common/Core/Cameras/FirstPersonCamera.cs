using HackingOps.Hacking;
using UnityEngine;

namespace HackingOps.Common.Core.Cameras
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private Transform _cameraRotator;

        [Header("Settings")]
        [SerializeField] private Vector2 _sensitivity = new(100f, 100f);
        [SerializeField] private Vector2 _clampedAxisDegrees = new(45f, 45f);

        private ICameraInput _cameraInput;
        private Vector2 _processedInput;

        private void Awake()
        {
            _cameraInput = GetComponent<ICameraInput>();
        }

        private void Update()
        {
            RotateCamera();
        }

        private void RotateCamera()
        {
            ProcessInput(_cameraInput.GetInput());
            ProcessAxisClamp();

            _cameraRotator.localRotation = Quaternion.Euler(_processedInput.y, _processedInput.x, 0f);
        }

        private void ProcessInput(Vector2 mouseInput)
        {
            _processedInput.x += mouseInput.x * _sensitivity.x * Time.deltaTime;
            _processedInput.y -= mouseInput.y * _sensitivity.y * Time.deltaTime;
        }

        private void ProcessAxisClamp()
        {
            _processedInput.x = Mathf.Clamp(_processedInput.x, -_clampedAxisDegrees.x, _clampedAxisDegrees.x);
            _processedInput.y = Mathf.Clamp(_processedInput.y, -_clampedAxisDegrees.y, _clampedAxisDegrees.y);
        }
    }
}
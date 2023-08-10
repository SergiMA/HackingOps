using UnityEngine;
using UnityEngine.InputSystem;

namespace HackingOps.Characters
{
    public class PlayerController : MonoBehaviour, IMovementReadable
    {
        [Header("Movement info")]
        [SerializeField] private float _speedWalking = 1f;      // m/s
        [SerializeField] private float _speedRunning = 2.5f;    // m/s
        [SerializeField] private float _jumpSpeed = 6f;         // m/s

        public enum MovementMode
        {
            RelativeToCamera,
            Local,
        }

        [SerializeField] private MovementMode _movementMode = MovementMode.Local;
        [SerializeField] private Transform _movementCamera;

        [Header("Orientation info")]
        [SerializeField] private float _angularSpeed = 360f;    // degrees/s
        public enum OrientationMode
        {
            MovementForward,
            CameraForward,
            LookAtTarget,
        }

        [SerializeField] private OrientationMode _orientationMode = OrientationMode.MovementForward;

        [SerializeField] private Transform _orientationCamera;
        [SerializeField] private Transform _orientationTarget;

        [Header("Camera info")]
        [SerializeField] private Transform _camera;


        // Bindings
        private CharacterController _characterController;

        // Inputs
        private bool _inputJump;
        private bool _inputRun;
        private Vector2 _inputMovement;

        // Movement
        private Vector3 _lastVelocity;

        // Falling physics
        float _currentVerticalSpeed = 0f;
        readonly float _gravity = -9.8f;     // m/s2

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            UpdateMovement();
            UpdateOrientation();
        }

        private void UpdateMovement()
        {
            Vector3 planeVelocity = UpdateMovementOnPlane();
            float verticalVelocity = UpdateMovementVertical();

            Vector3 combinedVelocity = planeVelocity + (verticalVelocity * Vector3.up);
            Vector3 adaptedVelocity = AdaptVelocityToGround(combinedVelocity);

            _characterController.Move(adaptedVelocity * Time.deltaTime);
            _lastVelocity = combinedVelocity;
        }

        private Vector3 UpdateMovementOnPlane()
        {
            float speed = _inputRun ? _speedRunning : _speedWalking;

            Vector3 localVelocity = _inputMovement * speed;
            localVelocity.z = localVelocity.y;
            localVelocity.y = 0f;

            Vector3 worldVelocity = Vector3.zero;
            switch (_movementMode)
            {
                case MovementMode.RelativeToCamera:
                    worldVelocity = _movementCamera.TransformDirection(localVelocity);
                    break;
                case MovementMode.Local:
                    worldVelocity = transform.TransformDirection(localVelocity);
                    break;
            }

            worldVelocity = Vector3.ProjectOnPlane(worldVelocity, Vector3.up);
            worldVelocity = worldVelocity.normalized * localVelocity.magnitude;

            return worldVelocity;
        }

        private float UpdateMovementVertical()
        {
            if (_characterController.isGrounded)
                _currentVerticalSpeed = 0f;

            if (_inputJump && _characterController.isGrounded)
                _currentVerticalSpeed = _jumpSpeed;
            else
                _currentVerticalSpeed += _gravity * Time.deltaTime;

            _inputJump = false;

            return _currentVerticalSpeed;
        }

        private Vector3 AdaptVelocityToGround(Vector3 velocity)
        {
            Vector3 adaptedVelocity = velocity;

            if (_characterController.isGrounded)
            {
                Vector3 XZVelocity = velocity;
                XZVelocity.y = 0f;

                if (Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, out RaycastHit hit, 1f))
                {
                    Quaternion directionRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    XZVelocity = directionRotation * XZVelocity;
                    adaptedVelocity = XZVelocity + Vector3.up * velocity.y;
                    adaptedVelocity.y += _gravity / 2f;
                }
            }

            return adaptedVelocity;
        }

        private void UpdateOrientation()
        {
            Vector3 velocityOnPlane = _lastVelocity;
            velocityOnPlane.y = 0f;

            if (velocityOnPlane.sqrMagnitude > 0f)
            {
                Vector3 desiredForward = GetDesiredForward(velocityOnPlane);

                float angularDistanceWithSign = Vector3.SignedAngle(transform.forward, desiredForward, Vector3.up);
                float angularDistanceWithoutSign = Mathf.Abs(angularDistanceWithSign);

                float angleToApply = _angularSpeed * Time.deltaTime;
                angleToApply = Mathf.Min(angularDistanceWithoutSign, angleToApply);
                angleToApply *= Mathf.Sign(angularDistanceWithSign);

                Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
                transform.rotation = rotationToApply * transform.rotation;
            }
        }

        private Vector3 GetDesiredForward(Vector3 velocityOnPlane)
        {
            Vector3 desiredForward = Vector3.zero;

            switch (_orientationMode)
            {
                case OrientationMode.MovementForward:
                    desiredForward = velocityOnPlane.normalized;
                    break;
                case OrientationMode.CameraForward:
                    desiredForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                    break;
                case OrientationMode.LookAtTarget:
                    desiredForward = Vector3.ProjectOnPlane(_orientationTarget.position - transform.position, Vector3.up);
                    break;
            }

            return desiredForward;
        }

        #region Input system implementation
        void OnJump() => _inputJump = true;
        void OnMove(InputValue value) => _inputMovement = value.Get<Vector2>();
        void OnRun(InputValue value) => _inputRun = value.Get<float>() > 0f;
        #endregion

        #region IMovementReadable implementation
        public Vector3 GetVelocity()
        {
            return _lastVelocity;
        }

        public float GetWalkSpeed()
        {
            return _speedWalking;
        }

        public float GetRunSpeed()
        {
            return _speedRunning;
        }

        public float GetJumpSpeed()
        {
            return _jumpSpeed;
        }

        public bool GetIsGrounded()
        {
            return _characterController.isGrounded;
        }
        #endregion
    }
}
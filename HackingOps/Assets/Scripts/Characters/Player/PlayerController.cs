using UnityEngine;
using HackingOps.Characters.Common;
using HackingOps.Characters.NPC.Senses;
using UnityEngine.Events;

namespace HackingOps.Characters.Player
{
    public class PlayerController : MonoBehaviour, IMovementReadable, IVisible
    {
        public UnityEvent OnStartCrouchingEvent;
        public UnityEvent OnStopCrouchingEvent;
        public UnityEvent OnStartBlockingEvent;
        public UnityEvent OnStopBlockingEvent;

        [Header("Bindings")]
        [SerializeField] Input.PlayerInputManager _inputManager;

        [Header("Locomotion properties")]
        [SerializeField] LocomotionPropertiesSO _standingProperties;
        [SerializeField] LocomotionPropertiesSO _crouchingProperties;

        public enum MovementMode
        {
            RelativeToCamera,
            Local,
        }

        [SerializeField] private Transform _movementCamera;

        public enum OrientationMode
        {
            MovementForward,
            CameraForward,
            LookAtTarget,
        }

        [SerializeField] private Transform _orientationTarget;

        [Header("Camera info")]
        [SerializeField] private Transform _camera;

        [Header("AI Visibility")]
        [SerializeField] private Transform _visibilityCheckpointsParent;
        private Transform[] _visibilityCheckpoints;

        [Header("Behaviour profiles properties")]
        [SerializeField] PlayerBehaviourProfileSO _behaviourProfile;

        // Internal bindings
        private CharacterController _characterController;
        private CrouchController _crouchController;
        private CharacterCombat _characterCombat;

        // Movement
        private Vector3 _lastVelocity;
        private bool _mustJump;

        // Current locomotion properties
        private LocomotionPropertiesSO _currentLocomotionProperties;
        private bool _isCrouched;

        // Falling physics
        private float _currentVerticalSpeed = 0f;
        private readonly float _gravity = -9.8f;            // m/s2

        // Restriction flags
        private bool _isRestricted;

        // Behaviour profiles
        PlayerBehaviourProfileSO _originalBehaviourProfile;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _crouchController = GetComponent<CrouchController>();
            _characterCombat = GetComponent<CharacterCombat>();

            _visibilityCheckpoints = new Transform[_visibilityCheckpointsParent.childCount];
            for (int i = 0; i < _visibilityCheckpoints.Length; i++)
            {
                _visibilityCheckpoints[i] = _visibilityCheckpointsParent.GetChild(i).transform;
            }
        }

        private void OnEnable()
        {
            _inputManager.OnJump += OnJump;
            _inputManager.OnCrouchPressed += SwitchCrouch;

            _characterCombat.OnMustAttack += OnAttack;
        }

        private void OnDisable()
        {
            _inputManager.OnJump -= OnJump;
            _inputManager.OnCrouchPressed -= SwitchCrouch;

            _characterCombat.OnMustAttack -= OnAttack;
        }

        private void Start()
        {
            _originalBehaviourProfile = _behaviourProfile;
            _currentLocomotionProperties = _standingProperties;
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
            float speed = _inputManager.IsRunning ? _currentLocomotionProperties.SpeedAccelerated : _currentLocomotionProperties.SpeedNormal;

            Vector3 localVelocity = _inputManager.MoveInput * speed;
            localVelocity.z = localVelocity.y;
            localVelocity.y = 0f;

            Vector3 worldVelocity = Vector3.zero;
            switch (_behaviourProfile.MovementMode)
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

            if (_mustJump && _characterController.isGrounded)
                _currentVerticalSpeed = _currentLocomotionProperties.SpeedJump;
            else
                _currentVerticalSpeed += _gravity * Time.deltaTime;

            _mustJump = false;

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

            if (velocityOnPlane.sqrMagnitude > 0f || _behaviourProfile.OrientateAlways)
            {
                Vector3 desiredForward = Vector3.zero;

                switch (_behaviourProfile.OrientationMode)
                {
                    case OrientationMode.MovementForward:
                        desiredForward = velocityOnPlane.normalized;
                        break;
                    case OrientationMode.CameraForward:
                        desiredForward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up);
                        break;
                    case OrientationMode.LookAtTarget:
                        desiredForward = Vector3.ProjectOnPlane(_orientationTarget.position - transform.position, Vector3.up);
                        break;
                    default:
                        Debug.LogWarning("You're using an Orientation Mode not " +
                        "yet implemented. Some things might not work as expected");
                        break;
                }

                float angularDistanceWithSign = Vector3.SignedAngle(transform.forward, desiredForward, Vector3.up);
                float angularDistanceWithoutSign = Mathf.Abs(angularDistanceWithSign);

                float angleToApply = _currentLocomotionProperties.SpeedAngular * Time.deltaTime;
                angleToApply = Mathf.Min(angularDistanceWithoutSign, angleToApply);
                angleToApply *= Mathf.Sign(angularDistanceWithSign);

                Quaternion rotationToApply = Quaternion.AngleAxis(angleToApply, Vector3.up);
                transform.rotation = rotationToApply * transform.rotation;
            }
        }

        public void SetOrientationTarget(Transform newTarget)
        {
            _orientationTarget = newTarget;
        }

        public void UseBehaviourProfileOverride(PlayerBehaviourProfileSO behaviourProfile)
        {
            _behaviourProfile = behaviourProfile;
        }

        public void RecoverOriginalProfileOverride()
        {
            _behaviourProfile = _originalBehaviourProfile;
        }

        void StartCrouching()
        {
            if (_crouchController.TryCrouchDown())
            {
                _currentLocomotionProperties = _crouchingProperties;
                _isCrouched = _crouchController.TryCrouchDown();

                OnStartCrouchingEvent.Invoke();
            }
        }

        void StopCrouching()
        {
            if (_crouchController.TryStandUp())
            {
                _currentLocomotionProperties = _standingProperties;
                _isCrouched = false;

                OnStopCrouchingEvent.Invoke();
            }
        }

        public void OnStartAimingPressed()
        {
            if (_isCrouched)
                return;

            _isRestricted = true;
            OnStartBlockingEvent.Invoke();
        }

        public void OnStopAimingPressed()
        {
            _isRestricted = false;
            OnStopBlockingEvent.Invoke();
        }

        public void OnBeginHacking()
        {
            _isRestricted = true;
        }

        public void OnEndHacking()
        {
            _isRestricted = false;
        }

        public void OnAttack()
        {
            OnStopAimingPressed();
        }

        #region Input system implementation
        void OnJump()
        {
            _mustJump = true;
        }

        void SwitchCrouch()
        {
            if (_isCrouched)
            {
                StopCrouching();
            }
            else
            {
                if (!_isRestricted)
                    StartCrouching();
            }
        }
        #endregion

        #region IMovementReadable implementation
        public Vector3 GetVelocity()
        {
            return _lastVelocity;
        }

        public float GetNormalSpeed()
        {
            return _currentLocomotionProperties.SpeedNormal;
        }

        public float GetAcceleratedSpeed()
        {
            return _currentLocomotionProperties.SpeedAccelerated;
        }

        public float GetJumpSpeed()
        {
            return _currentLocomotionProperties.SpeedJump;
        }

        public bool GetIsGrounded()
        {
            return _characterController.isGrounded;
        }

        public bool GetIsCrouched()
        {
            return _isCrouched;
        }
        #endregion

        #region IVisible implementation
        Vector3[] IVisible.GetCheckpoints()
        {
            Vector3[] checkpoints = new Vector3[_visibilityCheckpoints.Length];
            for (int i = 0; i < _visibilityCheckpoints.Length; i++)
            {
                checkpoints[i] = _visibilityCheckpoints[i].position;
            }

            return checkpoints;
        }

        public Transform GetTransform() => transform;
        #endregion
    }
}
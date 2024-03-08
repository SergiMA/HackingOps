using HackingOps.Animations.IK.EnvironmentInteractions.States;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

namespace HackingOps.Animations.IK.EnvironmentInteractions
{
    public class EnvironmentInteractor : MonoBehaviour, IEventObserver
    {
        [Header("IK dependencies")]
        [SerializeField] private TwoBoneIKConstraint _armIkConstraint;
        [SerializeField] private MultiRotationConstraint _armMultiRotationConstraint;
        [SerializeField] private Rig _environmentInteractionRig;

        [Header("Other dependencies")]
        [SerializeField] CharacterController _characterController;

        [Header("Settings - Durations")]
        [Range(0.01f, 4f)][SerializeField] private float _animationTransitionDuration = 1f;
        [Range(0.01f, 4f)][SerializeField] private float _approachMinimumDuration = 0.3f;

        [Header("Settings - Steps weights")]
        [Range(0f, 1f)][SerializeField] private float _approachWeight = 0.5f;
        [Range(0f, 1f)][SerializeField] private float _touchWeight = 1f;

        [Header("Settings - State conditions - Thresholds")]
        [Range(0.01f, 3f)][SerializeField] private float _approachDistanceThreshold = 3f;
        [Range(0.01f, 4f)][SerializeField] private float _touchDistanceThreshold = 0.4f;

        [Header("Settings - State conditions - Prediction")]
        [Range(0.01f, 1f)][SerializeField] private float _predictionDistance = 0.3f;
        [Range(-1f, 1f)][SerializeField] private float _minDotAngleAllowed = -0.5f;
        [Range(-1f, 1f)][SerializeField] private float _maxDotAngleAllowed = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool _showDebugGizmos;

        [Header("Debug - Closest Point Sphere")]
        [SerializeField] private Color _debugClosestPointSphereColor = Color.red;
        [SerializeField] private float _debugClosestPointSphereRadius = 0.1f;

        [Header("Debug - Line to Closest Point")]
        [SerializeField] private Color _debugLineToClosestPointColor = Color.cyan;

        // State bindings
        private EnvironmentInteractorBaseState _currentState;
        private EnvironmentInteractorStateFactory _stateFactory;

        // Properties
        private float _wingspan;
        private float _armLenght;
        private float _colliderCenterOffset;

        private Collider _currentColliderTarget;    // Collisioned collider

        // Transform components
        private Transform _rootTransform;
        private Transform _handTransform;
        private Transform _elbowTransform;
        private Transform _shoulderTransform;
        private Transform _ikTargetTransform;

        // Location calculation properties - Position
        private Vector3 _closestPointPosition = Vector3.positiveInfinity;
        private Vector3 _targetPointPosition;
        private float _targetPointPositionYOffset;

        // Loction calculation properties - Rotation
        private Vector3 _targetDesiredRotation;

        // Conditions to change states
        private float _cosTheta;    // Dot product between two directions

        // Other properties
        private IEventQueue _eventQueue;
        private bool _isDisabled;
        private bool _isRunning;
        private bool _isCrouching;
        private bool _isWeaponSheathed;
        private bool _isHacking;
        private bool _isUsingLaptop;
        private bool _isInCutscene;

        #region Getters & Setters
        // State machine properties
        public EnvironmentInteractorBaseState CurrentState { get => _currentState; set => _currentState = value; }

        // Transform properties
        public Transform RootTransform { get => _rootTransform; }
        public Transform HandTransform { get => _handTransform; }
        public Transform ElbowTransform { get => _elbowTransform; }
        public Transform ShoulderTransform { get => _shoulderTransform; }
        public Transform IkTargetTransform { get => _ikTargetTransform; }

        // Location calculation properties - Position
        public Vector3 ClosestPointPosition { get => _closestPointPosition; set => _closestPointPosition = value; }
        public Vector3 TargetPointPosition { get => _targetPointPosition; set => _targetPointPosition = value; }
        public float TargetPointPositionYOffset { get => _targetPointPositionYOffset; set => _targetPointPositionYOffset = value; }

        // Location calculation properties - Rotation
        public Vector3 TargetDesiredRotation { get => _targetDesiredRotation; set => _targetDesiredRotation = value; }

        // State conditions thresholds properties
        public float ApproachDistanceThreshold { get => _approachDistanceThreshold; }
        public float TouchDistanceThreshold { get => _touchDistanceThreshold; }

        // IK properties
        public Rig EnvironmentInteractionRig { get => _environmentInteractionRig; }
        public TwoBoneIKConstraint ArmIkConstraint { get => _armIkConstraint; }
        public MultiRotationConstraint ArmMultiRotationConstraint { get => _armMultiRotationConstraint; }

        public float ApproachWeight { get => _approachWeight; }
        public float TouchWeight { get => _touchWeight; }

        // Duration properties
        public float AnimationTransitionDuration { get => _animationTransitionDuration; }
        public float ApproachMinimumDuration { get => _approachMinimumDuration; }

        // Conditions to change states
        public float PredictionDistance { get => _predictionDistance; }
        public float MinDotAngleAllowed { get => _minDotAngleAllowed; }
        public float MaxDotAngleAllowed { get => _maxDotAngleAllowed; }
        public float CosTheta { get => _cosTheta; set => _cosTheta = value; }

        // Other properties
        public Collider CurrentColliderTarget { get => _currentColliderTarget; set => _currentColliderTarget = value; }
        public CharacterController CharacterController { get => _characterController; }
        public float ArmLength { get => _armLenght; }
        public bool IsDisabled { get => _isDisabled; set => _isDisabled = value; }
        #endregion

        #region Unity methods
        private void Awake()
        {
            ValidateDependencies();
            InitializeDependencies();
        }

        private void Start()
        {
            CreateDetectionCollider();
            InitializeStateMachine();
            SubscribeToEvents();
        }

        private void Update()
        {
            _currentState.UpdateState();
            CheckSystemState();
        }

        private void OnTriggerEnter(Collider other) => _currentState.OnTriggerEnter(other);
        private void OnTriggerStay(Collider other) => _currentState.OnTriggerStay(other);
        private void OnTriggerExit(Collider other) => _currentState.OnTriggerExit(other);

        private void OnDrawGizmosSelected()
        {
            if (!_showDebugGizmos) return;

            #region Closest point sphere
            if (_closestPointPosition == null) return;

            Gizmos.color = _debugClosestPointSphereColor;
            Gizmos.DrawSphere(_closestPointPosition, _debugClosestPointSphereRadius);
            #endregion

            #region Line between ClosestPointPosition and Shoulder position
            if (!_shoulderTransform) return;

            Gizmos.color = _debugLineToClosestPointColor;
            Gizmos.DrawLine(_closestPointPosition, _shoulderTransform.position);
            #endregion
        }
        #endregion

        private void ValidateDependencies()
        {
            Assert.IsNotNull(_armIkConstraint, $"<b>Arm IK constraint</b> is not assigned to <b>{name}</b>'s <b>{GetType().Name}</b> component");
            Assert.IsNotNull(_armMultiRotationConstraint, $"<b>Arm multi-rotation constraint</b> is not assigned to <b>{name}</b>'s <b>{GetType().Name}</b> component");
            Assert.IsNotNull(_characterController, $"<b>Character Controller</b> is not assigned to <b>{name}</b>'s <b>{GetType().Name}</b> component");
        }

        private void InitializeDependencies()
        {
            _rootTransform = _characterController.transform;
            _shoulderTransform = _armIkConstraint.data.root.transform;
            _elbowTransform = _armIkConstraint.data.mid.transform;
            _handTransform = _armIkConstraint.data.tip.transform;
            _ikTargetTransform = _armIkConstraint.data.target.transform;

            _wingspan = _characterController.height;
            _colliderCenterOffset = _wingspan / 2;

            Vector3 upperArm = _elbowTransform.position - _shoulderTransform.position;
            Vector3 foreArm = _handTransform.position - _elbowTransform.position;
            _armLenght = upperArm.magnitude + foreArm.magnitude;

            _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
        }

        private void InitializeStateMachine()
        {
            _stateFactory = new EnvironmentInteractorStateFactory(this);
            _currentState = _stateFactory.GetState(EnvironmentInteractorStateFactory.States.Search);
            _currentState.EnterState();
        }

        private void SubscribeToEvents()
        {
            _eventQueue.Subscribe(EventIds.WeaponSheathed, this);
            _eventQueue.Subscribe(EventIds.WeaponUnsheathed, this);
            _eventQueue.Subscribe(EventIds.BeginHackingMode, this);
            _eventQueue.Subscribe(EventIds.LeaveHackingMode, this);
            _eventQueue.Subscribe(EventIds.CutsceneStarted, this);
            _eventQueue.Subscribe(EventIds.CutsceneFinished, this);
        }

        private void CreateDetectionCollider()
        {
            float desiredCenterX = _elbowTransform.position.x - transform.position.x + transform.localPosition.x;

            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(desiredCenterX, _colliderCenterOffset, _wingspan / 2);
            boxCollider.size = new Vector3(_wingspan / 2f, _wingspan, _wingspan);
            boxCollider.isTrigger = true;
        }

        private void CheckSystemState()
        {
            if (_isRunning || _isCrouching || !_isWeaponSheathed || _isHacking || _isInCutscene)
                DisableSystem();
            else EnableSystem();
        }

        private void EnableSystem()
        {
            _currentState.EnableSystem();
        }

        private void DisableSystem()
        {
            _currentState.DisableSystem();
        }

        public void OnStartedRunning() => _isRunning = true;
        public void OnStartedWalking() => _isRunning = false;
        public void OnStoppedMoving() => _isRunning = false;
        public void OnStartedCrouching() => _isCrouching = true;
        public void OnStoppedCrouching() => _isCrouching = false;

        public void Log(string message) => Debug.Log(message);

        #region IEventObserver implementation
        void IEventObserver.Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.WeaponUnsheathed: _isWeaponSheathed = false; break;
                case EventIds.WeaponSheathed: _isWeaponSheathed = true; break;
                case EventIds.BeginHackingMode: _isHacking = true; break;
                case EventIds.LeaveHackingMode: _isHacking = false; break;
                case EventIds.CutsceneStarted: _isInCutscene = true; break;
                case EventIds.CutsceneFinished: _isInCutscene = false; break;
            }
        }
        #endregion
    }
}

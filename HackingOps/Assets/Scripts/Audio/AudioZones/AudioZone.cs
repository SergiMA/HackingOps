using HackingOps.Audio.AudioZones.States;
using HackingOps.Common.Followers;
using UnityEngine;
using UnityEngine.Assertions;

namespace HackingOps.Audio.AudioZones
{
    public class AudioZone : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Follower _follower;
        [SerializeField] private Transform[] _waypoints;

        [Header("Settings")]
        [SerializeField] private string _tagToCollide = "Player";

        [Space]
        [SerializeField] private float _blendingDurationTo3D = 0.5f;
        [SerializeField] private float _blendingDurationTo2D = 0.5f;

        [Space]
        [Tooltip("Seconds needed ot know if the target is inside the zone")]
        [SerializeField] private float _minDurationInsideZone = 0.1f;

        // State bindings
        private AudioZoneBaseState _currentState;
        private AudioZoneStateFactory _factory;

        // State properties
        private float _currentBlendingProgress;

        #region Getters & Setters
        // State machine properties
        public AudioZoneBaseState CurrentState { get => _currentState; set { _currentState = value; } }

        // Components properties
        public AudioSource AudioSource { get => _audioSource; }
        public Follower Follower { get => _follower; }
        public Transform[] Waypoints { get => _waypoints; }

        // State properties
        public float BlendingDurationTo3D { get => _blendingDurationTo3D; }
        public float BlendingDurationTo2D { get => _blendingDurationTo2D; }

        public float MinDurationInsideZone { get => _minDurationInsideZone; }

        public float CurrentBlendingProgress { get => _currentBlendingProgress; set => _currentBlendingProgress = value; }
        #endregion

        #region Unity methods
        private void Awake()
        {
            ValidateDependencies();
        }

        private void Start()
        {
            InitializeStateMachine();
        }

        private void Update()
        {
            _currentState.UpdateState();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(_tagToCollide)) return;

            _currentState.OnTriggerStay(other);
        }
        #endregion

        private void ValidateDependencies()
        {
            Assert.IsNotNull(_audioSource, $"<b>Audio Source</b> is not assigned to <b>{name}</b>'s <b>{GetType().Name}</b>");
            Assert.AreNotEqual(0f, _waypoints.Length, $"No <b>Waypoints</b> have been assigned to <b>{name}</b>'s <b>{GetType().Name}</b>");
        }

        private void InitializeStateMachine()
        {
            _factory = new AudioZoneStateFactory(this);
            _currentState = _factory.GetState(AudioZoneStateFactory.States.Blended3D);
            _currentState.EnterState();
        }
    }
}
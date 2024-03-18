using HackingOps.VFX.WetnessSystem.States;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace HackingOps.VFX.WetnessSystem
{
    public class WetTarget : MonoBehaviour
    {
        [Header("Settings - Collision detection")]
        [SerializeField] private string _tagToCollide = "WetZone";

        [Header("Settings - Durations")]
        [Range(0.01f, 20f)][SerializeField] private float _minWetDuration = 3f;
        [Range(0.01f, 20f)][SerializeField] private float _wettingDuration = 3f;
        [Range(0.01f, 20f)][SerializeField] private float _dryingDuration = 3f;

        // State bindings
        private WetTargetBaseState _currentState;
        private WetTargetStateFactory _stateFactory;

        // Settings - Shader
        private Renderer[] _renderers;
        private List<Material> _materials = new();
        private int _hash_WetnessProgress = Shader.PropertyToID("_WetnessProgress");

        // State properties
        private float _currentWetnessProgress;

        #region Getters & Setters
        // State machine properties
        public WetTargetBaseState CurrentState { get => _currentState; set { _currentState = value; } }

        // Shader properties
        public List<Material> Materials { get => _materials; }

        // State properties
        public float MinWetDuration { get => _minWetDuration; }
        public float WettingDuration { get => _wettingDuration; }
        public float DryingDuration { get => _dryingDuration; }
        public float CurrentWetnessProgress { get => _currentWetnessProgress; set => _currentWetnessProgress = value; }

        // Shader properties
        public int Hash_WetnessProgress { get => _hash_WetnessProgress; }

        #endregion

        #region Unity methods
        private void Awake()
        {
            GetDependencies();
            ValidateDependencies();
            InitializeDependencies();
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

        private void GetDependencies() => _renderers = GetComponentsInChildren<Renderer>();

        private void ValidateDependencies()
        {
            Assert.AreNotEqual(0f, _renderers.Length, $"No <b>Mesh Renderers</b> have been assigned to <b>{name}</b>'s <b>{GetType().Name}</b>");
        }

        private void InitializeDependencies()
        {
            foreach (Renderer renderer in _renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    _materials.Add(material);
                }
            }
        }

        private void InitializeStateMachine()
        {
            _stateFactory = new WetTargetStateFactory(this);
            _currentState = _stateFactory.GetState(WetTargetStateFactory.States.Dry);
            _currentState.EnterState();
        }

        public void Log(string message) => Debug.Log(message);
    }
}
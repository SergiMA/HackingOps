using HackingOps.Doors.AnimatedDoors.States;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Doors.AnimatedDoors
{
    public class AnimatedDoor : MonoBehaviour
    {
        public UnityEvent OnDoorClosed;

        [Header("Bindings")]
        [SerializeField] private Animator _animator;

        [Header("Settings")]
        [SerializeField] private AnimatedDoorStateFactory.States _startingState = AnimatedDoorStateFactory.States.Closed;
        [SerializeField] private float _openingDuration = 1f;
        [SerializeField] private float _closingDuration = 1f;

        [Header("Debug")]
        [SerializeField] private bool _debugOpen;
        [SerializeField] private bool _debugClose;

        // State bindings
        private AnimatedDoorBaseState _currentState;
        private AnimatedDoorStateFactory _stateFactory;

        // Settings
        private float _progress;
        private bool _isStartingState = true;

        #region Getters & Setters
        public AnimatedDoorBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

        public Animator Animator { get { return _animator; } }

        public float OpeningDuration { get { return _openingDuration; } }
        public float ClosingDuration { get { return _closingDuration; } }
        public float Progress { get { return _progress; } set { _progress = value; } }

        public bool IsStartingState { get { return _isStartingState; } set { _isStartingState = value; } }
        #endregion

        private void OnValidate()
        {
            if (_debugOpen)
            {
                _debugOpen = false;
                Open();
            }
            else if (_debugClose)
            {
                _debugClose = false;
                Close();
            }
        }

        private void Start()
        {
            _stateFactory = new AnimatedDoorStateFactory(this);
            _currentState = _stateFactory.GetState(_startingState);
            _currentState.EnterState();
        }

        private void Update()
        {
            _currentState.UpdateState();
        }

        public void Open()
        {
            _currentState.Open();
        }

        public void Close()
        {
            _currentState.Close();
        }
    }
}
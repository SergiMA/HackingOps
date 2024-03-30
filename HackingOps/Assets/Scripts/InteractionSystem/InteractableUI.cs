using HackingOps.Utilities.Timers;
using HackingOps.Zones;
using UnityEngine;

namespace HackingOps.InteractionSystem
{
    public class InteractableUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private CanvasGroup _suggestionCanvas;
        [SerializeField] private CanvasGroup _actionCanvas;
        [SerializeField] private Trigger _trigger;

        [Header("Settings")]
        [SerializeField] private bool _canUseUI = true;
        [SerializeField] private float _releaseCandidateDuration = 0.3f;

        private IInteractable _interactable;

        private CountdownTimer _countdownTimer;

        private bool _previousCanBeInteracted;
        private bool _isTargetInRange;
        private bool _previousIsTargetInRange;
        private bool _hasTimerEnded;

        #region Unity methods
        private void Awake()
        {
            _interactable = GetComponent<IInteractable>();

            _countdownTimer = new CountdownTimer(_releaseCandidateDuration);
            _countdownTimer.OnStop += () => _hasTimerEnded = true;
        }

        private void Start() => Hide();
        private void OnEnable() => SubscribeToEvents();
        private void OnDisable() => UnsubscribeFromEvents();

        private void Update()
        {
            _countdownTimer.Tick(Time.deltaTime);
            CheckInteractionChanges();

            if (_hasTimerEnded)
            {
                _hasTimerEnded = false;
                ShowCanvas(_suggestionCanvas);
            }
        }
        #endregion

        private void SubscribeToEvents()
        {
            _trigger.OnEnter += OnEnter;
            _trigger.OnExit += OnExit;

            _interactable.OnReceiveCandidateNotification += OnSelectedAsCandidate;
        }

        private void UnsubscribeFromEvents()
        {
            _trigger.OnEnter -= OnEnter;
            _trigger.OnExit -= OnExit;

            _interactable.OnReceiveCandidateNotification -= OnSelectedAsCandidate;
        }

        private void OnEnter(Trigger trigger)
        {
            _isTargetInRange = true;
        }

        private void OnExit(Trigger trigger)
        {
            _isTargetInRange = false;
        }

        private void OnSelectedAsCandidate()
        {
            if (!_countdownTimer.IsRunning)
            {
                _countdownTimer.Start();
                ShowCanvas(_actionCanvas);
            }
            else
                _countdownTimer.Reset();
        }

        private void CheckInteractionChanges()
        {
            bool hasInteractionChanged = _previousCanBeInteracted != _interactable.CanBeInteracted();
            bool hasTargetInRangeChanged = _previousIsTargetInRange != _isTargetInRange;

            if (hasInteractionChanged || hasTargetInRangeChanged)
                UpdateCanvasState();

            _previousCanBeInteracted = _interactable.CanBeInteracted();
            _previousIsTargetInRange = _isTargetInRange;
        }

        private void UpdateCanvasState()
        {
            if (_interactable.CanBeInteracted() && _isTargetInRange)
                Show();
            else
                Hide();
        }

        private void ShowCanvas(CanvasGroup canvasGroup)
        {
            Hide();

            if (!_canUseUI) return;

            canvasGroup.alpha = 1f;
        }

        public void Show()
        {
            if (!_canUseUI) return;
            if (!_interactable.CanBeInteracted() || !_isTargetInRange) return;

            _suggestionCanvas.alpha = 1f;
        }

        public void Hide()
        {
            _suggestionCanvas.alpha = 0f;
            _actionCanvas.alpha = 0f;

            _hasTimerEnded = false; // If no UI will be visible, there's no need to continue using the timer
        }

        public void EnableUI() => _canUseUI = true;
        public void DisableUI() => _canUseUI = false;
    }
}
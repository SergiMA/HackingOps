using HackingOps.Utilities.Timers;
using HackingOps.Zones;
using UnityEngine;

namespace HackingOps.Hacking
{
    public class HackableUI : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private CanvasGroup _suggestionCanvas;
        [SerializeField] private CanvasGroup _actionCanvas;

        [Header("Settings")]
        [SerializeField] private bool _canUseUI = true;
        [SerializeField] private float _releaseCandidateDuration = 0.3f;

        private IHackable _hackable;

        private CountdownTimer _countdownTimer;

        private bool _hasTimerEnded;

        #region Unity methods
        private void Awake()
        {
            _hackable = GetComponent<IHackable>();

            _countdownTimer = new CountdownTimer(_releaseCandidateDuration);
            _countdownTimer.OnStop += () => _hasTimerEnded = true;
        }

        private void Start() => Hide();
        private void OnEnable() => SubscribeToEvents();
        private void OnDisable() => UnsubscribeFromEvents();

        private void Update()
        {
            _countdownTimer.Tick(Time.deltaTime);

            if (_hasTimerEnded)
            {
                _hasTimerEnded = false;
                Hide();
            }
        }
        #endregion
        private void SubscribeToEvents() => _hackable.OnReceiveCandidateNotification += OnSelectedAsCandidate;

        private void UnsubscribeFromEvents() => _hackable.OnReceiveCandidateNotification -= OnSelectedAsCandidate;

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

        private void ShowCanvas(CanvasGroup canvasGroup)
        {
            Hide();

            if (!_canUseUI) return;

            canvasGroup.alpha = 1f;
        }

        private void Hide()
        {
            _suggestionCanvas.alpha = 0f;
            _actionCanvas.alpha = 0f;

            _hasTimerEnded = false; // If no UI will be visible, there's no need to continue using the timer
        }

        public void EnableUI() => _canUseUI = true;
        public void DisableUI() => _canUseUI = false;
    }
}
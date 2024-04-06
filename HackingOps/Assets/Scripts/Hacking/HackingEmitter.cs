using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Input;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Hacking
{
    public class HackingEmitter : MonoBehaviour, IEventObserver
    {
        public UnityEvent OnHackingEmitted;

        [Header("Bindings")]
        [SerializeField] private PlayerInputManager _inputManager;

        [Header("Settings")]
        [SerializeField] private float _sphereCastRadius = 1f;
        [SerializeField] private float _sphereCastMaxDistance = 10f;
        [SerializeField] private LayerMask _layerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private float _suggestHackableCooldown = 0.2f;

        private Transform _brainCameraTransform;
        private IHackable _lastTargetHacked;

        private float _currentSuggestHackableCooldown = 0.2f;
        private bool _isHacking;

        #region Unity methods
        private void Awake()
        {
            _brainCameraTransform = Camera.main.transform;

            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.BeginHackingMode, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.LeaveHackingMode, this);
        }

        private void OnEnable()
        {
            _inputManager.OnHack += OnHack;
        }

        private void OnDisable()
        {
            _inputManager.OnHack -= OnHack;
        }

        private void Update()
        {
            if (!_isHacking) return;

            SuggestHackableOverTime();
        }

        #endregion

        private void SuggestHackableOverTime()
        {
            if (Time.time > _currentSuggestHackableCooldown)
            {
                NotifyHackableCandidate();
                _currentSuggestHackableCooldown = Time.time + _suggestHackableCooldown;
            }
        }

        private void OnHack()
        {
            IHackable hackableTarget = LookForHackableTarget();

            if (hackableTarget == null) return;

            if (_lastTargetHacked != null && hackableTarget.IsControllable())
                _lastTargetHacked.StopHacking();

            _lastTargetHacked = hackableTarget;
            OnHackingEmitted.Invoke();
            hackableTarget.BeginHacking();
        }

        private void NotifyHackableCandidate()
        {
            IHackable hackableCandidate = LookForHackableTarget();

            if (hackableCandidate == null) return;

            hackableCandidate.ReceiveCandidateNotification();
        }

        private IHackable LookForHackableTarget()
        {
            if (Physics.SphereCast(_brainCameraTransform.position, _sphereCastRadius, _brainCameraTransform.forward, out RaycastHit hit, _sphereCastMaxDistance, _layerMask))
            {
                Debug.DrawLine(_brainCameraTransform.position, hit.point, Color.red, 1f);

                if (hit.transform.TryGetComponent(out IHackable hackableTarget))
                {
                    return hackableTarget;
                }
            }

            return null;
        }

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.BeginHackingMode: _isHacking = true; break;
                case EventIds.LeaveHackingMode: _isHacking = false; break;
            }
        }
        #endregion
    }
}
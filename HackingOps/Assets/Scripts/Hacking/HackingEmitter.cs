using HackingOps.Input;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Hacking
{
    public class HackingEmitter : MonoBehaviour
    {
        public UnityEvent OnHackingEmitted;

        [Header("Bindings")]
        [SerializeField] private PlayerInputManager _inputManager;

        [Header("Settings")]
        [SerializeField] private float _sphereCastRadius = 1f;
        [SerializeField] private float _sphereCastMaxDistance = 10f;
        [SerializeField] private LayerMask _layerMask = Physics.DefaultRaycastLayers;

        private Transform _brainCameraTransform;
        private IHackable _lastTargetHacked;

        private void Awake()
        {
            _brainCameraTransform = Camera.main.transform;
        }

        private void OnEnable()
        {
            _inputManager.OnHack += OnHack;
        }

        private void OnDisable()
        {
            _inputManager.OnHack -= OnHack;
        }

        private void OnHack()
        {
            if (Physics.SphereCast(_brainCameraTransform.position, _sphereCastRadius, _brainCameraTransform.forward, out RaycastHit hit, _sphereCastMaxDistance, _layerMask))
            {
                Debug.DrawLine(_brainCameraTransform.position, hit.point, Color.red, 1f);
                if (hit.transform.TryGetComponent(out IHackable hackableTarget))
                {
                    if (_lastTargetHacked != null && hackableTarget.IsControllable())
                    {
                        _lastTargetHacked.StopHacking();
                    }

                    _lastTargetHacked = hackableTarget;
                    OnHackingEmitted.Invoke();
                    hackableTarget.BeginHacking();
                }
            }
        }
    }
}
using Cinemachine;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using HackingOps.Input;
using System;
using UnityEngine;

namespace HackingOps.Hacking
{
    public class HackableCamera : MonoBehaviour, IHackable, ICameraInput, IEventObserver
    {
        public event Action OnReceiveCandidateNotification;

        [Header("Bindings")]
        [SerializeField] private PlayerInputManager _inputManager;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        private Vector2 _mouseInput = new Vector2();
        private bool _isHacked;

        #region Debug region
        [Header("Debug toggles")]
        [SerializeField] private bool _debugBeginHacking;
        [SerializeField] private bool _debugStopHacking;

        private void OnValidate()
        {
            if (_debugBeginHacking)
            {
                _debugBeginHacking = false;
                BeginHacking();
            }

            if (_debugStopHacking)
            {
                _debugStopHacking = false;
                StopHacking();
            }
        }
        #endregion

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.LeaveHackingMode, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.LeaveHackingMode, this);
        }

        private void Update()
        {
            if (!_isHacked)
                return;

            UpdateMouseInput();
        }

        private void UpdateMouseInput()
        {
            _mouseInput.x = _inputManager.MouseInput.x;
            _mouseInput.y = _inputManager.MouseInput.y;
        }

        #region ICameraInput implementation
        public Vector2 GetInput() => _mouseInput;
        #endregion

        #region IHackable implementation
        public void BeginHacking()
        {
            _isHacked = true;

            ServiceLocator.Instance.GetService<CutsceneCameraController>().SetCutsceneCamera(_virtualCamera);
        }

        public void StopHacking()
        {
            _isHacked = false;

            ServiceLocator.Instance.GetService<CutsceneCameraController>().UnsetCutsceneCamera();
        }

        public bool IsControllable() => true;
        public void ReceiveCandidateNotification() => OnReceiveCandidateNotification?.Invoke();
        #endregion

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.LeaveHackingMode)
                StopHacking();
        }
        #endregion
    }
}
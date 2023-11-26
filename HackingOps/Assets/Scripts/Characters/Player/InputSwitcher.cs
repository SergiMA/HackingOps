using Cinemachine;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Input;
using UnityEngine;

namespace HackingOps.Characters.Player
{
    public class InputSwitcher : MonoBehaviour, IEventObserver
    {
        [SerializeField] private PlayerInputManager _inputManager;
        [SerializeField] private CinemachineFreeLook _freeLookCamera;

        private float _freeLookPreviousXAxisMaxSpeed;
        private float _freeLookPreviousYAxisMaxSpeed;

        private bool _isDisabled;

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CutsceneStarted, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CutsceneFinished, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.BeginHackingMode, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.LeaveHackingMode, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.PauseGame, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.ResumeGame, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.CutsceneStarted, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.CutsceneFinished, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.BeginHackingMode, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.LeaveHackingMode, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.PauseGame, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.ResumeGame, this);
        }

        private void Start()
        {
            _freeLookPreviousXAxisMaxSpeed = _freeLookCamera.m_XAxis.m_MaxSpeed;
            _freeLookPreviousYAxisMaxSpeed = _freeLookCamera.m_YAxis.m_MaxSpeed;
        }

        private void RestoreFreeLookMaxSpeed()
        {
            _freeLookCamera.m_XAxis.m_MaxSpeed = _freeLookPreviousXAxisMaxSpeed;
            _freeLookCamera.m_YAxis.m_MaxSpeed = _freeLookPreviousYAxisMaxSpeed;
        }

        private void ImmobilizeFreeLookMaxSpeed()
        {
            _freeLookPreviousXAxisMaxSpeed = _freeLookCamera.m_XAxis.m_MaxSpeed;
            _freeLookPreviousYAxisMaxSpeed = _freeLookCamera.m_YAxis.m_MaxSpeed;

            _freeLookCamera.m_XAxis.m_MaxSpeed = 0f;
            _freeLookCamera.m_YAxis.m_MaxSpeed = 0f;
        }

        public void EnableInput()
        {
            _isDisabled = false;
            _inputManager.enabled = true;
            RestoreFreeLookMaxSpeed();
        }

        public void DisableInput()
        {
            if (_isDisabled)
                return;

            _isDisabled = true;
            _inputManager.enabled = false;
            ImmobilizeFreeLookMaxSpeed();
        }

        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.CutsceneStarted:
                    DisableInput();
                    break;
                case EventIds.CutsceneFinished:
                    EnableInput();
                    break;
                case EventIds.BeginHackingMode:
                    _inputManager.SwitchToHackingModeActionMap();
                    break;
                case EventIds.LeaveHackingMode:
                    _inputManager.SwitchToPlayerActionMap();
                    break;
                case EventIds.PauseGame:
                    _inputManager.SwitchToMenuActionMap();
                    break;
                case EventIds.ResumeGame:
                    _inputManager.SwitchToPlayerActionMap();
                    break;
            }
        }
    }
}
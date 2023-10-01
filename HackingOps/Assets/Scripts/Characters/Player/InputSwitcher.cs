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

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CutsceneStarted, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CutsceneFinished, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.CutsceneStarted, this);
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.CutsceneFinished, this);
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
            _inputManager.enabled = true;
            RestoreFreeLookMaxSpeed();
        }

        public void DisableInput()
        {
            _inputManager.enabled = false;
            ImmobilizeFreeLookMaxSpeed();
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.CutsceneStarted)
                DisableInput();
            else if (eventData.EventId == EventIds.CutsceneFinished)
                EnableInput();
        }
    }
}
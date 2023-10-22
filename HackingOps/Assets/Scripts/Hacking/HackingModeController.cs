using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Input;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Hacking
{
    public class HackingModeController : MonoBehaviour
    {
        public UnityEvent OnStartHackingMode;
        public UnityEvent OnFinishHackingMode;

        [SerializeField] private PlayerInputManager _inputManager;

        private bool _isInHackingMode;

        private void OnEnable()
        {
            _inputManager.OnEnterHackingMode += OnHackingButtonPressed;
            _inputManager.OnLeaveHackingMode += OnHackingButtonPressed;
        }

        private void OnDisable()
        {
            _inputManager.OnEnterHackingMode -= OnHackingButtonPressed;
            _inputManager.OnLeaveHackingMode -= OnHackingButtonPressed;
        }

        private void OnHackingButtonPressed()
        {
            if (!_isInHackingMode)
                EnterMode();
            else
                EndMode();
        }

        public void EnterMode()
        {
            _isInHackingMode = true;
            OnStartHackingMode.Invoke();
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.BeginHackingMode));
        }

        public void EndMode()
        {
            _isInHackingMode = false;
            OnFinishHackingMode.Invoke();
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.LeaveHackingMode));
        }
    }
}
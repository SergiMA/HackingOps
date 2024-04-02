using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Input;
using HackingOps.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Common.Core.Managers
{
    public class GameManager : MonoBehaviour, IEventObserver
    {
        public UnityEvent OnPause;
        public UnityEvent OnResume;

        [SerializeField] private PlayerInputManager _inputManager;

        private void Awake()
        {
            Time.timeScale = 1f;
        }

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.BeginHackingMode, this);
            SubscribeToPauseEvent();
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.LeaveHackingMode, this);
            UnsubscribeFromPauseEvent();
        }

        private void SubscribeToPauseEvent() => _inputManager.OnCancel += Pause;
        private void UnsubscribeFromPauseEvent() => _inputManager.OnCancel -= Pause;

        public void Pause()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.PauseGame));
            OnPause.Invoke();
        }

        public void Resume()
        {
            ReadyToResume();
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.ResumeGame));
            OnResume.Invoke();
        }

        public void Exit()
        {
            ServiceLocator.Instance.GetService<SceneLoader>().LoadNext();
        }

        public void ReadyToPause()
        {
            Time.timeScale = 0f;
        }

        public void ReadyToResume()
        {
            Time.timeScale = 1f;
        }

        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.BeginHackingMode: UnsubscribeFromPauseEvent(); break;
                case EventIds.LeaveHackingMode: SubscribeToPauseEvent(); break;
            }
        }
    }
}
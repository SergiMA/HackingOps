using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Input;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Common.Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public UnityEvent OnPause;
        public UnityEvent OnResume;

        [SerializeField] private PlayerInputManager _inputManager;

        private void OnEnable()
        {
            _inputManager.OnCancel += Pause;
        }

        private void OnDisable()
        {
            _inputManager.OnCancel -= Pause;
        }

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

        public void ReadyToPause()
        {
            Time.timeScale = 0f;
        }

        public void ReadyToResume()
        {
            Time.timeScale = 1f;
        }
    }
}
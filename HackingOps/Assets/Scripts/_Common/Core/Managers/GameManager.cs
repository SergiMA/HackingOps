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

        private IEventQueue _eventQueue;

        [SerializeField] private bool _isPlayingCutscene;
        private bool _previousIsPlayingCutscene;

        [SerializeField] private bool _isHacking;
        private bool _previousIsHacking;

        #region Unity methods
        private void Awake()
        {
            _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

            Time.timeScale = 1f;
        }

        private void Start()
        {
            _eventQueue.Subscribe(EventIds.BeginHackingMode, this);
            _eventQueue.Subscribe(EventIds.LeaveHackingMode, this);
            _eventQueue.Subscribe(EventIds.CutsceneStarted, this);
            _eventQueue.Subscribe(EventIds.CutsceneFinished, this);

            SubscribeToPauseEvent();
        }

        private void Update()
        {
            if (HasIsPlayingCutsceneChanged() || HasIsHackingChanged())
                ChangePauseEventSubscription();

            UpdatePreviousValues();
        }

        private void ChangePauseEventSubscription()
        {
            if (!_isHacking && !_isPlayingCutscene)
            {
                SubscribeToPauseEvent();
            }
            else
            {
                UnsubscribeFromPauseEvent();
            }
        }
        #endregion

        private void SubscribeToPauseEvent() => _inputManager.OnCancel += Pause;
        private void UnsubscribeFromPauseEvent() => _inputManager.OnCancel -= Pause;

        private bool HasIsPlayingCutsceneChanged() => _isPlayingCutscene != _previousIsPlayingCutscene;
        private bool HasIsHackingChanged() => _isHacking != _previousIsHacking;

        private void UpdatePreviousValues()
        {
            _previousIsPlayingCutscene = _isPlayingCutscene;
            _previousIsHacking = _isHacking;
        }

        public void Pause()
        {
            _eventQueue.EnqueueEvent(new EventData(EventIds.PauseGame));
            OnPause.Invoke();
        }

        public void Resume()
        {
            ReadyToResume();
            _eventQueue.EnqueueEvent(new EventData(EventIds.ResumeGame));
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
                case EventIds.BeginHackingMode: _isHacking = true; break;
                case EventIds.LeaveHackingMode: _isHacking = false; break;
                case EventIds.CutsceneStarted: _isPlayingCutscene = true; break;
                case EventIds.CutsceneFinished: _isPlayingCutscene = false; break;
            }
        }
    }
}
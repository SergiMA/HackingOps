using DG.Tweening;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.UI.Screens.CrosshairScreen
{
    public class Crosshair : MonoBehaviour, IEventObserver
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        private IEventQueue _eventQueue;

        private void Awake() => _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

        private void Start()
        {
            _eventQueue.Subscribe(EventIds.BeginHackingMode, this);
            _eventQueue.Subscribe(EventIds.LeaveHackingMode, this);
            _eventQueue.Subscribe(EventIds.StartAiming, this);
            _eventQueue.Subscribe(EventIds.StopAiming, this);
            _eventQueue.Subscribe(EventIds.CutsceneStarted, this);
        }

        public void Show()
        {
            DOVirtual.Float(_canvasGroup.alpha, 1, _fadeInDuration, x => { _canvasGroup.alpha = x; });
        }

        public void Hide()
        {
            DOVirtual.Float(_canvasGroup.alpha, 0, _fadeOutDuration, x => { _canvasGroup.alpha = x; });
        }

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.CutsceneStarted: Hide(); break;
                case EventIds.BeginHackingMode: Show(); break;
                case EventIds.LeaveHackingMode: Hide(); break;
                case EventIds.StartAiming: Show(); break;
                case EventIds.StopAiming: Hide(); break;
            }
        }
        #endregion
    }
}
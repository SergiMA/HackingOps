using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    [RequireComponent(typeof(TimelinePlayer))]
    public class MenuTimelineSetup : MonoBehaviour, IEventObserver
    {
        private TimelinePlayer _timelinePlayer;

        private void Awake() => _timelinePlayer = GetComponent<TimelinePlayer>();

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.PauseGame, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.PauseGame, this);
        }

        public void UseMenu() => _timelinePlayer.Play();

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.PauseGame)
                UseMenu();
        }
        #endregion
    }
}
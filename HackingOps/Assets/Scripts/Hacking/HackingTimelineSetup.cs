using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Hacking
{
    public class HackingTimelineSetup : MonoBehaviour, IEventObserver
    {
        [Header("Bindings - Dependencies")]
        [SerializeField] private TimelinePlayer _timelinePlayer;

        private void Start()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.BeginHackingMode, this);
        }

        public void BeginHacking()
        {
            _timelinePlayer.Play();
        }

        #region IEventObserver Implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.BeginHackingMode)
                BeginHacking();
        }
        #endregion
    }
}
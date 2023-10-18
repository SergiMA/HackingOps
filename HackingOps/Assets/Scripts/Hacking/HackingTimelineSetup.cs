using HackingOps.Characters.Player;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using System;
using UnityEngine;

namespace Assets.Scripts.Hacking
{
    public class HackingTimelineSetup : MonoBehaviour, IEventObserver
    {
        [Header("Bindings - Dependencies")]
        [SerializeField] private TimelinePlayer _timelinePlayer;

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.BeginHackingMode, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.BeginHackingMode, this);
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
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Zones;
using UnityEngine;

namespace HackingOps.Common.QuestSystem
{
    public class DestinationGoal : Goal
    {
        [SerializeField] private Zone _zone;

        private void Start()
        {
            _zone.EnableZone();
        }

        public override void Init()
        {
            base.Init();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.OnZoneStepped, this);
        }

        public override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();

            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.OnZoneStepped, this);
        }

        public override void Process(EventData eventData)
        {
            if (IsCompleted)
                return;

            if (eventData.EventId != EventIds.OnZoneStepped)
                return;

            ZoneSteppedData data = eventData as ZoneSteppedData;

            if (data.Zone != _zone)
                return;

            base.Process(data);

            CurrentAmount++;
            Evaluate();
        }
    }
}
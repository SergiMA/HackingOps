using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Common.QuestSystem
{
    public class TimelineAnimationGoal : Goal
    {
        [SerializeField] private CutsceneId _cutsceneId;

        public override void Init()
        {
            base.Init();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CutsceneFinished, this);
        }

        public override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();

            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.CutsceneFinished, this);
        }

        public override void Process(EventData eventData)
        {
            if (IsCompleted)
                return;

            if (eventData.EventId != EventIds.CutsceneFinished)
                return;

            CutsceneFinishedData data = eventData as CutsceneFinishedData;

            if (data.Id != _cutsceneId.Value)
                return;

            base.Process(data);

            CurrentAmount++;
            Evaluate();
        }
    }
}
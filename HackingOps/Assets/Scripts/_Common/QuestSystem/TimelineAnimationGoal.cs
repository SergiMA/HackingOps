using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Common.QuestSystem
{
    public class TimelineAnimationGoal : Goal
    {
        [SerializeField] private CutsceneId _cutsceneId;
        [SerializeField] private AnimationStatusCapture _statusCapture = AnimationStatusCapture.OnFinished;

        enum AnimationStatusCapture
        {
            OnStarted,
            OnFinished,
        }

        #region Goal overrides
        public override void Init()
        {
            base.Init();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();
            
            IEventQueue eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

            switch (_statusCapture)
            {
                case AnimationStatusCapture.OnStarted:
                    eventQueue.Subscribe(EventIds.CutsceneStarted, this);
                    break;
                case AnimationStatusCapture.OnFinished:
                    eventQueue.Subscribe(EventIds.CutsceneFinished, this);
                    break;
            }
        }

        public override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
        }

        public override void Process(EventData eventData)
        {
            if (IsCompleted)
                return;

            bool hasReceivedCutsceneStarted = eventData.EventId == EventIds.CutsceneStarted;
            bool hasReceivedCutsceneFinished = eventData.EventId == EventIds.CutsceneFinished;

            if (!hasReceivedCutsceneStarted && !hasReceivedCutsceneFinished)
                return;

            switch (_statusCapture)
            {
                case AnimationStatusCapture.OnStarted:
                    CutsceneStartedData cutsceneStartedData = eventData as CutsceneStartedData;
                    if (cutsceneStartedData.Id != _cutsceneId.Value)
                        return;

                    base.Process(cutsceneStartedData);
                    break;
                case AnimationStatusCapture.OnFinished:
                    CutsceneFinishedData cutsceneFinishedData = eventData as CutsceneFinishedData;
                    if (cutsceneFinishedData.Id != _cutsceneId.Value)
                        return;

                    base.Process(cutsceneFinishedData);
                    break;
            }

            CurrentAmount++;
            Evaluate();
        }
        #endregion
    }
}
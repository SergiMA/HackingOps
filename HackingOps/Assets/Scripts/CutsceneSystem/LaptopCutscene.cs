using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    public class LaptopCutscene : MonoBehaviour, IEventObserver
    {
        [SerializeField] private CutsceneSetup _cutsceneSetup;
        [SerializeField] private TimelinePlayer _timelinePlayer;

        [SerializeField] private bool _debugStandUp;

        private void OnValidate()
        {
            if (_debugStandUp)
            {
                _debugStandUp = false;
                PlayGetUpCutscene();
            }
        }

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.LeaveHackingMode, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.LeaveHackingMode, this);
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.StopUsingLaptop)
                PlayGetUpCutscene();

            switch (eventData.EventId)
            {
                case EventIds.StopUsingLaptop:
                    PlayGetUpCutscene();
                    break;
                case EventIds.LeaveHackingMode:
                    PlayGetUpAfterHacking();
                    break;
            }

        }

        public void PlayGetUpCutscene()
        {
            _cutsceneSetup.Play();
        }

        public void PlayGetUpAfterHacking()
        {
            _timelinePlayer.Play();
        }
    }
}
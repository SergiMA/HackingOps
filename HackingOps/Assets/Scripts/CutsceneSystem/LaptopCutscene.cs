using HackingOps.Common.Events;
using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    public class LaptopCutscene : MonoBehaviour, IEventObserver
    {
        [SerializeField] private CutsceneSetup _cutsceneSetup;

        [SerializeField] private bool _debugStandUp;

        private void OnValidate()
        {
            if (_debugStandUp)
            {
                _debugStandUp = false;
                PlayGetUpCutscene();
            }
        }

        public void Process(EventData eventData)
        {
            if (eventData.EventId != EventIds.StopUsingLaptop)
                return;

            PlayGetUpCutscene();
        }

        public void PlayGetUpCutscene()
        {
            _cutsceneSetup.Play();
        }
    }
}
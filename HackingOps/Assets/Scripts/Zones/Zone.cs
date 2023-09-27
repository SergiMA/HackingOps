using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Zones
{
    /// <summary>
    /// Represents a physical space. It is used to detect the presence of any GameObject. If the target GameObject gets inside the zone
    /// an event notifying it will be raised.
    /// 
    /// The zone consist of one or more Triggers. Once a Trigger has raised an event, it will get deactivated.
    /// </summary>
    public class Zone : MonoBehaviour
    {
        private Trigger[] _triggers;

        private void Awake()
        {
            _triggers = GetComponentsInChildren<Trigger>();
        }

        private void OnEnable()
        {
            foreach (Trigger trigger in _triggers)
                trigger.OnStepped += OnZoneStepped;
        }

        private void OnDisable()
        {
            foreach (Trigger trigger in _triggers)
                trigger.OnStepped -= OnZoneStepped;
        }

        private void ActivateTriggers()
        {
            foreach (Trigger trigger in _triggers)
                trigger.gameObject.SetActive(true);
        }

        private void DeactivateTriggers()
        {
            foreach (Trigger trigger in _triggers)
                trigger.gameObject.SetActive(false);
        }

        private void DeactivateTrigger(Trigger trigger)
        {
            trigger.gameObject.SetActive(false);
        }

        public void EnableZone()
        {
            ActivateTriggers();
        }

        public void OnZoneStepped(Trigger trigger)
        {
            DeactivateTrigger(trigger);

            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new ZoneSteppedData(this));
        }
    }
}
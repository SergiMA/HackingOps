using UnityEngine;

namespace HackingOps.Audio.AudioZones.States
{
    public abstract class AudioZoneBaseState
    {
        protected AudioZone _ctx;
        protected AudioZoneStateFactory _factory;

        public AudioZoneBaseState(AudioZone ctx, AudioZoneStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected void SwitchState(AudioZoneBaseState newState)
        {
            ExitState();
            newState.EnterState();

            _ctx.CurrentState = newState;
        }

        protected abstract void CheckSwitchState();
        public abstract void OnTriggerStay(Collider other);
    }
}
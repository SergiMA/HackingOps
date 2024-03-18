using UnityEngine;

namespace HackingOps.Audio.AudioZones.States
{
    public class Blended3DState : AudioZoneBaseState
    {
        bool _steppedInsideZone;

        public Blended3DState(AudioZone ctx, AudioZoneStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() { _ctx.CurrentBlendingProgress = 1f; }

        public override void UpdateState() => CheckSwitchState();

        public override void ExitState() => _steppedInsideZone = false;

        protected override void CheckSwitchState()
        {
            if (_steppedInsideZone)
                SwitchState(_factory.GetState(AudioZoneStateFactory.States.Blending2D));
        }

        public override void OnTriggerStay(Collider other) => _steppedInsideZone = true;
    }
}
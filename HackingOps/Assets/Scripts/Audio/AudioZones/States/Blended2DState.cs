using HackingOps.Utilities.Timers;
using UnityEngine;

namespace HackingOps.Audio.AudioZones.States
{
    public class Blended2DState : AudioZoneBaseState
    {
        private readonly CountdownTimer _countdownTimer;
        private bool _timerEnded;

        public Blended2DState(AudioZone ctx, AudioZoneStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;

            _countdownTimer = new CountdownTimer(_ctx.MinDurationInsideZone);
            _countdownTimer.OnStop += () => _timerEnded = true;
        }

        public override void EnterState()
        {
            _countdownTimer.Start();

            _ctx.CurrentBlendingProgress = 0f;
            _ctx.Follower.enabled = true;
        }

        public override void UpdateState()
        {
            _countdownTimer.Tick(Time.deltaTime);
            CheckSwitchState();
        }

        public override void ExitState()
        {
            _timerEnded = false;
            _ctx.Follower.enabled = false;
        }

        protected override void CheckSwitchState()
        {
            if (_timerEnded)
                SwitchState(_factory.GetState(AudioZoneStateFactory.States.Blending3D));
        }

        public override void OnTriggerStay(Collider other) => _countdownTimer.Reset();
    }
}
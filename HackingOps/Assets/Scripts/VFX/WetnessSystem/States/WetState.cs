using HackingOps.Utilities.Timers;
using UnityEngine;

namespace HackingOps.VFX.WetnessSystem.States
{
    public class WetState : WetTargetBaseState
    {
        private readonly CountdownTimer _countdownTimer;
        private bool _timerEnded;

        public WetState(WetTarget ctx, WetTargetStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;

            _countdownTimer = new CountdownTimer(_ctx.MinWetDuration);
            _countdownTimer.OnStop += () => _timerEnded = true;
        }

        public override void EnterState() { _countdownTimer.Start(); }
        public override void UpdateState()
        {
            _countdownTimer.Tick(Time.deltaTime);
            CheckSwitchState();
        }

        public override void ExitState() { _timerEnded = false; }
        protected override void CheckSwitchState()
        {
            if (_timerEnded)
                SwitchState(_factory.GetState(WetTargetStateFactory.States.Drying));
        }

        public override void OnTriggerStay(Collider other)
        {
            _countdownTimer.Reset();
        }
    }
}
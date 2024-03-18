using UnityEngine;

namespace HackingOps.VFX.WetnessSystem.States
{
    public abstract class WetTargetBaseState
    {
        protected WetTarget _ctx;
        protected WetTargetStateFactory _factory;

        public WetTargetBaseState(WetTarget ctx, WetTargetStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected void SwitchState(WetTargetBaseState newState)
        {
            ExitState();
            newState.EnterState();

            _ctx.CurrentState = newState;
        }

        protected abstract void CheckSwitchState();
        public abstract void OnTriggerStay(Collider other);
    }
}
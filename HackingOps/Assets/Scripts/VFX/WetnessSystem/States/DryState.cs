using UnityEngine;

namespace HackingOps.VFX.WetnessSystem.States
{
    public class DryState : WetTargetBaseState
    {
        private bool _gotWet;

        public DryState(WetTarget ctx, WetTargetStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() { }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void ExitState()
        {
            _gotWet = false;
        }

        protected override void CheckSwitchState()
        {
            if (_gotWet)
                SwitchState(_factory.GetState(WetTargetStateFactory.States.Wetting));
        }

        public override void OnTriggerStay(Collider other)
        {
            _gotWet = true;
        }
    }
}
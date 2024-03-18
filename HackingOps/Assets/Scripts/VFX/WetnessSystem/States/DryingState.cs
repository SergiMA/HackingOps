using UnityEngine;

namespace HackingOps.VFX.WetnessSystem.States
{
    public class DryingState : WetTargetBaseState
    {
        private bool _gotWet;

        public DryingState(WetTarget ctx, WetTargetStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() { }
        public override void UpdateState()
        {
            DecreaseWetness();
            CheckSwitchState();
        }

        public override void ExitState()
        {
            _gotWet = false;
        }

        protected override void CheckSwitchState()
        {
            if (_ctx.CurrentWetnessProgress <= 0)
                SwitchState(_factory.GetState(WetTargetStateFactory.States.Dry));
            else if (_gotWet)
                SwitchState(_factory.GetState(WetTargetStateFactory.States.Wetting));
        }

        public override void OnTriggerStay(Collider other)
        {
            _gotWet = true;
        }

        private void DecreaseWetness()
        {
            DecreaseWetnessProgress();
            ApplyWetnessProgressToAllMaterials();
        }

        private void DecreaseWetnessProgress()
        {
            _ctx.CurrentWetnessProgress -= (1f / _ctx.DryingDuration) * Time.deltaTime;
            _ctx.CurrentWetnessProgress = Mathf.Max(_ctx.CurrentWetnessProgress, 0f);
        }

        private void ApplyWetnessProgressToAllMaterials()
        {
            foreach (Material material in _ctx.Materials)
            {
                material.SetFloat(_ctx.Hash_WetnessProgress, _ctx.CurrentWetnessProgress);
            }
        }
    }
}
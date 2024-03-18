using UnityEngine;

namespace HackingOps.VFX.WetnessSystem.States
{
    public class WettingState : WetTargetBaseState
    {
        public WettingState(WetTarget ctx, WetTargetStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() { }
        public override void UpdateState()
        {
            IncreaseWetness();
            CheckSwitchState();
        }

        public override void ExitState() { }

        protected override void CheckSwitchState()
        {
            if (_ctx.CurrentWetnessProgress >= 1f)
                SwitchState(_factory.GetState(WetTargetStateFactory.States.Wet));
        }

        public override void OnTriggerStay(Collider other) { }

        private void IncreaseWetness()
        {
            IncreaseWetnessProgress();
            ApplyWetnessProgressToAllMaterials();
        }

        private void IncreaseWetnessProgress()
        {
            _ctx.CurrentWetnessProgress += (1f / _ctx.WettingDuration) * Time.deltaTime;
            _ctx.CurrentWetnessProgress = Mathf.Min(_ctx.CurrentWetnessProgress, 1f);
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
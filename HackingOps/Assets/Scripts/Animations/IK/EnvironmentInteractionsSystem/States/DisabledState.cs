using DG.Tweening;
using System.Diagnostics;

namespace HackingOps.Animations.IK.EnvironmentInteractions.States
{
    public class DisabledState : EnvironmentInteractorBaseState
    {
        private bool _mustEnable;

        public DisabledState(EnvironmentInteractor ctx, EnvironmentInteractorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() => ChangeRigWeight(0f);
        public override void UpdateState() => CheckSwitchState();
        public override void ExitState() => ChangeRigWeight(1f);

        protected override void CheckSwitchState()
        {
            if (!_ctx.IsDisabled)
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Search));
        }

        public override void EnableSystem() => _ctx.IsDisabled = false;

        private void ChangeRigWeight(float newWeight)
        {
            DOVirtual.Float(_ctx.EnvironmentInteractionRig.weight,
                            newWeight,
                            _ctx.AnimationTransitionDuration,
                            weight => { _ctx.EnvironmentInteractionRig.weight = weight; });
        }
    }
}

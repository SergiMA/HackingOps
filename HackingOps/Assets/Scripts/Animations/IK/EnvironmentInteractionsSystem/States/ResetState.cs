using DG.Tweening;
using UnityEngine;

namespace HackingOps.Animations.IK.EnvironmentInteractions.States
{
    public class ResetState : EnvironmentInteractorBaseState
    {
        private bool _armIkTransitionFinished;
        private bool _armMultiRotationTransitionFinished;

        public ResetState(EnvironmentInteractor ctx, EnvironmentInteractorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
            ChangeConstraintsWeight(0f);
            _ctx.ClosestPointPosition = Vector3.positiveInfinity;
        }

        public override void UpdateState() => CheckSwitchState();

        public override void ExitState()
        {
            _armIkTransitionFinished = false;
            _armMultiRotationTransitionFinished = false;
        }

        protected override void CheckSwitchState()
        {
            if (_armIkTransitionFinished && _armMultiRotationTransitionFinished)
            {
                if (_ctx.IsDisabled)
                    SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Disabled));
                else
                    SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Search));
            }
        }

        public override void DisableSystem() => _ctx.IsDisabled = true;
        public override void EnableSystem() => _ctx.IsDisabled = false;

        private void ChangeConstraintsWeight(float newWeight)
        {
            DOVirtual.Float(_ctx.ArmIkConstraint.weight,
                            newWeight,
                            _ctx.AnimationTransitionDuration,
                            weight => { _ctx.ArmIkConstraint.weight = weight; })
                .OnComplete(() => { _armIkTransitionFinished = true; });

            DOVirtual.Float(_ctx.ArmMultiRotationConstraint.weight,
                            newWeight,
                            _ctx.AnimationTransitionDuration,
                            weight => { _ctx.ArmMultiRotationConstraint.weight = weight; })
                .OnComplete(() => { _armMultiRotationTransitionFinished = true; });
        }
    }
}

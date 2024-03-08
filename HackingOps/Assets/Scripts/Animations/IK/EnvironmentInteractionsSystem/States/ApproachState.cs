using DG.Tweening;
using HackingOps.Utilities.Timers;
using UnityEngine;

namespace HackingOps.Animations.IK.EnvironmentInteractions.States
{
    public class ApproachState : EnvironmentInteractorBaseState
    {
        private bool _noTargetDetected;

        private bool _armIkTransitionFinished;
        private bool _armMultiRotationTransitionFinished;

        private readonly CountdownTimer _decreasingTimer;

        public ApproachState(EnvironmentInteractor ctx, EnvironmentInteractorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;

            _decreasingTimer = new CountdownTimer(_ctx.ApproachMinimumDuration);
            _decreasingTimer.OnStop += () => _noTargetDetected = true;
        }

        public override void EnterState() => ChangeConstraintsWeights(_ctx.ApproachWeight);

        public override void UpdateState()
        {
            _decreasingTimer.Tick(Time.deltaTime);
            CheckSwitchState();
        }

        public override void ExitState()
        {
            _noTargetDetected = false;
            _armIkTransitionFinished = false;
            _armMultiRotationTransitionFinished = false;
        }

        protected override void CheckSwitchState()
        {
            bool isClosestPointAvailable = _ctx.ClosestPointPosition != Vector3.positiveInfinity;
            bool isInTouchThreshold = Vector3.Distance(_ctx.ClosestPointPosition,
                                                      _ctx.ShoulderTransform.position) < _ctx.TouchDistanceThreshold;
            bool isInsideAngleThreshold = _ctx.CosTheta >= _ctx.MinDotAngleAllowed && _ctx.CosTheta <= _ctx.MaxDotAngleAllowed;

            if (_ctx.IsDisabled)
            {
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Reset));
                return;
            }

            if (isClosestPointAvailable
                && isInTouchThreshold
                && _armIkTransitionFinished
                && _armMultiRotationTransitionFinished
                && isInsideAngleThreshold)
            {
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Touch));
            }

            if (_noTargetDetected || !isInsideAngleThreshold)
            {
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Reset));
            }
        }

        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (_ctx.TargetPointPosition != Vector3.positiveInfinity)
                _ctx.IkTargetTransform.position = _ctx.TargetPointPosition;
        }

        public override void OnTriggerStay(Collider other)
        {
            base.OnTriggerStay(other);

            _decreasingTimer.Reset();
        }

        public override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);

            _decreasingTimer.Start();
        }

        public override void DisableSystem() => _ctx.IsDisabled = true;
        public override void EnableSystem() => _ctx.IsDisabled = false;

        private void ChangeConstraintsWeights(float newWeight)
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

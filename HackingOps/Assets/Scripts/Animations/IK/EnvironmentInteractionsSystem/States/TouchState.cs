using DG.Tweening;
using HackingOps.Utilities.Timers;
using UnityEngine;

namespace HackingOps.Animations.IK.EnvironmentInteractions.States
{
    public class TouchState : EnvironmentInteractorBaseState
    {
        private bool _timeWithOutCollisionExpired;

        private readonly CountdownTimer _decreasingTimer;

        public TouchState(EnvironmentInteractor ctx, EnvironmentInteractorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;

            _decreasingTimer = new CountdownTimer(_ctx.ApproachMinimumDuration);
            _decreasingTimer.OnStop += () => _timeWithOutCollisionExpired = true;
        }

        public override void EnterState()
        {
            ChangeConstraintsWeights(_ctx.TouchWeight);

            ElevateTargetPoint();
            _decreasingTimer.Start();
        }

        public override void UpdateState()
        {
            _decreasingTimer.Tick(Time.deltaTime);
            CheckSwitchState();
        }

        public override void ExitState()
        {
            _decreasingTimer.Stop();
            _timeWithOutCollisionExpired = false;
        }

        protected override void CheckSwitchState()
        {
            bool isInTouchThreshold = Vector3.Distance(_ctx.ClosestPointPosition,
                                                      _ctx.ShoulderTransform.position) < _ctx.TouchDistanceThreshold;

            if (_ctx.IsDisabled)
            {
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Reset));
                return;
            }

            if (_timeWithOutCollisionExpired || !isInTouchThreshold)
            {
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Reset));
            }

        }

        public override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            _decreasingTimer.Start();
        }

        public override void OnTriggerStay(Collider other)
        {
            Vector3 referenceLocation = new(_ctx.ShoulderTransform.position.x,
                                            _ctx.ShoulderTransform.position.y,
                                            _ctx.ShoulderTransform.position.z);

            Vector3 predictionOffset = _ctx.CharacterController.velocity * _ctx.PredictionDistance;

            _ctx.ClosestPointPosition = other.ClosestPoint(referenceLocation + predictionOffset);
            _ctx.TargetPointPosition = new Vector3(_ctx.ClosestPointPosition.x,
                                                 _ctx.TargetPointPositionYOffset,
                                                 _ctx.ClosestPointPosition.z);

            _ctx.IkTargetTransform.position = _ctx.TargetPointPosition;

            Vector3 closestPointPositionFlatened = new(_ctx.ClosestPointPosition.x,
                                                       _ctx.RootTransform.position.y,
                                                       _ctx.ClosestPointPosition.z);
            Vector3 directionToClosestPoint = (closestPointPositionFlatened - _ctx.RootTransform.position).normalized;
            _ctx.CosTheta = Vector3.Dot(_ctx.RootTransform.forward, directionToClosestPoint);

            _decreasingTimer.Reset();
        }

        public override void OnTriggerExit(Collider other) => base.OnTriggerExit(other);

        public override void DisableSystem() => _ctx.IsDisabled = true;
        public override void EnableSystem() => _ctx.IsDisabled = false;

        private void ChangeConstraintsWeights(float newWeight)
        {
            DOVirtual.Float(_ctx.ArmIkConstraint.weight,
                            newWeight,
                            _ctx.AnimationTransitionDuration,
                            weight => { _ctx.ArmIkConstraint.weight = weight; });

            DOVirtual.Float(_ctx.ArmMultiRotationConstraint.weight,
                            newWeight,
                            _ctx.AnimationTransitionDuration,
                            weight => { _ctx.ArmMultiRotationConstraint.weight = weight; });
        }

        private void ElevateTargetPoint()
        {
            DOVirtual.Float(_ctx.TargetPointPositionYOffset,
                            _ctx.ClosestPointPosition.y,
                            _ctx.AnimationTransitionDuration,
                            height =>
                            {
                                _ctx.TargetPointPositionYOffset = height;
                                if (_ctx.TargetPointPositionYOffset == Mathf.Infinity)
                                    _ctx.TargetPointPositionYOffset = _ctx.TargetPointPosition.y;
                            });
        }
    }
}

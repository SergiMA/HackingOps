using DG.Tweening;
using UnityEngine;

namespace HackingOps.Animations.IK.EnvironmentInteractions.States
{
    public class SearchState : EnvironmentInteractorBaseState
    {
        public SearchState(EnvironmentInteractor ctx, EnvironmentInteractorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        #region State structure
        public override void EnterState() { }
        public override void UpdateState() => CheckSwitchState();
        public override void ExitState() { }

        protected override void CheckSwitchState()
        {
            bool isClosestPointAvailable = _ctx.ClosestPointPosition != Vector3.positiveInfinity;
            bool isInApproachThreshold = Vector3.Distance(_ctx.ClosestPointPosition,
                                                          _ctx.ShoulderTransform.position) < _ctx.ApproachDistanceThreshold;

            if (_ctx.IsDisabled)
            {
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Disabled));
                return;
            }

            if (isClosestPointAvailable && isInApproachThreshold)
                SwitchState(_factory.GetState(EnvironmentInteractorStateFactory.States.Approach));
        }
        #endregion

        public override void OnTriggerEnter(Collider other) => base.OnTriggerEnter(other);
        public override void OnTriggerStay(Collider other) => base.OnTriggerStay(other);
        public override void OnTriggerExit(Collider other) => base.OnTriggerExit(other);

        public override void DisableSystem() => _ctx.IsDisabled = true;
        public override void EnableSystem() => _ctx.IsDisabled = false;
    }
}

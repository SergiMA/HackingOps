using UnityEngine;

namespace HackingOps.Animations.IK.EnvironmentInteractions.States
{
    public abstract class EnvironmentInteractorBaseState
    {
        protected EnvironmentInteractor _ctx;
        protected EnvironmentInteractorStateFactory _factory;

        public EnvironmentInteractorBaseState(EnvironmentInteractor ctx, EnvironmentInteractorStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected void SwitchState(EnvironmentInteractorBaseState newState)
        {
            ExitState();
            newState.EnterState();

            _ctx.CurrentState = newState;
        }

        protected abstract void CheckSwitchState();

        public virtual void OnTriggerEnter(Collider other)
        {
            _ctx.CurrentColliderTarget = other;

            Vector3 referenceLocation = new(_ctx.ShoulderTransform.position.x,
                                            _ctx.ShoulderTransform.position.y - _ctx.ArmLength,
                                            _ctx.ShoulderTransform.position.z);

            _ctx.ClosestPointPosition = other.ClosestPoint(referenceLocation);
            _ctx.TargetPointPosition = new Vector3(_ctx.ClosestPointPosition.x, 0, _ctx.ClosestPointPosition.z);
            _ctx.IkTargetTransform.position = _ctx.TargetPointPosition;
        }
        public virtual void OnTriggerStay(Collider other)
        {
            if (other != _ctx.CurrentColliderTarget) return;

            Vector3 referenceLocation = new(_ctx.ShoulderTransform.position.x,
                                            _ctx.ShoulderTransform.position.y,
                                            _ctx.ShoulderTransform.position.z);

            Vector3 predictionOffset = _ctx.CharacterController.velocity * _ctx.PredictionDistance;

            _ctx.ClosestPointPosition = other.ClosestPoint(referenceLocation + predictionOffset);
            _ctx.TargetPointPosition = new Vector3(_ctx.ClosestPointPosition.x, 0, _ctx.ClosestPointPosition.z);
            _ctx.IkTargetTransform.position = _ctx.TargetPointPosition;

            Vector3 closestPointPositionFlatened = new(_ctx.ClosestPointPosition.x,
                                                       _ctx.RootTransform.position.y,
                                                       _ctx.ClosestPointPosition.z);
            Vector3 directionToClosestPoint = (closestPointPositionFlatened - _ctx.RootTransform.position).normalized;
            _ctx.CosTheta = Vector3.Dot(_ctx.RootTransform.forward, directionToClosestPoint);
        }
        public virtual void OnTriggerExit(Collider other)
        {
            if (other != _ctx.CurrentColliderTarget) return;

            _ctx.CurrentColliderTarget = null;
            _ctx.ClosestPointPosition = Vector3.positiveInfinity;
        }

        public virtual void EnableSystem() { }
        public virtual void DisableSystem() { }
    }
}

using UnityEngine;

namespace HackingOps.Doors.AnimatedDoors.States
{
    public class AnimatedDoorOpenedState : AnimatedDoorBaseState
    {
        public AnimatedDoorOpenedState(AnimatedDoor ctx, AnimatedDoorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
            _ctx.Progress = 1f;
            _ctx.Animator.SetFloat("Progress", 1f);
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public override void Open()
        {
        }

        public override void Close()
        {
            SwitchState(_factory.GetState(AnimatedDoorStateFactory.States.Closing));
        }
    }
}
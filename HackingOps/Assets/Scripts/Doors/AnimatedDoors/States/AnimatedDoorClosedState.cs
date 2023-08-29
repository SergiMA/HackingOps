using UnityEngine;

namespace HackingOps.Doors.AnimatedDoors.States
{
    public class AnimatedDoorClosedState : AnimatedDoorBaseState
    {
        public AnimatedDoorClosedState(AnimatedDoor ctx, AnimatedDoorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
            _ctx.Progress = 0f;
            _ctx.Animator.SetFloat("Progress", 0f);

            if (!_ctx.IsStartingState)
            {
                _ctx.OnDoorClosed.Invoke();
            }
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public override void Open()
        {
            SwitchState(_factory.GetState(AnimatedDoorStateFactory.States.Opening));
        }

        public override void Close()
        {
        }
    }
}
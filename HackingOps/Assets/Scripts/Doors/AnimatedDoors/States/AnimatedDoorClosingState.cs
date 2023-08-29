using UnityEngine;

namespace HackingOps.Doors.AnimatedDoors.States
{
    public class AnimatedDoorClosingState : AnimatedDoorBaseState
    {
        public AnimatedDoorClosingState(AnimatedDoor ctx, AnimatedDoorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            _ctx.Progress -= (1f / _ctx.ClosingDuration) * Time.deltaTime;
            _ctx.Progress = Mathf.Max(_ctx.Progress, 0f);

            _ctx.Animator.SetFloat("Progress", _ctx.Progress);

            CheckSwitchState();
        }

        public override void ExitState()
        {
        }

        protected override void CheckSwitchState()
        {
            if (_ctx.Progress <= 0f)
            {
                SwitchState(_factory.GetState(AnimatedDoorStateFactory.States.Closed));
            }
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
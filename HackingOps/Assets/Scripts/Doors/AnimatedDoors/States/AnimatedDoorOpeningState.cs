using UnityEngine;

namespace HackingOps.Doors.AnimatedDoors.States
{
    public class AnimatedDoorOpeningState : AnimatedDoorBaseState
    {
        public AnimatedDoorOpeningState(AnimatedDoor ctx, AnimatedDoorStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            _ctx.Progress += (1f / _ctx.OpeningDuration) * Time.deltaTime;
            _ctx.Progress = Mathf.Min(_ctx.Progress, 1f);

            _ctx.Animator.SetFloat("Progress", _ctx.Progress);

            CheckSwitchState();
        }

        public override void ExitState()
        {
        }

        protected override void CheckSwitchState()
        {
            if (_ctx.Progress >= 1f)
            {
                SwitchState(_factory.GetState(AnimatedDoorStateFactory.States.Opened));
            }
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
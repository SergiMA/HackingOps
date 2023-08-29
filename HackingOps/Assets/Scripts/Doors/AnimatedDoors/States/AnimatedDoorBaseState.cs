namespace HackingOps.Doors.AnimatedDoors.States
{
    public abstract class AnimatedDoorBaseState
    {
        protected AnimatedDoor _ctx;
        protected AnimatedDoorStateFactory _factory;

        public AnimatedDoorBaseState(AnimatedDoor ctx, AnimatedDoorStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected void SwitchState(AnimatedDoorBaseState newState)
        {
            ExitState();
            newState.EnterState();

            _ctx.CurrentState = newState;

            _ctx.IsStartingState = false;
        }

        protected virtual void CheckSwitchState() { }
        public abstract void Open();
        public abstract void Close();
    }
}
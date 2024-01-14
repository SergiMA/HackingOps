namespace HackingOps.CombatSystem
{
    public abstract class CombatModeBaseState
    {
        protected CombatMode _ctx;
        protected CombatModeStateFactory _factory;

        public CombatModeBaseState(CombatMode ctx, CombatModeStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        
        protected void SwitchState(CombatModeBaseState newState)
        {
            ExitState();
            newState.EnterState();

            _ctx.CurrentState = newState;
        }

        protected virtual void CheckSwitchState() { }

        public abstract void CheckEntitiesInCombat();
    }
}
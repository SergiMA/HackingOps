using System.Collections;

namespace HackingOps.Characters.Player.WeaponSheatherSystem.States
{
    public abstract class WeaponSheatherBaseState
    {
        protected WeaponSheather _ctx;
        protected WeaponSheatherStateFactory _factory;

        public WeaponSheatherBaseState(WeaponSheather ctx, WeaponSheatherStateFactory factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected void SwitchState(WeaponSheatherBaseState newState)
        {
            ExitState();
            newState.EnterState();

            _ctx.CurrentState = newState;
        }

        protected abstract void CheckSwitchState();

        public virtual void OnEnterCombatMode() { }
        public virtual void OnEnterAlertMode() { }
        public virtual void OnExitCombatMode() { }
        public virtual void OnStopBlocking() { }
    }
}
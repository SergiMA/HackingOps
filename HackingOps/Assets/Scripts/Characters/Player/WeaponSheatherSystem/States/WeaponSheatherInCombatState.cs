namespace HackingOps.Characters.Player.WeaponSheatherSystem.States
{
    public class WeaponSheatherInCombatState : WeaponSheatherBaseState
    {
        public WeaponSheatherInCombatState(WeaponSheather ctx, WeaponSheatherStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() { _ctx.PlayerWeapons.Unsheath(); }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void ExitState() { }

        protected override void CheckSwitchState()
        {
            if (!_ctx.IsEngagedInCombat && !_ctx.IsBlocking && _ctx.IsUsingMeleeWeapon)
                SwitchState(_factory.GetState(WeaponSheatherStateFactory.States.Alert));
        }
    }
}
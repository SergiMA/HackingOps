using System.Diagnostics;

namespace HackingOps.Characters.Player.WeaponSheatherSystem.States
{
    public class WeaponSheatherPeacefulState : WeaponSheatherBaseState
    {
        private bool _hasBeenAlerted;

        public WeaponSheatherPeacefulState(WeaponSheather ctx, WeaponSheatherStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
            _hasBeenAlerted = false;
            _ctx.PlayerWeapons.Sheathe();
        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }

        public override void ExitState() => _ctx.PlayerWeapons.Unsheath();

        protected override void CheckSwitchState()
        {
            if (_ctx.IsEngagedInCombat || _ctx.IsBlocking)
                SwitchState(_factory.GetState(WeaponSheatherStateFactory.States.InCombat));

            if (_hasBeenAlerted)
                SwitchState(_factory.GetState(WeaponSheatherStateFactory.States.Alert));
        }

        public override void OnEnterAlertMode() => _hasBeenAlerted = true;
    }
}
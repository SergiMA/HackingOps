using HackingOps.Utilities.Timers;
using UnityEngine;

namespace HackingOps.Characters.Player.WeaponSheatherSystem.States
{
    public class WeaponSheatherAlertState : WeaponSheatherBaseState
    {
        private bool _needsToSheatheWeapon;
        private readonly CountdownTimer _countdownTimer;

        public WeaponSheatherAlertState(WeaponSheather ctx, WeaponSheatherStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
            _countdownTimer = new CountdownTimer(_ctx.CoolingDownDuration);
        }

        public override void EnterState()
        {
            _needsToSheatheWeapon = false;
            _ctx.PlayerWeapons.Unsheath();

            _countdownTimer.OnStop += OnTimerEnded;
            _countdownTimer.Start();
        }

        public override void UpdateState()
        {
            _countdownTimer.Tick(Time.deltaTime);
            CheckSwitchState();
        }

        public override void ExitState() 
        {
            _countdownTimer.OnStop -= OnTimerEnded;
            _countdownTimer.Stop(); 
        }

        protected override void CheckSwitchState()
        {
            if (_ctx.IsEngagedInCombat || !_ctx.IsUsingMeleeWeapon)
                SwitchState(_factory.GetState(WeaponSheatherStateFactory.States.InCombat));
            
            if (_needsToSheatheWeapon)
                SwitchState(_factory.GetState(WeaponSheatherStateFactory.States.Peaceful));

            if (_ctx.IsInCutscene)
                SwitchState(_factory.GetState(WeaponSheatherStateFactory.States.Peaceful));
        }

        public override void OnEnterAlertMode()
        {
            _ctx.PlayerWeapons.Unsheath();
            _countdownTimer.Reset();
        }

        private void OnTimerEnded() => _needsToSheatheWeapon = true;
    }
}
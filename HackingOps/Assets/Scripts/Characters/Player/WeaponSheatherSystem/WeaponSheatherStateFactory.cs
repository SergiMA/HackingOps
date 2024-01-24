using HackingOps.Characters.Player.WeaponSheatherSystem.States;
using System.Collections.Generic;

namespace HackingOps.Characters.Player.WeaponSheatherSystem
{
    public class WeaponSheatherStateFactory
    {
        public enum States
        {
            Peaceful,
            InCombat,
            Alert,
        }

        private WeaponSheather _ctx;
        private Dictionary<States, WeaponSheatherBaseState> _states = new();

        public WeaponSheatherStateFactory(WeaponSheather ctx)
        {
            _ctx = ctx;

            _states.Add(States.Peaceful, new WeaponSheatherPeacefulState(_ctx, this));
            _states.Add(States.InCombat, new WeaponSheatherInCombatState(_ctx, this));
            _states.Add(States.Alert, new WeaponSheatherAlertState(_ctx, this));
        }

        public WeaponSheatherBaseState GetState(States state) => _states[state];
    }
}
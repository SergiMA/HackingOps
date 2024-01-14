using System.Collections.Generic;

namespace HackingOps.CombatSystem
{
    public class CombatModeStateFactory
    {
        public enum States
        {
            Peaceful,
            InCombat,
            CoolingDown,
        }

        private CombatMode _ctx;
        private Dictionary<States, CombatModeBaseState> _states = new();

        public CombatModeStateFactory(CombatMode ctx)
        {
            _ctx = ctx;

            _states.Add(States.Peaceful, new CombatModePeacefulState(_ctx, this));
            _states.Add(States.InCombat, new CombatModeInCombatState(_ctx, this));
            _states.Add(States.CoolingDown, new CombatModeCoolingDownState(_ctx, this));
        }

        public CombatModeBaseState GetState(States state) => _states[state];
    }
}
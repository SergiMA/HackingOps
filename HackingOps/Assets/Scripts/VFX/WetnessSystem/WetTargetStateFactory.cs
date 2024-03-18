using HackingOps.VFX.WetnessSystem.States;
using System.Collections.Generic;

namespace HackingOps.VFX.WetnessSystem
{
    public class WetTargetStateFactory
    {
        public enum States
        {
            Wetting,
            Wet,
            Drying,
            Dry,
        }

        private WetTarget _ctx;
        private Dictionary<States, WetTargetBaseState> _states = new();

        public WetTargetStateFactory(WetTarget ctx)
        {
            _ctx = ctx;

            _states.Add(States.Wetting, new WettingState(_ctx, this));
            _states.Add(States.Wet, new WetState(_ctx, this));
            _states.Add(States.Drying, new DryingState(_ctx, this));
            _states.Add(States.Dry, new DryState(_ctx, this));
        }

        public WetTargetBaseState GetState(States state) => _states[state];
    }
}
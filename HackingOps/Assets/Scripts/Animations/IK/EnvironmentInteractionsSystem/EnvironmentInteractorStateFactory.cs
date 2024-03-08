using HackingOps.Animations.IK.EnvironmentInteractions.States;
using System.Collections.Generic;

namespace HackingOps.Animations.IK.EnvironmentInteractions
{
    public class EnvironmentInteractorStateFactory
    {
        public enum States
        {
            Disabled,
            Search,
            Approach,
            Touch,
            Reset,
        }

        private EnvironmentInteractor _ctx;
        private Dictionary<States, EnvironmentInteractorBaseState> _states = new();

        public EnvironmentInteractorStateFactory(EnvironmentInteractor ctx)
        {
            _ctx = ctx;

            _states.Add(States.Disabled, new DisabledState(_ctx, this));
            _states.Add(States.Search, new SearchState(_ctx, this));
            _states.Add(States.Approach, new ApproachState(_ctx, this));
            _states.Add(States.Touch, new TouchState(_ctx, this));
            _states.Add(States.Reset, new ResetState(_ctx, this));
        }

        public EnvironmentInteractorBaseState GetState(States state) => _states[state];
    }
}

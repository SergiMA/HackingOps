using HackingOps.Doors.AnimatedDoors.States;
using System.Collections.Generic;

namespace HackingOps.Doors.AnimatedDoors
{
    public class AnimatedDoorStateFactory
    {
        public enum States
        {
            Closed,
            Closing,
            Opened,
            Opening
        }

        AnimatedDoor _ctx;
        Dictionary<States, AnimatedDoorBaseState> _states = new Dictionary<States, AnimatedDoorBaseState>();

        public AnimatedDoorStateFactory(AnimatedDoor ctx)
        {
            _ctx = ctx;

            _states.Add(States.Opening, new AnimatedDoorOpeningState(_ctx, this));
            _states.Add(States.Closing, new AnimatedDoorClosingState(_ctx, this));
            _states.Add(States.Closed, new AnimatedDoorClosedState(_ctx, this));
            _states.Add(States.Opened, new AnimatedDoorOpenedState(_ctx, this));
        }

        public AnimatedDoorBaseState GetState(States state)
        {
            return _states[state];
        }
    }
}
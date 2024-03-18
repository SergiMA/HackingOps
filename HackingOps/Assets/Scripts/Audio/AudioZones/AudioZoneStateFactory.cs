using HackingOps.Audio.AudioZones.States;
using System.Collections.Generic;

namespace HackingOps.Audio.AudioZones
{
    public class AudioZoneStateFactory
    {
        public enum States
        {
            Blending3D,
            Blended3D,
            Blending2D,
            Blended2D,
        }

        private AudioZone _ctx;
        private Dictionary<States, AudioZoneBaseState> _states = new();

        public AudioZoneStateFactory(AudioZone ctx)
        {
            _ctx = ctx;

            CreateStates();
        }

        private void CreateStates()
        {
            _states.Add(States.Blending3D, new Blending3DState(_ctx, this));
            _states.Add(States.Blended3D, new Blended3DState(_ctx, this));
            _states.Add(States.Blending2D, new Blending2DState(_ctx, this));
            _states.Add(States.Blended2D, new Blended2DState(_ctx, this));
        }

        public AudioZoneBaseState GetState(States state) => _states[state];
    }
}
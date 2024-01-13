using HackingOps.Characters.NPC.Allegiance;
using System.Collections.Generic;
using UnityEngine;


namespace HackingOps.Characters.NPC.Senses.HearingSense
{
    public class Hearing : MonoBehaviour
    {
        [System.Serializable]
        public class PerceivedSound
        {
            public SoundEmitter SoundEmitter;
            public float HearingTime;
            public float LifeTime;
        }

        [SerializeField] public List<PerceivedSound> PerceivedSounds = new();

        private IAllegiance _allegiance;

        private void Awake()
        {
            _allegiance = GetComponent<IAllegiance>();
        }

        private void Update()
        {
            PerceivedSounds.RemoveAll(x => (Time.time - x.HearingTime) > x.SoundEmitter.Type.LifeTime);
        }

        internal void NotifyHears(SoundEmitter soundEmitter)
        {
            if (soundEmitter.gameObject == gameObject) return;

            if (soundEmitter.TryGetComponent(out IAllegiance emitterAllegiance))
            {
                bool areConfronted = AllegianceUtilities.AreConfronted(_allegiance, emitterAllegiance);
                if ((_allegiance != null && emitterAllegiance != null && areConfronted))
                {
                    PerceivedSound perceivedSound = new PerceivedSound();
                    perceivedSound.SoundEmitter = soundEmitter;
                    perceivedSound.HearingTime = Time.time;

                    PerceivedSounds.Add(perceivedSound);
                }
            }
        }
    }
}
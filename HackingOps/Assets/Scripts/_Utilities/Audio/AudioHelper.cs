using UnityEngine;

namespace HackingOps.Utilities.Audio
{
    public static class AudioHelper
    {
        public static float RandomizePitch(float basePitch, float pitchVariation)
        {
            return basePitch + Random.Range(-pitchVariation, pitchVariation);
        }

        public static AudioClip GetRandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];
    }
}
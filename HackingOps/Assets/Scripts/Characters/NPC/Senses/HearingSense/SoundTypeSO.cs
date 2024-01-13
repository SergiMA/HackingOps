using UnityEngine;

namespace HackingOps.Characters.NPC.Senses.HearingSense
{
    [CreateAssetMenu(fileName = "New Sound Type", menuName = "Hacking Ops/Audio/Sound Type")]
    public class SoundTypeSO : ScriptableObject
    {
        public float LifeTime = 1f;
        public float Range = 1f;
    }
}
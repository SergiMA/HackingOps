using UnityEngine;

namespace HackingOps.Audio
{
    [CreateAssetMenu(menuName = "Hacking Ops/Audio/AudioMixer Group ID", fileName = "AudioMixer Group ID")]
    public class AudioMixerGroupId : ScriptableObject
    {
        [SerializeField] private string _value;
        public string Value => _value;
    }
}
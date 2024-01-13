using UnityEngine;

namespace HackingOps.Characters.NPC.Senses.HearingSense
{
    public class CharacterSoundController : MonoBehaviour
    {
        [SerializeField] private SoundTypeSO _silenceSoundType;
        [SerializeField] private SoundTypeSO _crouchingStepSoundType;
        [SerializeField] private SoundTypeSO _walkingStepSoundType;
        [SerializeField] private SoundTypeSO _runningStepSoundType;

        private SoundEmitter _soundEmitter;

        private void Awake() => _soundEmitter = GetComponent<SoundEmitter>();

        public void OnStoppedMoving() => _soundEmitter.ChangeSound(_silenceSoundType);
        public void OnStartedWalkingWhileCrouching() => _soundEmitter.ChangeSound(_crouchingStepSoundType);
        public void OnStartedWalking() => _soundEmitter.ChangeSound(_walkingStepSoundType);
        public void OnStartedRunning() => _soundEmitter.ChangeSound(_runningStepSoundType);
    }
}
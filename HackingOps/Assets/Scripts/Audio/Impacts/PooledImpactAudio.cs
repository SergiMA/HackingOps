using HackingOps.Common.Audio;
using HackingOps.Utilities.Audio;
using HackingOps.Utilities.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.Audio.Impacts
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledImpactAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _impactSounds;
        [SerializeField] private float _basePitch = 1f;
        [SerializeField] private float _pitchVariation = 0.2f;

        private AudioSourceController _audioSourceController;
        private AudioSource _audioSource;
        private ObjectPool<PooledImpactAudio> _pool;

        private void Awake()
        {
            _audioSource = gameObject.GetOrAdd<AudioSource>();
            _audioSourceController = new(_audioSource);
        }

        private void OnEnable()
        {
            if (_impactSounds.Length == 0) return;

            AudioClip clip = AudioHelper.GetRandomClip(_impactSounds);
            _audioSource.pitch = AudioHelper.RandomizePitch(_basePitch, _pitchVariation);
            _audioSourceController.Play(clip);
        }

        private void Start() => _audioSourceController.OnAudioFinished += () => _pool.Release(this);
        private void Update() => _audioSourceController.Tick();
        public void SetPool(ObjectPool<PooledImpactAudio> pool) => _pool = pool;
        public void Play() => _audioSourceController.Play();
        public void Play(AudioClip clip) => _audioSourceController.Play(clip);
        public void Stop() => _audioSourceController.Stop();
        public void Pause() => _audioSourceController.Pause();
    }
}
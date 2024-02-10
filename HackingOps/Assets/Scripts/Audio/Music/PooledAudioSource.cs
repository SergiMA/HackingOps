using UnityEngine;
using UnityEngine.Pool;

namespace HackingOps.Audio.Music
{
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour
    {
        private AudioSource _audioSource;
        private ObjectPool<PooledAudioSource> _pool;

        private void Awake() => _audioSource = GetComponent<AudioSource>();

        private void Start() => _audioSource.loop = true;

        public void SetPool(ObjectPool<PooledAudioSource> pool) => _pool = pool;
        public float GetVolume() => _audioSource.volume;
        public void SetVolume(float volume)
        {
            _audioSource.volume = volume;

            if (volume <= 0) _pool.Release(this);
        }
        public void SetClip(AudioClip clip) => _audioSource.clip = clip;
        public void Play() => _audioSource.Play();
    }
}
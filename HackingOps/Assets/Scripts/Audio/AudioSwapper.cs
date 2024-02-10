using DG.Tweening;
using HackingOps.Audio.Music;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Audio
{
    [RequireComponent(typeof(AudioSourcePoolController))]
    public class AudioSwapper : MonoBehaviour
    {
        [SerializeField] private float _swappingDuration = 1.25f;   // Duration in seconds

        private AudioSourcePoolController _audioSourcePoolController;

        private PooledAudioSource _currentPooledAudioSource;
        private AudioClip _currentAudioClip;

        private void Awake() => _audioSourcePoolController = GetComponent<AudioSourcePoolController>();
        private void Start() => _currentPooledAudioSource = _audioSourcePoolController.Get();

        private void ChangeVolume(PooledAudioSource pooledAudioSource, float volume, float swappingDuration = 1.25f)
        {
            DOVirtual.Float(pooledAudioSource.GetVolume(), volume, swappingDuration, v => pooledAudioSource.SetVolume(v));
        }

        public void Swap(AudioClip clip)
        {
            if (_currentAudioClip == clip) return;

            PooledAudioSource freePooledAudioSource = _audioSourcePoolController.Get();
            freePooledAudioSource.SetClip(clip);
            freePooledAudioSource.Play();

            ChangeVolume(_currentPooledAudioSource, 0, _swappingDuration);
            ChangeVolume(freePooledAudioSource, 1, _swappingDuration);

            _currentPooledAudioSource = freePooledAudioSource;
            _currentAudioClip = clip;
        }
    }
}
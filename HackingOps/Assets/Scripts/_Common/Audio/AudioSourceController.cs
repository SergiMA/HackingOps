using System;
using UnityEngine;

namespace HackingOps.Common.Audio
{
    public class AudioSourceController
    {
        public event Action OnAudioFinished;

        private AudioSource _audioSource;
        private bool _previousIsPlaying;
        private bool _isPaused;

        public AudioSourceController(AudioSource audioSource)
        {
            _audioSource = audioSource;
        }

        private void NotifyWhenFinishPlaying()
        {
            if (!_previousIsPlaying) return;

            if (!_audioSource.isPlaying) OnAudioFinished?.Invoke();
        }

        public void Tick()
        {
            if (_isPaused) return;

            NotifyWhenFinishPlaying();

            _previousIsPlaying = _audioSource.isPlaying;
        }

        public void SetClip(AudioClip clip) => _audioSource.clip = clip;
        public void Play() => _audioSource.Play();

        public void Play(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public void Stop() => _audioSource.Stop();

        public void Pause()
        {
            _audioSource.Pause();
            _isPaused = true;
        }

        public void UnPause()
        {
            _audioSource.UnPause();
            _isPaused = false;
        }
    }
}
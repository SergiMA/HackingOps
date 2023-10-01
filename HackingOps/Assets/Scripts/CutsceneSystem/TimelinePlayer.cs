using Cinemachine;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using System;
using UnityEngine;
using UnityEngine.Playables;

namespace HackingOps.CutsceneSystem
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelinePlayer : MonoBehaviour
    {
        public event Action OnFinished;

        [SerializeField] private bool _notifyWhenFinished;

        private PlayableDirector _playableDirector;
        private bool _isPlaying;

        private void Awake()
        {
            _playableDirector = GetComponent<PlayableDirector>();
        }

        private void Update()
        {
            if (!_isPlaying)
                return;

            NotifyWhenFinished();
        }

        private void NotifyWhenFinished()
        {
            if (_playableDirector.state != PlayState.Playing)
            {
                _isPlaying = false;
                OnFinished?.Invoke();

                if (!_notifyWhenFinished)
                    return;

                if (TryGetComponent(out CutsceneIdentification cutsceneIdentification))
                {
                    ServiceLocator.Instance
                        .GetService<IEventQueue>()
                        .EnqueueEvent(new CutsceneFinishedData(cutsceneIdentification.Id));

                    ServiceLocator.Instance
                        .GetService<CutsceneCameraController>()
                        .UnsetCutsceneCamera();
                }
            }
        }

        public void Play()
        {
            _playableDirector.Play();
            _isPlaying = true;
        }

        public void OnUseCutsceneCameraSignalReceived(CinemachineVirtualCamera cutsceneCamera)
        {
            ServiceLocator.Instance
                .GetService<CutsceneCameraController>()
                .SetCutsceneCamera(cutsceneCamera);
        }
    }
}
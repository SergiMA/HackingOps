using DG.Tweening;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    public class CutsceneSetup : MonoBehaviour 
    {
        [Header("Bindings - Dependencies")]
        [SerializeField] private TimelinePlayer _timelinePlayer;

        [Header("Bindings - Moving transform <i>(optional)</i>")]
        [SerializeField] private Transform _transformToPosition;
        [SerializeField] private Transform _destination;

        [Header("Settings - Moving transform <i>(optional)</i>")]
        [SerializeField] private float _movingDurationInSeconds = 0.5f; 

        public void Play()
        {
            if (_transformToPosition == null || _destination == null)
                _timelinePlayer.Play();
            else
                MoveToPosition();
        }
        
        private void MoveToPosition()
        {
            Vector3 targetDestination = _destination.position;
            targetDestination.y = _transformToPosition.position.y;
            
            _transformToPosition.DOMove(targetDestination, _movingDurationInSeconds).onComplete = OnPlayerMoved;
            _transformToPosition.DORotateQuaternion(_destination.rotation, _movingDurationInSeconds);
        }

        private void OnPlayerMoved()
        {
            _timelinePlayer.Play();
            
            if (TryGetComponent(out CutsceneIdentification cutsceneIdentification))
            {
                ServiceLocator.Instance
                    .GetService<IEventQueue>()
                    .EnqueueEvent(new CutsceneStartedData(cutsceneIdentification.Id));
            }
        }
    }
}
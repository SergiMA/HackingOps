using Cinemachine;
using DG.Tweening;
using HackingOps.CutsceneSystem;
using UnityEngine;

namespace HackingOps.Common.QuestSystem.Quests
{
    public class CompletionQuestSwitchCamera : MonoBehaviour, ICompletionQuestAction
    {
        [Header("Bindings")]
        [SerializeField] private Quest _quest;
        [SerializeField] private TimelinePlayer _timelinePlayer;
        [SerializeField] private CinemachineVirtualCamera _camera;

        [Header("Settings")]
        [Tooltip("Delay in seconds to start the transition to another camera")]
        [SerializeField] private float _delay;
        [SerializeField] private bool _switchCameraImmediately;

        private void OnEnable()
        {
            _quest.OnQuestCompleted += ExecuteAction;
        }

        private void OnDisable()
        {
            _quest.OnQuestCompleted -= ExecuteAction;
        }

        public void ExecuteAction()
        {
            DOVirtual.DelayedCall(_delay, () =>
            {
                if (_switchCameraImmediately)
                    _timelinePlayer.OnImmediatelyUseCutsceneCameraSignalReceived(_camera);
                else
                    _timelinePlayer.OnUseCutsceneCameraSignalReceived(_camera);
            });
        }
    }
}
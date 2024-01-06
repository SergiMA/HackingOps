using Cinemachine;
using DG.Tweening;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Screens.UI;
using HackingOps.Utilities;
using UnityEngine;

namespace HackingOps.CutsceneSystem
{
    [RequireComponent(typeof(TimelinePlayer))]
    public class MenuTimelineSetup : MonoBehaviour, IEventObserver
    {
        [SerializeField] private CinemachineVirtualCamera _camera;
        [SerializeField] private LaptopScreen _laptopScreen;

        [Tooltip("Delay in seconds to start the transition to another camera")]
        [SerializeField] private float _delayCameraTransition;

        [Tooltip("Delay in seconds to free the cursor")]
        [SerializeField] private float _delayToFreeCursor;

        [Tooltip("Delay in seconds to show the menu on the laptop screen")]
        [SerializeField] private float _delayToLoadMenu;

        private TimelinePlayer _timelinePlayer;

        private void Awake() => _timelinePlayer = GetComponent<TimelinePlayer>();

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.PauseGame, this);
        }

        private void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.PauseGame, this);
        }

        public void UseMenu()
        {
            _timelinePlayer.Play();

            DOVirtual.DelayedCall(_delayCameraTransition, () => _timelinePlayer.OnUseCutsceneCameraSignalReceived(_camera));
            DOVirtual.DelayedCall(_delayToFreeCursor, () => { ServiceLocator.Instance.GetService<CursorLocker>().FreeCursor(); });
            DOVirtual.DelayedCall(_delayToLoadMenu, () => { _laptopScreen.LoadMenu(); });
        }

        #region IEventObserver implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.PauseGame)
                UseMenu();
        }
        #endregion
    }
}
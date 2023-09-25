using DG.Tweening;
using HackingOps.Doors.AnimatedDoors;
using HackingOps.InteractionSystem;
using UnityEngine;

namespace HackingOps.Platforms
{

    public class Elevator : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private AnimatedDoor[] _animatedDoors;
        [SerializeField] private PlatformParenter _platformParenter;
        [SerializeField] private WaypointTraveler _waypointTraveler;
        [SerializeField] private ButtonInteractable _controlPanel;

        [Header("Settings")]
        [SerializeField] private float _returnInteractionToControlPanelDelay = 2f; // seconds

        [Header("Debug")]
        [SerializeField] private bool _interact;
        [SerializeField] private bool _unload;

        private void OnValidate()
        {
            if (_interact)
            {
                _interact = false;

                Move();
            }

            if (_unload)
            {
                _unload = false;
                _platformParenter.Unload();
                foreach (AnimatedDoor animatedDoor in _animatedDoors)
                {
                    animatedDoor.Open();
                }
            }
        }

        public void Move()
        {
            _controlPanel.DisableInteractions();
            CloseDoors();
        }

        public void CloseDoors()
        {
            foreach (AnimatedDoor animatedDoor in _animatedDoors)
            {
                animatedDoor.Close();
            }
        }

        public void OpenDoors()
        {
            foreach (AnimatedDoor animatedDoor in _animatedDoors)
            {
                animatedDoor.Open();
            }
        }

        public void OnDoorClosed()
        {
            _platformParenter.Load();
        }

        public void OnScanPerformed()
        {
            _waypointTraveler.GoToNextWaypoint();
        }

        public void OnDestinationReached()
        {
            _platformParenter.Unload();

            OpenDoors();
            DOVirtual.DelayedCall(_returnInteractionToControlPanelDelay, () => _controlPanel.EnableInteractions());
        }
    }
}
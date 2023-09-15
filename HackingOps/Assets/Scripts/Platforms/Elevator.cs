using HackingOps.Doors.AnimatedDoors;
using UnityEngine;

namespace HackingOps.Platforms
{

    public class Elevator : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private AnimatedDoor[] _animatedDoors;
        [SerializeField] private PlatformParenter _platformParenter;
        [SerializeField] private WaypointTraveler _waypointTraveler;

        [Header("Debug")]
        [SerializeField] private bool _interact;
        [SerializeField] private bool _unload;

        private void OnValidate()
        {
            if (_interact)
            {
                _interact = false;

                foreach (AnimatedDoor animatedDoor in _animatedDoors)
                {
                    animatedDoor.Close();
                }
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

            foreach (AnimatedDoor animatedDoor in _animatedDoors)
            {
                animatedDoor.Open();
            }
        }
    }
}
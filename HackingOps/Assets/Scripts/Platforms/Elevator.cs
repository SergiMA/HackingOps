using HackingOps.Doors.AnimatedDoors;
using UnityEngine;

namespace HackingOps.Platforms
{

    public class Elevator : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private AnimatedDoor _animatedDoor;
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
                _animatedDoor.Close();
            }

            if (_unload)
            {
                _unload = false;
                _platformParenter.Unload();
                _animatedDoor.Open();
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
            _animatedDoor.Open();
        }
    }
}
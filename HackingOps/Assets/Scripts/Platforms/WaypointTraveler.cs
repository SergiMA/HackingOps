using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Platforms
{
    public class WaypointTraveler : MonoBehaviour
    {
        public UnityEvent OnDestinationReached;

        [SerializeField] private Transform _objectToMove;
        [SerializeField] private float _speed = 3f;
        [SerializeField] private Transform _waypointsParent;

        private Transform[] _waypoints;
        private int _currentWaypointIndex;
        private bool _isMoving;

        private void Awake()
        {
            _waypoints = _waypointsParent.GetComponentsInChildren<Transform>()
                .Where(t => t != _waypointsParent)
                .ToArray();
        }

        private void Update()
        {
            if (_isMoving)
            {
                _objectToMove.position = Vector3.MoveTowards(
                    _objectToMove.position, 
                    _waypoints[_currentWaypointIndex].position, 
                    _speed * Time.deltaTime);

                if (Vector3.Distance(_objectToMove.position, _waypoints[_currentWaypointIndex].position) == 0)
                {
                    _isMoving = false;
                    OnDestinationReached.Invoke();
                }
            }
        }

        public void GoToNextWaypoint()
        {
            _currentWaypointIndex++;

            if (_currentWaypointIndex >= _waypoints.Length)
            {
                _currentWaypointIndex = 0;
            }

            _isMoving = true;
        }
    }
}
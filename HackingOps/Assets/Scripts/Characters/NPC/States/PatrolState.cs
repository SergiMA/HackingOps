using UnityEngine;

namespace HackingOps.Characters.NPC.States
{
    public class PatrolState : State
    {
        [SerializeField] private Transform _currentRoute;
        [SerializeField] private int _startPointIndex = 0;
        [SerializeField] private float _arrivalDistance = 1f;

        private int _currentPointIndex = 0;

        protected override void StateAwake()
        {
            _currentPointIndex = _startPointIndex;
        }

        private void Update()
        {
            _entity.Agent.destination = _currentRoute.GetChild(_currentPointIndex).position;

            if (Vector3.SqrMagnitude(_entity.Agent.destination - transform.position) < _arrivalDistance * _arrivalDistance)
            {
                _currentPointIndex++;
                if (_currentPointIndex >= _currentRoute.childCount) { _currentPointIndex = 0; }
            }
        }
    }
}
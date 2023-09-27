using HackingOps.Characters.Common;
using HackingOps.Characters.NPC.Senses;
using HackingOps.Characters.NPC.States;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace HackingOps.Characters.Entities
{
    [DefaultExecutionOrder(-10)]
    public class Entity : MonoBehaviour
    {
        // Public internal bindings
        public NavMeshAgent Agent { get; set; }
        public Sight Sight { get; set; }

        [Header("Bindings - States")]
        [SerializeField] private State _startState;

        [field: Header("Settings - States")]
        [SerializeField] public float AngularSpeed { get; private set; } = 360f;

        [Header("Settings - Hiding points")]
        [SerializeField] float _hidingPointFindRadius = 30f;
        [SerializeField] LayerMask _hidingPointLayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] LayerMask _occludersLayerMask = Physics.DefaultRaycastLayers;

        State[] _states;

        Transform _currentHidingPoint;

        private void Awake()
        {
            _states = GetComponentsInChildren<State>();
            foreach (State s in _states)
            {
                s.enabled = s == _startState;
            }

            Agent = GetComponent<NavMeshAgent>();
            Sight = GetComponent<Sight>();
        }

        internal Transform GetCurrentHidingDestionationPoint() => _currentHidingPoint;

        internal void CurrentHidingPositionReached()
        {
            _currentHidingPoint = null;
        }

        internal void FindHidingPosition()
        {
            _currentHidingPoint = null;

            List<Collider> hidingPointCandidates =
                Physics.OverlapSphere(transform.position, _hidingPointFindRadius, _hidingPointLayerMask).ToList();

            if (hidingPointCandidates.Count > 0)
            {
                IVisible target = Sight.VisiblesInSight[0];

                hidingPointCandidates = FilterOutVisibleByTarget(hidingPointCandidates, target);
                hidingPointCandidates.OrderBy((c) => Vector3.Distance(c.transform.position, transform.position));
                _currentHidingPoint = hidingPointCandidates[0].transform;
            }
        }

        private List<Collider> FilterOutVisibleByTarget(List<Collider> hidingPointCandidates, IVisible target)
        {
            List<Collider> newHidingPointCandidates = new List<Collider>();

            foreach (Collider c in hidingPointCandidates)
            {
                if (Physics.Linecast(
                    target.GetTransform().position,
                    c.transform.position,
                    out RaycastHit hit,
                    _occludersLayerMask))
                {
                    if (hit.collider != c) { newHidingPointCandidates.Add(c); }
                }
                else
                {
                    newHidingPointCandidates.Add(c);
                }
            }

            return newHidingPointCandidates;
        }
    }
}

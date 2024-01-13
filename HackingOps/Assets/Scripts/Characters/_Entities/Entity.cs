using HackingOps.Characters.NPC.Allegiance;
using HackingOps.Characters.NPC.DecisionMaking;
using HackingOps.Characters.NPC.Senses.SightSense;
using HackingOps.Characters.NPC.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace HackingOps.Characters.Entities
{
    [DefaultExecutionOrder(-10)]
    public class Entity : MonoBehaviour, IAllegiance
    {
        // Events
        public event Action<Transform> OnTargetSet;
        public event Action<AgroDecision> OnAgroDecisionChanged;

        // Public internal bindings
        public EntityDecisionMaker DecisionMaker;
        public NavMeshAgent Agent { get; set; }
        public Sight Sight { get; set; }
        public Touching Touching { get; set; }

        [Header("Bindings - States")]
        [SerializeField] private State _startState;

        [Header("Settings - States")]
        public float AngularSpeed = 360f;

        [Header("Settings - Allegiance")]
        [SerializeField] private IAllegiance.Allegiance _allegiance = IAllegiance.Allegiance.Enemy;

        [Header("Settings - Hiding points")]
        [SerializeField] private float _hidingPointFindRadius = 30f;
        [SerializeField] private LayerMask _hidingPointLayerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private LayerMask _occludersLayerMask = Physics.DefaultRaycastLayers;

        private State[] _states;

        private Transform _currentHidingPoint;

        public enum AgroDecision
        {
            Alert,
            Decided,
        }

        public AgroDecision AgroDecisionState;

        private void Awake()
        {
            _states = GetComponentsInChildren<State>();
            foreach (State s in _states)
            {
                s.enabled = s == _startState;
            }

            Agent = GetComponent<NavMeshAgent>();
            Sight = GetComponent<Sight>();
            Touching = GetComponent<Touching>();
        }

        #region Hiding spot processing
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
        #endregion

        #region Agro processing
        public void OnHitReceived(Transform attackerTransform)
        {
            if (attackerTransform == null)
                return;

            AgroDecisionState = AgroDecision.Decided;
            OnTargetSet?.Invoke(attackerTransform);
            OnAgroDecisionChanged?.Invoke(AgroDecisionState);
        }

        public void OnAlertDropped()
        {
            AgroDecisionState = AgroDecision.Alert;
            OnAgroDecisionChanged?.Invoke(AgroDecisionState);
        }
        #endregion

        #region IAllegiance implementation
        public IAllegiance.Allegiance GetAllegiance() => _allegiance;
        #endregion
    }
}

using HackingOps.Characters.Entities;
using HackingOps.Characters.NPC.Senses.HearingSense;
using HackingOps.Characters.NPC.Senses.SightSense;
using HackingOps.Characters.NPC.States;
using HackingOps.Common.Events;
using HackingOps.Common.Events.EventsData;
using HackingOps.Common.Services;
using HackingOps.Weapons.Common;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    [DefaultExecutionOrder(-1)]
    public class EntityDecisionMaker : MonoBehaviour
    {
        [SerializeField] private Transform _enemy;

        private State[] _allStates;
        private Entity _entity;
        private EntityWeapons _entityWeapons;
        private Inventory _inventory;

        // Senses
        private Sight _sight;
        private Hearing _hearing;

        // Combat
        public Transform CurrentTarget { get; private set; }

        private Transform _previousCurrentTarget;

        private DecisionTreeNode _decisionRoot;

        private void Awake()
        {
            #region Components setup
            _allStates = _enemy.GetComponents<State>();
            _sight = _enemy.GetComponent<Sight>();
            _hearing = _enemy.GetComponent<Hearing>();
            _entity = _enemy.GetComponent<Entity>();
            _entityWeapons = _entity.GetComponent<EntityWeapons>();
            _inventory = _entity.GetComponent<Inventory>();
            _decisionRoot = transform.GetChild(0).GetComponent<DecisionTreeNode>();
            #endregion

            foreach (State s in _allStates) { s.enabled = false; }
        }

        private void Start()
        {
            _entity.OnTargetSet += SetCurrentTarget;
        }

        private void Update()
        {
            CurrentTarget = DecideCurrentTarget();
            SetState(_decisionRoot.Execute());

            CheckTargetChanges();
            _previousCurrentTarget = CurrentTarget;
        }

        private void OnDisable()
        {
            foreach (State s in _allStates) { s.enabled = false; }
        }

        private Transform DecideCurrentTarget()
        {
            switch (_entity.AgroDecisionState)
            {
                case Entity.AgroDecision.Alert:
                    List<Transform> potentialTargets = LookForNewTargets();
                    return potentialTargets.Count > 0 ? potentialTargets[0] : null;
                case Entity.AgroDecision.Decided:
                    return CurrentTarget;
                default:
                    return null;
            }
        }

        private List<Transform> LookForNewTargets()
        {
            List<Transform> potentialTargets = new();

            AddVisiblesToPotentialTargets(potentialTargets);
            AddSoundEmittersToPotentialTargets(potentialTargets);

            SortByDistance(potentialTargets);

            return potentialTargets;
        }

        private void AddSoundEmittersToPotentialTargets(List<Transform> potentialTargets)
        {
            if (_hearing == null) return;

            foreach (Hearing.PerceivedSound s in _hearing.PerceivedSounds)
                potentialTargets.Add(s.SoundEmitter.transform);
        }

        private void AddVisiblesToPotentialTargets(List<Transform> potentialTargets)
        {
            if (_sight == null) return;

            foreach (IVisible v in _sight.VisiblesInSight)
                potentialTargets.Add(v.GetTransform());
        }

        private List<Transform> SortByDistance(List<Transform> transforms)
        {
            transforms.Sort(
                (x, y) =>
                (
                    Vector3.Distance(transform.position, x.position) <
                    Vector3.Distance(transform.position, y.position)) ?
                    1 : 0
                );

            return transforms;
        }

        private void CheckTargetChanges()
        {
            if (CurrentTarget == _previousCurrentTarget) return;

            IEventQueue eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

            if (CurrentTarget != null)
                eventQueue.EnqueueEvent(new EntityEngagedCombatData(this));
            else
                eventQueue.EnqueueEvent(new EntityLeftCombatData(this));
        }

        internal void SetState(State state)
        {
            foreach (State s in _allStates)
            {
                bool mustBeEnabled = s == state;
                if (s.enabled != mustBeEnabled)
                    s.enabled = mustBeEnabled;
            }
        }

        internal EntityWeapons GetEntityWeapons() => _entityWeapons;
        internal Inventory GetInventory() => _inventory;

        internal Entity GetEntity() => _entity;
        public void SetCurrentTarget(Transform target) => CurrentTarget = target;
    }
}
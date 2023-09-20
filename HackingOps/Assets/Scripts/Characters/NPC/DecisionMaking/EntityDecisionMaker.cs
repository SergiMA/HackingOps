using HackingOps.Characters.Entities;
using HackingOps.Characters.NPC.Senses;
using HackingOps.Characters.NPC.States;
using HackingOps.Weapons.Common;
using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    [DefaultExecutionOrder(-1)]
    public class EntityDecisionMaker : MonoBehaviour
    {
        [SerializeField] private Transform _enemy;

        State[] _allStates;
        Entity _entity;
        EntityWeapons _entityWeapons;
        Inventory _inventory;

        // Senses
        Sight _sight;

        // Combat
        public IVisible CurrentTarget { get; private set; }

        DecisionTreeNode _decisionRoot;

        private void Awake()
        {
            _allStates = _enemy.GetComponents<State>();
            _sight = _enemy.GetComponent<Sight>();
            _entity = _enemy.GetComponent<Entity>();
            _entityWeapons = _entity.GetComponent<EntityWeapons>();
            _inventory = _entity.GetComponent<Inventory>();
            _decisionRoot = transform.GetChild(0).GetComponent<DecisionTreeNode>();

            foreach (State s in _allStates) { s.enabled = false; }
        }

        private void Update()
        {
            CurrentTarget = _sight.VisiblesInSight.Count > 0 ? _sight.VisiblesInSight[0] : null;
            SetState(_decisionRoot.Execute());
        }

        private void OnDisable()
        {
            foreach (State s in _allStates) { s.enabled = false; }
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
    }
}
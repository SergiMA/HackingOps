using HackingOps.Characters.NPC.DecisionMaking;
using HackingOps.Common.Events;
using HackingOps.Common.Events.EventsData;
using HackingOps.Common.Services;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.CombatSystem
{
    public class CombatMode : MonoBehaviour, IEventObserver
    {
        [Tooltip("Duration in seconds to change to a Peaceful state once there are no enemies in combat")]
        [SerializeField] private float _cooldownDuration = 5f;

        private IEventQueue _eventQueue;

        private HashSet<EntityDecisionMaker> entitiesInCombat = new();

        // State bindings
        private CombatModeBaseState _currentState;
        private CombatModeStateFactory _stateFactory;

        #region Getters & Setters
        public HashSet<EntityDecisionMaker> EntitiesInCombat { get { return entitiesInCombat; } }
        public float CooldownDuration { get { return _cooldownDuration; } }

        public CombatModeBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        #endregion

        private void Awake()
        {
            _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
        }

        private void OnEnable()
        {
            _eventQueue.Subscribe(EventIds.EntityEngageCombat, this);
            _eventQueue.Subscribe(EventIds.EntityLeaveCombat, this);
        }

        private void OnDisable()
        {
            _eventQueue.Unsubscribe(EventIds.EntityEngageCombat, this);
            _eventQueue.Unsubscribe(EventIds.EntityLeaveCombat, this);
        }

        private void Start()
        {
            _stateFactory = new CombatModeStateFactory(this);
            _currentState = _stateFactory.GetState(CombatModeStateFactory.States.Peaceful);
            _currentState.EnterState();
        }

        private void Update()
        {
            _currentState.UpdateState();
        }

        private void AddEntity(EntityDecisionMaker entity) => entitiesInCombat.Add(entity);
        private void RemoveEntity(EntityDecisionMaker entity) => entitiesInCombat.Remove(entity);

        private void CheckEntitiesInCombat()
        {
            _currentState.CheckEntitiesInCombat();
        }

        #region Implemented from IEventObserver
        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.EntityEngageCombat:
                    EntityEngagedCombatData entityEngagedCombatData = (EntityEngagedCombatData)eventData;
                    AddEntity(entityEngagedCombatData.EntityDecisionMaker);
                    CheckEntitiesInCombat();
                    break;
                case EventIds.EntityLeaveCombat:
                    EntityLeftCombatData entityLeftCombatData = (EntityLeftCombatData)eventData;
                    RemoveEntity(entityLeftCombatData.EntityDecisionMaker);
                    CheckEntitiesInCombat();
                    break;
            }
        }
        #endregion
    }
}
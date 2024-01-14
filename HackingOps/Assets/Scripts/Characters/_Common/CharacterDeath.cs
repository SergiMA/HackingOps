using HackingOps.Characters.Entities;
using HackingOps.Characters.NPC.DecisionMaking;
using HackingOps.Characters.NPC.States;
using HackingOps.Characters.Player;
using HackingOps.CombatSystem.HitHurtBox;
using HackingOps.Common.Events;
using HackingOps.Common.Events.EventsData;
using HackingOps.Common.Services;
using HackingOps.Weapons.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace HackingOps.Characters.Common
{
    public class CharacterDeath : MonoBehaviour
    {
        public UnityEvent OnDead;

        [SerializeField] private float pushForce = 10f;

        [Header("Debug")]
        [SerializeField] private bool _debugDie;
        [SerializeField] private Vector3 _debugDieDirection;

        private void OnValidate()
        {
            if (_debugDie)
            {
                _debugDie = false;
                Die(_debugDieDirection);
            }
        }

        private void Awake()
        {
            GetComponent<HurtBoxWithLife>()?.OnNotifyHitWithLifeAndDirection.AddListener(OnNotifyHitWithLifeAndDirection);
        }

        private float _lastNotifiedLife = Mathf.Infinity;

        private void OnNotifyHitWithLifeAndDirection(float currentLife, float maxLife, Vector3 direction)
        {
            if (_lastNotifiedLife > 0)
            {
                if (currentLife <= 0f) { Die(direction); }
                _lastNotifiedLife = currentLife;
            }
        }

        public void Die(Vector3 direction)
        {
            if (TryGetComponent(out NavMeshAgent agent)) { agent.enabled = false; }
            if (TryGetComponent(out Collider collider)) { collider.enabled = false; }
            if (TryGetComponent(out SeekTargetState nonPlayableCharacter)) { nonPlayableCharacter.enabled = false; }
            if (TryGetComponent(out PlayerController playerController)) { playerController.enabled = false; }

            EntityDecisionMaker entityDecisionMaker = transform.parent.GetComponentInChildren<EntityDecisionMaker>();
            entityDecisionMaker?.gameObject.SetActive(false);

            if (transform.parent.TryGetComponent(out EntityDecisionMaker decisionMaker))
            {
                decisionMaker.gameObject.SetActive(false);
            }

            Animator animator = GetComponentInChildren<Animator>();
            if (animator) { animator.enabled = false; }

            GetComponentInChildren<CharacterRagdollController>()?.ActivateRagdoll(direction * pushForce);

            OnDead.Invoke();

            IEventQueue eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();

            if (TryGetComponent(out CharacterIdentification identification))
                eventQueue.EnqueueEvent(new CharacterDiedData(identification.Id));

            if (TryGetComponent(out Entity entity))
            {
                if (entity.DecisionMaker.CurrentTarget != null)
                    eventQueue.EnqueueEvent(new EntityLeftCombatData(entity.DecisionMaker));
            }

            if (TryGetComponent(out Inventory inventory)) { inventory.enabled = false; }
        }
    }
}
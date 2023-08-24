using HackingOps.Characters.NPC.DecisionMaking;
using HackingOps.Characters.NPC.States;
using HackingOps.Characters.Player;
using UnityEngine;
using UnityEngine.AI;

namespace HackingOps.Characters.Common
{
    public class CharacterDeath : MonoBehaviour
    {
        [SerializeField] private bool _debugDie;

        private void OnValidate()
        {
            if (_debugDie)
            {
                _debugDie = false;
                Die();
            }
        }

        private void Awake()
        {
            // TODO: Add component HurtBoxWithLife and add a listener to OnNotifyWithWithLife, executing this.OnNotifyHitWithLife()
        }

        private float _lastNotifiedLife = Mathf.Infinity;
        private void OnNotifyHitWithLife(float currentLife, float maxLife)
        {
            if (_lastNotifiedLife > 0)
            {
                if (currentLife <= 0f) { Die(); }
                _lastNotifiedLife = currentLife;
            }
        }

        public void Die()
        {
            if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent)) { agent.enabled = false; }
            if (TryGetComponent<Collider>(out Collider collider)) { collider.enabled = false; }
            if (TryGetComponent<SeekTargetState>(out SeekTargetState nonPlayableCharacter)) { nonPlayableCharacter.enabled = false; }
            if (TryGetComponent<PlayerController>(out PlayerController playerController)) { playerController.enabled = false; }

            EntityDecisionMaker entityDecisionMaker = transform.parent.GetComponentInChildren<EntityDecisionMaker>();
            entityDecisionMaker?.gameObject.SetActive(false);

            if (transform.parent.TryGetComponent<EntityDecisionMaker>(out EntityDecisionMaker decisionMaker))
            {
                decisionMaker.gameObject.SetActive(false);
            }

            Animator animator = GetComponentInChildren<Animator>();
            if (animator) { animator.enabled = false; }

            GetComponentInChildren<CharacterRagdollController>()?.ActivateRagdoll();
        }
    }
}
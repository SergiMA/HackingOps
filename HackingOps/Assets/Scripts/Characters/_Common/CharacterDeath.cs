using HackingOps.Characters.NPC.DecisionMaking;
using HackingOps.Characters.NPC.States;
using HackingOps.Characters.Player;
using HackingOps.CombatSystem.HitHurtBox;
using UnityEngine;
using UnityEngine.AI;

namespace HackingOps.Characters.Common
{
    public class CharacterDeath : MonoBehaviour
    {
        [SerializeField] private bool _debugDie;
        [SerializeField] private Vector3 _debugDieDirection;
        [SerializeField] private float pushForce = 10f;

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
            //GetComponent<HurtBoxWithLife>()?.OnNotifyHitWithLife.AddListener(OnNotifyHitWithLife);
            GetComponent<HurtBoxWithLife>()?.OnNotifyHitWithLifeAndDirection.AddListener(OnNotifyHitWithLifeAndDirection);
        }

        private float _lastNotifiedLife = Mathf.Infinity;
        //private void OnNotifyHitWithLife(float currentLife, float maxLife)
        //{
        //    if (_lastNotifiedLife > 0)
        //    {
        //        if (currentLife <= 0f){ Die(); } 
        //        _lastNotifiedLife = currentLife;
        //    }
        //}

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

            GetComponentInChildren<CharacterRagdollController>()?.ActivateRagdoll(direction * pushForce);
        }
    }
}
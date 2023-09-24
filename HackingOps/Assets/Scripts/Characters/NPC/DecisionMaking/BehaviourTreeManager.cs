using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class BehaviourTreeManager : MonoBehaviour
    {
        private EntityDecisionMaker _entityDecisionMaker;

        [Header("Debug")]
        [SerializeField] private bool _debugEnableBehaviour;
        [SerializeField] private bool _debugDisableBehaviour;

        private void OnValidate()
        {
            if (_debugEnableBehaviour)
            {
                _debugEnableBehaviour = false;
                EnableBehaviour();
            }

            if (_debugDisableBehaviour)
            {
                _debugDisableBehaviour = false;
                DisableBehaviour();
            }
        }

        private void Awake()
        {
            _entityDecisionMaker = GetComponentInChildren<EntityDecisionMaker>();
        }

        public void EnableBehaviour()
        {
            _entityDecisionMaker.enabled = true;
        }

        public void DisableBehaviour()
        {
            _entityDecisionMaker.enabled = false;
        }
    }
}
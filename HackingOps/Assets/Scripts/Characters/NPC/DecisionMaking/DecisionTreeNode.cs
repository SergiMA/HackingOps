using HackingOps.Characters.NPC.States;
using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public abstract class DecisionTreeNode : MonoBehaviour
    {
        protected EntityDecisionMaker _decisionMaker;

        private void Awake()
        {
            _decisionMaker = GetComponentInParent<EntityDecisionMaker>();
            InternalAwake();
        }

        protected virtual void InternalAwake() { }

        internal abstract State Execute();
    }
}
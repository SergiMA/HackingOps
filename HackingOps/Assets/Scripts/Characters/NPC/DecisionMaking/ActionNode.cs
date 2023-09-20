using HackingOps.Characters.NPC.States;
using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class ActionNode : DecisionTreeNode
    {
        [SerializeField] private State _state;

        internal override State Execute() => _state;
    }
}
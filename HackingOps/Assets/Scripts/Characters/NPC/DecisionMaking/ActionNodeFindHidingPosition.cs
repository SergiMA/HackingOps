using HackingOps.Characters.NPC.States;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class ActionNodeFindHidingPosition : ActionNode
    {
        internal override State Execute()
        {
            _decisionMaker.GetEntity().FindHidingPosition();
            return null;
        }
    }
}
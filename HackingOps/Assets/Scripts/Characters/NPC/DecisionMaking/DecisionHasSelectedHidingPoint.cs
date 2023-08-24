namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class DecisionHasSelectedHidingPoint : DecisionNode
    {
        protected override bool CheckCondition()
        {
            return _decisionMaker.GetEntity().GetCurrentHidingDestionationPoint() != null;
        }
    }
}
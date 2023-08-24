namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class DecisionHasCurrentTarget : DecisionNode
    {
        protected override bool CheckCondition()
        {
            return _decisionMaker.CurrentTarget != null;
        }
    }
}
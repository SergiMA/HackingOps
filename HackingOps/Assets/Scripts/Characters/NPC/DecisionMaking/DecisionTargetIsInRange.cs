using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class DecisionTargetIsInRange : DecisionNode
    {
        protected override bool CheckCondition()
        {
            return
                Vector3.Distance(_decisionMaker.CurrentTarget.GetTransform().position,
                _decisionMaker.GetEntity().transform.position) <
                _decisionMaker.GetEntityWeapons().GetEffectiveRange();
        }
    }
}
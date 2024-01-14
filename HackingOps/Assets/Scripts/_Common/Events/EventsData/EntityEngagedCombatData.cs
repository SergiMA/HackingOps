using HackingOps.Characters.NPC.DecisionMaking;

namespace HackingOps.Common.Events.EventsData
{
    public class EntityEngagedCombatData : EventData
    {
        public readonly EntityDecisionMaker EntityDecisionMaker;

        public EntityEngagedCombatData(EntityDecisionMaker entityDecisionMaker) : base(EventIds.EntityEngageCombat)
        {
            EntityDecisionMaker = entityDecisionMaker;
        }
    }

    public class EntityLeftCombatData : EventData
    {
        public readonly EntityDecisionMaker EntityDecisionMaker;

        public EntityLeftCombatData(EntityDecisionMaker entityDecisionMaker) : base(EventIds.EntityLeaveCombat)
        {
            EntityDecisionMaker = entityDecisionMaker;
        }
    }
}
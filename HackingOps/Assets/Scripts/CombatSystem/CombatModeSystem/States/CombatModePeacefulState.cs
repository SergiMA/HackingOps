using HackingOps.Common.Events;
using HackingOps.Common.Services;

namespace HackingOps.CombatSystem
{
    public class CombatModePeacefulState : CombatModeBaseState
    {
        public CombatModePeacefulState(CombatMode ctx, CombatModeStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() 
        {
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.OnLeaveCombatMode));
        }
        public override void UpdateState() { }
        public override void ExitState() { }

        protected override void CheckSwitchState()
        {
            if (_ctx.EntitiesInCombat.Count > 0)
                SwitchState(_factory.GetState(CombatModeStateFactory.States.InCombat));
        }

        public override void CheckEntitiesInCombat()
        {
            CheckSwitchState();
        }
    }
}
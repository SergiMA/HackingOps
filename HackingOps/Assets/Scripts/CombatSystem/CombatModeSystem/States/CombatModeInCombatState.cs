using HackingOps.Common.Events;
using HackingOps.Common.Services;

namespace HackingOps.CombatSystem
{
    public class CombatModeInCombatState : CombatModeBaseState
    {
        public CombatModeInCombatState(CombatMode ctx, CombatModeStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().EnqueueEvent(new EventData(EventIds.OnEnterCombatMode));
        }

        public override void UpdateState() { }
        public override void ExitState() { }

        protected override void CheckSwitchState()
        {
            if (_ctx.EntitiesInCombat.Count == 0)
                SwitchState(_factory.GetState(CombatModeStateFactory.States.CoolingDown));
        }

        public override void CheckEntitiesInCombat() => CheckSwitchState();
    }
}
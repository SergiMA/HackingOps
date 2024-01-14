using UnityEngine;

namespace HackingOps.CombatSystem
{
    public class CombatModeCoolingDownState : CombatModeBaseState
    {
        private float _currentCooldownDuration;

        public CombatModeCoolingDownState(CombatMode ctx, CombatModeStateFactory factory) : base(ctx, factory)
        {
            _ctx = ctx;
            _factory = factory;
        }

        public override void EnterState() => _currentCooldownDuration = _ctx.CooldownDuration;

        public override void UpdateState()
        {
            _currentCooldownDuration -= Time.deltaTime;
            CheckSwitchState();
        }

        public override void ExitState() { }

        protected override void CheckSwitchState()
        {
            if (_ctx.EntitiesInCombat.Count > 0)
                SwitchState(_factory.GetState(CombatModeStateFactory.States.InCombat));

            if (_currentCooldownDuration <= 0)
                SwitchState(_factory.GetState(CombatModeStateFactory.States.Peaceful));
        }

        public override void CheckEntitiesInCombat()
        {
            CheckSwitchState();
        }
    }
}
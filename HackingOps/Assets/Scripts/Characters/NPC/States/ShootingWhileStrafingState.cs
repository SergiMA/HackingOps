using UnityEngine;

namespace HackingOps.Characters.NPC.States
{
    public class ShootingWhileStrafingState : ShootingStandingState
    {
        private void LateUpdate()
        {
            _entity.Agent.SetDestination(_entity.transform.position + _entity.transform.right);
        }
    }
}
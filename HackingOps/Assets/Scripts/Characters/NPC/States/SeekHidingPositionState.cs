using UnityEngine;

namespace HackingOps.Characters.NPC.States
{
    public class SeekHidingPositionState : State
    {
        private void Update()
        {
            Transform hidingPosition = _entity.GetCurrentHidingDestionationPoint();
            if (hidingPosition != null)
            {
                _entity.Agent.SetDestination(hidingPosition.position);
            }

            if (!hidingPosition || Vector3.Distance(hidingPosition.position, _entity.transform.position) < 1f)
            {
                _entity.CurrentHidingPositionReached();
            }
        }
    }
}
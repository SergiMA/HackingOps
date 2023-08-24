using HackingOps.Characters.Common;
using UnityEngine;

namespace HackingOps.Characters.NPC.States
{
    public class SeekTargetState : State, IMovementReadable
    {
        private void Update()
        {
            _entity.Agent.destination = _entity.Sight.VisiblesInSight[0].GetTransform().position;
        }

        public float GetAcceleratedSpeed()
        {
            return _entity.Agent.speed;
        }

        public bool GetIsCrouched()
        {
            return false;
        }

        public bool GetIsGrounded()
        {
            return true;
        }

        public float GetJumpSpeed()
        {
            return _entity.Agent.speed;
        }

        public float GetNormalSpeed()
        {
            return _entity.Agent.speed;
        }

        public Vector3 GetVelocity()
        {
            return _entity.Agent.velocity;
        }
    }
}
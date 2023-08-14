using UnityEngine;

namespace HackingOps.Characters.Common
{
    public interface IMovementReadable
    {
        Vector3 GetVelocity();
        float GetNormalSpeed();
        float GetAcceleratedSpeed();
        float GetJumpSpeed();
        bool GetIsGrounded();
        bool GetIsCrouched();
    }
}
using UnityEngine;

public interface IMovementReadable
{
    Vector3 GetVelocity();
    float GetWalkSpeed();
    float GetRunSpeed();
    float GetJumpSpeed();
    bool GetIsGrounded();
}

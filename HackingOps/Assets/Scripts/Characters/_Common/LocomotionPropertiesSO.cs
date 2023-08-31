using UnityEngine;

namespace HackingOps.Characters.Common
{
    [CreateAssetMenu(fileName = "New Locomotion Properties", menuName = "Hacking Ops/Characters/LocomotionProperties")]
    public class LocomotionPropertiesSO : ScriptableObject
    {
        [Header("Movement info")]
        public float SpeedNormal = 1.5f;
        public float SpeedAccelerated = 4f;
        public float SpeedJump = 6f;

        [Header("Orientation info")]
        public float SpeedAngular = 360f;
    }
}
using UnityEngine;

namespace HackingOps.Platforms
{
    public struct ObjectOnPlatform
    {

        public Transform ObjectTransform;
        public Transform PreviousParent;

        public ObjectOnPlatform(Transform objectTransform, Transform previousParent)
        {
            ObjectTransform = objectTransform;
            PreviousParent = previousParent;
        }
    }
}
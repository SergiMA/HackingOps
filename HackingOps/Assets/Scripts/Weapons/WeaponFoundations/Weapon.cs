using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public abstract class Weapon : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool _debugUse;
        [SerializeField] private bool _debugStartUsing;
        [SerializeField] private bool _debugStopUsing;

        private void OnValidate()
        {
            if (_debugUse)
            {
                _debugUse = false;
                Use();
            }

            if (_debugStartUsing)
            {
                _debugStartUsing = false;
                StartUsing();
            }

            if (_debugStopUsing)
            {
                StopUsing();
                _debugStopUsing = false;
            }
        }

        private void Awake()
        {
            InternalAwake();
        }

        protected virtual void InternalAwake() { }

        public virtual void Use() { }
        public virtual void StartUsing() { }
        public virtual void StopUsing() { }

        public abstract bool CanUse();
        public abstract bool CanContinuouslyUse();
        public abstract float GetEffectiveRange();

        public virtual void AdaptToVerticalAngle(Vector3 aimForward) { }

        public virtual bool HasGrabPoints() { return false; }
        public virtual Transform GetLeftArmHint() { return null; }
        public virtual Transform GetLeftArmTarget() { return null; }
        public virtual Transform GetRightArmHint() { return null; }
        public virtual Transform GetRightArmTarget() { return null; }
    }
}
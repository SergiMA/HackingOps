using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public class MeleeWeaponOneUse : MeleeWeapon
    {
        [SerializeField] private float _effectiveRange = 1f;
        [SerializeField] private float _damageByHit = 1f;

        public override void Use() { }
        public override bool CanUse() => true;
        public override bool CanContinuouslyUse() => false;
        public override float GetEffectiveRange() => _effectiveRange;
        public override void ResetRotation()
        {
            base.ResetRotation();
        }

        public float GetDamageByHit() => _damageByHit;
    }
}
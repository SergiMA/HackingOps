namespace HackingOps.Weapons.WeaponFoundations
{
    public class FireWeaponOneShot : FireWeapon
    {
        public override bool CanContinuouslyUse() => false;
        public override bool CanUse() => true;
    }
}
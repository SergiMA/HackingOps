using HackingOps.Weapons.Barrels;
using System.Collections;
using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public class FireWeaponContinuousShot : FireWeapon
    {
        public override bool CanContinuouslyUse()
        {
            return true;
        }

        public override bool CanUse()
        {
            return false;
        }
    }
}
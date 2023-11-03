using UnityEngine;

namespace HackingOps.Weapons.Barrels.BarrelsForProjectiles
{
    public class BarrelByMultipleInstantiation : BarrelByInstantiation 
    {
        [SerializeField] private float _amountOfProjectiles = 10f;

        protected override void InternalShot()
        {
            for (int i = 0; i < _amountOfProjectiles; i++)
                base.InternalShot();
        }
    }
}
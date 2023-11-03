using UnityEngine;

namespace HackingOps.Weapons.Barrels
{
    public class BarrelByMultipleRaycasting : BarrelByRaycasting
    {
        [SerializeField] float amountOfRaycast = 10f;

        protected override void InternalShot()
        {
            for (int i = 0; i < amountOfRaycast; i++)
            {
                base.InternalShot();
            }
        }
    }
}
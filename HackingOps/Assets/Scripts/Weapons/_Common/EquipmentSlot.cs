using HackingOps.Weapons.WeaponFoundations;

namespace HackingOps.Weapons.Common
{
    public class EquipmentSlot
    {
        public WeaponSlot Slot;
        public Weapon Weapon;

        public EquipmentSlot(WeaponSlot slot, Weapon weapon)
        {
            Slot = slot;
            Weapon = weapon;
        }
    }
}
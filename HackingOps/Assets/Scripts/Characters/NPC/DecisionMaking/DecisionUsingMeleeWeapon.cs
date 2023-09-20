using HackingOps.Weapons.WeaponFoundations;
using UnityEngine;

namespace HackingOps.Characters.NPC.DecisionMaking
{
    public class DecisionUsingMeleeWeapon : DecisionNode
    {
        Weapon _weapon;

        private void OnEnable()
        {
            _decisionMaker.GetInventory().OnWeaponSwitched += OnWeaponSwitched;
        }

        private void OnDisable()
        {
            _decisionMaker.GetInventory().OnWeaponSwitched -= OnWeaponSwitched;
        }

        protected override bool CheckCondition() => _weapon is MeleeWeapon;

        private void OnWeaponSwitched(Weapon oldWeapon, Weapon newWeapon) => _weapon = newWeapon;
    }
}
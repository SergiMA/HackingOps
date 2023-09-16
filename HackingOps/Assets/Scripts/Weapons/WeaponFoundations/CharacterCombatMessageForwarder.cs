using HackingOps.Characters.Common;
using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public class CharacterCombatMessageForwarder : MonoBehaviour
    {
        private CharacterCombat _characterCombat;

        private void Awake()
        {
            _characterCombat = GetComponentInParent<CharacterCombat>();
        }

        void OnAnimationAttack(string s)
        {
            _characterCombat.OnAnimationAttack(s);
        }

        void OnAnimationStartAttack()
        {
            _characterCombat.OnAnimationStartAttack();
        }

        void OnAnimationFinishAttack()
        {
            _characterCombat.OnAnimationFinishAttack();
        }
    }
}
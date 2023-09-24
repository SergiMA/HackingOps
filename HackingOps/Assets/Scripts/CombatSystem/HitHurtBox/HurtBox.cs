using HackingOps.Characters.Common;
using HackingOps.Characters.Common.CombatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HurtBox : MonoBehaviour
    {
        public UnityEvent OnNotifyHit;
        public UnityEvent<float> OnNotifyHitWithDamage;
        public UnityEvent<float, float, Vector3> OnNotifyHitWithLifeAndDirection;
        public UnityEvent<CharacterCombat> OnNotifyHitCulprit;

        public virtual void NotifyHit(float damage = 1f, Transform damageDealerTransform = null, CharacterCombat damageDealer = null)
        {
            OnNotifyHit.Invoke();
            OnNotifyHitWithDamage.Invoke(damage);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HurtBox : MonoBehaviour
    {
        public UnityEvent OnNotifyHit;
        public UnityEvent<float> OnNotifyHitWithDamage;

        public virtual void NotifyHit(float damage = 1f)
        {
            Debug.Log($"<color=cyan>{name}</color> has been hit!");
            OnNotifyHit.Invoke();
            OnNotifyHitWithDamage.Invoke(damage);
        }
    }
}
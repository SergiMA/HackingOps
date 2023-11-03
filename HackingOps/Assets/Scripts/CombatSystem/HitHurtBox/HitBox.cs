using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.CombatSystem.HitHurtBox
{
    public class HitBox : MonoBehaviour
    {
        public UnityEvent OnDeliveredHit;

        private void OnCollisionEnter(Collision collision) => DeliverHit(collision.collider);
        private void OnTriggerEnter(Collider collider) => DeliverHit(collider);

        public virtual void DeliverHit(Collider collider)
        {
            HurtBox hurtBox = collider.GetComponent<HurtBox>();
            hurtBox?.NotifyHit();
            OnDeliveredHit?.Invoke();
        }
    }
}
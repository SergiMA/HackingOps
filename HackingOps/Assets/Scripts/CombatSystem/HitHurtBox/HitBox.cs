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

        private void DeliverHit(Collider collider)
        {
            HurtBox hurtBox = collider.GetComponent<HurtBox>();
            hurtBox?.NotifyHit();
            OnDeliveredHit?.Invoke();
        }
    }
}
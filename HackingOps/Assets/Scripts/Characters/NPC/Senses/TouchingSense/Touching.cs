using HackingOps.CombatSystem.HitHurtBox;
using HackingOps.Utilities.Timers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Characters.NPC.Senses.SightSense
{
    public class Touching : MonoBehaviour
    {
        public UnityEvent<Transform> OnHitPerformerFound;
        public UnityEvent OnAlertDropped;

        [SerializeField] private float _sensingRadius = 1f;
        [SerializeField] private float _alertDuration = 2f;

        private List<IVisible> _visibles = new();

        private CountdownTimer _touchTimer;

        private void Awake()
        {
            GetComponent<HurtBoxWithLife>()?.OnNotifyHitWithLifeAndDirection.AddListener(HitReceived);

            _touchTimer = new CountdownTimer(_alertDuration);
            _touchTimer.OnStop += () =>
            {
                OnAlertDropped?.Invoke();
            };
        }

        private void Update()
        {
            _touchTimer.Tick(Time.deltaTime);
        }

        private void FillVisiblesList(Collider[] colliders)
        {
            foreach (Collider c in colliders)
            {
                if (c.TryGetComponent(out IVisible visible))
                {
                    _visibles.Add(visible);
                }
            }
        }

        private void HitReceived(float _, float __, Vector3 direction)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _sensingRadius);
            FillVisiblesList(colliders);

            var orderedVisibles = _visibles
                .OrderBy(v => Vector3.Distance(transform.position, v.GetTransform().position));

            OnHitPerformerFound.Invoke(orderedVisibles.FirstOrDefault().GetTransform());

            _touchTimer?.Start();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _sensingRadius);
        }
    }
}
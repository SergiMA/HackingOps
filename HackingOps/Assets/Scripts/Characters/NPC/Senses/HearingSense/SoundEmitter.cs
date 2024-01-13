using System.Collections;
using UnityEngine;

namespace HackingOps.Characters.NPC.Senses.HearingSense
{
    public class SoundEmitter : MonoBehaviour
    {
        [Header("Data")]
        public SoundTypeSO Type;

        [Header("Settings")]
        [SerializeField] private float _range = 10f;
        [SerializeField] private float _checksPerSecond = 3f;
        [SerializeField] private LayerMask _layerMask = Physics.DefaultRaycastLayers;
        [SerializeField] private bool _startEmittingByDefault;
        
        private void OnEnable()
        {
            if (_startEmittingByDefault)
                StartCoroutine(EmittingCoroutine());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator EmittingCoroutine()
        {
            while (true)
            {
                Emit();
                yield return new WaitForSeconds(1f / _checksPerSecond);
            }
        }

        private void Emit()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _range, _layerMask);
            foreach (Collider c in colliders)
            {
                if (c.TryGetComponent(out Hearing hearing))
                    hearing.NotifyHears(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _range);
        }

        public void ChangeSound(SoundTypeSO soundType)
        {
            Type = soundType;
            _range = Type.Range;
        }
    }
}
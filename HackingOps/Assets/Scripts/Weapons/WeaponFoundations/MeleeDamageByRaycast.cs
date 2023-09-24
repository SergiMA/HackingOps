using HackingOps.CombatSystem.HitHurtBox;
using System;
using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public class MeleeDamageByRaycast : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private float _rayMargin = 0.005f;
        [SerializeField] private LayerMask _occludersLayerMask = Physics.DefaultRaycastLayers;

        private Vector3 _lastStartPoint;
        private Vector3 _lastEndPoint;

        private MeleeDamageByRaycastManager _manager;
        private Transform _wielder;

        void Start()
        {
            StoreLastPoints();
        }

        void Update()
        {
            float distance =
                Mathf.Max(
                    Vector3.Distance(_startPoint.position, _lastStartPoint),
                    Vector3.Distance(_endPoint.position, _lastEndPoint));

            int numRays = 1 + Mathf.CeilToInt(distance / _rayMargin);

            for (int i = 0; i < numRays; i++)
            {
                float t = (float)i / (float)numRays;
                Vector3 actualStartPoint = Vector3.Lerp(_startPoint.position, _lastStartPoint, t);
                Vector3 actualEndPoint = Vector3.Lerp(_endPoint.position, _lastEndPoint, t);

                if (Physics.Linecast(actualStartPoint, actualEndPoint, out RaycastHit hit, _occludersLayerMask))
                {
                    _manager.RayImpactedOn(hit);
                }

                Debug.DrawLine(actualStartPoint, actualEndPoint, Color.red, 1f);
            }

            StoreLastPoints();
        }

        private void StoreLastPoints()
        {
            _lastStartPoint = _startPoint.position;
            _lastEndPoint = _endPoint.position;
        }

        public void SetManager(MeleeDamageByRaycastManager manager)
        {
            _manager = manager;
        }

        public void SetWielder(Transform wielder)
        {
            _wielder = wielder;
        }
    }
}
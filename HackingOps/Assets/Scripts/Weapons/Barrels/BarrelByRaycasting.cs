using HackingOps.CombatSystem.HitHurtBox;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.Barrels;
using UnityEngine;

namespace HackingOps.Weapons.Barrels
{
    public class BarrelByRaycasting : Barrel
    {
        [Header("Raycast bindings")]
        [SerializeField] GameObject _shotTracePrefab;
        [SerializeField] Transform _shootPoint;

        [Header("Raycast settings")]
        [SerializeField] private float _range = 100f;
        [Range(0f, 1f)][SerializeField] float _spread = 0.2f;   // In degrees
        [SerializeField] LayerMask _shotLayerMask = Physics.DefaultRaycastLayers;

        protected override void InternalShot()
        {
            Vector3 shootingDirection = _shootPoint.forward;
            shootingDirection.x += Random.Range(-_spread, _spread);
            shootingDirection.y += Random.Range(-_spread, _spread);
            shootingDirection.z += Random.Range(-_spread, _spread);

            ShotTrace shotTrace = null;
            if (_shotTracePrefab)
            {
                GameObject shotTraceGO = Instantiate(_shotTracePrefab);
                shotTrace = shotTraceGO.GetComponent<ShotTrace>();
            }

            if (Physics.Raycast(_shootPoint.position, shootingDirection, out RaycastHit hit, _range, _shotLayerMask))
            {
                hit.collider.GetComponent<HurtBox>()?.NotifyHit(1f, transform);
                shotTrace?.Init(_shootPoint.position, _shootPoint.position + (shootingDirection * hit.distance));
            }
            else
            {
                float shotTraceRange = Mathf.Min(_range, 200f);
                shotTrace?.Init(_shootPoint.position, _shootPoint.position + (shootingDirection * shotTraceRange));
            }
        }

        protected override void InternalStartShooting()
        {
        }

        protected override void InternalStopShooting()
        {
        }
    }
}
using HackingOps.Weapons.Projectiles;
using UnityEngine;

namespace HackingOps.Weapons.Barrels.BarrelsForProjectiles
{
    public class BarrelByInstantiation : Barrel
    {
        [Header("Projectiles bindings")]
        [SerializeField] private GameObject _prefabToInstantiate;
        [SerializeField] private Transform _shootPoint;

        [Header("Projectiles settings")]
        [SerializeField] private float _launchSpeed = 10f;
        [Range(0f, 1f)][SerializeField] private float _spread = 0.2f;

        protected override void InternalShot()
        {
            Vector3 shootingDirection = _shootPoint.forward;
            shootingDirection.x += Random.Range(-_spread, _spread);
            shootingDirection.y += Random.Range(-_spread, _spread);
            shootingDirection.z += Random.Range(-_spread, _spread);

            Quaternion shootingRotation = Quaternion.LookRotation(shootingDirection);

            GameObject newInstance = Instantiate(_prefabToInstantiate, _shootPoint.position, shootingRotation);
            newInstance.GetComponent<Rigidbody>()?.AddForce(shootingDirection * _launchSpeed, ForceMode.VelocityChange);
            newInstance.GetComponent<Projectile>()?.SetOriginTransform(transform);
        }

        protected override void InternalStartShooting()
        {
        }

        protected override void InternalStopShooting()
        {
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Characters.NPC.Senses
{
    public class Sight : MonoBehaviour
    {
        [SerializeField] public List<IVisible> VisiblesInSight = new List<IVisible>();

        [SerializeField] private Transform _sightPoint;

        [SerializeField] float _checksPerSecond = 4f;

        [SerializeField] private float _range = 20f;
        [SerializeField] private float _horizontalAngle = 120f;
        [SerializeField] private float _verticalAngle = 90f;
        [SerializeField] private LayerMask _occludersLayerMask = Physics.DefaultRaycastLayers;

        private double _lastCheckTime = 0f;

        private void Awake()
        {
            _lastCheckTime = Time.time + Random.Range(0f, 1f / _checksPerSecond);
        }

        private void Update()
        {
            if ((Time.time - _lastCheckTime) > (1f / _checksPerSecond))
            {
                _lastCheckTime = Time.time;
                CheckSight();
            }
        }

        private void OnDrawGizmos()
        {
            if (!_sightPoint)
                return;

            Vector3 halfExtents = CalcHalfExtents();

            Vector3 origin = _sightPoint.position;
            Vector3 up = _sightPoint.up;
            Vector3 right = _sightPoint.right;
            Vector3 forward = _sightPoint.forward;

            Vector3 topLeftCorner = origin + (forward * (halfExtents.z * 2f)) + (-right * halfExtents.x) + (up * halfExtents.y);
            Vector3 topRightCorner = origin + (forward * (halfExtents.z * 2f)) + (right * halfExtents.x) + (up * halfExtents.y);
            Vector3 bottomLeftCorner = origin + (forward * (halfExtents.z * 2f)) + (-right * halfExtents.x) + (-up * halfExtents.y);
            Vector3 bottomRightCorner = origin + (forward * (halfExtents.z * 2f)) + (right * halfExtents.x) + (-up * halfExtents.y);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(origin, topLeftCorner);
            Gizmos.DrawLine(origin, topRightCorner);
            Gizmos.DrawLine(origin, bottomLeftCorner);
            Gizmos.DrawLine(topLeftCorner, topRightCorner);
            Gizmos.DrawLine(topRightCorner, bottomRightCorner);
            Gizmos.DrawLine(bottomRightCorner, bottomLeftCorner);
            Gizmos.DrawLine(bottomLeftCorner, topLeftCorner);
        }

        private void CheckSight()
        {
            Vector3 halfExtents = new Vector3(_range / 2f, _range / 2f, _range / 2f);
            Collider[] collidersInBox =
                Physics.OverlapBox(
                    transform.position + _sightPoint.forward * _range / 2f,    // Center
                    halfExtents,
                    _sightPoint.rotation
                );

            VisiblesInSight.Clear();
            foreach (Collider c in collidersInBox)
            {
                if (c.TryGetComponent(out IVisible visible))
                {
                    Vector3 directionToVisible = c.transform.position - _sightPoint.position;

                    Vector3 directionOnXZPlane = Vector3.ProjectOnPlane(directionToVisible, Vector3.up);
                    Vector3 forwardOnXZPlane = Vector3.ProjectOnPlane(_sightPoint.forward, Vector3.up);

                    Vector3 directionOnLocalYZPlane = Vector3.ProjectOnPlane(directionToVisible, _sightPoint.right);
                    Vector3 forwardOnLocalYZPlane = Vector3.ProjectOnPlane(_sightPoint.forward, _sightPoint.right);

                    if ((Vector3.Angle(directionOnXZPlane, forwardOnXZPlane) < _horizontalAngle / 2f) &&
                        (Vector3.Angle(directionOnLocalYZPlane, forwardOnLocalYZPlane) < _verticalAngle / 2f))
                    {
                        bool isAnyPointVisible = false;
                        int i = 0;
                        while (!isAnyPointVisible && (i < visible.GetCheckpoints().Length))
                        {
                            Debug.DrawLine(_sightPoint.position, visible.GetCheckpoints()[i], Color.yellow, 0.5f);

                            if (Physics.Linecast(_sightPoint.position, visible.GetCheckpoints()[i], out RaycastHit hit, _occludersLayerMask))
                            {
                                isAnyPointVisible = hit.collider == c;
                            }

                            i++;
                        }

                        if (isAnyPointVisible)
                        {
                            VisiblesInSight.Add(visible);
                        }
                    }
                }
            }
        }

        private Vector3 CalcHalfExtents()
        {
            float halfSightWidth = _range / Mathf.Tan((90f - _horizontalAngle / 2f) * Mathf.Deg2Rad);
            float halfSightHeight = _range / Mathf.Tan(90f - (_verticalAngle / 2f) * Mathf.Deg2Rad);

            return new Vector3(halfSightWidth, halfSightHeight, _range / 2f);
        }
    }
}
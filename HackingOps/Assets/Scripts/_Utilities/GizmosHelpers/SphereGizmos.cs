using UnityEngine;

namespace HackingsOps.Utilities.GizmosHelpers
{
    public class SphereGizmos : MonoBehaviour
    {
        [SerializeField] private float _radius = 0.1f;
        [SerializeField] private Color _color = Color.red;
        [SerializeField] private bool _showWireframed;

        private void OnDrawGizmos()
        {
            Gizmos.color = _color;
            if (_showWireframed)
            {
                Gizmos.DrawWireSphere(transform.position, _radius);
            }
            else
            {
                Gizmos.DrawSphere(transform.position, _radius);
            }
        }
    }
}
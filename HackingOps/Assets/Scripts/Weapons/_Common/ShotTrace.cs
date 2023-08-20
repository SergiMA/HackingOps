using DG.Tweening;
using UnityEngine;

namespace HackingOps.Weapons.Common
{
    public class ShotTrace : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 0.25f;

        LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Init(Vector3 origin, Vector3 destination)
        {
            _lineRenderer.SetPosition(0, origin);
            _lineRenderer.SetPosition(1, destination);
            DOTween.To(
                () => _lineRenderer.widthMultiplier,
                (x) => _lineRenderer.widthMultiplier = x,
                0f,
                _lifeTime).
                OnComplete(() => Destroy(gameObject));
        }
    }
}
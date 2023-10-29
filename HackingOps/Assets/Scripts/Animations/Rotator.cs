using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace HackingOps.Animations
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _angularSpeed;
        [SerializeField] private float _changingSpeedDuration = 5f;

        [Space]
        [Header("Debug")]
        [SerializeField] private bool _debugChangeRotationAngleSpeed;
        [SerializeField] private Vector3 _debugNewRotationAngleSpeed;
        [SerializeField] private float _debugSpeedChangeDuration = 5f;

        private void OnValidate()
        {
            if (_debugChangeRotationAngleSpeed)
            {
                _debugChangeRotationAngleSpeed = false;
                ChangeAngularSpeed(_debugNewRotationAngleSpeed, _debugSpeedChangeDuration);
            }
        }

        private void Update()
        {
            transform.Rotate(_angularSpeed * Time.deltaTime);
        }

        private void ApplyAngularSpeedChange(Vector3 newAngularSpeed, float changingSpeedDuration)
        {
            DOVirtual.Vector3(_angularSpeed, newAngularSpeed, changingSpeedDuration, (x) =>
            {
                _angularSpeed = x;
            });
        }

        public void ChangeAngularSpeed(Vector3 newAngularSpeed)
        {
            ApplyAngularSpeedChange(newAngularSpeed, _changingSpeedDuration);
        }

        public void ChangeAngularSpeed(Vector3 newAngularSpeed, float changingSpeedDuration)
        {
            ApplyAngularSpeedChange(newAngularSpeed, changingSpeedDuration);
        }
    }
}
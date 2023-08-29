using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Platforms
{
    public class PlatformParenter : MonoBehaviour
    {
        public UnityEvent OnScanPerformed;

        [Header("Bindings")]
        [SerializeField] private Collider _scannerCollider;

        [Header("Settings")]
        [SerializeField] private float _scanningDuration = 0.1f;

        private List<ObjectOnPlatform> _objectsOnPlatform = new List<ObjectOnPlatform>();
        private float _currentScanningDuration;

        private void Start()
        {
            _scannerCollider.enabled = false;
        }

        private void Update()
        {
            if (_currentScanningDuration > 0)
            {
                _currentScanningDuration -= Time.deltaTime;
                _currentScanningDuration = Mathf.Max(_currentScanningDuration, 0f);

                if (_currentScanningDuration <= 0f)
                {
                    _scannerCollider.enabled = false;
                    OnScanPerformed.Invoke();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Transform objectDetected = other.transform;

            ObjectOnPlatform newObject = new ObjectOnPlatform(objectDetected, objectDetected.parent);
            _objectsOnPlatform.Add(newObject);

            objectDetected.SetParent(transform);
        }

        public void Load()
        {
            _objectsOnPlatform.Clear();
            _scannerCollider.enabled = true;
            _currentScanningDuration = _scanningDuration;
        }

        public void Unload()
        {
            foreach (ObjectOnPlatform objectOnPlatform in _objectsOnPlatform)
            {
                objectOnPlatform.ObjectTransform.SetParent(objectOnPlatform.PreviousParent);
            }
        }
    }
}
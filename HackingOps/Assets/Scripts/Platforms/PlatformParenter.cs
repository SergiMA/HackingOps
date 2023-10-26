using System.Collections.Generic;
using System.Net.Http.Headers;
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
        [SerializeField] private string[] _allowedTags = { "Player" };

        private HashSet<ObjectOnPlatform> _objectsOnPlatform = new();
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
                    OnScanPerformed.Invoke();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsTagAllowed(other.tag))
                return;

            Transform objectDetected = other.transform;
            ObjectOnPlatform newObject = new ObjectOnPlatform(objectDetected, objectDetected.parent);

            if (ContainsDetectedObject(newObject))
                return;

            _objectsOnPlatform.Add(newObject);

            objectDetected.SetParent(transform);
        }

        private bool IsTagAllowed(string tagToCheck)
        {
            foreach (string tag in _allowedTags)
            {
                if (tagToCheck == tag)
                    return true;
            }

            return false;
        }

        private bool ContainsDetectedObject(ObjectOnPlatform newObject)
        {
            foreach (ObjectOnPlatform objectOnPlatform in _objectsOnPlatform)
            {
                if (objectOnPlatform.ObjectTransform == newObject.ObjectTransform)
                    return true;
            }

            return false;
        }    

        public void Load()
        {
            _objectsOnPlatform.Clear();
            _scannerCollider.enabled = true;
            _currentScanningDuration = _scanningDuration;
        }

        public void Unload()
        {
            _scannerCollider.enabled = false;
            foreach (ObjectOnPlatform objectOnPlatform in _objectsOnPlatform)
            {
                objectOnPlatform.ObjectTransform.SetParent(objectOnPlatform.PreviousParent);
            }
        }
    }
}
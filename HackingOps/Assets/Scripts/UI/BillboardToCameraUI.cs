using System.Collections;
using UnityEngine;

namespace HackingOps.UI
{
    public class BillboardToCameraUI : MonoBehaviour
    {
        Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (_camera == null) return;

            transform.LookAt(transform.position + (_camera.transform.rotation * Vector3.forward),
                             _camera.transform.rotation * Vector3.up);
        }
    }
}
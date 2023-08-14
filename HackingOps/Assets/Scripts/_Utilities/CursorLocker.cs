using UnityEngine;

namespace HackingOps.Utilities
{
    public class CursorLocker : MonoBehaviour
    {
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

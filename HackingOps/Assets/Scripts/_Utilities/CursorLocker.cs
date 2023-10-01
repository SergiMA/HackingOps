using UnityEngine;

namespace HackingOps.Utilities
{
    public class CursorLocker : MonoBehaviour
    {
        [SerializeField] private CursorLockState _cursorLockState = CursorLockState.Locked;

        private CursorLockState _previousCursorLockState;

        private enum CursorLockState
        {
            Confined,
            Locked,
            Free,
        }

        private void Start()
        {
            ApplyCursorLockState(_cursorLockState);
        }

        private void ApplyCursorLockState(CursorLockState cursorLockState)
        {
            switch (cursorLockState)
            {
                case CursorLockState.Confined: ConfineCursor(); break;
                case CursorLockState.Locked: LockCursor(); break;
                case CursorLockState.Free: FreeCursor(); break;
            }
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;

            _previousCursorLockState = _cursorLockState;
            _cursorLockState = CursorLockState.Locked;
        }

        public void ConfineCursor()
        {
            Cursor.lockState = CursorLockMode.Confined;

            _previousCursorLockState = _cursorLockState;
            _cursorLockState = CursorLockState.Confined;
        }

        public void FreeCursor()
        {
            Cursor.lockState = CursorLockMode.None;

            _previousCursorLockState = _cursorLockState;
            _cursorLockState = CursorLockState.Free;
        }

        public void RecoverPreviousCursor()
        {
            ApplyCursorLockState(_previousCursorLockState);
        }
    }
}

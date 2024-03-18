using UnityEngine;

namespace HackingOps.Common.Followers
{
    public class Follower : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Transform _targetToFollow;

        [Header("Offset")]
        [SerializeField] private Vector3 _offset = Vector3.zero;

        [Space]
        [SerializeField] private bool _restrictXAxis;
        [SerializeField] private bool _restrictYAxis;
        [SerializeField] private bool _restrictZAxis;

        [Header("Update method")]
        [SerializeField] private UpdateMode _updateMode = UpdateMode.Update;
        
        // Enums
        private enum UpdateMode
        {
            Update,
            FixedUpdate,
            LateUpdate,
        }

        #region Getters & Setters
        public Transform TargetToFollow {  get { return _targetToFollow; } }
        #endregion

        #region Unity methods
        private void Start()
        {
            FollowTarget();
        }

        private void Update()
        {
            if (_updateMode != UpdateMode.Update) return;
            FollowTarget();
        }

        private void FixedUpdate()
        {
            if (_updateMode != UpdateMode.FixedUpdate) return;
            FollowTarget();
        }

        private void LateUpdate()
        {
            if (_updateMode != UpdateMode.LateUpdate) return;
            FollowTarget();
        }
        #endregion

        public Vector3 GetTargetPosition()
        {
            Vector3 targetPosition = _targetToFollow.position + _offset;

            if (_restrictXAxis) targetPosition.x = transform.position.x;
            if (_restrictYAxis) targetPosition.y = transform.position.y;
            if (_restrictZAxis) targetPosition.z = transform.position.z;

            return targetPosition;
        }

        private void FollowTarget()
        {
            Vector3 targetPosition = GetTargetPosition();
            transform.position = targetPosition;
        }
    }
}
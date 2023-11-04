using DG.Tweening;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HackingOps.Characters.Common
{
    public class CharacterCombat : MonoBehaviour, IAttackReadable
    {
        public UnityEvent OnStartBlock;
        public UnityEvent OnStopBlock;
        public UnityEvent OnStartAiming;
        public UnityEvent OnStopAiming;
        public UnityEvent OnParried;
        public UnityEvent OnStunnedExpired;

        public event Action OnMustAttack;

        [SerializeField] private Transform _hitBoxesParent;
        [SerializeField] private float _stunnedDuration = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool _debugAttack;

        bool _mustAttack;
        bool _isBlocking;
        bool _isCombatWeapon;
        bool _isUnarmed;

        Weapon weapon;

        MeleeDamageByRaycastManager _meleeDamageByRaycastManager;


        private void OnValidate()
        {
            if (_debugAttack)
            {
                _debugAttack = false;
                _mustAttack = true;
            }
        }

        private void OnEnable()
        {
            GetComponent<Inventory>().OnWeaponSwitched += OnWeaponSelected;
        }

        private void OnDisable()
        {
            GetComponent<Inventory>().OnWeaponSwitched -= OnWeaponSelected;
        }

        private void Start()
        {
            foreach (Transform t in _hitBoxesParent)
                t.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            _mustAttack = false;
        }

        bool IAttackReadable.MustAttack() => _mustAttack;

        private void OnShoot(float analogValue)
        {
            if (_isCombatWeapon)
            {
                if (analogValue > 0f) _mustAttack = true;
            }
        }

        public void Attack()
        {
            if (_isUnarmed)
                return;

            if (_isCombatWeapon)
            {
                _mustAttack = true;
                OnMustAttack?.Invoke();
            }
        }

        private void OnWeaponSelected(Weapon oldWeapon, Weapon newWeapon)
        {
            _isCombatWeapon = (newWeapon == null) || (newWeapon is not FireWeapon);

            if (newWeapon != null)
            {
                _meleeDamageByRaycastManager = newWeapon.GetComponentInChildren<MeleeDamageByRaycastManager>();
                _meleeDamageByRaycastManager?.SetWielder(this);
            }
        }

        private void UpdateStunnedStatus()
        {
            OnStunnedExpired.Invoke();
        }

        public void OnAnimationAttack(string s)
        {
            GameObject hitBoxGO = _hitBoxesParent.Find(s)?.gameObject;

            if (hitBoxGO)
            {
                hitBoxGO.SetActive(true);
                DOVirtual.DelayedCall(0.2f, () => hitBoxGO.SetActive(false));
            }
        }

        public void OnAnimationStartAttack()
        {
            if (weapon == null && !_isCombatWeapon) return;

            _meleeDamageByRaycastManager?.StartDamageArea();
        }

        public void OnAnimationFinishAttack()
        {
            if (weapon == null && !_isCombatWeapon) return;

            _meleeDamageByRaycastManager?.EndDamageArea();
        }

        public void OnStartLockReceived()
        {
            if (_isUnarmed)
                return;

            if (_isCombatWeapon)
                OnStartBlock.Invoke();
            else
                OnStartAiming.Invoke();
        }

        public void OnStopLockReceived()
        {
            if (_isCombatWeapon)
                OnStopBlock.Invoke();
            else
                OnStopAiming.Invoke();
        }

        public void OnBlockStarted()
        {
            _isBlocking = true;
        }

        public void OnBlockEnded()
        {
            _isBlocking = false;
        }

        public bool IsBlocking() => _isBlocking;
        public void OnPerfectParryReceived()
        {
            Invoke(nameof(UpdateStunnedStatus), _stunnedDuration);
            OnParried.Invoke();
        }

        public void OnGotUnarmed()
        {
            _isUnarmed = true;
        }

        public void OnGotArmed()
        {
            _isUnarmed = false;
        }
    }
}
using DG.Tweening;
using HackingOps.Characters.Player;
using HackingOps.Input;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Characters.Common
{
    public class CharacterCombat : MonoBehaviour, IAttackReadable
    {
        [SerializeField] PlayerInputManager _inputManager;
        [SerializeField] private Transform _hitBoxesParent;
        [SerializeField] private bool _debugAttack;

        bool _mustAttack;
        bool _isCombatWeapon;
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
            _inputManager.OnShoot += OnShoot;
        }

        private void OnDisable()
        {
            GetComponent<Inventory>().OnWeaponSwitched -= OnWeaponSelected;
            _inputManager.OnShoot -= OnShoot;
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

        private void OnWeaponSelected(Weapon oldWeapon, Weapon newWeapon)
        {
            _isCombatWeapon = (newWeapon == null) || (newWeapon is not FireWeapon);

            if (newWeapon != null)
            {
                _meleeDamageByRaycastManager = newWeapon.GetComponentInChildren<MeleeDamageByRaycastManager>();
                _meleeDamageByRaycastManager?.SetWielder(transform);
            }
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
    }
}
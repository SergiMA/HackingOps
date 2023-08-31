using DG.Tweening;
using HackingOps.Characters.Player;
using HackingOps.Input;
using HackingOps.Weapons.WeaponFoundations;
using System;
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

        private void OnValidate()
        {
            if (_debugAttack)
            {
                _debugAttack = false;
                _mustAttack = true;
            }
        }

        private void Awake()
        {
            GetComponent<PlayerWeapons>().OnWeaponSelected.AddListener(OnWeaponSelected);
        }

        private void OnEnable()
        {
            _inputManager.OnShoot += OnShoot;
        }

        private void OnDisable()
        {
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

        private void OnWeaponSelected(Weapon weapon)
        {
            _isCombatWeapon = (weapon == null) || (weapon is not FireWeapon);
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
    }
}
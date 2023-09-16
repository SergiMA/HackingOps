using HackingOps.CombatSystem.HitHurtBox;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Weapons.WeaponFoundations
{
    public class MeleeDamageByRaycastManager : MonoBehaviour
    {
        [SerializeField] private MeleeWeaponOneUse _meleeWeapon;
        [SerializeField] private Transform rayParent;

        private MeleeDamageByRaycast[] _meleeDamageByRaycast;
        private Transform _wielder;
        private HashSet<HurtBox> _damagedHurtBoxes = new();
        private float _damageByHit;

        private void Awake()
        {
            _meleeDamageByRaycast = rayParent.GetComponentsInChildren<MeleeDamageByRaycast>(true);
        }

        private void Start()
        {
            foreach (MeleeDamageByRaycast meleeDamageByRaycast in _meleeDamageByRaycast)
            {
                meleeDamageByRaycast.SetManager(this);
            }
        }

        public void SetWielder(Transform wielder)
        {
            _wielder = wielder;
        }

        public void StartDamageArea()
        {
            foreach (MeleeDamageByRaycast meleeDamageByRaycast in _meleeDamageByRaycast)
            {
                meleeDamageByRaycast.enabled = true;
            }
        }

        public void EndDamageArea()
        {
            foreach (MeleeDamageByRaycast meleeDamageByRaycast in _meleeDamageByRaycast)
            {
                meleeDamageByRaycast.enabled = false;
            }

            _damagedHurtBoxes.Clear();
        }

        public void RayImpactedOn(RaycastHit hit)
        {
            if (hit.transform != _wielder)
            {
                if (hit.collider.TryGetComponent(out HurtBox hurtBox))
                {
                    if (!_damagedHurtBoxes.Contains(hurtBox))
                    {
                        _damagedHurtBoxes.Add(hurtBox);
                        hurtBox.NotifyHit(_meleeWeapon.GetDamageByHit(), transform);
                    }
                }
            }
        }
    }
}
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Weapons.WeaponFoundations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HackingOps.Weapons.Common
{
    public class Inventory : MonoBehaviour, IEventObserver
    {
        public event Action<Weapon> OnWeaponAdded;
        public event Action<Weapon, Weapon> OnWeaponSwitched;
        public event Action<Weapon> OnWeaponDropped;

        [SerializeField] private Transform _weaponParent;
        [SerializeField] private Weapon[] _startingWeapons;
        [SerializeField] private bool _startUnarmed;

        private List<EquipmentSlot> _equipmentSlots = new();
        public int _currentSlotIndex { get; private set; } = 0;

        private void Awake()
        {
            InitializeSlots();
        }

        private void OnEnable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.Interaction, this);
        }

        public void OnDisable()
        {
            ServiceLocator.Instance.GetService<IEventQueue>().Unsubscribe(EventIds.Interaction, this);
        }

        private void Start()
        {
            AddStartingWeapons();
            if (_startUnarmed)
                SwitchSlot(GetCurrentWeaponSlot(), _equipmentSlots[0]);
        }

        private void InitializeSlots()
        {
            _equipmentSlots.Clear();

            foreach (string slotName in Enum.GetNames(typeof(WeaponSlot)))
            {
                EquipmentSlot equipmentSlot = new EquipmentSlot((WeaponSlot)Enum.Parse(typeof(WeaponSlot), slotName), null);
                _equipmentSlots.Add(equipmentSlot);
            }
        }

        private void AddStartingWeapons()
        {
            foreach (Weapon weapon in _startingWeapons)
            {
                AddWeapon(weapon);
            }
        }

        private void SwitchSlot(EquipmentSlot oldSlot, EquipmentSlot newSlot)
        {
            OnWeaponSwitched?.Invoke(oldSlot.Weapon, newSlot.Weapon);

            if (oldSlot.Weapon != null)  // Unarmed is considered as a null weapon
            {
                oldSlot.Weapon.ResetWeapon();
                oldSlot.Weapon.Store();
            }

            if (newSlot.Weapon != null) // Unarmed is considered as a null weapon
            {
                newSlot.Weapon.ResetWeapon();
                newSlot.Weapon.Grab();
            }

            _currentSlotIndex = GetEquipmentSlotIndexByWeaponSlot(newSlot.Slot);
        }

        public void AddWeapon(Weapon weapon)
        {
            if (!weapon) return;

            if (GetEquipmentSlotByWeaponSlot(weapon.Slot).Weapon != null)
            {
                DropWeaponFromSlot(GetEquipmentSlotByWeaponSlot(weapon.Slot));
            }

            GetEquipmentSlotByWeaponSlot(weapon.Slot).Weapon = weapon;

            OnWeaponAdded?.Invoke(weapon);

            if (GetCurrentSlot().Weapon == null)
            {
                SwitchSlot(GetCurrentWeaponSlot(), GetEquipmentSlotByWeaponSlot(weapon.Slot));
            }
            else
            {
                weapon.Store();
            }
        }

        public void DropWeaponFromSlot(EquipmentSlot slotToEmpty)
        {
            if (slotToEmpty.Weapon == null) return;

            EquipmentSlot equipmentSlot = GetEquipmentSlotByWeaponSlot(slotToEmpty.Slot);
            OnWeaponDropped?.Invoke(equipmentSlot.Weapon);
            equipmentSlot.Weapon.Drop();
            equipmentSlot.Weapon = null;
            if (GetCurrentSlot().Weapon == null)
                SwitchSlot(GetCurrentSlot(), GetEquipmentSlotByWeaponSlot(WeaponSlot.Unarmed));
        }

        public EquipmentSlot GetCurrentWeaponSlot()
        {
            return _equipmentSlots[_currentSlotIndex];
        }

        public void ChangeToNextWeapon()
        {
            int newSlotIndex = _currentSlotIndex;

            bool weaponIsNotNull;
            bool slotIsUnarmed;
            do
            {
                newSlotIndex++;

                if (newSlotIndex >= _equipmentSlots.Count)
                    newSlotIndex = 0;

                weaponIsNotNull = _equipmentSlots[newSlotIndex].Weapon != null;
                slotIsUnarmed = _equipmentSlots[newSlotIndex].Slot == WeaponSlot.Unarmed;
            } while (!weaponIsNotNull && !slotIsUnarmed);

            EquipmentSlot oldSlot = _equipmentSlots[_currentSlotIndex];
            EquipmentSlot newSlot = _equipmentSlots[newSlotIndex];

            SwitchSlot(oldSlot, newSlot);

            _currentSlotIndex = newSlotIndex;
        }

        public void ChangeToPreviousWeapon()
        {
            int newSlotIndex = _currentSlotIndex;

            bool weaponIsNotNull;
            bool slotIsUnarmed;
            do
            {
                newSlotIndex--;

                if (newSlotIndex < 0)
                    newSlotIndex = _equipmentSlots.Count - 1;

                weaponIsNotNull = _equipmentSlots[newSlotIndex].Weapon != null;
                slotIsUnarmed = _equipmentSlots[newSlotIndex].Slot == WeaponSlot.Unarmed;
            } while (!weaponIsNotNull && !slotIsUnarmed);


            EquipmentSlot oldSlot = _equipmentSlots[_currentSlotIndex];
            EquipmentSlot newSlot = _equipmentSlots[newSlotIndex]; ;

            SwitchSlot(oldSlot, newSlot);

            _currentSlotIndex = newSlotIndex;
        }

        public EquipmentSlot GetCurrentSlot() => _equipmentSlots[_currentSlotIndex];
        public EquipmentSlot GetEquipmentSlotByWeaponSlot(WeaponSlot weaponSlot)
        {
            foreach (EquipmentSlot equipmentSlot in _equipmentSlots)
            {
                if (weaponSlot == equipmentSlot.Slot)
                {
                    return equipmentSlot;
                }
            }

            Debug.LogError($"No slot named {weaponSlot} has been found in the Equipment Slots", gameObject);
            return null;
        }

        public int GetEquipmentSlotIndexByWeaponSlot(WeaponSlot weaponSlot)
        {
            for (int i = 0; i < _equipmentSlots.Count; i++)
            {
                if (weaponSlot == _equipmentSlots[i].Slot)
                {
                    return i;
                }
            }

            Debug.LogError($"Can't get equipment slot's index because no slot named {weaponSlot} has been found in the Equipment Slots.", gameObject);
            return -1;
        }

        #region IInteractable implementation
        public void Process(EventData eventData)
        {
            if (eventData.EventId != EventIds.Interaction) return;

            InteractionEventData data = eventData as InteractionEventData;
            if (data.InteractableTransform.TryGetComponent(out Weapon weapon))
            {
                if (weapon.CanBeInteracted())
                {
                    AddWeapon(weapon);
                }
            }
        }
        #endregion
    }
}
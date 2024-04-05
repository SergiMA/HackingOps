using HackingOps.Characters.Common;
using HackingOps.Characters.Player.WeaponSheatherSystem.States;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using HackingOps.Weapons.Common;
using HackingOps.Weapons.WeaponFoundations;
using System;
using Unity.VisualScripting;
using UnityEngine;

namespace HackingOps.Characters.Player.WeaponSheatherSystem
{
    public class WeaponSheather : MonoBehaviour, IEventObserver
    {
        [SerializeField] float _coolingDownDuration = 5f;

        private PlayerWeapons _playerWeapons;
        private CharacterCombat _characterCombat;
        private Inventory _inventory;
        private IEventQueue _eventQueue;

        private bool _isBlocking;
        private bool _isEngagedInCombat;
        private bool _isUsingMeleeWeapon;
        private bool _isInCutscene;

        // State bindings
        private WeaponSheatherBaseState _currentState;
        private WeaponSheatherStateFactory _stateFactory;

        #region Getters & Setters
        public WeaponSheatherBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public PlayerWeapons PlayerWeapons { get { return _playerWeapons; } }
        public float CoolingDownDuration { get { return _coolingDownDuration; } }
        public bool IsBlocking { get { return _isBlocking; } }
        public bool IsEngagedInCombat { get { return _isEngagedInCombat; } }
        public Inventory Inventory { get { return _inventory; } }
        public bool IsUsingMeleeWeapon { get { return _isUsingMeleeWeapon; } }
        public bool IsInCutscene {  get { return _isInCutscene; } }
        #endregion

        private void Awake()
        {
            _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
            _playerWeapons = GetComponent<PlayerWeapons>();
            _characterCombat = GetComponent<CharacterCombat>();
            _inventory = GetComponent<Inventory>();
        }

        private void OnEnable()
        {
            _eventQueue.Subscribe(EventIds.OnEnterCombatMode, this);
            _eventQueue.Subscribe(EventIds.OnLeaveCombatMode, this);
            _eventQueue.Subscribe(EventIds.CutsceneStarted, this);
            _eventQueue.Subscribe(EventIds.CutsceneFinished, this);
        }

        private void OnDisable()
        {
            _eventQueue.Unsubscribe(EventIds.OnEnterCombatMode, this);
            _eventQueue.Unsubscribe(EventIds.OnLeaveCombatMode, this);
            _eventQueue.Unsubscribe(EventIds.CutsceneStarted, this);
            _eventQueue.Unsubscribe(EventIds.CutsceneFinished, this);
        }

        private void Start()
        {
            _stateFactory = new WeaponSheatherStateFactory(this);
            _currentState = _stateFactory.GetState(WeaponSheatherStateFactory.States.Peaceful);
            _currentState.EnterState();

            _characterCombat.OnMustAttack += () => _currentState.OnEnterAlertMode();
            _inventory.OnWeaponSwitched += OnWeaponSwitched;
        }

        private void Update()
        {
            _currentState.UpdateState();
        }

        public void OnStartBlocking()
        {
            _isBlocking = true;
            _currentState.OnEnterCombatMode();
        }

        public void OnStopBlocking() => _isBlocking = false;

        public void OnWeaponSwitched(Weapon oldWeapon, Weapon newWeapon)
        {
            if (newWeapon)
            {
                if (newWeapon.Slot == WeaponSlot.MeleeWeapon) _isUsingMeleeWeapon = true;
                else _isUsingMeleeWeapon = false;
            }

            _currentState.OnEnterAlertMode();
        }

        #region Implemented from IEventObserver
        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.OnEnterCombatMode:
                    _isEngagedInCombat = true;
                    _currentState.OnEnterCombatMode();
                    break;
                case EventIds.OnLeaveCombatMode:
                    _isEngagedInCombat = false;
                    _currentState.OnExitCombatMode();
                    break;
                case EventIds.CutsceneStarted:
                    _isInCutscene = true;
                    break;
                case EventIds.CutsceneFinished:
                    _isInCutscene = false;
                    break;
            }
        }
        #endregion
    }
}
using HackingOps.Input;
using UnityEngine;

namespace HackingOps.Characters.Common.CombatSystem
{
    public class PlayableCombatInputForwarder : MonoBehaviour
    {
        [Header("Bindings")]
        [SerializeField] private PlayerInputManager _inputManager;

        [Header("Debug")]
        [SerializeField] private bool _debugAttack;

        CharacterCombat _characterCombat;

        private float _previousAnalogValue;

        private void OnValidate()
        {
            if (_debugAttack)
            {
                _debugAttack = false;
                _characterCombat.Attack();
            }
        }

        private void Awake()
        {
            _characterCombat = GetComponent<CharacterCombat>();
        }

        private void OnEnable()
        {
            _inputManager.OnShoot += OnAttack;

            _inputManager.OnStartAiming += OnStartAiming;
            _inputManager.OnStopAiming += OnStopAiming;
        }

        private void OnDisable()
        {
            _inputManager.OnShoot -= OnAttack;

            _inputManager.OnStartAiming -= OnStartAiming;
            _inputManager.OnStopAiming -= OnStopAiming;
        }


        private void OnAttack(float analogValue)
        {
            if (analogValue > 0f)
            {
                _characterCombat.Attack();
            }
        }

        private void OnBlock(float analogValue)
        {
            if (_previousAnalogValue != analogValue)
            {
                if (analogValue > 0)
                    _characterCombat.OnStartLockReceived();
                else
                    _characterCombat.OnStopLockReceived();
            }

            _previousAnalogValue = analogValue;
        }

        private void OnStartAiming()
        {
            _characterCombat.OnStartLockReceived();
        }

        private void OnStopAiming()
        {
            _characterCombat.OnStopLockReceived();
        }
    }
}
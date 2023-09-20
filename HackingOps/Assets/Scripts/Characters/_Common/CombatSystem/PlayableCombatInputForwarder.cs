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
        }

        private void OnDisable()
        {
            _inputManager.OnShoot -= OnAttack;
        }


        private void OnAttack(float analogValue)
        {
            if (analogValue > 0f)
            {
                _characterCombat.Attack();
            }
        }
    }
}
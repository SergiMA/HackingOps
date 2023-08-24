using UnityEngine;
using HackingOps.Characters.Entities;

namespace HackingOps.Characters.NPC.States
{
    public class State : MonoBehaviour
    {
        protected Entity _entity;

        private void Awake()
        {
            _entity = GetComponent<Entity>();

            StateAwake();
        }

        private void OnEnable()
        {
            StateOnEnable();
        }

        private void OnDisable()
        {
            StateOnDisable();
        }

        protected virtual void StateAwake() { }
        protected virtual void StateOnEnable() { }
        protected virtual void StateOnDisable() { }
    }
}
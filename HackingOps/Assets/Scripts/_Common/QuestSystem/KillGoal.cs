using HackingOps.Characters.Common;
using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Common.QuestSystem
{
    public class KillGoal : Goal
    {
        [SerializeField] CharacterId[] _characterIds;

        public override void Init()
        {
            base.Init();
        }

        public override void SubscribeToEvents()
        {
            base.SubscribeToEvents();

            ServiceLocator.Instance.GetService<IEventQueue>().Subscribe(EventIds.CharacterDied, this);
        }

        public override void UnsubscribeFromEvents()
        {
            base.UnsubscribeFromEvents();
        }

        private bool ValidateId(string idToCheck)
        {
            foreach (CharacterId id in _characterIds)
            {
                if (idToCheck == id.Value)
                    return true;
            }

            return false;
        }

        public override void Process(EventData eventData)
        {
            if (IsCompleted)
                return;

            if (eventData.EventId != EventIds.CharacterDied)
                return;


            CharacterDiedData data = eventData as CharacterDiedData;

            if (ValidateId(data.Id) == false)
                return;

            base.Process(data);

            CurrentAmount++;
            Evaluate();
        }
    }
}
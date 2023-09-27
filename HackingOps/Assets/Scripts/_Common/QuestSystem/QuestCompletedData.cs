using HackingOps.Common.Events;

namespace HackingOps.Common.QuestSystem
{
    public class QuestCompletedData : EventData
    {
        public readonly Quest Quest;

        public QuestCompletedData(Quest quest) : base(EventIds.QuestCompleted)
        {
            Quest = quest;
        }
    }
}
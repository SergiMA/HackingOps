namespace HackingOps.Common.Events
{
    public class DialogueCompleted : EventData
    {
        public readonly string Id;

        public DialogueCompleted(string id) : base(EventIds.DialogueCompleted)
        {
            Id = id;
        }
    }
}
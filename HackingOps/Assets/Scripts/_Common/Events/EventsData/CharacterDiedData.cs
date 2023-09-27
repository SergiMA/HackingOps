namespace HackingOps.Common.Events
{
    public class CharacterDiedData : EventData
    {
        public readonly string Id;
        
        public CharacterDiedData(string id) : base(EventIds.CharacterDied)
        {
            Id = id;
        }
    }
}
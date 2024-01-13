namespace HackingOps.Characters.NPC.Allegiance
{
    public interface IAllegiance
    {
        public enum Allegiance
        {
            Ally,
            Enemy,
        }

        public Allegiance GetAllegiance();
    }
}
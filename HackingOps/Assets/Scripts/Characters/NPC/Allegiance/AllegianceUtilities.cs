namespace HackingOps.Characters.NPC.Allegiance
{
    public class AllegianceUtilities
    {
        public static bool AreConfronted(IAllegiance a, IAllegiance b)
        {
            return a.GetAllegiance() != b.GetAllegiance();
        }
    }
}
namespace HackingOps.Hacking
{
    public interface IHackable
    {
        void BeginHacking();
        void StopHacking();
        bool IsControllable();
    }
}
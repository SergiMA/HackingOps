namespace HackingOps.Common.Events
{
    public interface IEventObserver
    {
        void Process(EventData eventData);
    }
}
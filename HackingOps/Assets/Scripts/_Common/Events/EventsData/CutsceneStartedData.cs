using Cinemachine;

namespace HackingOps.Common.Events
{
    public class CutsceneStartedData : EventData
    {
        public readonly string Id;

        public CutsceneStartedData(string id) : base(EventIds.CutsceneStarted)
        {
            Id = id;
        }
    }
}
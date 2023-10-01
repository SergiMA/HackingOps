using UnityEngine;

namespace HackingOps.Common.Events
{
    public class CutsceneFinishedData : EventData
    {
        public readonly string Id;

        public CutsceneFinishedData(string id) : base(EventIds.CutsceneFinished)
        {
            Id = id;
        }
    }
}
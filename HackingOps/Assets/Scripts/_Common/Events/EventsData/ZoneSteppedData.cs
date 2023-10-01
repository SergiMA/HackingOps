using HackingOps.Zones;

namespace HackingOps.Common.Events
{
    public class ZoneSteppedData : EventData 
    {
        public readonly Zone Zone;

        public ZoneSteppedData(Zone zone) : base(EventIds.ZoneStepped)
        {
            Zone = zone;
        }
    }
}
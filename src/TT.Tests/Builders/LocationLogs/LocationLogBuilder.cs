using TT.Domain.Entities.LocationLogs;

namespace TT.Tests.Builders.LocationLogs
{
    public class LocationLogBuilder : Builder<LocationLog, int>
    {
        public LocationLogBuilder()
        {
            Instance = Create();
            With(u => u.Id, 151);
        }

       
    }
}

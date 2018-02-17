using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Statics;

namespace TT.Domain.Players.Queries
{
    public class GetBusStops : DomainQuery<BusStop>
    {
        public string currentLocation { get; set; }

        public override IEnumerable<BusStop> Execute(IDataContext context)
        {

            var stops = new List<BusStop>();
            foreach (var stop in LocationsStatics.BusStops)
            {

                var loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == stop);

                var busStop = new BusStop
                {
                    Name = loc.Name,
                    dbName = loc.dbName
                };

                busStop.Cost = LocationsStatics.GetTicketPriceBetweenLocations(currentLocation, loc.dbName);

                stops.Add(busStop);
            }


            return stops;

        }


    }
}

using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.Queries
{
    public class GetBusStops : DomainQuery<BusStop>
    {
        public string currentLocation { get; set; }

        public override IEnumerable<BusStop> Execute(IDataContext context)
        {

            List<BusStop> stops = new List<BusStop>();
            foreach (string stop in LocationsStatics.BusStops)
            {

                Location loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == stop);

                BusStop busStop = new BusStop
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

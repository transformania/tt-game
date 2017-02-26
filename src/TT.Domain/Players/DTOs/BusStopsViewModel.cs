using System;
using System.Collections.Generic;
using TT.Domain.ViewModels;

namespace TT.Domain.Players.DTOs
{
    public class BusStopsViewModel
    {
        public IEnumerable<BusStop> Stops { get; set; }
        public Location CurrentLocation { get; set; }
        public Decimal PlayerCurrecy { get; set; }
    }
}

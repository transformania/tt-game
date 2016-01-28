using System.Collections.Generic;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class MapViewModel
    {
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<LocationInfo> LocationInfo { get; set; }
    }
}
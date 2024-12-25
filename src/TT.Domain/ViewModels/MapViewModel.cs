using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class MapViewModel
    {
        public IEnumerable<Location> Locations { get; set; }
        public IEnumerable<LocationInfo> LocationInfo { get; set; }
        public decimal MinX { get; set; }
        public decimal MaxY { get; set; }
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public bool IsInDungeon { get; set; }
        public List<string> SpellMap { get; set; }
    }
}

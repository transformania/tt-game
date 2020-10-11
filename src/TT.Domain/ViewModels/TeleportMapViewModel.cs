using System.Collections.Generic;

namespace TT.Domain.ViewModels
{
    public class TeleportMapViewModel
    {
        public IEnumerable<Location> Destinations { get; set; }
        public int ItemId { get; set; }
    }
}

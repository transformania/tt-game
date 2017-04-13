using System.Collections.Generic;
using TT.Domain.World.DTOs;

namespace TT.Domain.ViewModels.World
{
    public class OldItemLeaderboardViewModel
    {
        public int Round { get; set; }
        public IEnumerable<ItemLeaderboardEntryDetail> Entries { get; set; }
    }
}

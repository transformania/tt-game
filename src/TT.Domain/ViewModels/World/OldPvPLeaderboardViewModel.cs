using System.Collections.Generic;
using TT.Domain.World.DTOs;

namespace TT.Domain.ViewModels.World
{
    public class OldPvPLeaderboardViewModel
    {
        public int Round { get; set; }
        public IEnumerable<PvPLeaderboardEntryDetail> Entries { get; set; }
    }
}

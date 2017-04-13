using System.Collections.Generic;
using TT.Domain.World.DTOs;

namespace TT.Domain.ViewModels.World
{
    public class OldXpLeaderboardViewModel
    {
        public int Round { get; set; }
        public IEnumerable<XpLeaderboardEntryDetail> Entries { get; set; }
    }
}

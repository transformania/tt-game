using System.Collections.Generic;
using TT.Domain.Entities.Players;

namespace TT.Domain.DTOs.RPClassifiedAds
{
    public class RPClassifiedAdAndPlayerDetail
    {
        public RPClassifiedAdDetail RPClassifiedAd { get; set; }

        public ICollection<PlayerDetail> Players { get; set; }
    }

    public class PlayerDetail : BaseDTO<Player, int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
    }
}

using System.Collections.Generic;
using TT.Domain.Players.Entities;

namespace TT.Domain.ClassifiedAds.DTOs
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

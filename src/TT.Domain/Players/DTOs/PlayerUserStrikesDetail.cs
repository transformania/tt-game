using System.Collections.Generic;
using TT.Domain.Identity.DTOs;

namespace TT.Domain.Players.DTOs
{
    public class PlayerUserStrikesDetail
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserStrikeDetail User { get; set; }
    }

    public class UserStrikeDetail
    {
        public string UserName { get; set; }
        public IEnumerable<StrikeDetail> Strikes { get; set; }
    }
}

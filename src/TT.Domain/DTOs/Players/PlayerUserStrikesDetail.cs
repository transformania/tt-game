using System.Collections.Generic;
using TT.Domain.DTOs.Identity;

namespace TT.Domain.DTOs.Players
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

using TT.Domain.DTOs.AI;
using TT.Domain.DTOs.Identity;

namespace TT.Domain.DTOs.Players
{
    public class PlayerMessageDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DonatorLevel { get; set; }
        public string Nickname { get; set; }
    }
}

using TT.Domain.Entities.Identity;

namespace TT.Domain.DTOs.Players
{
    public class PlayerDetail
    {
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }
        public User User { get; protected set; }
    }
}

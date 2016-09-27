using TT.Domain.Entities.Identity;

namespace TT.Domain.DTOs.Identity
{
    public class UserDonatorDetail
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public Donator Donator { get; set; }
    }
}

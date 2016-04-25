using TT.Domain.Entities.Identity;
using TT.Domain.Entities.NPCs;

namespace TT.Domain.DTOs.Players
{
    public class PlayerDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public User User { get; set; }
        public NPC NPC { get; set; }
    }
}

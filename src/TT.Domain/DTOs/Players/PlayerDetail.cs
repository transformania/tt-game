using TT.Domain.DTOs.AI;
using TT.Domain.DTOs.Identity;

namespace TT.Domain.DTOs.Players
{
    public class PlayerDetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserDetail User { get; set; }
        public NPCDetail NPC { get; set; }
    }
}

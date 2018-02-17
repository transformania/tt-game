using TT.Domain.Players.DTOs;

namespace TT.Domain.ViewModels
{
    public class AddStrikeViewModel
    {
        public string UserId { get; set; }
        public string Reason { get; set; }

        public PlayerUserStrikesDetail PlayerUserStrikesDetail { get; set; }

    }
}

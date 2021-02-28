using TT.Domain.Models;
using TT.Domain.Statics;

namespace TT.Domain.ViewModels
{
    public class PlayerFormViewModel
    {
        public Player_VM Player { get; set; }
        public Form Form { get; set; }
        public string Mobility { get; set; }

        public bool CanAccessChat()
        {
            return Player != null && (PvPStatics.ChaosMode || Player.BotId == AIStatics.ActivePlayerBotId) && !string.IsNullOrWhiteSpace(Player.FirstName) && !string.IsNullOrWhiteSpace(Player.LastName);
        }
    }
}
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class PlayerFormViewModel
    {
        public Player_VM Player { get; set; }
        public Form Form { get; set; }

        public bool CanAccessChat()
        {
            return Player != null && Player.BotId == 0 && !string.IsNullOrWhiteSpace(Player.FirstName) && !string.IsNullOrWhiteSpace(Player.LastName);
        }
    }
}
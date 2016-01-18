using System.Collections.Generic;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class PlayerSearchViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<Player> PlayersFound { get; set;}
        public Player ExactPlayer { get; set; }
        public bool FoundThem { get; set; }
    }
}
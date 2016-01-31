using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
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
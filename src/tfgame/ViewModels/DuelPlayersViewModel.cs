using System.Collections.Generic;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class DuelPlayersViewModel
    {
        public Duel Duel { get; set; }
        public List<PlayerFormViewModel> Combatants { get; set; }
    }
}
using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class DuelPlayersViewModel
    {
        public Duel Duel { get; set; }
        public List<PlayerFormViewModel> Combatants { get; set; }
    }
}
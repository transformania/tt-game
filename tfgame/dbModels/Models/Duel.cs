using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class Duel
    {
        public int Id { get; set; }
        public int ProposalTurn { get; set; }
        public int StartTurn { get; set; }
        public int CompletionTurn { get; set; }
        public string Status { get; set; }
    }

    public class DuelCombatant
    {
        public int Id { get; set; }
        public int DuelId { get; set; }
        public int PlayerId { get; set; }
        public int Team { get; set; }
        public int StartForm { get; set; }
    }

    public class DuelRules
    {
        public int Id { get; set; }
        public int DuelId { get; set; }
        public bool NoItems { get; set; }
        public bool AnimateSpellsOnly { get; set; }
        public bool NoCleansing { get; set; }
        public bool NoMeditating { get; set; }
        public int CastsPerRound { get; set; }
    }
}
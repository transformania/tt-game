﻿using System;
using System.Collections.Generic;

namespace TT.Domain.Models
{
    public class Duel
    {
        public int Id { get; set; }
        public int ProposalTurn { get; set; }
        public int StartTurn { get; set; }
        public int CompletionTurn { get; set; }
        public string Status { get; set; }
        public DateTime LastResetTimestamp { get; set; }

        
        public virtual DuelRules Rules { get; set; }
        public virtual List<DuelCombatant> Combatants { get; set; }

    }

    public class DuelCombatant
    {
        public int Id { get; set; }
        public int DuelId { get; set; }
        public int PlayerId { get; set; }
        public int Team { get; set; }
        public int? StartFormSourceId { get; set; }
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
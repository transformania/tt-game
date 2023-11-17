﻿using System.Collections.Generic;
using TT.Domain.Models;

namespace TT.Domain.ViewModels
{
    public class CovenantViewModel
    {
        public Covenant dbCovenant { get; set; }
        public IEnumerable<PlayerFormViewModel> Members { get; set; }
        public bool IsFull { get; set; }
        public Player Leader { get; set; }
        public Player Mascot { get; set; }
    }

    public class CovenantListItemViewModel
    {
        public Covenant dbCovenant { get; set; }
        public int MemberCount { get; set; }
        public int AnimateMemberCount { get; set; }
        public Player Leader { get; set; }
    }

    public class CovenantNameFlag
    {
        public string Name { get; set; }
        public string FlagUrl { get; set; }
        public string HomeLocation { get; set; }
        public int CovLevel { get; set; }
        public string CovenBlurb { get; set; }
        public int CovenMascot { get; set; }
    }

    public class CovenantUpdateViewModel
    {
        public string Description { get; set; }
        public string Flag { get; set; }
    }

    public class CovenantApplicationViewModel
    {
        public Player dbPlayer { get; set; }
        public CovenantApplication dbCovenantApplication { get; set; }
    }




}
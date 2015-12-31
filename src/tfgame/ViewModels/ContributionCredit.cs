using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class ContributionCredit
    {
        public string OwnerMembershipId { get; set; }
        public string AuthorName { get; set; }
        public int SpellCount { get; set; }
        public int AnimateFormCount { get; set; }
        public int InanimateFormCount { get; set; }
        public int AnimalFormCount { get; set; }
        public int EffectCount { get; set; }
        public string Website { get; set; }
    }
}
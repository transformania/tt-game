using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.ViewModels
{
    public class BioPageContributionViewModel
    {
        public string SpellName { get; set; }
        public string FormName { get; set; }
    }

    public class BioPageEffectContributionViewModel
    {
        public string EffectName { get; set; }
        public string SpellName { get; set; }
    }
}
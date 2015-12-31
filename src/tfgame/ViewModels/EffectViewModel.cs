using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class EffectViewModel
    {
        public Effect dbEffect {  get; set;}
        public StaticEffect Effect { get; set; }
    }

    public class EffectViewModel2
    {
        public Effect_VM dbEffect { get; set; }
        public StaticEffect Effect { get; set; }
    }
}
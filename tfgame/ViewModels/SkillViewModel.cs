using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.ViewModels
{
    public class SkillViewModel
    {
        // this class bundles a skill that the player has with all of its definitions from the file, such as description, cost, etc.

        public Skill dbSkill { get; set; }
        public StaticSkill Skill { get; set; }
        public string MobilityType { get; set; }
        
    }

    public class SkillViewModel2
    {
        public Skill_VM dbSkill { get; set; }
        public StaticSkill Skill { get; set; }
        public string MobilityType { get; set; }
    }

}
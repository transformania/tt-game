using System.Collections.Generic;
using tfgame.Procedures.BossProcedures;

namespace tfgame.ViewModels
{
    public static class BossSummonDictionary
    {
        public static Dictionary<string, BossSummon> GlobalBossSummonDictionary = new Dictionary<string, BossSummon> {
    { "ranch_bedroom", new BossSummon { BossName="Donna", MinimumTurn = 2600, ActivationText = "" }},

     { "castle_armory", new BossSummon { BossName="Valentine", MinimumTurn = 1000, ActivationText = "" }},

     { "stripclub_bar_seats", new BossSummon { BossName="BimboBoss", MinimumTurn = 1800, ActivationText = "" }},

     { "tavern_pool", new BossSummon { BossName="Thieves", MinimumTurn = 144, ActivationText = "" }},

     { "college_foyer", new BossSummon { BossName="Sisters", MinimumTurn = 3200, ActivationText = "" }},

      { BossProcedures_FaeBoss.SpawnLocation, new BossSummon { BossName="FaeBoss", MinimumTurn = 3800, ActivationText = "" }},

    };

        public static string GetActivationText(string bossName)
        {
            string filename = System.Web.HttpContext.Current.Server.MapPath("~/XMLs/BossSummonText/" + bossName + ".txt");
            string text = System.IO.File.ReadAllText(filename);
            return text;
        }

    }

    public class BossSummon
    {
        public string BossName { get; set; }
        public int MinimumTurn { get; set; }
        public string ActivationText { get; set; }
    }
}
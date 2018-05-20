using System.Collections.Generic;
using System.IO;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;

namespace TT.Domain.ViewModels
{
    public static class BossSummonDictionary
    {
        public static Dictionary<string, BossSummon> GlobalBossSummonDictionary = new Dictionary<string, BossSummon> {
    { "ranch_bedroom", new BossSummon { BossId = AIStatics.DonnaBotId, MinimumTurn = 2600, ActivationText = "" }},

     { "castle_armory", new BossSummon { BossId = AIStatics.ValentineBotId, MinimumTurn = 99999, ActivationText = "" }},

     { "stripclub_bar_seats", new BossSummon { BossId = AIStatics.BimboBossBotId, MinimumTurn = 1800, ActivationText = "" }},

     { "tavern_pool", new BossSummon { BossId = AIStatics.FemaleRatBotId, MinimumTurn = 144, ActivationText = "" }},

     { "college_foyer", new BossSummon { BossId = AIStatics.MouseNerdBotId, MinimumTurn = 3800, ActivationText = "" }},

      { BossProcedures_FaeBoss.SpawnLocation, new BossSummon { BossId = AIStatics.FaebossBotId, MinimumTurn = 3200, ActivationText = "" }},

    { BossProcedures_MotorcycleGang.SpawnLocation, new BossSummon { BossId = AIStatics.MotorcycleGangLeaderBotId, MinimumTurn = 1, ActivationText = "" }},

    };

        public static string GetActivationText(string bossName)
        {
            var resource = $"TT.Domain.XMLs.BossSummonText.{bossName}.txt";
            var stream = typeof(BossSummon).Assembly.GetManifestResourceStream(resource);

            if (stream == null)
            {
                return "";
            }
            else
            {
                return new StreamReader(stream).ReadToEnd();
            }
            
        }

    }

    public class BossSummon
    {
        public int BossId { get; set; }
        public int MinimumTurn { get; set; }
        public string ActivationText { get; set; }
    }
}
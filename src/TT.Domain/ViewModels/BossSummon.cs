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
    { "ranch_bedroom", new BossSummon { BossId = AIStatics.DonnaBotId, MinimumTurn = TurnTimesStatics.GetDonnaMinSpawnTurn(), ActivationText = "" }},

     { "forest_pinecove", new BossSummon { BossId = AIStatics.ValentineBotId, MinimumTurn = TurnTimesStatics.GetValentineMinSpawnTurn(), ActivationText = "" }},  //Normally castle_armory

     { "stripclub_bar_seats", new BossSummon { BossId = AIStatics.BimboBossBotId, MinimumTurn = TurnTimesStatics.GetBimboBossMinSpawnTurn(), ActivationText = "" }},

     { "tavern_pool", new BossSummon { BossId = AIStatics.FemaleRatBotId, MinimumTurn = TurnTimesStatics.GetRatThievesMinSpawnTurn(), ActivationText = "" }},

     { "college_foyer", new BossSummon { BossId = AIStatics.MouseNerdBotId, MinimumTurn = TurnTimesStatics.GetMouseSistersMinSpawnTurn(), ActivationText = "" }},

      { BossProcedures_FaeBoss.SpawnLocation, new BossSummon { BossId = AIStatics.FaebossBotId, MinimumTurn = TurnTimesStatics.GetFaeBossMinSpawnTurn(), ActivationText = "" }},

    { BossProcedures_MotorcycleGang.SpawnLocation, new BossSummon { BossId = AIStatics.MotorcycleGangLeaderBotId, MinimumTurn = TurnTimesStatics.GetMotorcycleGangMinSpawnTurn(), ActivationText = "" }},

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
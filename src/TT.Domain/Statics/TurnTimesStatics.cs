using System.Collections.Generic;
using System.Linq;

namespace TT.Domain.Statics
{
    public static class TurnTimesStatics
    {
        public static string ActiveConfiguration = FiveMinuteTurns;

        public const string FiveMinuteTurns = "5min";
        public const string ThreeMinuteTurns = "3min";

        public static Dictionary<string, TurnTimeConfiguration> TurnTimeConfigurations =
            new Dictionary<string, TurnTimeConfiguration>
            {
                {
                    FiveMinuteTurns,
                    new TurnTimeConfiguration
                    {
                        TurnLengthInSeconds = 300,
                        StartTurnNoAttackSeconds = 30,
                        EndTurnNoAttackSeconds = 30,
                        OfflineAfterXMinutes = 30,
                        MinutesSinceLastCombatBeforeQuestingOrDuelling = 30,
                        ItemMaxTurnsBuildup = 96
                    }
                },
                {
                    ThreeMinuteTurns,
                    new TurnTimeConfiguration
                    {
                        TurnLengthInSeconds = 180,
                        StartTurnNoAttackSeconds = 15,
                        EndTurnNoAttackSeconds = 15,
                        OfflineAfterXMinutes = 18,
                        MinutesSinceLastCombatBeforeQuestingOrDuelling = 18,
                        ItemMaxTurnsBuildup = 160
                    }
                }
            };

        public static int GetTurnLengthInSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.TurnLengthInSeconds;
        }

        public static int GetStartTurnNoAttackSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.StartTurnNoAttackSeconds;
        }

        public static int GetEndTurnNoAttackSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.EndTurnNoAttackSeconds;
        }

        public static int GetOfflineAfterXMinutes()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.OfflineAfterXMinutes;
        }

        public static int GetMinutesSinceLastCombatBeforeQuestingOrDuelling()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.MinutesSinceLastCombatBeforeQuestingOrDuelling;
        }

        public static int GetItemMaxTurnsBuildup()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.ItemMaxTurnsBuildup;
        }

        public static bool IsValidConfiguration(string configuration)
        {
            return TurnTimeConfigurations.ContainsKey(configuration);
        }

    }

    public class TurnTimeConfiguration
    {
        public int TurnLengthInSeconds { get; set; }
        public int StartTurnNoAttackSeconds { get; set; }
        public int EndTurnNoAttackSeconds { get; set; }
        public int OfflineAfterXMinutes { get; set; }
        public int MinutesSinceLastCombatBeforeQuestingOrDuelling { get; set; }
        public int ItemMaxTurnsBuildup { get; set; }
    }
}

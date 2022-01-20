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
                        ItemMaxTurnsBuildup = 96,
                        StruggleXPBeforeItemPermanentLock = -320,
                        NoMovingAfterAttackSeconds = 45,
                        ActionPointLimit = 120,
                        ActionPointReserveLimit = 360,
                        DonnaMinSpawnTurn = 2600,
                        ValentineMinSpawnTurn = 99999,
                        BimboBossMinSpawnTurn = 1800,
                        RatThievesMinSpawnTurn = 144,
                        MouseSistersMinSpawnTurn = 3800,
                        FaeBossMinSpawnTurn = 3200,
                        MotorcycleGangMinSpawnTurn = 1000
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
                        ItemMaxTurnsBuildup = 160,
                        StruggleXPBeforeItemPermanentLock = -600,
                        NoMovingAfterAttackSeconds = 27,
                        ActionPointLimit = 200,
                        ActionPointReserveLimit = 600,
                        DonnaMinSpawnTurn = 2600,
                        ValentineMinSpawnTurn = 300,
                        BimboBossMinSpawnTurn = 2000,
                        RatThievesMinSpawnTurn = 200,
                        MouseSistersMinSpawnTurn = 6000,
                        FaeBossMinSpawnTurn = 4000,
                        MotorcycleGangMinSpawnTurn = 1500
                    }
                }
            };

        public static int GetTurnLengthInSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.TurnLengthInSeconds;
        }

        public static int GetStartTurnNoAttackSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .StartTurnNoAttackSeconds;
        }

        public static int GetEndTurnNoAttackSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .EndTurnNoAttackSeconds;
        }

        public static int GetOfflineAfterXMinutes()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.OfflineAfterXMinutes;
        }

        public static int GetMinutesSinceLastCombatBeforeQuestingOrDuelling()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .MinutesSinceLastCombatBeforeQuestingOrDuelling;
        }

        public static int GetItemMaxTurnsBuildup()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.ItemMaxTurnsBuildup;
        }

        public static int GetStruggleXPBeforeItemPermanentLock()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .StruggleXPBeforeItemPermanentLock;
        }

        public static int GetNoMovingAfterAttackSeconds()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .NoMovingAfterAttackSeconds;
        }

        public static int GetActionPointLimit()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.ActionPointLimit;
        }

        public static int GetActionPointReserveLimit()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .ActionPointReserveLimit;
        }

        public static int GetDonnaMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value
                .DonnaMinSpawnTurn;
        }

        public static int GetValentineMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.ValentineMinSpawnTurn;
        }

        public static int GetBimboBossMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.BimboBossMinSpawnTurn;
        }

        public static int GetRatThievesMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.RatThievesMinSpawnTurn;
        }

        public static int GetMouseSistersMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.MouseSistersMinSpawnTurn;
        }

        public static int GetFaeBossMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.FaeBossMinSpawnTurn
                ;
        }

        public static int GetMotorcycleGangMinSpawnTurn()
        {
            return TurnTimeConfigurations.SingleOrDefault(c => c.Key == ActiveConfiguration).Value.MotorcycleGangMinSpawnTurn
                ;
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
        public int StruggleXPBeforeItemPermanentLock { get; set; }
        public int NoMovingAfterAttackSeconds { get; set; }
        public int ActionPointLimit { get; set; }
        public int ActionPointReserveLimit { get; set; }
        public int DonnaMinSpawnTurn { get; set; }
        public int ValentineMinSpawnTurn { get; set; }
        public int BimboBossMinSpawnTurn { get; set; }
        public int RatThievesMinSpawnTurn { get; set; }
        public int MouseSistersMinSpawnTurn { get; set; }
        public int FaeBossMinSpawnTurn { get; set; }
        public int MotorcycleGangMinSpawnTurn { get; set; }
    }

}

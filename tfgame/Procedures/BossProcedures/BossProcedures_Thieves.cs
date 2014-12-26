using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_Thieves
    {

        private const string MaleBossFirstName = "Brother Lujako";
        private const string MaleBossLastName = "Seekshadow";
        private const string FemaleBossFirstName = "Sister Lujienne";
        private const string FemaleBossLastName = "Seekshadow";

        public static void SpawnThieves()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player malethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == MaleBossFirstName && f.LastName == MaleBossLastName);

            if (malethief == null)
            {
                malethief = new Player()
                {
                    FirstName = MaleBossFirstName,
                    LastName = MaleBossLastName,
                    ActionPoints = 120,
                    dbLocationName = "tavern_pool",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "male",
                    Health = 1000,
                    Mana = 1000,
                    MaxHealth = 1000,
                    MaxMana = 1000,
                    Form = "x",
                    IsPetToId = -1,
                    Money = 2000,
                    Mobility = "full",
                    Level = 5,
                    MembershipId = -5,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(malethief);
                malethief = PlayerProcedures.ReadjustMaxes(malethief, ItemProcedures.GetPlayerBuffs(malethief));
                playerRepo.SavePlayer(malethief);

            }


            Player femalethief = playerRepo.Players.FirstOrDefault(f => f.FirstName == FemaleBossFirstName && f.LastName == FemaleBossLastName);

            if (femalethief == null)
            {
                femalethief = new Player()
                {
                    FirstName = FemaleBossFirstName,
                    LastName = FemaleBossLastName,
                    ActionPoints = 120,
                    dbLocationName = "tavern_pool",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 1000,
                    Mana = 1000,
                    MaxHealth = 1000,
                    MaxMana = 1000,
                    Form = "x",
                    IsPetToId = -1,
                    Money = 6000,
                    Mobility = "full",
                    Level = 5,
                    MembershipId = -5,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(femalethief);

                femalethief = PlayerProcedures.ReadjustMaxes(malethief, ItemProcedures.GetPlayerBuffs(malethief));

                playerRepo.SavePlayer(femalethief);

            }

        }
    }

}
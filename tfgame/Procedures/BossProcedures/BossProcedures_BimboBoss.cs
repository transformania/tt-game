using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_BimboBoss
    {

        public static void SpawnBimboBoss()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");

            if (bimboBoss == null)
            {
                bimboBoss = new Player()
                {
                    FirstName = "Lady",
                    LastName = "Lovebringer",
                    ActionPoints = 120,
                    dbLocationName = "apartment_dog_park",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_Perverted_Fairy_Judoo",
                    IsPetToId = -1,
                    Money = 1000,
                    Mobility = "full",
                    Level = 7,
                    MembershipId = -6,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(bimboBoss);

                bimboBoss = PlayerProcedures.ReadjustMaxes(bimboBoss, ItemProcedures.GetPlayerBuffs(bimboBoss));

                playerRepo.SavePlayer(bimboBoss);

                // give fae the plague spell to attack with
                bimboBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");
                DbStaticSkill skillToAdd = SkillStatics.GetStaticSkill("hey_listed_Varn");

                SkillProcedures.GiveSkillToPlayer(bimboBoss.Id, skillToAdd);

                // set up her AI directive so it is not deleted
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective directive = new AIDirective
                {
                    OwnerId = bimboBoss.Id,
                    Timestamp = DateTime.UtcNow,
                    SpawnTurn = PvPWorldStatProcedures.GetWorldTurnNumber(),
                    DoNotRecycleMe = true,
                };

                aiRepo.SaveAIDirective(directive);

            }
        }

    }
}
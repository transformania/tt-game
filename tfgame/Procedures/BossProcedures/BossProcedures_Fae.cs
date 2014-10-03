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
    public static class BossProcedures_Fae
    {
        public static void SpawnFae()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player fae = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");

            if (fae == null)
            {
                fae = new Player()
                {
                    FirstName = "Jewdewfae",
                    LastName = "the Pervfae",
                    ActionPoints = 120,
                    dbLocationName = "forest_hotspring",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
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

                playerRepo.SavePlayer(fae);

                fae = PlayerProcedures.ReadjustMaxes(fae, ItemProcedures.GetPlayerBuffs(fae));

                playerRepo.SavePlayer(fae);

                // give fae the fairy spell to counterattack with
                fae = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Jewdewfae" && f.LastName == "the Pervfae");
                DbStaticSkill skillToAdd = SkillStatics.GetStaticSkill("hey_listed_Varn");

                SkillProcedures.GiveSkillToPlayer(fae.Id, skillToAdd);




            }
        }
    }
}
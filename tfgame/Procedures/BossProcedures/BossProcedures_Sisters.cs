using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures.BossProcedures
{
    public static class BossProcedures_Sisters
    {
        public const string NerdBossFirstName = "Headmistress Adrianna";
        public const string NerdBossForm = "form_Head_Beautician_of_Blazes_and_Glamour_Judoo_and_Elyn";
        public const string BimboBossFirstName = "Beautrician Candice";
        public const string BimboBossForm = "form_Headmistress_of_SCCC_Elyn_and_Judoo";
        public const string BossesLastName = "Brisby";

        public const string BimboSpell = "skill_Pinky!_Judoo";
        public const string NerdSpell = "skill_The_Brain_Elynsynos";

        public static void SpawnSisters()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player nerdBoss = playerRepo.Players.FirstOrDefault(f => f.FirstName == NerdBossFirstName && f.LastName == BossesLastName);

            if (nerdBoss == null)
            {
                nerdBoss = new Player()
                {
                    FirstName = NerdBossFirstName,
                    LastName = BossesLastName,
                    ActionPoints = 120,
                    dbLocationName = "college_foyer",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = BimboBossForm,
                    IsPetToId = -1,
                    Money = 2000,
                    Mobility = "full",
                    Level = 25,
                    MembershipId = -11,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(nerdBoss);
            }


            Player bimboboss = playerRepo.Players.FirstOrDefault(f => f.FirstName == BimboBossFirstName && f.LastName == BossesLastName);

            if (bimboboss == null)
            {
                bimboboss = new Player()
                {
                    FirstName = BimboBossFirstName,
                    LastName = BossesLastName,
                    ActionPoints = 120,
                    dbLocationName = "salon_front_desk",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = NerdBossForm,
                    IsPetToId = -1,
                    Money = 6000,
                    Mobility = "full",
                    Level = 25,
                    MembershipId = -12,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(bimboboss);
            }



        }

        public static string AttackValidation(Player attacker, Player target)
        {
            return "";
        }

        public static void EndEvent()
        {

        }
    }
}
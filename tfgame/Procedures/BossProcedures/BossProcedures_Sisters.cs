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
        public const string NerdBossForm = "form_Headmistress_of_SCCC_Elyn_and_Judoo";

        public const string BimboBossFirstName = "Beautrician Candice";
        public const string BimboBossForm = "form_Head_Beautician_of_Blazes_and_Glamour_Judoo_and_Elyn";
        
        public const string BossesLastName = "Brisby";

        public const string BimboSpell = "skill_Pinky!_Judoo";
        public const string BimboSpellForm = "form_Bimbo_Mousegirl_Judoo";
        public const string NerdSpell = "skill_The_Brain_Elynsynos";
        public const string NerdSpellForm = "form_Nerdy_Mousegirl_Elynsynos";

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

        public static string SpellIsValid(Player attacker, Player target, string spellDbName)
        {

            // only allow nerd mouse girls to attack bimbo boss
            if (target.FirstName == NerdBossFirstName && attacker.Form != BimboSpellForm)
            {
                return "You can't seem to find the right peeved-off mindset to cast this spell against Adrianna.  Maybe you'd have better luck if you were casting magic against her as a Bimbo Mousegirl...";
            }

            // only allow nerd mouse girls to attack bimbo boss
            if (target.FirstName == BimboBossFirstName && attacker.Form != NerdSpellForm)
            {
                return "You can't seem to find the right peeved-off mindset to cast this spell against Candice.  Maybe you'd have better luck if you were casting magic against her as a Nerdy Mousegirl...";
            }
            // only allow bimbo mouse girls to attack nerd boss

            // only allow bimbo spell against nerd boss
            if (target.Form == NerdBossForm && spellDbName != BimboSpell)
            {
                return "This spell won't work against Adrianna.";
            }
            else if (target.Form == BimboBossForm && spellDbName != NerdSpell)
            {
                return "This spell won't work against Candice.";
            }

            //disallow spells cast against the bosses if they have changed animate forms
            if ((target.FirstName == NerdBossFirstName && target.Form != NerdBossForm) || (target.FirstName == BimboBossFirstName && target.Form != BimboBossForm))
            {
                return "One of the Brisby sisters have already been transformed; there's no need to attack them any further.";
            }

            return "";
            
        }

        public static void CounterAttack(Player attacker, Player bossTarget)
        {

            AIProcedures.DealBossDamage(bossTarget, attacker, true, 1);

            // nerd counters with nerd spell unless she has changed form
            if (bossTarget.FirstName == NerdBossFirstName && bossTarget.Form == NerdBossForm)
            {
                AttackProcedures.Attack(bossTarget, attacker, NerdSpell);
                AttackProcedures.Attack(bossTarget, attacker, NerdSpell);
                AttackProcedures.Attack(bossTarget, attacker, NerdSpell);
                AIProcedures.DealBossDamage(bossTarget, attacker, false, 3);
            }
           

            // bimbo counters with bimbo spell unless she has changed form
            else if (bossTarget.FirstName == BimboBossFirstName && bossTarget.Form == BimboBossForm)
            {
                AttackProcedures.Attack(bossTarget, attacker, BimboSpell);
                AttackProcedures.Attack(bossTarget, attacker, BimboSpell);
                AttackProcedures.Attack(bossTarget, attacker, BimboSpell);
                AIProcedures.DealBossDamage(bossTarget, attacker, false, 3);
            }

        }

        public static void RunSistersAction()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player nerdBoss = playerRepo.Players.FirstOrDefault(p => p.MembershipId == -11);
            Player bimboBoss = playerRepo.Players.FirstOrDefault(p => p.MembershipId == -12);

            // check to see if a sister has been TFed and the event should end
            if (nerdBoss.Form != NerdBossForm || bimboBoss.Form != BimboBossForm)
            {
                // end event
            }
            else
            {
                // get all of the players in the room by nerd
                List<Player> playersByNerd = PlayerProcedures.GetPlayersAtLocation(nerdBoss.dbLocationName).ToList();
                playersByNerd = playersByNerd.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.MembershipId > 0 && p.Id != nerdBoss.Id && p.Form != NerdSpellForm).ToList();


                // get all of the players in the room by bimbo
                List<Player> playersByBimbo = PlayerProcedures.GetPlayersAtLocation(bimboBoss.dbLocationName).ToList();
                playersByBimbo = playersByBimbo.Where(p => p.Mobility == "full" && PlayerProcedures.PlayerIsOffline(p) == false && p.MembershipId > 0 && p.Id != bimboBoss.Id && p.Form != BimboSpellForm).ToList();

                foreach (Player p in playersByNerd)
                {
                    AttackProcedures.Attack(nerdBoss, p, NerdSpell);
                    AttackProcedures.Attack(nerdBoss, p, NerdSpell);
                    AttackProcedures.Attack(nerdBoss, p, NerdSpell);
                    AIProcedures.DealBossDamage(nerdBoss, p, false, 3);
                }


                foreach (Player p in playersByBimbo)
                {
                    AttackProcedures.Attack(bimboBoss, p, BimboSpell);
                    AttackProcedures.Attack(bimboBoss, p, BimboSpell);
                    AttackProcedures.Attack(bimboBoss, p, BimboSpell);
                    AIProcedures.DealBossDamage(bimboBoss, p, false, 3);
                }

            }


        }

        public static void EndEvent()
        {

        }
    }
}
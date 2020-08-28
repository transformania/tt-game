using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_Sisters
    {
        public const string NerdBossFirstName = "Headmistress Adrianna";
        public const int NerdBossFormSourceId = 317;

        public const string BimboBossFirstName = "Beautician Candice";
        public const int BimboBossFormSourceId = 522;

        public const string BossesLastName = "Brisby";

        public const int BimboSpellSourceId = 627;

        public const int BimboSpellFormSourceId = 318;

        public const int NerdSpellSourceId = 628;

        public const int NerdSpellFormSourceId = 319;

        public const int MakeupKitSpellSourceId = 1177;
        public const int MicroscopeSpellSourceId = 1183;

        private const int NerdBossFormId = 317;
        private const int BimboBossFormId = 522;

        public static void SpawnSisters()
        {
            var nerdBoss = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.MouseNerdBotId });

            if (nerdBoss == null)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = NerdBossFirstName,
                    LastName = BossesLastName,
                    Location = "college_foyer",
                    Gender = PvPStatics.GenderFemale,
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = NerdBossFormId,
                    Money = 2000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 25,
                    BotId = AIStatics.MouseNerdBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var nerdBossEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                nerdBossEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(nerdBossEF));
                playerRepo.SavePlayer(nerdBossEF);
            }


            var bimboboss = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.MouseBimboBotId });

            if (bimboboss == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = BimboBossFirstName,
                    LastName = BossesLastName,
                    Location = "salon_front_desk",
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    FormSourceId = BimboBossFormId,
                    Money = 6000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 25,
                    BotId = AIStatics.MouseBimboBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var bimboBossEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                bimboBossEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(bimboBossEF));
                playerRepo.SavePlayer(bimboBossEF);

            }

        }

        public static string SpellIsValid(Player attacker, Player target, int spellSourceId)
        {

            // only allow bimbo mouse girls to attack nerd boss
            if (target.BotId == AIStatics.MouseNerdBotId && attacker.FormSourceId != BimboSpellFormSourceId)
            {
                return "You can't seem to find the right peeved-off mindset to cast this spell against Adrianna.  Maybe you'd have better luck if you were casting magic against her as a Bimbo Mousegirl...";
            }

            // only allow nerd mouse girls to attack bimbo boss
            if (target.BotId == AIStatics.MouseBimboBotId && attacker.FormSourceId != NerdSpellFormSourceId)
            {
                return "You can't seem to find the right peeved-off mindset to cast this spell against Candice.  Maybe you'd have better luck if you were casting magic against her as a Nerdy Mousegirl...";
            }

            // only allow bimbo spell against nerd boss and nerd spells against bimbo boss
            if (target.FormSourceId == NerdBossFormSourceId && spellSourceId != BimboSpellSourceId)
            {
                return "This spell won't work against Adrianna.";
            }
            else if (target.FormSourceId == BimboBossFormSourceId && spellSourceId != NerdSpellSourceId)
            {
                return "This spell won't work against Candice.";
            }

            //disallow spells cast against the bosses if they have changed animate forms
            if ((target.BotId == AIStatics.MouseNerdBotId && target.FormSourceId != NerdBossFormSourceId) || (target.BotId == AIStatics.MouseBimboBotId && target.FormSourceId != BimboBossFormSourceId))
            {
                return "One of the Brisby sisters have already been transformed; there's no need to attack them any further.";
            }

            return "";
            
        }

        public static void CounterAttack(Player attacker, Player bossTarget)
        {

            AIProcedures.DealBossDamage(bossTarget, attacker, true, 1);

            var spell = ChooseSpell(bossTarget);

            // nerd/bimbo counters with nerd/bimbo spell unless she has changed form
            if (bossTarget.BotId == AIStatics.MouseNerdBotId && bossTarget.FormSourceId == NerdBossFormSourceId || bossTarget.BotId == AIStatics.MouseBimboBotId && bossTarget.FormSourceId == BimboBossFormSourceId)
            {
                AttackProcedures.Attack(bossTarget, attacker, spell);
                AttackProcedures.Attack(bossTarget, attacker, spell);
                AttackProcedures.Attack(bossTarget, attacker, spell);
                AIProcedures.DealBossDamage(bossTarget, attacker, false, 3);
            }

        }

        private static int ChooseSpell(Player sister)
        {
            if (sister.BotId == AIStatics.MouseBimboBotId)
            {
                return IsAtThreeQuartersHealthOrLower(sister) ? MakeupKitSpellSourceId : BimboSpellSourceId;
            }
            return IsAtThreeQuartersHealthOrLower(sister) ? MicroscopeSpellSourceId : NerdSpellSourceId;
        }

        private static bool IsAtThreeQuartersHealthOrLower(Player player)
        {
            return player.Health < (player.MaxHealth / 4) * 3;
        }

        public static void RunSistersAction()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var nerdBoss = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.MouseNerdBotId);
            var bimboBoss = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.MouseBimboBotId);

            // check to see if a sister has been TFed and the event should end
            if (nerdBoss == null || nerdBoss.FormSourceId != NerdBossFormSourceId || bimboBoss == null || bimboBoss.FormSourceId != BimboBossFormSourceId)
            {
                EndEvent();
            }
            else
            {
                // get all of the players in the room by nerd
                var playersByNerd = PlayerProcedures.GetPlayersAtLocation(nerdBoss.dbLocationName).ToList();
                playersByNerd = playersByNerd.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                    !PlayerProcedures.PlayerIsOffline(p) &&
                    p.BotId == AIStatics.ActivePlayerBotId &&
                    p.Id != nerdBoss.Id &&
                    p.FormSourceId != NerdSpellFormSourceId &&
                    p.InDuel <= 0  &&
                    p.InQuest <= 0).ToList();


                // get all of the players in the room by bimbo
                var playersByBimbo = PlayerProcedures.GetPlayersAtLocation(bimboBoss.dbLocationName).ToList();
                playersByBimbo = playersByBimbo.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                    !PlayerProcedures.PlayerIsOffline(p) &&
                    p.BotId == AIStatics.ActivePlayerBotId &&
                    p.Id != bimboBoss.Id &&
                    p.FormSourceId != BimboSpellFormSourceId &&
                    p.InDuel <= 0 &&
                    p.InQuest <= 0).ToList();

                foreach (var p in playersByNerd)
                {
                    AttackProcedures.Attack(nerdBoss, p, NerdSpellSourceId);
                    AttackProcedures.Attack(nerdBoss, p, NerdSpellSourceId);
                    AttackProcedures.Attack(nerdBoss, p, NerdSpellSourceId);
                    AIProcedures.DealBossDamage(nerdBoss, p, false, 3);
                }


                foreach (var p in playersByBimbo)
                {
                    AttackProcedures.Attack(bimboBoss, p, BimboSpellSourceId);
                    AttackProcedures.Attack(bimboBoss, p, BimboSpellSourceId);
                    AttackProcedures.Attack(bimboBoss, p, BimboSpellSourceId);
                    AIProcedures.DealBossDamage(bimboBoss, p, false, 3);
                }

            }


        }

        public static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndSisters();

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var nerdBoss = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.MouseNerdBotId);
            var bimboBoss = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.MouseBimboBotId);

            var winner = "";

            if (nerdBoss.FormSourceId != NerdBossFormSourceId) {
                winner = "bimbo";
            } else if (bimboBoss.FormSourceId != BimboBossFormSourceId) {
                winner = "nerd";
            } else {
                return;
            }

            // find the players who dealt the most damage and award them with XP
            List<BossDamage> damages = null;
            
            if (winner == "bimbo") {
                damages = AIProcedures.GetTopAttackers(nerdBoss.BotId, 10);
            } else if (winner == "nerd") {
                damages  = AIProcedures.GetTopAttackers(bimboBoss.BotId, 10);
            }

            // top player gets maximum XP reward, each player down the line receives a little less
            var i = 0;
            var maxReward = 1000;

            for (var r = 0; r < damages.Count; r++)
            {
                var damage = damages.ElementAt(r);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                if (victor == null)
                {
                    continue;
                }
                var reward = maxReward - (i * 50);
                victor.XP += reward;
                i++;

                if (winner == "bimbo")
                {
                    PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + nerdBoss.GetFullName() + ", " + bimboBoss.GetFullName() + " gifts you with " + reward + " XP from her powerful magic of seduction!</b>", true);

                    // top three get runes
                    if (r <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                    {
                        DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.HEADMISTRESS_RUNE, PlayerId = victor.Id });
                    }

                }
                else
                {
                    PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + bimboBoss.GetFullName() + ", " + nerdBoss.GetFullName() + " gifts you with " + reward + " XP from her unchallenged mastery of the natural world!</b>", true);

                    // top three get runes
                    if (r <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                    {
                        DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.BIMBO_RUNE, PlayerId = victor.Id });
                    }

                }

                playerRepo.SavePlayer(victor);
                
            }

            DomainRegistry.Repository.Execute(new DeletePlayer {PlayerId = nerdBoss.Id});
            DomainRegistry.Repository.Execute(new DeletePlayer { PlayerId = bimboBoss.Id });


        }
    }
}
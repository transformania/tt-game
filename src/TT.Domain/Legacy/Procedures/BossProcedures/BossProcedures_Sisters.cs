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
        public const string NerdBossForm = "form_Headmistress_of_SCCC_Elyn_and_Judoo";

        public const string BimboBossFirstName = "Beautician Candice";
        public const string BimboBossForm = "form_Head_Beautician_of_Blazes_and_Glamour_Judoo_and_Elyn";
        
        public const string BossesLastName = "Brisby";

        public const int BimboSpellSourceId = 627;

        public const string BimboSpellForm = "form_Bimbo_Mousegirl_Judoo";

        public const int NerdSpellSourceId = 628;

        public const string NerdSpellForm = "form_Nerdy_Mousegirl_Elynsynos";

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
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = NerdBossForm,
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
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = BimboBossForm,
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

            // only allow nerd mouse girls to attack bimbo boss
            if (target.BotId == AIStatics.MouseNerdBotId && attacker.Form != BimboSpellForm)
            {
                return "You can't seem to find the right peeved-off mindset to cast this spell against Adrianna.  Maybe you'd have better luck if you were casting magic against her as a Bimbo Mousegirl...";
            }

            // only allow nerd mouse girls to attack bimbo boss
            if (target.BotId == AIStatics.MouseBimboBotId && attacker.Form != NerdSpellForm)
            {
                return "You can't seem to find the right peeved-off mindset to cast this spell against Candice.  Maybe you'd have better luck if you were casting magic against her as a Nerdy Mousegirl...";
            }
            // only allow bimbo mouse girls to attack nerd boss

            // only allow bimbo spell against nerd boss
            if (target.Form == NerdBossForm && spellSourceId != BimboSpellSourceId)
            {
                return "This spell won't work against Adrianna.";
            }
            else if (target.Form == BimboBossForm && spellSourceId != NerdSpellSourceId)
            {
                return "This spell won't work against Candice.";
            }

            //disallow spells cast against the bosses if they have changed animate forms
            if ((target.BotId == AIStatics.MouseNerdBotId && target.Form != NerdBossForm) || (target.BotId == AIStatics.MouseBimboBotId && target.Form != BimboBossForm))
            {
                return "One of the Brisby sisters have already been transformed; there's no need to attack them any further.";
            }

            return "";
            
        }

        public static void CounterAttack(Player attacker, Player bossTarget)
        {

            AIProcedures.DealBossDamage(bossTarget, attacker, true, 1);

            var spell = ChooseSpell(bossTarget);

            // nerd counters with nerd spell unless she has changed form
            if (bossTarget.BotId == AIStatics.MouseNerdBotId && bossTarget.Form == NerdBossForm || bossTarget.BotId == AIStatics.MouseBimboBotId && bossTarget.Form == BimboBossForm)
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
            if (nerdBoss == null || nerdBoss.Form != NerdBossForm || bimboBoss == null || bimboBoss.Form != BimboBossForm)
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
                    p.Form != NerdSpellForm &&
                    p.InDuel <= 0  &&
                    p.InQuest <= 0).ToList();


                // get all of the players in the room by bimbo
                var playersByBimbo = PlayerProcedures.GetPlayersAtLocation(bimboBoss.dbLocationName).ToList();
                playersByBimbo = playersByBimbo.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                    !PlayerProcedures.PlayerIsOffline(p) &&
                    p.BotId == AIStatics.ActivePlayerBotId &&
                    p.Id != bimboBoss.Id &&
                    p.Form != BimboSpellForm &&
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

            if (nerdBoss.Form != NerdBossForm) {
                winner = "bimbo";
            } else if (bimboBoss.Form != BimboBossForm) {
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

            // top player gets 500 XP, each player down the line receives 25 fewer
            var i = 0;
            var maxReward = 1000;

            for (var r = 0; r < damages.Count; r++)
            {
                var damage = damages.ElementAt(r);
                var victor = playerRepo.Players.FirstOrDefault(p => p.Id == damage.PlayerId);
                var reward = maxReward - (i * 50);
                victor.XP += reward;
                i++;

                if (winner == "bimbo")
                {
                    PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + nerdBoss.GetFullName() + ", " + bimboBoss.GetFullName() + " gifts you with " + reward + " XP from her powerful magic of seduction!</b>", true);

                    // top three get runes
                    if (r <= 2)
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
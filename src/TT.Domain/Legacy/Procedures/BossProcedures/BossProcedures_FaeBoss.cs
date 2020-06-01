using System;
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
    public class BossProcedures_FaeBoss
    {

        public const string FirstName = "Narcissa";
        public const string LastName = "the Exiled";
        public const string SpawnLocation = "fairygrove_greathall";

        public const int GreatFaeSpellSourceId = 565;
        public const int GreatFaeFormSourceId = 257;

        public const int DarkFaeSpellSourceId = 879;


        public const int DarkFaeFormSourceId = 531;

        public const int EnchantedTreeSpellSourceId = 344;
        public const int EnchantedTreeFormSourceId = 50;

        /// <summary>
        /// A list of all the animate spells Narcissa can cast
        /// </summary>
        public static readonly int[] animateSpellsToCast = { GreatFaeSpellSourceId, DarkFaeSpellSourceId, EnchantedTreeSpellSourceId };

        public const int FlowerSpellSourceId = 929;

        public const int ServantOfLunarFaeSpellSourceId = 930;

        /// <summary>
        /// This is the only spell players can cast against Narcissa.
        /// </summary>
        public const int SpellUsedAgainstNarcissaSourceId = 931;

        /// <summary>
        /// A list of the inanimate and pet spells Narcissa can cast
        /// </summary>
        public static readonly int[] inanimateSpellsToCast = { FlowerSpellSourceId, ServantOfLunarFaeSpellSourceId };

        /// <summary>
        /// Probability of drawing Narcissa's aggro when she already has a target set
        /// </summary>
        private const double AggroChance = .2D;

        /// <summary>
        /// The base number of tiles Narcissa will move to catch up to her target or seek out new ones
        /// </summary>
        private const int MovementBaseDistance = 8;

        /// <summary>
        /// The random number of additional tiles Narcissa will move to catch up to her target or seek out new ones
        /// </summary>
        private const int MovementRandomExtraDistance = 6;

        private const int SpellChangeTurnFrequency = 5;

        private const int FaeBossFormId = 582;

        /// <summary>
        /// Spawns Narcissa into the world and sets her initial blank AI Directive
        /// </summary>
        public static void SpawnFaeBoss()
        {
            var faeboss = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.FaebossBotId });

            if (faeboss == null)
            {
                var cmd = new CreatePlayer
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Location = SpawnLocation,
                    Gender = PvPStatics.GenderFemale,
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    FormSourceId = FaeBossFormId,
                    Money = 1000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 25,
                    BotId = AIStatics.FaebossBotId,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var faebossEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                faebossEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(faebossEF));
                playerRepo.SavePlayer(faebossEF);

                AIDirectiveProcedures.GetAIDirective(id);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.NARCISSA_RUNE, PlayerId = faebossEF.Id });
                }
            }

        }

        /// <summary>
        /// Returns whether the spell is valid against Narcissa.
        /// </summary>
        /// <param name="spellSourceId">source id of the spell whose cast is being attempted</param>
        /// <param name="caster">Player attempting to cast the spell</param>
        /// <returns></returns>
        public static Tuple<bool, string> SpellIsValid(int spellSourceId, Player caster)
        {

            if (caster.FormSourceId == GreatFaeFormSourceId || caster.FormSourceId == DarkFaeFormSourceId || caster.FormSourceId == EnchantedTreeFormSourceId)
            {
                return new Tuple<bool, string>(false, "You try to cast upon " + FirstName + ", " + "but the fae's mastery over your current form is overwhelming and you find that you cannot!");
            }

            if (spellSourceId != SpellUsedAgainstNarcissaSourceId)
            {
                return new Tuple<bool, string>(false, "This spell has no effect on " + FirstName + "!  Maybe you should talk to Rusty at the bar and get some advice...");
            }
            return new Tuple<bool, string>(true, "");
        }

        /// <summary>
        /// Perform Narcissa's regular actions when a new turn has started.  If Narcissa has no aggroed target, she seeks to transform random people into
        /// certain animate forms.  If she has aggro, she will attempt to chase them and cast a pet spell on them.  If she can't catch up, she'll cast the animate
        /// spells in the area instead and resume pursuit next turn.
        /// </summary>
        public static void RunTurnLogic()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossBotId);

            // fae boss is no longer animate; end the event
            if (faeboss.Mobility!=PvPStatics.MobilityFull)
            {
                EndEvent();
                return;
            }

            // have Narcissa periodically drop all of her pets/belongings so she doesn't get OP with them
            if (PvPWorldStatProcedures.GetWorldTurnNumber() % 12 == 0)
            {
                DomainRegistry.Repository.Execute(new DropAllItems {PlayerId = faeboss.Id, IgnoreRunes = true});
            }

            var faeBuffs = ItemProcedures.GetPlayerBuffs(faeboss);

            // have Narcissa meditate to get her mana back up
            if (faeboss.Mana < faeboss.MaxMana / 2)
            {
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = faeboss.Id, Buffs = faeBuffs, NoValidate = true });
            }
            else if (faeboss.Mana < faeboss.MaxMana / 3)
            {
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = faeboss.Id, Buffs = faeBuffs, NoValidate = true });
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = faeboss.Id, Buffs = faeBuffs, NoValidate = true });
            }
            else if (faeboss.Mana < faeboss.MaxMana / 4)
            {
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = faeboss.Id, Buffs = faeBuffs, NoValidate = true });
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = faeboss.Id, Buffs = faeBuffs, NoValidate = true });
                DomainRegistry.Repository.Execute(new Meditate { PlayerId = faeboss.Id, Buffs = faeBuffs, NoValidate = true });
            }


            var directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            // no target, go out and hit some random people with animate spells
            if (!HasValidTarget(directive))
            {
                ResetTarget(directive);
                var newTargetLocation = GetLocationWithMostEligibleTargets();
                var newActualLocation = AIProcedures.MoveTo(faeboss, newTargetLocation, GetRandomChaseDistance());
                faeboss.dbLocationName = newActualLocation;
                playerRepo.SavePlayer(faeboss);

                CastAnimateSpellsAtLocation(faeboss);
            }

            // Narcissa has a valid target, go for them
            else
            {
                var target = PlayerProcedures.GetPlayer((int)directive.Var1);
                var newTargetLocation = target.dbLocationName;
                var newActualLocation = AIProcedures.MoveTo(faeboss, newTargetLocation, GetRandomChaseDistance());
                faeboss.dbLocationName = newActualLocation;
                playerRepo.SavePlayer(faeboss);

                if (faeboss.dbLocationName == target.dbLocationName)
                {
                    var spell = ChooseSpell(PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityPet);

                    for (var i = 0; i < 4; i++)
                    {
                        AttackProcedures.Attack(faeboss, target, spell);
                    }
                }
                else {
                    CastAnimateSpellsAtLocation(faeboss);
                }
            }

        }

        /// <summary>
        /// Attack an attacking player back.  There is a random chance to draw Narcissa's aggro from doing this if she has a target.  If she has no active target,
        /// the attacker instantly becomes her new target.
        /// </summary>
        /// <param name="attacker"></param>
        public static void CounterAttack(Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossBotId);

            AIProcedures.DealBossDamage(faeboss, attacker, true, 1); // log attack for human on boss

            var spell = ChooseSpell(PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityInanimate);

            for (var i = 0; i < 3; i++)
            {
                AttackProcedures.Attack(faeboss, attacker, spell);
                AIProcedures.DealBossDamage(faeboss, attacker, false, 1); // log attack for boss on human
            }

            var directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            // random chance to aggro faeboss
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var num = rand.NextDouble();

            if (num < AggroChance || directive.Var1==0)
            {
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                var dbDirective = aiRepo.AIDirectives.FirstOrDefault(a => a.Id == directive.Id);

                dbDirective.Var1 = attacker.Id;
                aiRepo.SaveAIDirective(dbDirective);
            }
        }

        /// <summary>
        /// Calculate which spell for Narcissa to cast depending on the attacking player.  Narcissa will change spells every now and then based on the world turn
        /// number.
        /// </summary>
        /// <param name="turnNumber">World turn number</param>
        /// <param name="spellMobilityType">The spell type to cast with.  Narcissa targets neutral players with animate spells and her targets with inanimate/pet spells</param>
        /// <returns></returns>
        public static int ChooseSpell(int turnNumber, string spellMobilityType)
        {

            // index = Math.Floor(turn_number / spell_swap_frequeny) % spell_counts

            if (spellMobilityType==PvPStatics.MobilityFull)
            {
                var index = (int)Math.Floor((double)turnNumber / SpellChangeTurnFrequency) % animateSpellsToCast.Count();
                return animateSpellsToCast[index];
            } else
            {
                var index = (int)Math.Floor((double)turnNumber / SpellChangeTurnFrequency) % inanimateSpellsToCast.Count();
                return inanimateSpellsToCast[index];
            }
        }

        /// <summary>
        /// End the faeboss boss event and distribute XP to players who fought her
        /// </summary>
        public static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndFaeBoss();

            var damages = AIProcedures.GetTopAttackers(AIStatics.FaebossBotId, 25);

            // top player gets 1000 XP, each player down the line receives 35 fewer
            var l = 0;
            var maxReward = 1000;

            for (var i = 0; i < damages.Count; i++)
            {
                var damage = damages.ElementAt(i);

                var victor = PlayerProcedures.GetPlayer(damage.PlayerId);
                var reward = maxReward - (l * 35);
                victor.XP += reward;
                l++;

                PlayerProcedures.GiveXP(victor, reward);
                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + FirstName + " " + LastName + ", you earn " + reward + " XP from your spells cast against traitorous fae.</b>", true);

                // top three get runes
                if (i <= 2 && victor.Mobility == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = RuneStatics.NARCISSA_RUNE, PlayerId = victor.Id });
                }

            }

        }

        /// <summary>
        /// Return a list of all the eligible players in a location for Narcissa to attack
        /// </summary>
        /// <param name="player">Narciss</param>
        /// <returns></returns>
        private static List<Player> GetEligibleTargetsInLocation(Player player)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());
            var playersHere = PlayerProcedures.GetPlayersAtLocation(player.dbLocationName).Where(m => m.Mobility == PvPStatics.MobilityFull &&
            m.Id != player.Id &&
            m.BotId >= AIStatics.PsychopathBotId &&
            m.LastActionTimestamp > cutoff &&
            m.InDuel <= 0 &&
            m.InQuest <= 0).ToList();

            return playersHere;
        }

        /// <summary>
        /// Return the tile of the map which has the most targets that Narcissa is allowed to transform
        /// </summary>
        /// <returns>dbName of location found</returns>
        private static string GetLocationWithMostEligibleTargets()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
            p.LastActionTimestamp > cutoff &&
            p.FormSourceId != GreatFaeFormSourceId &&
            !p.dbLocationName.Contains("dungeon_") &&
            p.InDuel <= 0 &&
            p.InQuest <= 0).GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key);

            return locs.FirstOrDefault() ?? "fairygrove_entrance"; // default to fairygrove entrance if, for some reason, 0 valid targets exist
        }

        /// <summary>
        /// Determines whether Narcissa is currently chasing someone who is valid to be transformed still
        /// </summary>
        /// <param name="directive">AI Directive containing target Id</param>
        /// <returns>True if target is valid, false if not</returns>
        private static bool HasValidTarget(AIDirective directive)
        {
            if (directive == null || directive.Var1 == 0)
            {
                return false;
            }

            var target = PlayerProcedures.GetPlayer((int)directive.Var1);

            // TODO:  This can probably be swapped out with Arrhae's "CanBeAttacked" method in the future.  But for now...
            if (target == null ||
                        target.Mobility != PvPStatics.MobilityFull ||
                        PlayerProcedures.PlayerIsOffline(target) ||
                        target.IsInDungeon() ||
                        target.InDuel > 0 ||
                        target.InQuest > 0)
            {
                return false;
            }

            return true;
            
        }

        /// <summary>
        /// Clears Narcissa's target
        /// </summary>
        /// <param name="directive"></param>
        private static void ResetTarget(AIDirective directive)
        {
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            var ai = aiRepo.AIDirectives.FirstOrDefault(a => a.Id == directive.Id);
            ai.Var1 = 0;
            aiRepo.SaveAIDirective(ai);
        }

        /// <summary>
        ///  Returns 14.
        /// </summary>
        /// <returns>14</returns>
        private static int GetRandomChaseDistance()
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());
            var num = rand.NextDouble()*6;
            return MovementBaseDistance + (int)MovementRandomExtraDistance;
        }

        /// <summary>
        /// Cast 1 animate spell on each player in Narcissa's current location.  She will not change her aggro for this.
        /// </summary>
        /// <param name="faeboss">Player casting the spells.  In this case, always Narcissa.</param>
        private static void CastAnimateSpellsAtLocation(Player faeboss)
        {
            var playersHere = GetEligibleTargetsInLocation(faeboss);

            foreach (var p in playersHere)
            {
                var spell = ChooseSpell(PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityFull);
                AttackProcedures.Attack(faeboss, p, spell);
                AIProcedures.DealBossDamage(faeboss, p, false, 1); // log attack for human on boss
            }
        }

    }
}
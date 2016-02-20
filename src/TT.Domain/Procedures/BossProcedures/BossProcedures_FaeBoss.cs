using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_FaeBoss
    {

        public const string FirstName = "Narcissa";
        public const string LastName = "the Exiled";
        public const string Form = "form_Corrupted_Lunar_Fae_Roxanne246810(Rachael_Victor/Yuki_Kitsu)";
        public const string SpawnLocation = "fairygrove_greathall";

        public const string GreatFaeSpell = "skill_Midsummer's_Eve_Vivien_Gemai";
        public const string GreatFaeForm = "form_Great_Fairy_Vivien_Gemai";

        public const string DarkFaeSpell = "skill_Darkness_My_Old_Friend_Larissa_Fay";
        public const string DarkFaeForm = "form_Dark_Fae_Larissa_Fay";

        public const string EnchantedTreeSpell = "skill_Take_Root_Sherry_Gray";
        public const string EnchantedTreeForm = "form_Enchanted_Tree_Sherry_Gray";

        /// <summary>
        /// A list of all the animate spells Narcissa can cast
        /// </summary>
        public static readonly string[] animateSpellsToCast = { GreatFaeSpell, DarkFaeSpell, EnchantedTreeSpell };

        public const string FlowerSpell = "skill_Tainted_Flower_Roxanne246810(Rachael_Victor/Yuki_Kitsu)";
        public const string ServantOfLunarFaeSpell = "skill_Touch_of_the_Moon_Roxanne246810(Rachael_Victor/Yuki_Kitsu)"; // NOT YET READY FOR RELEASE

        /// <summary>
        /// This is the only spell players can cast against Narcissa.
        /// </summary>
        public const string SpellUsedAgainstNarcissa = "skill_The_Cheekiest_of_Counterspells_Roxanne246810(Rachael_Victor/Yuki_Kitsu)";

        /// <summary>
        /// A list of the inanimate and pet spells Narcissa can cast
        /// </summary>
        public static readonly string[] inanimateSpellsToCast = { FlowerSpell }; // TODO:  Add in ServantOfLunarFaeSpell when it is ready

        /// <summary>
        /// Probability of drawing Narcissa's aggro when she already has a target set
        /// </summary>
        private const double AggroChance = .2D;

        /// <summary>
        /// The base number of tiles Narcissa will move to catch up to her target or seek out new ones
        /// </summary>
        private const int MovementBaseDistance = 7;

        /// <summary>
        /// The random number of additional tiles Narcissa will move to catch up to her target or seek out new ones
        /// </summary>
        private const int MovementRandomExtraDistance = 6;

        private const int SpellChangeTurnFrequency = 5;

        /// <summary>
        /// Spawns Narcissa into the world and sets her initial blank AI Directive
        /// </summary>
        /// <returns></returns>
        public static Player SpawnFaeBoss()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);

            if (faeboss == null)
            {
                faeboss = new Player()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    ActionPoints = 120,
                    dbLocationName = SpawnLocation,
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender =  PvPStatics.GenderFemale,
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = Form,
                    Money = 1000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = 25,
                    MembershipId = AIStatics.FaebossId.ToString(),
                    BotId = AIStatics.FaebossId,
                    ActionPoints_Refill = 360,
                };

            }

            playerRepo.SavePlayer(faeboss);

            AIDirective directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            return faeboss;
        }

        /// <summary>
        /// Returns whether the spell is valid against Narcissa.
        /// </summary>
        /// <param name="spellName">db name of the spell whose cast is being attempted</param>
        /// <param name="caster">Player attempting to cast the spell</param>
        /// <returns></returns>
        public static Tuple<bool, string> SpellIsValid(string spellName, Player caster)
        {

            if (caster.Form == GreatFaeForm || caster.Form == DarkFaeForm || caster.Form == EnchantedTreeForm)
            {
                return new Tuple<bool, string>(false, "You try to cast upon " + FirstName + ", " + "but the fae's mastery over your current form is overwhelming and you find that you cannot!");
            }

            if (spellName != SpellUsedAgainstNarcissa)
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
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);

            // fae boss is no longer animate; end the event
            if (faeboss.Mobility!=PvPStatics.MobilityFull)
            {
                EndEvent();
                return;
            }

            // have Narcissa periodically drop all of her pets/belongings so she doesn't get OP with them
            if (PvPWorldStatProcedures.GetWorldTurnNumber() % 12 == 0)
            {
                ItemProcedures.DropAllItems(faeboss);
            }

            BuffBox faeBuffs = ItemProcedures.GetPlayerBuffsSQL(faeboss);

            // have Narcissa meditate to get her mana back up
            if (faeboss.Mana < faeboss.MaxMana / 2)
            {
                PlayerProcedures.Meditate(faeboss, faeBuffs);
            }
            else if (faeboss.Mana < faeboss.MaxMana / 3)
            {
                PlayerProcedures.Meditate(faeboss, faeBuffs);
                PlayerProcedures.Meditate(faeboss, faeBuffs);
            }
            else if (faeboss.Mana < faeboss.MaxMana / 4)
            {
                PlayerProcedures.Meditate(faeboss, faeBuffs);
                PlayerProcedures.Meditate(faeboss, faeBuffs);
                PlayerProcedures.Meditate(faeboss, faeBuffs);
            }


            AIDirective directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            // no target, go out and hit some random people with animate spells
            if (HasValidTarget(directive) ==false)
            {
                ResetTarget(directive);
                string newTargetLocation = GetLocationWithMostEligibleTargets();
                string newActualLocation = AIProcedures.MoveTo(faeboss, newTargetLocation, GetRandomChaseDistance());
                faeboss.dbLocationName = newActualLocation;
                playerRepo.SavePlayer(faeboss);

                CastAnimateSpellsAtLocation(faeboss);
            }

            // Narcissa has a valid target, go for them
            else
            {
                Player target = PlayerProcedures.GetPlayer((int)directive.Var1);
                string newTargetLocation = target.dbLocationName;
                string newActualLocation = AIProcedures.MoveTo(faeboss, newTargetLocation, GetRandomChaseDistance());
                faeboss.dbLocationName = newActualLocation;
                playerRepo.SavePlayer(faeboss);

                if (faeboss.dbLocationName == target.dbLocationName)
                {
                    string spell = ChooseSpell(target, PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityPet);

                    for (int i = 0; i < 4; i++)
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
            Player faeboss = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.FaebossId);

            AIProcedures.DealBossDamage(faeboss, attacker, true, 1); // log attack for human on boss

            string spell = ChooseSpell(attacker, PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityInanimate);

            for (var i = 0; i < 3; i++)
            {
                AttackProcedures.Attack(faeboss, attacker, spell);
                AIProcedures.DealBossDamage(faeboss, attacker, false, 1); // log attack for boss on human
            }

            AIDirective directive = AIDirectiveProcedures.GetAIDirective(faeboss.Id);

            // random chance to aggro faeboss
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble();

            if (num < AggroChance || directive.Var1==0)
            {
                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
                AIDirective dbDirective = aiRepo.AIDirectives.FirstOrDefault(a => a.Id == directive.Id);

                dbDirective.Var1 = attacker.Id;
                aiRepo.SaveAIDirective(dbDirective);
            }
        }

        /// <summary>
        /// Calculate which spell for Narcissa to cast depending on the attacking player.  Narcissa will change spells every now and then based on the world turn
        /// number.
        /// </summary>
        /// <param name="attacker">The attacking player</param>
        /// <param name="turnNumber">World turn number</param>
        /// <param name="spellMobilityType">The spell type to cast with.  Narcissa targets neutral players with animate spells and her targets with inanimate/pet spells</param>
        /// <returns></returns>
        public static string ChooseSpell(Player attacker, int turnNumber, string spellMobilityType)
        {

            // index = Math.Floor(turn_number / spell_swap_frequeny) % spell_counts

            if (spellMobilityType==PvPStatics.MobilityFull)
            {
                int index = (int)Math.Floor((double)turnNumber / SpellChangeTurnFrequency) % animateSpellsToCast.Count();
                return animateSpellsToCast[index];
            } else
            {
                int index = (int)Math.Floor((double)turnNumber / SpellChangeTurnFrequency) % inanimateSpellsToCast.Count();
                return inanimateSpellsToCast[index];
            }
        }

        /// <summary>
        /// End the faeboss boss event and distribute XP to players who fought her
        /// </summary>
        public static void EndEvent()
        {
            PvPWorldStatProcedures.Boss_EndFaeBoss();

            List<BossDamage> damages = AIProcedures.GetTopAttackers(AIStatics.FaebossId, 25);

            // top player gets 1000 XP, each player down the line receives 35 fewer
            int l = 0;
            int maxReward = 1000;

            foreach (BossDamage damage in damages)
            {

                Player victor = PlayerProcedures.GetPlayer(damage.PlayerId);
                int reward = maxReward - (l * 35);
                victor.XP += reward;
                l++;

                PlayerProcedures.GiveXP(victor, reward);
                PlayerLogProcedures.AddPlayerLog(victor.Id, "<b>For your contribution in defeating " + FirstName + " " + LastName + ", you earn " + reward + " XP from your spells cast against traitorous fae.</b>", true);
            }

        }

        /// <summary>
        /// Return a list of all the eligible players in a location for Narcissa to attack
        /// </summary>
        /// <param name="player">Narciss</param>
        /// <returns></returns>
        private static List<Player> GetEligibleTargetsInLocation(Player player)
        {
            DateTime cutoff = DateTime.UtcNow.AddMinutes(-30);
            List<Player> playersHere = PlayerProcedures.GetPlayersAtLocation(player.dbLocationName).Where(m => m.Mobility == PvPStatics.MobilityFull &&
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
            DateTime cutoff = DateTime.UtcNow.AddMinutes(-30);
            IEnumerable<string> locs = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
            p.LastActionTimestamp > cutoff &&
            p.Form != GreatFaeForm &&
            p.dbLocationName.Contains("dungeon_") == false &&
            p.InDuel <= 0 &&
            p.InQuest <= 0).GroupBy(p => p.dbLocationName).OrderByDescending(p => p.Count()).Select(p => p.Key);
            return locs.First();
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

            Player target = PlayerProcedures.GetPlayer((int)directive.Var1);

            // TODO:  This can probably be swapped out with Arrhae's "CanBeAttacked" method in the future.  But for now...
            if (target == null ||
                        target.Mobility != PvPStatics.MobilityFull ||
                        PlayerProcedures.PlayerIsOffline(target) ||
                        target.IsInDungeon() == true ||
                        target.InDuel > 0 ||
                        target.InQuest > 0)
            {
                return false;
            }

            return true;
            
        }

        /// <summary>
        /// Clears Narciss'a target
        /// </summary>
        /// <param name="directive"></param>
        private static void ResetTarget(AIDirective directive)
        {
            IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();
            AIDirective ai = aiRepo.AIDirectives.FirstOrDefault(a => a.Id == directive.Id);
            ai.Var1 = 0;
            aiRepo.SaveAIDirective(ai);
        }

        /// <summary>
        ///  Returns a number between 7 and 12 to give Narcissa some randomness on how far she is willing to chase a target in one turn.
        /// </summary>
        /// <returns>Random int between 7 and 12</returns>
        private static int GetRandomChaseDistance()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double num = rand.NextDouble()*6;
            return MovementBaseDistance + (int)MovementRandomExtraDistance;
        }

        /// <summary>
        /// Cast 1 animate spell on each player in Narcissa's current location.  She will not change her aggro for this.
        /// </summary>
        /// <param name="faeboss">Player casting the spells.  In this case, always Narcissa.</param>
        private static void CastAnimateSpellsAtLocation(Player faeboss)
        {
            List<Player> playersHere = GetEligibleTargetsInLocation(faeboss);

            foreach (Player p in playersHere)
            {
                string spell = ChooseSpell(p, PvPWorldStatProcedures.GetWorldTurnNumber(), PvPStatics.MobilityFull);
                AttackProcedures.Attack(faeboss, p, spell);
                AIProcedures.DealBossDamage(faeboss, p, false, 1); // log attack for human on boss
            }
        }

    }
}
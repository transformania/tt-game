using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Legacy.Services;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.World.Queries;

namespace TT.Domain.Legacy.Procedures.BossProcedures
{

    public class MinibossData
    {
        public string Title { get; set; }
        public int FormSourceId { get; set; }
        public string FormName { get; set; }
        public int RuneIdToGive { get; set; }
        public string Region { get; set; }
        public List<string> Spells { get; set; }
        public int BotId { get; set; }
    }

    public static class BossProcedures_Minibosses
    {

        private static int SpellChangeTurnFrequency = 6;
        private static double ChanceToRespawn = .005;
        private static Random rand = new Random();

        private static Dictionary<string, MinibossData> bossData = new Dictionary<string, MinibossData>
        {
            {
                "sororityMother",
                new MinibossData {
                    FormSourceId = 957,
                    FormName = "form_Sorority_House_Mother_Judoo",
                    Title = "Sorority House Mother",
                    Region = "sorority",
                    Spells = new List<string> { "skill_Naughty_Knitted_Nethers_Duhad", "skill_Lingerie_Inveigh__PsychoticPie", "skill_Floral_Upholding_Passerby"},
                    RuneIdToGive = RuneStatics.MINIBOSS_SORORITY_MOTHER_RUNE,
                    BotId = AIStatics.MinibossSororityMotherId
,                }
            },
            {
                "popGoddess",
                new MinibossData {
                    FormSourceId = 956,
                    FormName = "form_Pop_Goddess_Judoo",
                    Title = "Pop Goddess",
                    Region = "concert_hall",
                    Spells = new List<string> { "skill_Almost_A_Shirt_Kinello", "skill_Reflecius_Fabricos_Haretia", "skill_It's_Got_Ruffles!_Martiandawn"},
                    BotId = AIStatics.MinibossPopGoddessId,
                    RuneIdToGive = RuneStatics.MINIBOSS_POP_GODDESS_RUNE,
                }
            },
            {
                "possessedMaid",
                new MinibossData {
                    FormSourceId = 958,
                    FormName = "form_Possessed_Maid_Judoo",
                    Title = "Maid",
                    Region = "mansion",
                    Spells = new List<string> { "skill_Haute_Couture_Hanna", "skill_Dusty_Hanna", "skill_Crown_of_Order_Hanna"},
                    BotId = AIStatics.MinibossPossessedMaidId,
                    RuneIdToGive = RuneStatics.MINIBOSS_POSSESSED_MAID_RUNE,
                }
            },
            {
                "seamstress",
                new MinibossData {
                    FormSourceId = 959,
                    FormName = "form_Sanguine_Seamstress_Judoo",
                    Title = "Seamstress",
                    Region = "clothing",
                    Spells = new List<string> { "skill_Soft_But_Wild_Lara_Ithilwen", "skill_So_Fluffy!_Gina_Victor_/_Rena_Kitsu", "skill_Ensoften_Frederika_Rein"},
                    BotId = AIStatics.MinibossSeamstressId,
                    RuneIdToGive = RuneStatics.MINIBOSS_SEAMSTRESS_RUNE,
                }
            },
            {
                "disgruntledGroundskeeper",
                new MinibossData {
                    FormSourceId = 976,
                    FormName = "form_Disgruntled_Groundskeeper_Judoo",
                    Title = "Groundskeeper",
                    Region = "park",
                    Spells = new List<string> { "skill_Woodland's_Reach_Ai_Noai", "skill_Bewitching_Flight_Crystal_Moyer", "skill_Water_Water_Every_Who_Merp"},
                    BotId = AIStatics.MinibossGroundskeeperId,
                    RuneIdToGive = RuneStatics.MINIBOSS_GROUNDSKEEPER_RUNE
                }
            },
            {
                "exchangeProfessor",
                new MinibossData {
                    FormSourceId = 979,
                    FormName = "form__Exchange_Professor_Judoo",
                    Title = "Professor",
                    Region = "lab",
                    Spells = new List<string> { "skill_Milkify_Soul", "skill_Liquefy_V2.0_GooGirl", "skill_Day_of_the_Tentacle_Varn"},
                    BotId = AIStatics.MinibossExchangeProfessorId,
                    RuneIdToGive = RuneStatics.MINIBOSS_PROFESSOR_RUNE
                }
            },
            //{
            //    "fiendishFarmhand",
            //    new MinibossData {
            //        FormSourceId = 978,
            //        FormName = "form_Fiendish_Farmhand_Judoo",
            //        Title = "Farmhand",
            //        Region = "ranch_outside",
            //        Spells = new List<string> { "skill_Dairy_Factory_Sampleguy", "skill_Man's_Best_Friend_Soren_Reus", "skill_Pulling_Wool_Over_Their_Eyes_Kim_Steele"},
            //        BotId = AIStatics.MinibossFiendishFarmhandId,
            //        RuneIdToGive = RuneStatics.MINIBOSS_FARMHAND_RUNE
            //    }
            //},
            //{
            //    "lazyLifeguard",
            //    new MinibossData {
            //        FormSourceId = 977,
            //        FormName = "form_Lazy_Lifeguard_Judoo",
            //        Title = "Lifeguard",
            //        Region = "pool",
            //        Spells = new List<string> { "skill_Game_on_the_Beach_Judoo", "skill_Slippery_Swimsuit_Illia_Malvusin", "skill_Summer_Fun_Alexander", "skill_Sink_or_Swim_Illia_Malvusin"},
            //        BotId = AIStatics.MinibossLazyLifeguardId,
            //        RuneIdToGive = RuneStatics.MINIBOSS_LIFEGUARD_RUNE
            //    }
            //},
        };

        public static List<Exception> RunAll(int turnNumber)
        {
            var exceptions = new List<Exception>();
            foreach (var data in bossData)
            {
                try
                {
                    Run(turnNumber, data.Value);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
                
            }

            return exceptions;
        }

        public static void Run(int turnNumber, MinibossData data)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var miniboss =
                playerRepo.Players.FirstOrDefault(p => p.BotId == data.BotId && p.Mobility == PvPStatics.MobilityFull);

            // spawn a new boss if last is null
            if (miniboss == null && rand.NextDouble() < ChanceToRespawn)
            {

                var spawnLocation = LocationsStatics.GetRandomLocation_InRegion(data.Region);

                var cmd = new CreatePlayer
                {
                    FirstName = data.Title,
                    LastName = NameService.GetRandomLastName(),
                    Location = spawnLocation,
                    Gender = PvPStatics.GenderFemale,
                    Health = 10000,
                    Mana = 10000,
                    MaxHealth = 10000,
                    MaxMana = 10000,
                    Form = data.FormName,
                    FormSourceId = data.FormSourceId,
                    Money = 2000,
                    Mobility = PvPStatics.MobilityFull,
                    Level = GetLevel(turnNumber),
                    BotId = data.BotId
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var minibossEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                minibossEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(minibossEF));
                playerRepo.SavePlayer(minibossEF);

                for (var i = 0; i < 2; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = data.RuneIdToGive, PlayerId = minibossEF.Id });
                }

            }

            if (miniboss != null && miniboss.Mobility == PvPStatics.MobilityFull)
            {
                // move to a randomn location in this region
                var nextLocation = LocationsStatics.GetRandomLocation_InRegion(data.Region);
                var actualNextLocation = AIProcedures.MoveTo(miniboss, nextLocation, 10);
                miniboss.dbLocationName = actualNextLocation;
                miniboss.Mana = miniboss.MaxMana;
                playerRepo.SavePlayer(miniboss);
                var playersHere = GetEligibleTargetsAtLocation(actualNextLocation);
                foreach (var target in playersHere)
                {
                    AttackProcedures.Attack(miniboss, target, ChooseSpell(turnNumber, data.Spells));
                }
            }
            
        }

        public static void CounterAttack(Player victim, Player boss)
        {
            var definition = bossData.SingleOrDefault(d => d.Value.BotId == boss.BotId);
            var world = DomainRegistry.Repository.FindSingle(new GetWorld());

            var counterAttackTimes = GetCounterAttackTimes(boss.Health, boss.MaxHealth, world.TurnNumber); 
            for (var i = 0; i < counterAttackTimes; i++)
            {
                AttackProcedures.Attack(boss, victim, ChooseSpell(world.TurnNumber, definition.Value.Spells));
            }
        }

        private static List<Player> GetEligibleTargetsAtLocation(string location)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cutoff = GetOnlineCutoffTime();
            return playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                                                 p.LastActionTimestamp > cutoff &&
                                                    (p.BotId == AIStatics.ActivePlayerBotId ||
                                                     p.BotId == AIStatics.PsychopathBotId) &&
                                                 p.dbLocationName == location &&
                                                 p.InDuel <= 0 &&
                                                 p.InQuest <= 0).ToList();

        }

        private static DateTime GetOnlineCutoffTime()
        {
            return DateTime.UtcNow.AddMinutes(-PvPStatics.OfflineAfterXMinutes);
        }

        private static string ChooseSpell(int turnNumber, List<string> spells)
        {
            var index = (int)Math.Floor((double)turnNumber / SpellChangeTurnFrequency) % spells.Count;
            return spells[index];
        }

        private static int GetLevel(int turnNumber)
        {
            return turnNumber / 350 + 8;
        }

        private static int GetCounterAttackTimes(decimal currentHealth, decimal maxHealth, int turnNumber)
        {
            decimal value = currentHealth / maxHealth;
            var maxBonusAttackTimes = turnNumber / 1250;
            var bonusAttackTimes = (int)(.5 + rand.NextDouble() * maxBonusAttackTimes);
            if (value > .5m)
            {
                return  1 + bonusAttackTimes;
            }
            else if (value > .25m)
            {
                return 2 + bonusAttackTimes;
            }
            else if (value > .1m)
            { 
                return 3 + bonusAttackTimes;
            }

            return 4 + bonusAttackTimes;
        }

    }
}

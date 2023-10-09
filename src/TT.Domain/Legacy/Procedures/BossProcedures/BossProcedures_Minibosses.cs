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
        public List<int> Spells { get; set; }
        public int BotId { get; set; }
        public DateTime LastActionTime { get; set; }
    }

    public static class BossProcedures_Minibosses
    {

        private static int ActiveSpells = 3;
        private static int SpellChangeTurnFrequency = 6;
        private static int CyclesBeforeSpellSwap = 8;
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
                    Spells = new List<int> { 522, 1421, 1436, 521, 1332, 354, 784, 678, 868, 1584},
                    RuneIdToGive = RuneStatics.MINIBOSS_SORORITY_MOTHER_RUNE,
                    BotId = AIStatics.MinibossSororityMotherId,
                    LastActionTime = DateTime.MinValue
,                }
            },
            {
                "popGoddess",
                new MinibossData {
                    FormSourceId = 956,
                    FormName = "form_Pop_Goddess_Judoo",
                    Title = "Pop Goddess",
                    Region = "concert_hall",
                    Spells = new List<int> { 1102, 360, 1325, 363, 441, 1501, 530, 1221, 431, 1386},
                    BotId = AIStatics.MinibossPopGoddessId,
                    RuneIdToGive = RuneStatics.MINIBOSS_POP_GODDESS_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "possessedMaid",
                new MinibossData {
                    FormSourceId = 958,
                    FormName = "form_Possessed_Maid_Judoo",
                    Title = "Maid",
                    Region = "mansion",
                    Spells = new List<int> { 361, 1285, 862, 1536, 390, 376, 1224, 1539, 1558, 1289},
                    BotId = AIStatics.MinibossPossessedMaidId,
                    RuneIdToGive = RuneStatics.MINIBOSS_POSSESSED_MAID_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "seamstress",
                new MinibossData {
                    FormSourceId = 959,
                    FormName = "form_Sanguine_Seamstress_Judoo",
                    Title = "Seamstress",
                    Region = "clothing",
                    Spells = new List<int> { 1230, 976, 683, 1145, 1037, 1449, 1557, 313, 1061, 336},
                    BotId = AIStatics.MinibossSeamstressId,
                    RuneIdToGive = RuneStatics.MINIBOSS_SEAMSTRESS_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "disgruntledGroundskeeper",
                new MinibossData {
                    FormSourceId = 976,
                    FormName = "form_Disgruntled_Groundskeeper_Judoo",
                    Title = "Groundskeeper",
                    Region = "park",
                    Spells = new List<int> { 947, 1242, 1190, 436, 1199, 1049, 1050, 791, 456, 1331},
                    BotId = AIStatics.MinibossGroundskeeperId,
                    RuneIdToGive = RuneStatics.MINIBOSS_GROUNDSKEEPER_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "exchangeProfessor",
                new MinibossData {
                    FormSourceId = 979,
                    FormName = "form_Exchange_Professor_Judoo",
                    Title = "Professor",
                    Region = "lab",
                    Spells = new List<int> { 891, 1435, 883, 966, 446, 1237, 1356, 1095, 1116, 420},
                    BotId = AIStatics.MinibossExchangeProfessorId,
                    RuneIdToGive = RuneStatics.MINIBOSS_PROFESSOR_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "plushAngel",
                new MinibossData {
                    FormSourceId = 1234,
                    FormName = "form_Smol_Angel_Plushie_Breenarox",
                    Title = "Angelic",
                    Region = "streets",
                    Spells = new List<int> { 943, 765, 391},
                    BotId = AIStatics.MinibossPlushAngelId,
                    RuneIdToGive = RuneStatics.MINIBOSS_PLUSHANGEL_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "eventAngel",
                new MinibossData {
                    FormSourceId = 1259,
                    FormName = "form_Archangel",
                    Title = "Archangel",
                    Region = "forest",
                    Spells = new List<int> { 1340, 1343, 1345},
                    BotId = AIStatics.MinibossArchangelId,
                    RuneIdToGive = RuneStatics.MINIBOSS_ARCHANGEL_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "eventDemon",
                new MinibossData {
                    FormSourceId = 1258,
                    FormName = "form_Archdemon",
                    Title = "Archdemon",
                    Region = "castle",
                    Spells = new List<int> { 941, 954, 1351},
                    BotId = AIStatics.MinibossArchdemonId,
                    RuneIdToGive = RuneStatics.MINIBOSS_ARCHDEMON_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "dungeonSlime",
                new MinibossData {
                    FormSourceId = 1316,
                    FormName = "",
                    Title = "Slime Host",
                    Region = "dungeon",
                    Spells = new List<int> { 349, 446, 841, 891, 955, 1245, 1392, 1428, 1529},
                    BotId = AIStatics.MinibossDungeonSlimeId,
                    RuneIdToGive = RuneStatics.MINIBOSS_DUNGEONSLIME_RUNE[rand.Next(0, RuneStatics.MINIBOSS_DUNGEONSLIME_RUNE.Length)], // Give a random item from its list.
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "plushDemon",
                new MinibossData {
                    FormSourceId = 1535,
                    FormName = "form_Smol_Succubus_Plushie_Breenarox",
                    Title = "Demonic",
                    Region = "dungeon",
                    Spells = new List<int> { 630, 857, 950},
                    BotId = AIStatics.MinibossPlushDemonId,
                    RuneIdToGive = RuneStatics.MINIBOSS_PLUSHDEMON_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "fiendishFarmhand",
                new MinibossData {
                    FormSourceId = 978,
                    FormName = "form_Fiendish_Farmhand_Judoo",
                    Title = "Farmhand",
                    Region = "ranch_outside",
                    Spells = new List<int> { 404, 1256, 355, 583, 508, 484, 1127, 1255},
                    BotId = AIStatics.MinibossFiendishFarmhandId,
                    RuneIdToGive = RuneStatics.MINIBOSS_FARMHAND_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "lazyLifeguard",
                new MinibossData {
                    FormSourceId = 977,
                    FormName = "form_Lazy_Lifeguard_Judoo",
                    Title = "Lifeguard",
                    Region = "pool",
                    Spells = new List<int> { 458, 974, 678, 1126, 510, 1044, 994, 1290, 455, 1432, 589, 679 }, // There are a lot of pool-related forms. Twelve should be enough.
                    BotId = AIStatics.MinibossLazyLifeguardId,
                    RuneIdToGive = RuneStatics.MINIBOSS_LIFEGUARD_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "maleRatThief",
                new MinibossData {
                    FormSourceId = 278,
                    FormName = "form_Male_Rat_Thief_Miniboss",
                    Title = "Brother Lukajo Seekshadow",
                    Region = "dungeon",
                    Spells = new List<int> { 609 },
                    BotId = AIStatics.MinibossMaleThiefId,
                    RuneIdToGive = RuneStatics.RAT_THIEF_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "femaleRatThief",
                new MinibossData {
                    FormSourceId = 279,
                    FormName = "form_Female_Rat_Thief_Miniboss",
                    Title = "Sister Lujienne Seekshadow",
                    Region = "dungeon",
                    Spells = new List<int> { 609 },
                    BotId = AIStatics.MinibossFemaleThiefId,
                    RuneIdToGive = RuneStatics.RAT_THIEF_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "roadQueen",
                new MinibossData {
                    FormSourceId = 934,
                    FormName = "form_Road_Queen_Miniboss",
                    Title = "Road Queen Harley Punksplitter",
                    Region = "dungeon",
                    Spells = new List<int> { 1197 },
                    BotId = AIStatics.MinibossRoadQueenId,
                    RuneIdToGive = RuneStatics.MOTORCYCLE_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "bimboBoss",
                new MinibossData {
                    FormSourceId = 233,
                    FormName = "form_Lovebringer_Miniboss",
                    Title = "Lady Lovebringer, PHD",
                    Region = "dungeon",
                    Spells = new List<int> { 406, 428, 558, 691, 908, 909, 915, 1175, 1419, 1425, 1438, 1484  }, //Lovebringer has no inanimate spells. We'll give her twelve thematic ones like the Slime boss.
                    BotId = AIStatics.MinibossBimbossId,
                    RuneIdToGive = RuneStatics.BIMBO_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "donnaMilton",
                new MinibossData {
                    FormSourceId = 287,
                    FormName = "form_Donna_Miniboss",
                    Title = "'Aunt' Donna Milton",
                    Region = "dungeon",
                    Spells = new List<int> { 465, 595, 596, 597, 649 },
                    BotId = AIStatics.MinibossDonnaId,
                    RuneIdToGive = RuneStatics.DONNA_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "faeBoss",
                new MinibossData {
                    FormSourceId = 582,
                    FormName = "form_Narcissa_Miniboss",
                    Title = "Narcissa the Exiled",
                    Region = "dungeon",
                    Spells = new List<int> { 929, 930 },
                    BotId = AIStatics.MinibossNarcissaId,
                    RuneIdToGive = RuneStatics.NARCISSA_RUNE,
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "nerdMouse",
                new MinibossData {
                    FormSourceId = 317,
                    FormName = "form_Nerd_Mouse_Miniboss",
                    Title = "Headmistress Arianna Brisby",
                    Region = "dungeon",
                    Spells = new List<int> { 1183 },
                    BotId = AIStatics.MinibossNerdMouseId,
                    RuneIdToGive = RuneStatics.MINIBOSS_DUNGEONSLIME_RUNE[rand.Next(0, RuneStatics.MINIBOSS_DUNGEONSLIME_RUNE.Length)], //Mouse sisters don't seem to have a rune... Steal from the Slime here
                    LastActionTime = DateTime.MinValue
                }
            },
            {
                "bimboMouse",
                new MinibossData {
                    FormSourceId = 522,
                    FormName = "form_Bimbo_Mouse_Miniboss",
                    Title = "Beautician Candice Brisby",
                    Region = "dungeon",
                    Spells = new List<int> { 1177 },
                    BotId = AIStatics.MinibossBimboMouseId,
                    RuneIdToGive = RuneStatics.MINIBOSS_DUNGEONSLIME_RUNE[rand.Next(0, RuneStatics.MINIBOSS_DUNGEONSLIME_RUNE.Length)], //Mouse sisters don't seem to have a rune... Steal from the Slime here
                    LastActionTime = DateTime.MinValue
                }
            },
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
                int id;

                /* Round bosses spawn in the dungeon under 3 conditions
                 * 1. The main round version was defeated
                 * 2. They do not currently have an animate version
                 * 3. It has been at least 1 hour since their last action (moving, attacking, spawning)
                 */
                if (data.BotId <= AIStatics.MinibossMaleThiefId && DateTime.UtcNow - data.LastActionTime > TimeSpan.FromHours(1)) 
                {
                    //Determine which boss value to check
                    var stats = PvPWorldStatProcedures.GetWorldStats();
                    string isCompleted = "";
                    switch (data.BotId)
                    { 
                        case AIStatics.MinibossMaleThiefId:
                            isCompleted = stats.Boss_Thief;
                            break;
                        case AIStatics.MinibossFemaleThiefId:
                            isCompleted = stats.Boss_Thief;
                            break;
                        case AIStatics.MinibossRoadQueenId:
                            isCompleted = stats.Boss_MotorcycleGang;
                            break;
                        case AIStatics.MinibossBimbossId:
                            isCompleted = stats.Boss_Bimbo;
                            break;
                        case AIStatics.MinibossDonnaId:
                            isCompleted = stats.Boss_Donna;
                            break;
                        case AIStatics.MinibossNarcissaId:
                            isCompleted = stats.Boss_Faeboss;
                            break;
                        case AIStatics.MinibossNerdMouseId:
                            isCompleted = stats.Boss_Sisters;
                            break;
                        case AIStatics.MinibossBimboMouseId:
                            isCompleted = stats.Boss_Sisters;
                            break;
                    }
                    
                    //COMPLETED is private const where it's defined. Either redefine, hardcode, or make public
                    if (isCompleted.Equals("completed"))
                    {
                        id = SpawnBossRematch(turnNumber, LocationsStatics.GetRandomLocation_InRegion(data.Region), data);
                    }
                    else
                    {
                        //Boss rematch failed. Exit early
                        return;
                    }

                }

                //Regular miniboss check
                else
                {
                    id = SpawnMiniboss(turnNumber, LocationsStatics.GetRandomLocation_InRegion(data.Region), data);
                }

                // Give the parasitic slime lots of buffs for all of the things they've clearly eaten!
                // And also the old bosses, they're big threats after all!
                if (data.BotId == AIStatics.MinibossDungeonSlimeId || data.BotId <= AIStatics.MinibossMaleThiefId)
                {

                    int effectsTotal = 3;
                    for (int i = 0; i < effectsTotal; i++)
                    {
                        // Grab a random effect.
                        EffectProcedures.GivePerkToPlayer(EffectStatics.SUPER_PSYCHO_EFFECT[rand.Next(0, EffectStatics.SUPER_PSYCHO_EFFECT.Length)], id);
                    }
                }

                var minibossEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                minibossEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(minibossEF));
                playerRepo.SavePlayer(minibossEF);

                int itemTotal = 2;

                if (data.BotId == AIStatics.MinibossPlushDemonId)
                {
                    itemTotal = 1;
                }

                for (var i = 0; i < itemTotal; i++)
                {
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = data.RuneIdToGive, PlayerId = minibossEF.Id });
                }

            }

            if (miniboss != null && miniboss.Mobility == PvPStatics.MobilityFull)
            {
                // move to a randomn location in this region
                data.LastActionTime = DateTime.UtcNow;
                var nextLocation = LocationsStatics.GetRandomLocation_InRegion(data.Region);
                var actualNextLocation = AIProcedures.MoveTo(miniboss, nextLocation, 11);
                miniboss.dbLocationName = actualNextLocation;
                miniboss.Mana = miniboss.MaxMana;
                playerRepo.SavePlayer(miniboss);
                var playersHere = GetEligibleTargetsAtLocation(actualNextLocation);
                foreach (var target in playersHere)
                {
                    var (complete, _) = AttackProcedures.Attack(miniboss, target, ChooseSpell(miniboss, turnNumber, data.Spells));

                    if (complete)
                    {
                        AIProcedures.EquipDefeatedPlayer(miniboss, target);
                    }
                }
            }
            
        }

        public static int SpawnMiniboss(int turnNumber, string spawnLocation, MinibossData miniBossData)
        {

            var cmd = new CreatePlayer
            {
                FirstName = miniBossData.Title,
                LastName = NameService.GetRandomLastName(),
                Location = spawnLocation,
                Gender = PvPStatics.GenderFemale,
                Health = 100000,
                Mana = 100000,
                MaxHealth = 100000,
                MaxMana = 100000,
                FormSourceId = miniBossData.FormSourceId,
                Money = 2000,
                Mobility = PvPStatics.MobilityFull,
                Level = GetLevel(turnNumber),
                BotId = miniBossData.BotId
            };
            return DomainRegistry.Repository.Execute(cmd);
        }

        public static int SpawnBossRematch(int turnNumber, string spawnLocation, MinibossData miniBossData)
        {

            var cmd = new CreatePlayer
            {
                FirstName = NameService.GetRandomBossRematchName(),
                LastName = miniBossData.Title,
                Location = spawnLocation,
                Gender = PvPStatics.GenderFemale,
                Health = 100000,
                Mana = 100000,
                MaxHealth = 100000,
                MaxMana = 100000,
                FormSourceId = miniBossData.FormSourceId,
                Money = 2000,
                Mobility = PvPStatics.MobilityFull,
                Level = GetLevel(turnNumber),
                BotId = miniBossData.BotId
            };
            return DomainRegistry.Repository.Execute(cmd);
        }

        public static void CounterAttack(Player victim, Player boss)
        {
            var definition = bossData.SingleOrDefault(d => d.Value.BotId == boss.BotId);
            var world = DomainRegistry.Repository.FindSingle(new GetWorld());

            var counterAttackTimes = GetCounterAttackTimes(boss.Health, boss.MaxHealth, world.TurnNumber);
            var complete = false;
            for (var i = 0; i < counterAttackTimes && !complete; i++)
            {
                (complete, _) = AttackProcedures.Attack(boss, victim, ChooseSpell(boss, world.TurnNumber, definition.Value.Spells));

                //Update last action here for respawn delay on boss rematches
                definition.Value.LastActionTime = DateTime.UtcNow;
            }

            if (complete)
            {
                AIProcedures.EquipDefeatedPlayer(boss, victim);
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
            return DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());
        }

        private static int ChooseSpell(Player bot, int turnNumber, List<int> spells)
        {
            var activeSpell = (turnNumber / SpellChangeTurnFrequency) % ActiveSpells;
            var offset = (turnNumber + bot.Id) / (SpellChangeTurnFrequency * ActiveSpells * CyclesBeforeSpellSwap);
            var index = (activeSpell + offset) % spells.Count;
            return spells[index];
        }

        private static int GetLevel(int turnNumber)
        {
            return turnNumber / 350 + 8;
        }

        private static int GetCounterAttackTimes(decimal currentHealth, decimal maxHealth, int turnNumber)
        {
            decimal value = currentHealth / maxHealth;
            if (value > .5m)
            {
                return  1;
            }
            else if (value > .1m)
            { 
                return 2;
            }

            return 3;
        }

    }
}

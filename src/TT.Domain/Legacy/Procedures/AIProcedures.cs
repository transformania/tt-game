using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.Utilities;

namespace TT.Domain.Procedures
{
    public static class AIProcedures
    {

        private const string Psycho1MName = "botform_psychopathic_spellslinger_male";
        private const string Psycho1FName = "botform_psychopathic_spellslinger_female";
        private const string Psycho3MName = "botform_psychopathic_spellslinger_male_3";
        private const string Psycho3FName = "botform_psychopathic_spellslinger_female_3";
        private const string Psycho5MName = "botform_psychopathic_spellslinger_male_5";
        private const string Psycho5FName = "botform_psychopathic_spellslinger_female_5";
        private const string Psycho7MName = "botform_psychopathic_spellslinger_male_7";
        private const string Psycho7FName = "botform_psychopathic_spellslinger_male_7";
        private const string Psycho9MName = "botform_psychopathic_spellslinger_male_9";
        private const string Psycho9FName = "botform_psychopathic_spellslinger_male_9";

        private const int Psycho1MId = 13;
        private const int Psycho1FId = 14;
        private const int Psycho3MId = 837;
        private const int Psycho3FId = 838;
        private const int Psycho5MId = 839;
        private const int Psycho5FId = 840;
        private const int Psycho7MId = 841;
        private const int Psycho7FId = 842;
        private const int Psycho9MId = 843;
        private const int Psycho9FId = 844;

        private static Tuple<int,string> GetPsychoFormFromLevelAndSex(int level, string sex)
        {
            if (level == 1)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho1MId, Psycho1MName) : new Tuple<int, string>(Psycho1FId, Psycho1FName);
            }
            else if (level == 3)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho3MId, Psycho3MName) : new Tuple<int, string>(Psycho3FId, Psycho3FName);
            }
            else if (level == 5)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho5MId, Psycho5MName) : new Tuple<int, string>(Psycho5FId, Psycho5FName);
            }
            else if (level == 7)
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho7MId, Psycho7MName) : new Tuple<int, string>(Psycho7FId, Psycho7FName);
            }
            else
            {
                return sex == PvPStatics.GenderMale ? new Tuple<int, string>(Psycho9MId, Psycho9MName) : new Tuple<int, string>(Psycho9FId, Psycho9FName);
            }
        }

        public static void SpawnAIPsychopaths(int count)
        {

            // load up the random names XML
            var lastNames = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.LastNames.xml");

            var rand = new Random();


            IPlayerRepository playerRepo = new EFPlayerRepository();

            var turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();

            var botCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId);

            for (var i = (0 + botCount); i < (count + botCount); i++)
            {

                var cmd = new CreatePlayer
                {
                    FirstName = "Psychopath",
                    Location = LocationsStatics.GetRandomLocation(),
                    Health = 200,
                    MaxHealth = 200,
                    Mana = 200,
                    MaxMana = 200,
                    BotId = AIStatics.PsychopathBotId,
                    UnusedLevelUpPerks = 0,
                    XP = 0,
                    Money = 100,
                };

                double lastNameMax = lastNames.Count();
                var randLastNameIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * lastNameMax));

                cmd.LastName = lastNames.ElementAt(randLastNameIndex);
                cmd.Gender = i % 2 == 1 ? PvPStatics.GenderMale : PvPStatics.GenderFemale;
                cmd.OriginalForm = cmd.Form;

                var strength = GetPsychopathLevel(turnNumber);

                if (strength == 1)
                {
                    cmd.Level = 1;
                }
                else if (strength == 3)
                {
                    cmd.FirstName = "Fierce " + cmd.FirstName;
                    cmd.Level = 3;
                }
                else if (strength == 5)
                {
                    cmd.FirstName = "Wrathful " + cmd.FirstName;
                    cmd.Level = 5;
                }
                else if (strength == 7)
                {
                    cmd.FirstName = "Loathful " + cmd.FirstName;
                    cmd.Level = 7;
                }
                else if (strength == 9)
                {
                    cmd.FirstName = "Soulless " + cmd.FirstName;
                    cmd.Level = 9;
                }

                var idAndFormName = GetPsychoFormFromLevelAndSex(cmd.Level, cmd.Gender);
                cmd.FormSourceId = idAndFormName.Item1;
                cmd.Form = idAndFormName.Item2;

                var id = 0;

                // assert this name isn't already taken
                var ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == cmd.FirstName && p.LastName == cmd.LastName);
                if (ghost != null)
                {
                    continue;
                }
                else
                {
                    id = DomainRegistry.Repository.Execute(cmd);
                }

                // give this bot a random skill
                var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();

                double max = eligibleSkills.Count();
                var randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                var skillToLearn = eligibleSkills.ElementAt(randIndex);
                SkillProcedures.GiveSkillToPlayer(id, skillToLearn);

                // give this bot the Psychpathic perk
                if (strength == 1)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic", id);
                }
                else if (strength == 3)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl3", id);
                }
                else if (strength == 5)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl5", id);
                }
                else if (strength == 7)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl7", id);
                }
                else if (strength == 9)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl9", id);
                }

                // give this psycho a new rune with some random chance it is a higher level than they are, to a max of level 13
                var random = new Random(Guid.NewGuid().GetHashCode());
                var roll = random.NextDouble();

                if (roll < .1)
                {
                    strength += 4;
                }
                else if (roll < .3)
                {
                    strength += 2;
                }

                var quantity = Math.Floor(random.NextDouble()*2) + 1; // 1 or 2

                for (var c = 0; c < quantity; c++)
                {
                    var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = strength, Random = random});
                    DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = id });
                }

                var psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
                playerRepo.SavePlayer(psychoEF);

            }
        }

        public static List<Exception> RunPsychopathActions()
        {

            var errors = new List<Exception>();

            IPlayerRepository playerRepo = new EFPlayerRepository();


            //spawn in more bots if there are less than the default
            var botCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull);
            if (botCount < PvPStatics.PsychopathDefaultAmount)
            {
                SpawnAIPsychopaths(PvPStatics.PsychopathDefaultAmount - botCount);
            }

            var bots = playerRepo.Players.Where(p => p.BotId == AIStatics.PsychopathBotId && p.Mobility == PvPStatics.MobilityFull).ToList();

            foreach (var bot in bots)
            {

                try
                {

                    // if bot is no longer fully animate or is null, skip them
                    if (bot == null || bot.Mobility != PvPStatics.MobilityFull)
                    {
                        continue;
                    }

                    #region drop excess items

                    var botItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = bot.Id }).ToList();

                    string[] itemTypes =
                    {
                        PvPStatics.ItemType_Hat, PvPStatics.ItemType_Accessory, PvPStatics.ItemType_Pants,
                        PvPStatics.ItemType_Pet, PvPStatics.ItemType_Shirt, PvPStatics.ItemType_Shoes,
                        PvPStatics.ItemType_Underpants, PvPStatics.ItemType_Undershirt
                    };

                    foreach (var typeToDrop in itemTypes)
                    {
                        if (botItems.Count(i => i.ItemSource.ItemType == typeToDrop) > 1)
                        {
                            var dropList = botItems.Where(i => i.ItemSource.ItemType == typeToDrop).Skip(1).ToList();

                            foreach (var i in dropList)
                            {
                                ItemProcedures.DropItem(i.Id);

                                if (i.ItemSource.ItemType == PvPStatics.ItemType_Pet)
                                {
                                    LocationLogProcedures.AddLocationLog(bot.dbLocationName,
                                        "<b>" + bot.GetFullName() + "</b> released <b>" + i.FormerPlayer.FullName +
                                        "</b> the pet <b>" + i.ItemSource.FriendlyName + "</b> here.");
                                }
                                else
                                {
                                    LocationLogProcedures.AddLocationLog(bot.dbLocationName,
                                        "<b>" + bot.GetFullName() + "</b> dropped <b>" + i.FormerPlayer.FullName +
                                        "</b> the <b>" + i.ItemSource.FriendlyName + "</b> here.");
                                }
                            }
                        }
                    }

                    #endregion

                    var botbuffs = ItemProcedures.GetPlayerBuffs(bot);

                    var meditates = 0;

                    // meditate if needed
                    if (bot.Mana < bot.MaxMana * .5M)
                    {
                        var manarand = new Random(DateTime.Now.Millisecond);
                        var manaroll = (int) Math.Floor(manarand.NextDouble() * 4.0D);
                        for (var i = 0; i < manaroll; i++)
                        {
                            DomainRegistry.Repository.Execute(new Meditate
                            {
                                PlayerId = bot.Id,
                                Buffs = botbuffs,
                                NoValidate = true
                            });
                            meditates++;
                        }
                    }

                    // cleanse if needed, less if psycho has cleansed lately
                    if (bot.Health < bot.MaxHealth * .5M)
                    {
                        var healthrand = new Random(DateTime.Now.Millisecond);
                        var healthroll = (int) Math.Floor(healthrand.NextDouble() * 4.0D);
                        for (var i = meditates; i < healthroll; i++)
                        {
                            DomainRegistry.Repository.Execute(new Cleanse
                            {
                                PlayerId = bot.Id,
                                Buffs = botbuffs,
                                NoValidate = true
                            });
                        }
                    }


                    var directive = AIDirectiveProcedures.GetAIDirective(bot.Id);
                    var skill = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id).FirstOrDefault(s =>
                        s.dbSkill.Name != "lowerHealth" && s.Skill.ExclusiveToForm == null &&
                        s.Skill.ExclusiveToItem == null);

                    // the bot has an attack target, so go chase it
                    if (directive.State == "attack")
                    {
                        var myTarget = PlayerProcedures.GetPlayer(directive.TargetPlayerId);

                        // if the target is offline, no longer animate, in the dungeon, or in the same form as the spells' target, go into idle mode
                        if (PlayerProcedures.PlayerIsOffline(myTarget) ||
                            myTarget.Mobility != PvPStatics.MobilityFull ||
                            skill == null ||
                            myTarget.Form == skill.Skill.FormdbName ||
                            myTarget.IsInDungeon() ||
                            myTarget.InDuel > 0 ||
                            myTarget.InQuest > 0)
                        {
                            AIDirectiveProcedures.SetAIDirective_Idle(bot.Id);
                        }

                        // the target is okay for attacking
                        else
                        {
                            // the bot must move to its target location.
                            if (myTarget.dbLocationName != bot.dbLocationName)
                            {
                                if (botbuffs.MoveActionPointDiscount() > -100)
                                {
                                    var newplace = MoveTo(bot, myTarget.dbLocationName, 4);
                                    bot.dbLocationName = newplace;
                                }
                            }

                            // if the bot is now in the same place as the target, attack away, so long as the target is online and animate
                            if (bot.dbLocationName == myTarget.dbLocationName &&
                                !PlayerProcedures.PlayerIsOffline(myTarget) &&
                                myTarget.Mobility == PvPStatics.MobilityFull)
                            {
                                playerRepo.SavePlayer(bot);
                                if (bot.Mana >= 7)
                                {
                                    AttackProcedures.Attack(bot, myTarget, skill);
                                }

                                if (bot.Mana >= 14)
                                {
                                    AttackProcedures.Attack(bot, myTarget, skill);
                                }

                                if (bot.Mana >= 21)
                                {
                                    AttackProcedures.Attack(bot, myTarget, skill);
                                }

                            }
                        }

                    }

                    // the bot has no target, so wander and try to find new targets and attack them.
                    else
                    {
                        if (botbuffs.MoveActionPointDiscount() > -100)
                        {
                            var newplace = MoveTo(bot, LocationsStatics.GetRandomLocation(), 4);
                            bot.dbLocationName = newplace;
                        }


                        // attack stage
                        var playersHere = playerRepo.Players.Where(p =>
                                p.dbLocationName == bot.dbLocationName && p.Mobility == PvPStatics.MobilityFull &&
                                p.Id != bot.Id && p.BotId == AIStatics.PsychopathBotId && p.Level >= bot.Level)
                            .ToList(); // don't attack the merchant

                        // filter out offline players and Lindella
                        var onlinePlayersHere = playersHere.Where(p => !PlayerProcedures.PlayerIsOffline(p)).ToList();

                        if (onlinePlayersHere.Any())
                        {
                            var rand = new Random(DateTime.Now.Millisecond);
                            var roll = Math.Floor(rand.NextDouble() * onlinePlayersHere.Count());
                            var victim = onlinePlayersHere.ElementAt((int) roll);
                            AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, victim.Id);
                            playerRepo.SavePlayer(bot);
                            AttackProcedures.Attack(bot, victim, skill);
                            AttackProcedures.Attack(bot, victim, skill);
                            AttackProcedures.Attack(bot, victim, skill);
                        }


                    }

                    playerRepo.SavePlayer(bot);

                }
                catch (Exception e)
                {
                    errors.Add(e);
                }
            }

            return errors;

        }

        public static void CounterAttack(Player personAttackin, Player bot)
        {

            var myskills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id);

            var rand = new Random(personAttackin.LastName.GetHashCode());
            var roll = Math.Floor(rand.NextDouble() * (double)myskills.Count());


            var selectedSkill = myskills.ElementAt((int)roll);

            if (personAttackin.BotId == AIStatics.ActivePlayerBotId) {
                AttackProcedures.Attack(bot, personAttackin, selectedSkill);
            }
        }

        public static void CheckAICounterattackRoutine(Player personAttacking, Player bot)
        {
            // person attacking is a boss and not a psychopath, so do nothing
            if (personAttacking.BotId < AIStatics.PsychopathBotId)
            {
                return;
            }

            // attacking the psychopath.  Random chance the psychopath will set the attacker as their target.
            if (bot.BotId == AIStatics.PsychopathBotId)
            {

                if (bot.FirstName.Contains("Loathful "))
                {
                    AIProcedures.CounterAttack(personAttacking, bot);
                }
                else if (bot.FirstName.Contains("Soulless "))
                {
                    AIProcedures.CounterAttack(personAttacking, bot);
                    AIProcedures.CounterAttack(personAttacking, bot);
                }

                var directive = AIDirectiveProcedures.GetAIDirective(bot.Id);

                // no previous target, so set this player as the new one 
                if (directive.TargetPlayerId == -1 || directive.State == "idle")
                {
                    AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, personAttacking.Id);
                }

                // random chance to see if the attacker becomes the new target
                else
                {
                    var rand = new Random();
                    var roll = rand.NextDouble();
                    if (roll < .08)
                    {
                        AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, personAttacking.Id);
                    }
                }

            }

            // if the target is the merchant, run the counterattack procedure
            if (bot.BotId == AIStatics.LindellaBotId)
            {
                AIProcedures.CounterAttack(personAttacking, bot);
            }

            // if the target is Donna, counterattack and set that player as her target immediately
            if (bot.BotId == AIStatics.DonnaBotId)
            {
                BossProcedures_Donna.DonnaCounterattack(personAttacking, bot);
            }

            // Valentine counterattack
            if (bot.BotId == AIStatics.ValentineBotId)
            {
                BossProcedures_Valentine.CounterAttack(personAttacking, bot);
            }

            // Bimbo boss counterattack
            else if (bot.BotId == AIStatics.BimboBossBotId)
            {
                BossProcedures_BimboBoss.CounterAttack(personAttacking, bot);
            }

            // rat thieves counterattack
            else if (bot.BotId == AIStatics.MaleRatBotId || bot.BotId == AIStatics.FemaleRatBotId)
            {
                AIProcedures.DealBossDamage(bot, personAttacking, true, 1);
                BossProcedures_Thieves.CounterAttack(personAttacking);
            }

            // fae boss counterattack
            else if (bot.BotId == AIStatics.FaebossId)
            {
                AIProcedures.DealBossDamage(bot, personAttacking, true, 1);
                BossProcedures_FaeBoss.CounterAttack(personAttacking);
            }

            // Wuffie counterattack
            else if (bot.BotId == AIStatics.WuffieBotId)
            {
                BossProcedures_PetMerchant.CounterAttack(personAttacking);
            }

            // mouse sisters counterattack
            else if (bot.BotId == AIStatics.MouseNerdBotId || bot.BotId == AIStatics.MouseBimboBotId)
            {
                BossProcedures_Sisters.CounterAttack(personAttacking, bot);
            }

            // demon counterattack
            else if (bot.BotId == AIStatics.DemonBotId)
            {
                BossProcedures_DungeonDemon.CounterAttack(bot, personAttacking);
            }


        }

        public static string MoveTo(Player bot, string locationDbName, int distance)
        {
            var start = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == bot.dbLocationName);
            var end = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locationDbName);

            if (bot.dbLocationName == locationDbName)
            {
                return bot.dbLocationName;
            }

            var path = PathfindingProcedures.GetMovementPath(start, end);

            var pathTiles = path.Split(';');

            pathTiles = pathTiles.Take(pathTiles.Length - 1).ToArray();

            var xDistance = distance;
            if (pathTiles.Length < distance)
            {
                xDistance = pathTiles.Length - 1;
            }

            var output = "";

            // the first movement, from current to 2nd place, is not in the array... write it manually
            if (pathTiles.Length > 0)
            {
                var enteringMessage = bot.FirstName + " " + bot.LastName + " entered from " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == start.dbName).Name;
                var leavingMessage = bot.FirstName + " " + bot.LastName + " left toward " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[0]).Name;
                LocationLogProcedures.AddLocationLog(start.dbName, leavingMessage);
                LocationLogProcedures.AddLocationLog(pathTiles[0], enteringMessage);
                output += leavingMessage + "<br>";
                output += "----<br>";

                for (var i = 0; i < xDistance; i++)
                {
                    try
                    {
                        enteringMessage = bot.FirstName + " " + bot.LastName + " entered from " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[i]).Name;
                        leavingMessage = bot.FirstName + " " + bot.LastName + " left toward " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[i + 1]).Name;
                        LocationLogProcedures.AddLocationLog(pathTiles[i], leavingMessage);
                        LocationLogProcedures.AddLocationLog(pathTiles[i + 1], enteringMessage);

                        output += leavingMessage + "<br>";
                        output += enteringMessage + "<br>";
                        output += "----<br>";
                    }
                    catch
                    {

                    }

                }

            }

            try
            {
                return pathTiles[xDistance];
            }
            catch
            {
                return pathTiles[xDistance - 1];
            }

            //  output += "AT:  " + pathTiles[xDistance];

            //  return output;

        }

        public static int GetPsychopathLevel(int turnNumber)
        {

            // 1 = regular
            // 3 = fierce
            // 5 = wrathful
            // 7 = loathful
            // 9 = soulless
            
            // regular psychopath
            if (turnNumber < 300) {
                return 1;
            }

            var rand = new Random(Guid.NewGuid().GetHashCode());
            var roll = rand.NextDouble();

            if (turnNumber >= 300 && turnNumber < 600)
            {
                // 15% chance to roll a fierce (lvl 3)
                if (roll < .15D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 600 && turnNumber < 900)
            {
                if (roll < .04D)
                {
                    return 5;
                }
                else if (roll < .15D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 900 && turnNumber < 1500)
            {
                if (roll < .03D)
                {
                    return 7;
                }
                else if (roll < .1D)
                {
                    return 5;
                }
                else if (roll < .25D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }
            else if (turnNumber >= 1500)
            {
                if (roll < .015D)
                {
                    return 9;
                }
                else if (roll < .03D)
                {
                    return 7;
                }
                else if (roll < .13D)
                {
                    return 5;
                }
                else if (roll < .35D)
                {
                    return 3;
                }
                else
                {
                    return 1;
                }
            }

            return 1;

        }

        

        public static void DealBossDamage(Player boss, Player attacker, bool humanAttacker, int attackCount)
        {
            IBossDamageRepository repo = new EFBossDamageRepository();

            var damage = repo.BossDamages.FirstOrDefault(bf => bf.PlayerId == attacker.Id && bf.BossBotId == boss.BotId);

            if (damage == null)
            {
                damage = new BossDamage
                {
                    PlayerId = attacker.Id,
                    BossBotId = boss.BotId,
                    Timestamp = DateTime.UtcNow,
                };
            }

            if (humanAttacker)
            {
                damage.PlayerAttacksOnBoss += attackCount;
               
            } else {
                damage.BossAttacksOnPlayer += attackCount;
            }

            // calculate a unique score to add, weighted a little in favor of higher level human attackers / victims
            damage.TotalPoints += (float)attackCount * (.75F + .25F * (float)attacker.Level);
            damage.Timestamp = DateTime.UtcNow;

            repo.SaveBossDamage(damage);

        }

        public static List<BossDamage> GetTopAttackers(int bossBotId, int amount)
        {
            IBossDamageRepository repo = new EFBossDamageRepository();
            return repo.BossDamages.Where(b => b.BossBotId == bossBotId && b.PlayerAttacksOnBoss > 0).OrderByDescending(b => b.TotalPoints).Take(amount).ToList();
        }

        

    }

}

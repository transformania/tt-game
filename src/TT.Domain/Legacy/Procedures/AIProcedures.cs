﻿using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class AIProcedures
    {

        public static void SpawnAIPsychopaths(int count)
        {

            // load up the random names XML
            var lastNames = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.LastNames.xml");

            Random rand = new Random();


            IPlayerRepository playerRepo = new EFPlayerRepository();

            int turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();

            var botCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId);

            for (int i = (0 + botCount); i < (count + botCount); i++)
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
                int randLastNameIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * lastNameMax));

                cmd.LastName = lastNames.ElementAt(randLastNameIndex);

                if (i % 2 == 1)
                {
                    cmd.Form = "botform_psychopathic_spellslinger_male";
                    cmd.FormSourceId = AIStatics.MalePsychoFormId;
                    cmd.Gender = PvPStatics.GenderMale;
                }
                else
                {
                    cmd.Form = "botform_psychopathic_spellslinger_female";
                    cmd.FormSourceId = AIStatics.FemalePsychoFormId;
                    cmd.Gender = PvPStatics.GenderFemale;
                }

                cmd.OriginalForm = cmd.Form;

                int strength = GetPsychopathLevel(turnNumber);

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

                var id = 0;

                // assert this name isn't already taken
                Player ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == cmd.FirstName && p.LastName == cmd.LastName);
                if (ghost != null)
                {
                    continue;
                }
                else
                {
                    id = DomainRegistry.Repository.Execute(cmd);
                }

                // give this bot a random skill
                List<DbStaticSkill> eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();

                double max = eligibleSkills.Count();
                int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                DbStaticSkill skillToLearn = eligibleSkills.ElementAt(randIndex);
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

                Player psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
                playerRepo.SavePlayer(psychoEF);

            }
        }

        public static void RunPsychopathActions()
        {
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
                // if bot is no longer fully animate or is null, skip them
                if (bot == null || bot.Mobility != PvPStatics.MobilityFull)
                {
                    continue;
                }

                #region drop excess items
                List<ItemViewModel> botItems = ItemProcedures.GetAllPlayerItems(bot.Id).ToList();

                string[] itemTypes = { PvPStatics.ItemType_Hat, PvPStatics.ItemType_Accessory, PvPStatics.ItemType_Pants, PvPStatics.ItemType_Pet, PvPStatics.ItemType_Shirt, PvPStatics.ItemType_Shoes, PvPStatics.ItemType_Underpants, PvPStatics.ItemType_Undershirt };

                foreach (string typeToDrop in itemTypes)
                {
                    if (botItems.Count(i => i.Item.ItemType == typeToDrop) > 1)
                    {
                        List<ItemViewModel> dropList = botItems.Where(i => i.Item.ItemType == typeToDrop).Skip(1).ToList();

                        foreach (ItemViewModel i in dropList)
                        {
                            ItemProcedures.DropItem(i.dbItem.Id, bot.dbLocationName);

                            if (i.Item.ItemType == PvPStatics.ItemType_Pet)
                            {
                                LocationLogProcedures.AddLocationLog(bot.dbLocationName, "<b>" + bot.GetFullName() + "</b> released <b>" + i.dbItem.GetFullName() + "</b> the pet <b>" + i.Item.FriendlyName + "</b> here.");
                            }
                            else
                            {
                                LocationLogProcedures.AddLocationLog(bot.dbLocationName, "<b>" + bot.GetFullName() + "</b> dropped <b>" + i.dbItem.GetFullName() + "</b> the <b>" + i.Item.FriendlyName + "</b> here.");
                            }
                        }
                    }
                }
                #endregion

                BuffBox botbuffs = ItemProcedures.GetPlayerBuffs(bot);

                int meditates = 0;

                // meditate if needed
                if (bot.Mana < bot.MaxMana * .5M)
                {
                    Random manarand = new Random(DateTime.Now.Millisecond);
                    int manaroll = (int)Math.Floor(manarand.NextDouble() * 4.0D);
                    for (int i = 0; i < manaroll; i++)
                    {
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = bot.Id, Buffs = botbuffs, NoValidate = true });
                        meditates++;
                    }
                }

                // cleanse if needed, less if psycho has cleansed lately
                if (bot.Health < bot.MaxHealth * .5M)
                {
                    Random healthrand = new Random(DateTime.Now.Millisecond);
                    int healthroll = (int)Math.Floor(healthrand.NextDouble() * 4.0D);
                    for (int i = meditates; i < healthroll; i++)
                    {
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = bot.Id, Buffs = botbuffs, NoValidate = true });
                    }
                }


                AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);
                SkillViewModel skill = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id).FirstOrDefault(s => s.dbSkill.Name != "lowerHealth" && s.Skill.ExclusiveToForm == null && s.Skill.ExclusiveToItem == null);

                // the bot has an attack target, so go chase it
                if (directive.State == "attack")
                {
                    Player myTarget = PlayerProcedures.GetPlayer(directive.TargetPlayerId);

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
                                string newplace = MoveTo(bot, myTarget.dbLocationName, 4);
                                bot.dbLocationName = newplace;
                            }
                        }

                        // if the bot is now in the same place as the target, attack away, so long as the target is online and animate
                        if (bot.dbLocationName == myTarget.dbLocationName && !PlayerProcedures.PlayerIsOffline(myTarget) && myTarget.Mobility == PvPStatics.MobilityFull)
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
                        string newplace = MoveTo(bot, LocationsStatics.GetRandomLocation(), 4);
                        bot.dbLocationName = newplace;
                    }


                    // attack stage
                    var playersHere = playerRepo.Players.Where(p => p.dbLocationName == bot.dbLocationName && p.Mobility == PvPStatics.MobilityFull && p.Id != bot.Id && p.BotId == AIStatics.PsychopathBotId && p.Level >= bot.Level).ToList(); // don't attack the merchant

                    // filter out offline players and Lindella
                    var onlinePlayersHere = playersHere.Where(p => !PlayerProcedures.PlayerIsOffline(p)).ToList();

                    if (onlinePlayersHere.Any())
                    {
                        var rand = new Random(DateTime.Now.Millisecond);
                        var roll = Math.Floor(rand.NextDouble() * onlinePlayersHere.Count());
                        Player victim = onlinePlayersHere.ElementAt((int)roll);
                        AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, victim.Id);
                        playerRepo.SavePlayer(bot);
                        AttackProcedures.Attack(bot, victim, skill);
                        AttackProcedures.Attack(bot, victim, skill);
                        AttackProcedures.Attack(bot, victim, skill);
                    }


                }

                playerRepo.SavePlayer(bot);
            }


        }

       

        

        public static void CounterAttack(Player personAttackin, Player bot)
        {

            IEnumerable<SkillViewModel> myskills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id);

            Random rand = new Random(personAttackin.LastName.GetHashCode());
            double roll = Math.Floor(rand.NextDouble() * (double)myskills.Count());


            SkillViewModel selectedSkill = myskills.ElementAt((int)roll);

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

                AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);

                // no previous target, so set this player as the new one 
                if (directive.TargetPlayerId == -1 || directive.State == "idle")
                {
                    AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, personAttacking.Id);
                }

                // random chance to see if the attacker becomes the new target
                else
                {
                    Random rand = new Random();
                    double roll = rand.NextDouble();
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
            Location start = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == bot.dbLocationName);
            Location end = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == locationDbName);

            if (bot.dbLocationName == locationDbName)
            {
                return bot.dbLocationName;
            }

            string path = PathfindingProcedures.GetMovementPath(start, end);

            String[] pathTiles = path.Split(';');

            pathTiles = pathTiles.Take(pathTiles.Length - 1).ToArray();

            int xDistance = distance;
            if (pathTiles.Length < distance)
            {
                xDistance = pathTiles.Length - 1;
            }

            string output = "";

            // the first movement, from current to 2nd place, is not in the array... write it manually
            if (pathTiles.Length > 0)
            {
                string enteringMessage = bot.FirstName + " " + bot.LastName + " entered from " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == start.dbName).Name;
                string leavingMessage = bot.FirstName + " " + bot.LastName + " left toward " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[0]).Name;
                LocationLogProcedures.AddLocationLog(start.dbName, leavingMessage);
                LocationLogProcedures.AddLocationLog(pathTiles[0], enteringMessage);
                output += leavingMessage + "<br>";
                output += "----<br>";

                for (int i = 0; i < xDistance; i++)
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

            Random rand = new Random(Guid.NewGuid().GetHashCode());
            double roll = rand.NextDouble();

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

            BossDamage damage = repo.BossDamages.FirstOrDefault(bf => bf.PlayerId == attacker.Id && bf.BossBotId == boss.BotId);

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Procedures.BossProcedures;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures
{
    public static class AIProcedures
    {
        public const int PsychopathMembershipId = -2;
        public const int LindellaMembershipId = -3;
        public const int WuffieMembershipId = -10;
        public const int JewdewfaeMembershipId = -6;
        public const int BartenderMembershipId = -14;

        public const int MouseNerdMembershipId = -11;
        public const int MouseBimboMembershipId = -12;

        public const int LoremasterMembershipId = -15;

        // Bot ID code:
        //  0 (active player)
        // -1 (player has rerolled, player is abandoned)
        // -2 (psychopath spellslinger)
        // -3 item merchant
        // -4 Donna Milton
        // -5 Lord Valentine
        // -6 Jewdewfae
        // -7 Bimbo Boss
        // -8 Male rat thief
        // -9 Female rat thief
        // -10 pet merchant
        // -11 Nerd mouse sister
        // -12 bimbo mouse sister
        // -13 dungeon demon
        // -14 bartender
        // -15 loremaster

        public static void SpawnAIPsychopaths(int count, int offset)
        {

            // load up the random names XML
            string path = HttpContext.Current.Server.MapPath("~/XMLs/LastNames.xml");
            List<string> names = new List<string>();

            var serializer = new XmlSerializer(typeof(List<string>));
            using (var reader = XmlReader.Create(path))
            {
                names = (List<string>)serializer.Deserialize(reader);
            }

            Random rand = new Random();


            IPlayerRepository playerRepo = new EFPlayerRepository();

            int turnNumber = PvPWorldStatProcedures.GetWorldTurnNumber();

            int botCount = playerRepo.Players.Where(b => b.BotId == -2).Count();

            for (int i = (0 + botCount); i < (count + botCount); i++)
            {

                Player bot = new Player();


                bot.FirstName = "Psychopath";

                double lastNameMax = names.Count();
                int randLastNameIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * lastNameMax));

                bot.LastName = names.ElementAt(randLastNameIndex);

                bot.ActionPoints = 120;
                bot.dbLocationName = LocationsStatics.GetRandomLocation();
                bot.FlaggedForAbuse = false;
                bot.Level = 1;
                bot.Health = 200;
                bot.MaxHealth = 200;
                bot.Mana = 200;
                bot.MaxMana = 200;
                bot.MembershipId = "-2";
                bot.BotId = -2;
                bot.Mobility = "full";
                //bot.IsPetToId = -1;
                bot.UnusedLevelUpPerks = 0;
                bot.XP = 0;
                bot.LastActionTimestamp = DateTime.UtcNow;
                bot.OnlineActivityTimestamp = DateTime.UtcNow;
                bot.Money = 100;
                bot.LastCombatTimestamp = DateTime.UtcNow;
                bot.LastCombatAttackedTimestamp = DateTime.UtcNow;
                bot.CleansesMeditatesThisRound = 0;
                bot.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;

                if (i % 2 == 1)
                {
                    bot.Form = "botform_psychopathic_spellslinger_male";
                    bot.Gender = "male";
                }
                else
                {
                    bot.Form = "botform_psychopathic_spellslinger_female";
                    bot.Gender = "female";
                }

                bot.OriginalForm = bot.Form;

                int strength = GetPsychopathLevel(turnNumber);

                if (strength == 1)
                {
                    bot.Level = 1;
                }
                else if (strength == 3)
                {
                    bot.FirstName = "Fierce " + bot.FirstName;
                    bot.Level = 3;
                }
                else if (strength == 5)
                {
                    bot.FirstName = "Wrathful " + bot.FirstName;
                    bot.Level = 5;
                }
                else if (strength == 7)
                {
                    bot.FirstName = "Loathful " + bot.FirstName;
                    bot.Level = 7;
                }
                else if (strength == 9)
                {
                    bot.FirstName = "Soulless " + bot.FirstName;
                    bot.Level = 9;
                }
                
                // assert this name isn't already taken
                Player ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == bot.FirstName && p.LastName == bot.LastName);
                if (ghost != null)
                {
                    continue;
                }
                else
                {
                    playerRepo.SavePlayer(bot);
                }

                // give this bot a random skill
                List<DbStaticSkill> eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();

                double max = eligibleSkills.Count();
                int randIndex = Convert.ToInt32(Math.Floor(rand.NextDouble() * max));

                DbStaticSkill skillToLearn = eligibleSkills.ElementAt(randIndex);
                string output = SkillProcedures.GiveSkillToPlayer(bot.Id, skillToLearn);

                // give this bot the Psychpathic perk
                if (strength == 1)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic", bot);
                }
                else if (strength == 3)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl3", bot);
                }
                else if (strength == 5)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl5", bot);
                }
                else if (strength == 7)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl7", bot);
                }
                else if (strength == 9)
                {
                    EffectProcedures.GivePerkToPlayer("bot_psychopathic_lvl9", bot);
                }

            }
        }

        public static void RunPsychopathActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IServerLogRepository serverLogRepo = new EFServerLogRepository();

            int worldTurnNumber = PvPWorldStatProcedures.GetWorldTurnNumber() - 1;
            ServerLog log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == worldTurnNumber);

            //spawn in more bots if there are less than the default
            int botCount = playerRepo.Players.Where(b => b.BotId == -2 && b.Mobility == "full").Count();
            if (botCount < PvPStatics.PsychopathDefaultAmount)
            {
                AIProcedures.SpawnAIPsychopaths(PvPStatics.PsychopathDefaultAmount - botCount, 0);
                log.AddLog("Spawned a new psychopath.");
            }

            List<int> botIds = playerRepo.Players.Where(p => p.BotId == -2).Where(b => b.Mobility == "full").Select(b => b.Id).ToList();

            foreach (int botId in botIds)
            {

                try
                {

                    Player bot = playerRepo.Players.FirstOrDefault(p => p.Id == botId);

                    // if bot is no longer fully animate or is null, skip them
                    if (bot == null || bot.Mobility != "full")
                    {
                        continue;
                    }

                    #region drop excess items
                    List<ItemViewModel> botItems = ItemProcedures.GetAllPlayerItems(bot.Id).ToList();

                    string[] itemTypes = { PvPStatics.ItemType_Hat, PvPStatics.ItemType_Accessory, PvPStatics.ItemType_Pants, PvPStatics.ItemType_Pet, PvPStatics.ItemType_Shirt, PvPStatics.ItemType_Shoes, PvPStatics.ItemType_Underpants, PvPStatics.ItemType_Undershirt };

                    foreach (string typeToDrop in itemTypes)
                    {
                        if (botItems.Where(i => i.Item.ItemType == typeToDrop).Count() > 1)
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

                    BuffBox botbuffs = ItemProcedures.GetPlayerBuffsSQL(bot);

                    // meditate if needed
                    if (bot.Mana < bot.MaxMana * .75M)
                    {
                        Random manarand = new Random(DateTime.Now.Millisecond);
                        int manaroll = (int)Math.Floor(manarand.NextDouble() * 4.0D);
                        for (int i = 0; i < manaroll; i++)
                        {
                            PlayerProcedures.Meditate(bot, botbuffs);
                        }
                    }

                    // cleanse if needed
                    if (bot.Health < bot.MaxHealth * .5M)
                    {
                        Random healthrand = new Random(DateTime.Now.Millisecond);
                        int healthroll = (int)Math.Floor(healthrand.NextDouble() * 4.0D);
                        for (int i = 0; i < healthroll; i++)
                        {
                            PlayerProcedures.Cleanse(bot, botbuffs);
                        }
                    }

                   


                    AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);
                    log.AddLog(bot.FirstName + " " + bot.LastName + ":  Directive is to:  " + directive.State + " target ID: " + directive.TargetPlayerId);
                    SkillViewModel2 skill = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id).FirstOrDefault(s => s.dbSkill.Name != "lowerHealth" && s.Skill.ExclusiveToForm == null && s.Skill.ExclusiveToItem == null);

                    Random rand = new Random(DateTime.Now.Millisecond);
                    double roll = 0;

                    // the bot has an attack target, so go chase it
                    if (directive.State == "attack")
                    {
                        Player myTarget = PlayerProcedures.GetPlayer(directive.TargetPlayerId);

                        // if the target is offline, no longer animate, in the dungeon, or in the same form as the spells' target, go into idle mode
                        if (PlayerProcedures.PlayerIsOffline(myTarget) || 
                            myTarget.Mobility != "full" ||
                            myTarget.Form == skill.Skill.FormdbName || 
                            myTarget.IsInDungeon() == true ||
                            myTarget.InDuel > 0)
                        {
                            AIDirectiveProcedures.SetAIDirective_Idle(bot.Id);
                            log.AddLog(bot.FirstName + " " + bot.LastName + ":  target is invalid.  Idling.");
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
                                    log.AddLog(bot.FirstName + " " + bot.LastName + ":  moved to " + newplace + " to chase target.");
                                }
                            }

                            // if the bot is now in the same place as the target, attack away, so long as the target is online and animate
                            if (bot.dbLocationName == myTarget.dbLocationName && !PlayerProcedures.PlayerIsOffline(myTarget) && myTarget.Mobility == "full")
                            {
                                playerRepo.SavePlayer(bot);
                                AttackProcedures.Attack(bot, myTarget, skill);
                                AttackProcedures.Attack(bot, myTarget, skill);
                                AttackProcedures.Attack(bot, myTarget, skill);
                                log.AddLog(bot.FirstName + " " + bot.LastName + ":  attacked target " + myTarget.FirstName + " " + myTarget.LastName);
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
                            log.AddLog(bot.FirstName + " " + bot.LastName + ":  moved to " + newplace + " to search for a target.");
                        }


                        // attack stage
                        List<Player> playersHere = playerRepo.Players.Where(p => p.dbLocationName == bot.dbLocationName)
                            .Where(p => p.Mobility == "full").ToList(); // don't attack the merchant

                        // filter out offline players and Lindella
                        List<Player> onlinePlayersHere = new List<Player>();

                        foreach (Player p in playersHere)
                        {
                            if (!PlayerProcedures.PlayerIsOffline(p) && p.Id != bot.Id && p.Mobility == "full" && p.BotId == -2 && p.Level >= bot.Level)
                            {
                                onlinePlayersHere.Add(p);
                            }
                        }

                        playersHere = playersHere.Where(p => p.Health >= .75M * p.MaxHealth).ToList();

                        if (onlinePlayersHere.Count() > 0)
                        {
                            rand = new Random(DateTime.Now.Millisecond);
                            roll = Math.Floor(rand.NextDouble() * onlinePlayersHere.Count());
                            Player victim = onlinePlayersHere.ElementAt((int)roll);
                            AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, victim.Id);
                            playerRepo.SavePlayer(bot);
                            log.AddLog(bot.FirstName + " " + bot.LastName + ":  Picked target and began attacking " + victim.FirstName + " " + victim.LastName);
                            AttackProcedures.Attack(bot, victim, skill);
                            AttackProcedures.Attack(bot, victim, skill);
                            AttackProcedures.Attack(bot, victim, skill);
                        }


                    }

                    playerRepo.SavePlayer(bot);

                }
                catch
                {

                }

                serverLogRepo.SaveServerLog(log);

            }


        }

        public static void SpawnLindella()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player merchant = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lindella" && f.LastName == "the Soul Vendor" && f.Mobility == "full");

            if (merchant == null)
            {
                merchant = new Player();
                merchant.MembershipId = "-3";
                merchant.BotId = -3;
                merchant.Level = 5;
                merchant.FirstName = "Lindella";
                merchant.LastName = "the Soul Vendor";
                merchant.Health = 5000;
                merchant.Mana = 5000;
                merchant.MaxHealth = 500;
                merchant.MaxMana = 500;
                merchant.Mobility = "full";
                merchant.Money = 1000;
                merchant.TimesAttackingThisUpdate = 0;
                merchant.UnusedLevelUpPerks = 0;
                merchant.LastActionTimestamp = DateTime.UtcNow;
                merchant.LastCombatTimestamp = DateTime.UtcNow;
                merchant.LastCombatAttackedTimestamp = DateTime.UtcNow;
                merchant.OnlineActivityTimestamp = DateTime.UtcNow;
                merchant.Form = "form_Soul_Item_Vendor_Judoo";
                merchant.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;
                merchant.dbLocationName = "270_west_9th_ave"; // Lindella starts her rounds here
                merchant.Gender = "female";
               // merchant.IsItemId = -1;
               // merchant.IsPetToId = -1;
                merchant.ActionPoints = 120;

                playerRepo.SavePlayer(merchant);

                merchant = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lindella" && f.LastName == "the Soul Vendor" && f.Mobility == "full");

                Player Lindella = playerRepo.Players.FirstOrDefault(f => f.BotId == -3);

                AIDirectiveProcedures.GetAIDirective(Lindella.Id);
                AIDirectiveProcedures.SetAIDirective_MoveTo(Lindella.Id, "street_15th_south");



            }
        }

        public static void RunAIMerchantActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player merchant = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lindella" && f.LastName == "the Soul Vendor" && f.Mobility == "full");

           

            if (merchant != null && merchant.Mobility == "full")
            {

                string oldLocation = merchant.dbLocationName;

                merchant.Form = "form_Soul_Item_Vendor_Judoo";
                merchant.Level = 5;

                AIDirective directive = AIDirectiveProcedures.GetAIDirective(merchant.Id);

                if (directive.TargetLocation != merchant.dbLocationName)
                {
                    string newplace = MoveTo(merchant, directive.TargetLocation, 5);
                    merchant.dbLocationName = newplace;
                }

                // if the merchant has arrived, set a new target for next time.
                if (directive.TargetLocation == merchant.dbLocationName)
                {
                    if (merchant.dbLocationName == "270_west_9th_ave")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_15th_south");
                    }
                    else if (merchant.dbLocationName == "street_15th_south")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_220_sunnyglade_drive");
                    }
                    else if (merchant.dbLocationName == "street_220_sunnyglade_drive")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_70e9th");
                    }
                    else if (merchant.dbLocationName == "street_70e9th")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_130_main");
                    }
                    else if (merchant.dbLocationName == "street_130_main")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "270_west_9th_ave");
                    }
                }

                playerRepo.SavePlayer(merchant);
                BuffBox box = ItemProcedures.GetPlayerBuffsSQL(merchant);

                if ((merchant.Health / merchant.MaxHealth) < .75M)
                {
                    if (merchant.Health < merchant.MaxHealth)
                    {
                        PlayerProcedures.Cleanse(merchant, box);
                        PlayerProcedures.Cleanse(merchant, box);
                        PlayerProcedures.Meditate(merchant, box);
                    }
                }
                else
                {
                    if (merchant.Mana < merchant.MaxMana)
                    {
                        PlayerProcedures.Meditate(merchant, box);
                        PlayerProcedures.Meditate(merchant, box);
                        PlayerProcedures.Cleanse(merchant, box);
                    }
                }

#region restock inventory
                if (turnNumber % 16 == 1)
                {

                    //Player lorekeeper = PlayerProcedures.GetPlayerFromBotId(AIProcedures.LoremasterMembershipId);

                    IItemRepository itemRepo = new EFItemRepository();
                    List<Item> lindellasItems = itemRepo.Items.Where(i => i.OwnerId == merchant.Id && i.Level == 0).ToList();
                  //  List<Item> lorekeeperItems = itemRepo.Items.Where(i => i.OwnerId == lorekeeper.Id && i.Level == 0).ToList();

                    string path = HttpContext.Current.Server.MapPath("~/XMLs/RestockList.xml");
                    List<RestockListItem> restockItems = new List<RestockListItem>();

                    var serializer = new XmlSerializer(typeof(List<RestockListItem>));
                    using (var reader = XmlReader.Create(path))
                    {
                        restockItems = (List<RestockListItem>)serializer.Deserialize(reader);
                    }

                    foreach (RestockListItem item in restockItems)
                    {

                        if (item.Merchant == "Lindella")
                        {
                            int currentCount = lindellasItems.Where(i => i.dbName == item.dbName).Count();
                            if (currentCount < item.AmountBeforeRestock)
                            {
                                for (int x = 0; x < item.AmountToRestockTo - currentCount; x++)
                                {
                                    Item newItem = new Item
                                {
                                    dbName = item.dbName,
                                    dbLocationName = "",
                                    OwnerId = merchant.Id,
                                    IsEquipped = false,
                                    IsPermanent = true,
                                    Level = 0,
                                    PvPEnabled = -1,
                                    TimeDropped = DateTime.UtcNow,
                                    TurnsUntilUse = 0,
                                    VictimName = "",
                                    EquippedThisTurn = false
                                };
                                    itemRepo.SaveItem(newItem);
                                }

                            }
                        }

                        // TODO:  Uncomment this out when lorekeeper goes live
                        //else if (item.Merchant == "Lorekeeper")
                        //{
                        //    int currentCount = lorekeeperItems.Where(i => i.dbName == item.dbName).Count();
                        //    if (currentCount < item.AmountBeforeRestock)
                        //    {
                        //        for (int x = 0; x < item.AmountToRestockTo - currentCount; x++)
                        //        {
                        //            Item newItem = new Item
                        //            {
                        //                dbName = item.dbName,
                        //                dbLocationName = "",
                        //                OwnerId = lorekeeper.Id,
                        //                IsEquipped = false,
                        //                IsPermanent = true,
                        //                Level = 0,
                        //                PvPEnabled = -1,
                        //                TimeDropped = DateTime.UtcNow,
                        //                TurnsUntilUse = 0,
                        //                VictimName = "",
                        //                EquippedThisTurn = false
                        //            };
                        //            itemRepo.SaveItem(newItem);
                        //        }

                        //    }
                        //}


                    }
                }
#endregion

            }

        }

        

        public static void CounterAttack(Player personAttackin, Player bot)
        {

            IEnumerable<SkillViewModel2> myskills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id);

            Random rand = new Random(personAttackin.LastName.GetHashCode());
            double roll = Math.Floor(rand.NextDouble() * (double)myskills.Count());


            SkillViewModel2 selectedSkill = myskills.ElementAt((int)roll);

            if (personAttackin.BotId == 0) {
                AttackProcedures.Attack(bot, personAttackin, selectedSkill);
            }
        }

        public static void CheckAICounterattackRoutine(Player personAttacking, Player bot)
        {
            // person attacking is a boss and not a psychopath, so do nothing
            if (personAttacking.BotId < -2)
            {
                return;
            }

            // attacking the psychopath.  Random chance the psychopath will set the attacker as their target.
            if (bot.BotId == -2)
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
            if (bot.BotId == -3)
            {
                AIProcedures.CounterAttack(personAttacking, bot);
            }

            // if the target is Donna, counterattack and set that player as her target immediately
            if (bot.BotId == -4)
            {
                BossProcedures_Donna.DonnaCounterattack(personAttacking, bot);
            }

            // Valentine counterattack
            if (bot.BotId == -5)
            {
                BossProcedures_Valentine.CounterAttack(personAttacking, bot);
            }

            // Bimbo boss counterattack
            else if (bot.BotId == -7)
            {
                BossProcedures_BimboBoss.CounterAttack(personAttacking, bot);
            }

            // rat thieves counterattack
            else if (bot.BotId == -8 || bot.BotId == -9)
            {
                AIProcedures.DealBossDamage(bot, personAttacking, true, 1);
                BossProcedures_Thieves.CounterAttack(personAttacking);
            }

            // Wuffie counterattack
            else if (bot.BotId == -10)
            {
                BossProcedures_PetMerchant.CounterAttack(personAttacking);
            }

            // mouse sisters counterattack
            else if (bot.BotId == -11 || bot.BotId == -12)
            {
                BossProcedures_Sisters.CounterAttack(personAttacking, bot);
            }

            // demon counterattack
            else if (bot.BotId == -13)
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

        public static bool IsAntiBossSkill(string skillName, Player target)
        {
            bool isBoss = false;
            if (target.BotId == -5) // Fighting Valentine
            {
                isBoss = true;
            }
            else
            {
                return false;
            }

            bool isBossSkill = false;
            if (skillName == "duel_valentine")
            {
                isBossSkill = true;
            }
            else
            {
                return false;
            }

            if (isBoss == true && isBossSkill == true)
            {
                return true;
            }
            else
            {
                return false;
            }



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

        public static void SpawnBartender()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player bartender = playerRepo.Players.FirstOrDefault(f => f.BotId == AIProcedures.BartenderMembershipId);

            if (bartender == null)
            {
                bartender = new Player()
                {
                    FirstName = "Rusty",
                    LastName = "Steamstein the Automaton Bartender",
                    ActionPoints = 120,
                    dbLocationName = "tavern_counter",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    LastCombatAttackedTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "male",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_The_Perfect_Barman_Judoo",
                    //IsPetToId = -1,
                    Money = 0,
                    Mobility = "full",
                    Level = 15,
                    MembershipId = "-14",
                    BotId = -14,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(bartender);

            }
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

            if (humanAttacker == true)
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

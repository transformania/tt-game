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


        private const string LindellaItem_WPRoot_DbName = "item_consumeable_willflower_root_sparse";
        private const int LindellaItem_WPRoot_Count = 8;

        private const string LindellaItem_ManaRoot_DbName = "item_consumeable_spellweaver_root_sparse";
        private const int LindellaItem_ManaRoot_Count = 8;

        private const string LindellaItem_TeleportScroll_DbName = "item_consumeable_teleportation_scroll";
        private const int LindellaItem_TeleportScroll_Count = 3;

        private const string LindellaItem_ClassicMe_DbName = "item_consumeable_ClassicMeLotion";
        private const int LindellaItem_ClassicMe_Count = 3;

        private const string LindellaItem_LullabyWhistle_DbName = "item_consumeable_Lullaby_Whistle";
        private const int LindellaItem_LullabyWhistle_Count = 7;

        private const string LindellaItem_FireFritter_DbName = "item_consumaeable_fire_fritter";
        private const int LindellaItem_FireFritter_Count = 6;

        private const string LindellaItem_BarricadeBrownie_DbName = "item_barricade_brownie";
        private const int LindellaItem_BarricadeBrownie_Count = 6;

        private const string LindellaItem_TrueshotTruffles_DbName = "item_consumeable_trueshot_truffles";
        private const int LindellaItem_TrueshotTruffles_Count = 6;

        private const string LindellaItem_SelfCastItem_DbName = "item_consumable_selfcaster";
        private const int LindellaItem_SelfCastItem_Count = 5;

        private const string LindellaItem_Spellbook_Small_DbName = "item_consumable_spellbook_small";
        private const int LindellaItem_Spellbook_Small_Count = 4;

        private const string LindellaItem_Spellbook_Medium_DbName = "item_consumable_spellbook_medium";
        private const int LindellaItem_Spellbook_Medium_Count = 3;

        private const string LindellaItem_Spellbook_Large_DbName = "item_consumable_spellbook_large";
        private const int LindellaItem_Spellbook_Large_Count = 2;

        private const string LindellaItem_Spellbook_Giant_DbName = "item_consumable_spellbook_giant";
        private const int LindellaItem_Spellbook_Giant_Count = 1;

        private const string LindellaItem_CurseRemover_DbName = "item_consumable_curselifter";
        private const int LindellaItem_CurseRemover_Count = 6;


        private const string LindellaItem_XpBook1_DbName = "item_consumable_tome-founding";
        private const int LindellaItem_XpBook1_Count = 5;




        // Membership ID code:  
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

            int botCount = playerRepo.Players.Where(b => b.MembershipId == -2).Count();

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
                bot.MembershipId = -2;
                bot.Mobility = "full";
                bot.IsPetToId = -1;
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
            int botCount = playerRepo.Players.Where(b => b.MembershipId == -2 && b.Mobility == "full").Count();
            if (botCount < PvPStatics.PsychopathDefaultAmount)
            {
                AIProcedures.SpawnAIPsychopaths(PvPStatics.PsychopathDefaultAmount - botCount, 0);
                log.AddLog("Spawned a new psychopath.");
            }

            List<int> botIds = playerRepo.Players.Where(p => p.MembershipId == -2).Where(b => b.Mobility == "full").Select(b => b.Id).ToList();

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

                    BuffBox botbuffs = ItemProcedures.GetPlayerBuffs(bot);

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

                        // if the target is offline, no longer animate, or in the same form as the spells' target, go into idle mode
                        if (PlayerProcedures.PlayerIsOffline(myTarget) || myTarget.Mobility != "full" || myTarget.Form == skill.Skill.FormdbName)
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
                            if (!PlayerProcedures.PlayerIsOffline(p) && p.Id != bot.Id && p.Mobility == "full" && p.MembershipId == -2 && p.Level >= bot.Level)
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
                merchant.MembershipId = -3;
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
                merchant.Form = "botform_clothes_merchant";
                merchant.NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow;
                merchant.dbLocationName = "270_west_9th_ave"; // Lindella starts her rounds here
                merchant.Gender = "female";
                merchant.IsItemId = -1;
                merchant.IsPetToId = -1;
                merchant.ActionPoints = 120;

                playerRepo.SavePlayer(merchant);

                merchant = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lindella" && f.LastName == "the Soul Vendor" && f.Mobility == "full");

                List<DbStaticSkill> skillsToGive = new List<DbStaticSkill>();

                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_A_Mother's_Touch_Purple_Autumn"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_lingerie_inveigh_PsychoticPie"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Haute_Couture_Hanna"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_reflecius_fabricos_Haretia"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Schoolyard_Strut_Christopher"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Put_a_Sock_In_It_Christopher"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Fit_to_Be_Tied!_Martiandawn"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_It's_Got_Ruffles!_Martiandawn"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Lucky_Lips_Greg_Mackenzie_and_Fiona_Mason"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Spank'Me_Zatur"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Who_Wears_the_Pants_Arbitrary_Hal"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Clothes_Make_the_Man_Arbitrary_Hal"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Sgnikcots_Tenhsif_Zatur"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Sea_of_Frills_Hanna"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_schooltop_bop_PsychoticPie"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_schooltop_bop_PsychoticPie"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_tip_of_the_hat_Zatur"));
                skillsToGive.Add(SkillStatics.GetStaticSkill("skill_Atexlay_Abray_Arrhae"));

                foreach (DbStaticSkill s in skillsToGive)
                {
                    SkillProcedures.GiveSkillToPlayer(merchant.Id, s);
                }

                Player Lindella = playerRepo.Players.FirstOrDefault(f => f.MembershipId == -3);

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

                merchant.Form = "botform_clothes_merchant";
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
                BuffBox box = ItemProcedures.GetPlayerBuffs(merchant);

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
                    IItemRepository itemRepo = new EFItemRepository();
                    List<Item> lindellasItems = itemRepo.Items.Where(i => i.OwnerId == merchant.Id && i.Level == 0).ToList();


                    // sparse willpower roots
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_WPRoot_DbName).Count() < LindellaItem_WPRoot_Count)
                    {
                        for (int x = 0; x < LindellaItem_WPRoot_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_WPRoot_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // sparse spellweaver roots
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_ManaRoot_DbName).Count() < LindellaItem_ManaRoot_Count)
                    {
                        for (int x = 0; x < LindellaItem_ManaRoot_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_ManaRoot_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // teleportation scrolls
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_TeleportScroll_DbName).Count() < LindellaItem_TeleportScroll_Count)
                    {
                        for (int x = 0; x < LindellaItem_TeleportScroll_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_TeleportScroll_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // Classic Me potions
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_ClassicMe_DbName).Count() < LindellaItem_ClassicMe_Count)
                    {
                        for (int x = 0; x < LindellaItem_ClassicMe_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_ClassicMe_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // Lullaby Whistles
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_LullabyWhistle_DbName).Count() < LindellaItem_LullabyWhistle_Count)
                    {
                        for (int x = 0; x < LindellaItem_LullabyWhistle_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_LullabyWhistle_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // Fire Fritters
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_FireFritter_DbName).Count() < LindellaItem_FireFritter_Count)
                    {
                        for (int x = 0; x < LindellaItem_FireFritter_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_FireFritter_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // LindellaItem_FireFritter_DbName
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_BarricadeBrownie_DbName).Count() < LindellaItem_BarricadeBrownie_Count)
                    {
                        for (int x = 0; x < LindellaItem_BarricadeBrownie_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_BarricadeBrownie_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // Trueshot Truffles
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_TrueshotTruffles_DbName).Count() < LindellaItem_TrueshotTruffles_Count)
                    {
                        for (int x = 0; x < LindellaItem_TrueshotTruffles_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_TrueshotTruffles_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    
                         // Self cast machines
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_SelfCastItem_DbName).Count() < LindellaItem_SelfCastItem_Count)
                    {
                        for (int x = 0; x < LindellaItem_SelfCastItem_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_SelfCastItem_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // small spellbooks
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_Spellbook_Small_DbName).Count() < LindellaItem_Spellbook_Small_Count)
                    {
                        for (int x = 0; x < LindellaItem_SelfCastItem_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_Spellbook_Small_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // medium spellbooks
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_Spellbook_Medium_DbName).Count() < LindellaItem_Spellbook_Medium_Count)
                    {
                        for (int x = 0; x < LindellaItem_SelfCastItem_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_Spellbook_Medium_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // large spellbooks
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_Spellbook_Large_DbName).Count() < LindellaItem_Spellbook_Large_Count)
                    {
                        for (int x = 0; x < LindellaItem_SelfCastItem_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_Spellbook_Large_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // giant spellbooks
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_Spellbook_Giant_DbName).Count() < LindellaItem_Spellbook_Giant_Count)
                    {
                        for (int x = 0; x < LindellaItem_SelfCastItem_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_Spellbook_Giant_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                     // curse lifters
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_CurseRemover_DbName).Count() < LindellaItem_CurseRemover_Count)
                    {
                        for (int x = 0; x < LindellaItem_CurseRemover_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_CurseRemover_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
                    }

                    // xp books
                    if (lindellasItems.Where(i => i.dbName == LindellaItem_XpBook1_DbName).Count() < LindellaItem_XpBook1_Count)
                    {
                        for (int x = 0; x < LindellaItem_XpBook1_Count; x++)
                        {
                            Item wproot = new Item
                            {
                                dbName = LindellaItem_XpBook1_DbName,
                                dbLocationName = "",
                                OwnerId = merchant.Id,
                                IsEquipped = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = false,
                                TimeDropped = DateTime.UtcNow,
                                TurnsUntilUse = 0,
                                VictimName = "",
                                EquippedThisTurn = false,
                            };
                            itemRepo.SaveItem(wproot);
                        }
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

            if (personAttackin.MembershipId > 0) {
                AttackProcedures.Attack(bot, personAttackin, selectedSkill);
            }
        }

        public static void CheckAICounterattackRoutine(Player personAttacking, Player bot)
        {
            // person attacking is a boss and not a psychopath, so do nothing
            if (personAttacking.MembershipId < -2)
            {
                return;
            }

            // attacking the psychopath.  Random chance the psychopath will set the attacker as their target.
            if (bot.MembershipId == -2)
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
            if (bot.MembershipId == -3)
            {
                AIProcedures.CounterAttack(personAttacking, bot);
            }

            // if the target is Donna, counterattack and set that player as her target immediately
            if (bot.MembershipId == -4)
            {
                BossProcedures_Donna.DonnaCounterattack(personAttacking, bot);
            }

            // Valentine counterattack
            if (bot.MembershipId == -5)
            {
                BossProcedures_Valentine.CounterAttack(personAttacking, bot);
            }

            // Bimbo boss counterattack
            else if (bot.MembershipId == -7)
            {
                BossProcedures_BimboBoss.CounterAttack(personAttacking, bot);
            }

            // rat thieves counterattack
            else if (bot.MembershipId == -8 || bot.MembershipId == -9)
            {
                BossProcedures_Thieves.CounterAttack(personAttacking);
            }

            else if (bot.MembershipId == -10)
            {
                BossProcedures_PetMerchant.CounterAttack(personAttacking);
            }

            else if (bot.MembershipId == -11 || bot.MembershipId == -12)
            {
                BossProcedures_Sisters.CounterAttack(personAttacking, bot);
            }


        }

        public static string MoveTo(Player bot, string locationDbName, int distance)
        {
            Location start = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == bot.dbLocationName);
            Location end = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == locationDbName);

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
                string enteringMessage = bot.FirstName + " " + bot.LastName + " entered from " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == start.dbName).Name;
                string leavingMessage = bot.FirstName + " " + bot.LastName + " left toward " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[0]).Name;
                LocationLogProcedures.AddLocationLog(start.dbName, leavingMessage);
                LocationLogProcedures.AddLocationLog(pathTiles[0], enteringMessage);
                output += leavingMessage + "<br>";
                output += "----<br>";

                for (int i = 0; i < xDistance; i++)
                {
                    try
                    {
                        enteringMessage = bot.FirstName + " " + bot.LastName + " entered from " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[i]).Name;
                        leavingMessage = bot.FirstName + " " + bot.LastName + " left toward " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == pathTiles[i + 1]).Name;
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
            if (target.MembershipId == -5) // Fighting Valentine
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

        

    }

}

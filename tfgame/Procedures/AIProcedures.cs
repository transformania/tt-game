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

        // Membership ID code:  
        // -1 (player has rerolled, player is abandoned)
        // -2 (psychopath spellslinger)
        // -3 item merchant
        // -4 Donna Milton
        // -5 Lord Valentine
        // -6 Jewdewfae

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


                // spawn in some stronger psychopaths based on update number
                if (turnNumber % 13 == 0){
                    bot.FirstName = "Fierce " + bot.FirstName;
                    bot.Level = 3;
                }
                else if (turnNumber % 27 == 0)
                {
                    bot.FirstName = "Wrathful " + bot.FirstName;
                    bot.Level = 6;
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
                EffectProcedures.GivePerkToPlayer("bot_psychopathic", bot);

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

            List<Player> bots = playerRepo.Players.Where(p => p.MembershipId == -2).Where(b => b.Mobility == "full").ToList();

            foreach (Player bot in bots)
            {

                try
                {

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
                    SkillViewModel2 skill = SkillProcedures.GetSkillViewModelsOwnedByPlayer(bot.Id).FirstOrDefault(s => s.dbSkill.Name != "lowerHealth" && s.Skill.ExclusiveToForm == null);

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
                            if (!PlayerProcedures.PlayerIsOffline(p) && p.Id != bot.Id && p.Mobility == "full" && p.InPvP == false && p.MembershipId >= -2 && p.Level >= bot.Level -2)
                            {
                                onlinePlayersHere.Add(p);
                            }
                        }

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

                Player Lindella = playerRepo.Players.FirstOrDefault(f => f.MembershipId == -2);

                AIDirectiveProcedures.GetAIDirective(Lindella.Id);
                AIDirectiveProcedures.SetAIDirective_MoveTo(Lindella.Id, "street_15th_south");



            }
        }

        public static void RunAIMerchantActions()
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

            }

        }

        public static void SpawnDonna()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player donna = playerRepo.Players.FirstOrDefault(f => f.FirstName == "'Aunt' Donna" && f.LastName == "Milton");

            if (donna == null)
            {
                donna = new Player()
                {
                    FirstName = "'Aunt' Donna",
                    LastName = "Milton",
                    ActionPoints = 120,
                    dbLocationName = "ranch_bedroom",
                    LastActionTimestamp = DateTime.UtcNow,
                    LastCombatTimestamp = DateTime.UtcNow,
                    OnlineActivityTimestamp = DateTime.UtcNow,
                    NonPvP_GameOverSpellsAllowedLastChange = DateTime.UtcNow,
                    Gender = "female",
                    Health = 9999,
                    Mana = 9999,
                    MaxHealth = 9999,
                    MaxMana = 9999,
                    Form = "form_Donna_LexamTheGemFox",
                    IsPetToId = -1,
                    Money = 1000,
                    Mobility = "full",
                    Level = 20,
                    MembershipId = -4,
                    ActionPoints_Refill = 360,
                };

                playerRepo.SavePlayer(donna);

                donna = PlayerProcedures.ReadjustMaxes(donna, ItemProcedures.GetPlayerBuffs(donna));

                playerRepo.SavePlayer(donna);


                donna = playerRepo.Players.FirstOrDefault(f => f.FirstName == "'Aunt' Donna" && f.LastName == "Milton");
                DbStaticSkill skillToAdd = SkillStatics.GetStaticSkill("skill_Donna's_Bitch_LexamTheGemFox");
                SkillProcedures.GiveSkillToPlayer(donna.Id, skillToAdd);
            }



        }

        public static void RunDonnaActions()
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IServerLogRepository serverLogRepo = new EFServerLogRepository();

             int worldTurnNumber = PvPWorldStatProcedures.GetWorldTurnNumber() - 1;
             ServerLog log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == worldTurnNumber);

            Player donna = playerRepo.Players.FirstOrDefault(p => p.MembershipId == -4);

            if (donna.Mobility != "full")
            {
                PvPWorldStatProcedures.Boss_EndDonna();
            }

            else if (donna.Mobility == "full")
            {

                log.AddLog("Starting AI for Donna.");

                donna.Form = "form_Donna_LexamTheGemFox";

                BuffBox donnasBuffs = ItemProcedures.GetPlayerBuffs(donna);

                // have donna meditate and cleanse if needed
                if (donna.Health < donna.MaxHealth/2)
                {
                    PlayerProcedures.Cleanse(donna, donnasBuffs);
                }
                if (donna.Mana < donna.MaxMana)
                {
                    PlayerProcedures.Meditate(donna, donnasBuffs);
                }

                AIDirective directive = AIDirectiveProcedures.GetAIDirective(donna.Id);

                if (directive.State == "attack" || directive.State == "idle")
                {

                    

                    Player target = playerRepo.Players.FirstOrDefault(p => p.Id == directive.TargetPlayerId);

                    if (target != null)
                    {
                        log.AddLog("Donna is set to attack " + target.FirstName + " " + target.LastName);
                    }
                    

                    // if Donna's target goes offline, have her teleport back to the ranch
                    if (target == null || target.Mobility != "full" || PlayerProcedures.PlayerIsOffline(target))
                    {

                        if (donna.dbLocationName != "ranch_bedroom")
                        {
                            log.AddLog("Donna's target, " + target.FirstName + " " + target.LastName + ", is invalid.  Teleporting home and idling.");
                            LocationLogProcedures.AddLocationLog(donna.dbLocationName, donna.FirstName + " " + donna.LastName + " vanished from here in a flash of smoke.");
                            donna.dbLocationName = "ranch_bedroom";
                            LocationLogProcedures.AddLocationLog(donna.dbLocationName, donna.FirstName + " " + donna.LastName + " appeared here in a flash of smoke.");
                            playerRepo.SavePlayer(donna);
                        }


                        AIDirectiveProcedures.SetAIDirective_Idle(donna.Id);
                        
                    }

                    // Donna has a valid target; go chase it down and attack.  Donna does not look for new targets.
                    else
                    {
                        string newplace = MoveTo(donna, target.dbLocationName, 9);
                        donna.dbLocationName = newplace;
                        playerRepo.SavePlayer(donna);

                        log.AddLog("Donna has moved to " + newplace + " to attack " + target.FirstName + " " + target.LastName);

                        if (target.dbLocationName == newplace)
                        {
                            
                            SkillViewModel2 skill = SkillProcedures.GetSkillViewModelsOwnedByPlayer(donna.Id).FirstOrDefault(s => s.dbSkill.Name != "lowerHealth");

                            Random rand = new Random();
                            double roll = rand.NextDouble() * 3 + 2;
                            for (int i = 0; i < roll; i++)
                            {
                                AttackProcedures.Attack(donna, target, skill);
                                log.AddLog("Donna attacked " + target.FirstName + " " + target.LastName + ".");
                            }
                        }
                        else
                        {
                            log.AddLog("Donna did not move far enough to get to target.");
                        }

                    }
                }
                else
                {
                    log.AddLog("Donna is idling.");
                }

                // have Donna equip all the pets she owns
                IItemRepository itemRepo = new EFItemRepository();
                IEnumerable<Item> donnasItems = itemRepo.Items.Where(i => i.OwnerId == donna.Id && i.IsEquipped == false && i.Level > 3);
                List<Item> itemsToEquip = new List<Item>();
                foreach (Item i in donnasItems)
                {
                    itemsToEquip.Add(i);
                }
                foreach (Item i in itemsToEquip)
                {
                    i.IsEquipped = true;
                    i.dbLocationName = donna.dbLocationName;
                    itemRepo.SaveItem(i);
                    log.AddLog("Donna equipped a pet.");
                }

                List<Player> donnasPlayerPets = playerRepo.Players.Where(p => p.IsPetToId == donna.Id).ToList();
                List<Player> donnasPlayerPetsToSave = new List<Player>();
                foreach (Player p in donnasPlayerPets)
                {
                    p.dbLocationName = donna.dbLocationName;
                    donnasPlayerPetsToSave.Add(p);
                }
                foreach (Player p in donnasPlayerPetsToSave)
                {
                    playerRepo.SavePlayer(p);
                }

                // have Donna release her weakest pet if she has more than 10 already
                if (donnasPlayerPets.Count() > 10)
                {
                    IEnumerable<Item> weakest = itemRepo.Items.Where(i => i.OwnerId == donna.Id).OrderBy(i => i.Level);
                    Item weakestItem = weakest.First();
                    ItemProcedures.DropItem(weakestItem.Id, donna.dbLocationName);
                    log.AddLog("Donna released " + weakestItem.VictimName);
                    LocationLogProcedures.AddLocationLog(donna.dbLocationName, "Donna released one of her weaker pets, " + weakestItem.VictimName + ", here.");
                    Player luckyVictim = PlayerProcedures.GetPlayerWithExactName(weakestItem.VictimName);
                    PlayerLogProcedures.AddPlayerLog(luckyVictim.Id, "Donna has released you, allowing you to wander about or be tamed by a new owner.", true);
                }
                log.AddLog("Donna actions completed.");
                serverLogRepo.SaveServerLog(log);

            }

        }

        public static void CounterAttack(Player personAttackin, Player merchant)
        {

            IEnumerable<SkillViewModel2> myskills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(merchant.Id);

            Random rand = new Random(personAttackin.LastName.GetHashCode());
            double roll = Math.Floor(rand.NextDouble() * (double)myskills.Count());


            SkillViewModel2 selectedSkill = myskills.ElementAt((int)roll);

            if (personAttackin.MembershipId > 0) { 
                AttackProcedures.Attack(merchant, personAttackin, selectedSkill);
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

            #region Donna
            // if the target is Donna, counterattack and set that player as her target immediately
            if (bot.MembershipId == -4)
            {

                Random rand = new Random();
                double roll = rand.NextDouble() * 4;

                for (int i = 0; i < roll; i++)
                {
                    AIProcedures.CounterAttack(personAttacking, bot);
                }


                // if Donna is weak enough start having her mega-attack anyone in the room
                if (bot.Health < bot.MaxHealth / 5)
                {
                    List<PlayerFormViewModel> PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(bot.dbLocationName).ToList();
                    List<PlayerFormViewModel> targets = new List<PlayerFormViewModel>();

                    foreach (PlayerFormViewModel p in PlayersHere)
                    {
                        if (p.Player.MembershipId > 0 && p.Player.Level > 3 && p.Player.Mobility == "full" && !PlayerProcedures.PlayerIsOffline(p.Player) && p.Player.Id != personAttacking.Id)
                        {
                            AIProcedures.CounterAttack(p.Player.ToDbPlayer(), bot);
                        }
                    }


                }


                //AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);
                AIDirective directive = AIDirectiveProcedures.GetAIDirective(bot.Id);

                // if Donna has no target or by a random chance, make her target this attacker
                if (directive.TargetPlayerId == -1 || directive.State == "idle" || roll < 1)
                {
                    AIDirectiveProcedures.SetAIDirective_Attack(bot.Id, personAttacking.Id);
                }

            }
            #endregion

            // Valentine counterattack
            if (bot.MembershipId == -5)
            {
                BossProcedures_Valentine.CounterAttack(personAttacking, bot);
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


    }

}

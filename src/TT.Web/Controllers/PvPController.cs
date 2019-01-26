using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Web.CustomHtmlHelpers;
using TT.Domain;
using FeatureSwitch;
using TT.Domain.Assets.Queries;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Queries;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Messages.Queries;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Skills.Queries;
using TT.Domain.TFEnergy.Commands;
using TT.Domain.World.DTOs;
using TT.Domain.World.Queries;
using TT.Web.Extensions;
using TT.Web.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class PvPController : Controller
    {

        public virtual ActionResult Play()
        {

            var loadtime = "";
            var updateTimer = new Stopwatch();
            updateTimer.Start();

            // load up the covenant bindings into memory
            if (!CovenantDictionary.IdNameFlagLookup.Any())
            {
                CovenantProcedures.LoadCovenantDictionary();
            }

            var myMembershipId = User.Identity.GetUserId();

            // assert that the player is logged in; otherwise ask them to do so
            if (myMembershipId == null)
            {
                return RedirectToAction(MVC.Account.Login());
            }

            ViewBag.MyMembershipId = myMembershipId;
            ViewBag.MaxLogSize = PvPStatics.MaxLogMessagesPerLocation;

            // if the player is logged in but has no character, go to a playercreation screen
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            if (me == null)
            {
                return RedirectToAction(MVC.PvP.Restart());
            }

            if (Session["ContributionId"] == null)
            {
                Session["ContributionId"] = -1;
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            // redirect to Quest page if player is currently in a quest
            if (me.InQuest > 0)
            {
                return RedirectToAction(MVC.Quest.Questing());
            }

            var markOnlineCutoff = DateTime.UtcNow.AddMinutes(-2);

            // update the player's "last online" attribute if it's been long enough
            if (me.OnlineActivityTimestamp < markOnlineCutoff && !PvPStatics.AnimateUpdateInProgress)
            {
                PlayerProcedures.MarkOnlineActivityTimestamp(me);
            }

            var world = DomainRegistry.Repository.FindSingle(new GetWorld());


            ViewBag.UpdateInProgress = false;

            var secondsSinceUpdate = Math.Abs(Math.Floor(world.LastUpdateTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));
            ViewBag.SecondsUntilUpdate = TurnTimesStatics.GetTurnLengthInSeconds() - (int)secondsSinceUpdate;

            // turn off world update toggle if it's simply been too long
            if (secondsSinceUpdate > 90 && (PvPStatics.AnimateUpdateInProgress || world.WorldIsUpdating))
            {
                PvPStatics.AnimateUpdateInProgress = false;
                PvPWorldStatProcedures.StopUpdatingWorld();
            }

            if (world.WorldIsUpdating && secondsSinceUpdate < 90)
            {
                ViewBag.UpdateInProgress = true;
            }



            // load the update date into memory
            PvPStatics.LastGameUpdate = world.GameNewsDate;

            ViewBag.WorldTurnNumber = world.TurnNumber;

            // set viewbag to show offline players is the link has been clicked
            if (TempData["ShowOffline"] != null)
            {
                ViewBag.ShowOffline = TempData["ShowOffline"];
            }
            else
            {
                ViewBag.ShowOffline = false;
            }

            var renderCaptcha = false;
            if (FeatureContext.IsEnabled<UseCaptcha>() && me.Mobility != PvPStatics.MobilityFull)
            {
                try
                {
                    renderCaptcha = DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired { UserId = me.MembershipId });
                }
                catch (DomainException)
                {
                    DomainRegistry.Repository.Execute(new CreateCaptchaEntry { UserId = me.MembershipId });
                }
            }

            // player is inanimate, load up the inanimate endgame page
            if (me.Mobility == PvPStatics.MobilityInanimate)
            {
                var inanimateOutput = new InanimatePlayPageViewModel
                {
                    RenderCaptcha = renderCaptcha,
                    LastUpdateTimestamp = world.LastUpdateTimestamp,
                    WorldStats = PlayerProcedures.GetWorldPlayerStats(),
                    World = world,
                    Player = me,
                    Form = FormStatics.GetForm(me.FormSourceId),
                    Item = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer
                    {
                        PlayerId = me.Id
                    }),
                    NewMessageCount =
                        DomainRegistry.Repository.FindSingle(new GetUnreadMessageCountByPlayer {OwnerId = me.Id}),
                    PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse(),
                    StruggleChance = InanimateXPProcedures.GetStruggleChance(me)
                };
                inanimateOutput.PlayerLogImportant = inanimateOutput.PlayerLog.Where(l => l.IsImportant);

                if (inanimateOutput.Item.Owner == null)
                {
                    // Not owned
                    inanimateOutput.AtLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == inanimateOutput.Item.dbLocationName)?.Name ?? "Unknown";
                    inanimateOutput.LocationLog = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = inanimateOutput.Item.dbLocationName, ConcealmentLevel = 0 });
                    inanimateOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(inanimateOutput.Item.dbLocationName, myMembershipId);
                }
                else
                {
                    // Owned
                    inanimateOutput.WornBy = ItemProcedures.BeingWornBy(me);
                    var actionsHere = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = inanimateOutput.WornBy.Player.dbLocationName, ConcealmentLevel = 0 });

                    var validActionsHere = new List<LocationLogDetail>();
                    foreach (var log in actionsHere)
                    {
                        if (log.ConcealmentLevel <= 0 && !log.Message.Contains("entered from") && !log.Message.Contains("left toward"))
                        {
                            validActionsHere.Add(log);
                        }
                    }

                    inanimateOutput.AtLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == inanimateOutput.WornBy.Player.dbLocationName)?.Name ?? "Unknown";
                    inanimateOutput.LocationLog = validActionsHere;
                    inanimateOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(inanimateOutput.WornBy.Player.dbLocationName, myMembershipId);

                }

                var playersHere = new List<PlayerFormViewModel>();
                foreach (var p in inanimateOutput.PlayersHere)
                {
                    if (p.Player.Mobility == PvPStatics.MobilityFull)
                    {
                        playersHere.Add(p);
                    }
                }

                inanimateOutput.PlayersHere = playersHere.OrderByDescending(p => p.Player.Level);

                return View(MVC.PvP.Views.Play_Inanimate, inanimateOutput);
            }

            // player is an animal, load up the inanimate endgame page
            if (me.Mobility == PvPStatics.MobilityPet)
            {
                var animalOutput = new AnimalPlayPageViewModel();
                animalOutput.You = me;
                animalOutput.World = world;
                animalOutput.RenderCaptcha = renderCaptcha;

                animalOutput.Form = FormStatics.GetForm(me.FormSourceId);

                animalOutput.YouItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });
                if (animalOutput.YouItem.Owner != null)
                {
                    animalOutput.OwnedBy = PlayerProcedures.GetPlayerFormViewModel(animalOutput.YouItem.Owner.Id);

                    // move player over to owner
                    if (me.dbLocationName != animalOutput.OwnedBy.Player.dbLocationName)
                    {
                        PlayerProcedures.MovePlayer_InstantNoLog(me.Id, animalOutput.OwnedBy.Player.dbLocationName);
                    }

                }


                animalOutput.WorldStats = PlayerProcedures.GetWorldPlayerStats();

                if (animalOutput.OwnedBy != null)
                {
                    animalOutput.Location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == animalOutput.OwnedBy.Player.dbLocationName);
                }
                else
                {
                    animalOutput.Location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
                }

                animalOutput.Location.FriendlyName_North = LocationsStatics.GetConnectionName(animalOutput.Location.Name_North);
                animalOutput.Location.FriendlyName_East = LocationsStatics.GetConnectionName(animalOutput.Location.Name_East);
                animalOutput.Location.FriendlyName_South = LocationsStatics.GetConnectionName(animalOutput.Location.Name_South);
                animalOutput.Location.FriendlyName_West = LocationsStatics.GetConnectionName(animalOutput.Location.Name_West);

                animalOutput.PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse();
                animalOutput.PlayerLogImportant = animalOutput.PlayerLog.Where(l => l.IsImportant);

                animalOutput.LocationLog = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = animalOutput.Location.dbName, ConcealmentLevel = 0 });

                var animalLocationItemsCmd = new GetItemsAtLocationVisibleToGameMode { dbLocationName = animalOutput.Location.dbName, gameMode = me.GameMode };
                animalOutput.LocationItems = DomainRegistry.Repository.Find(animalLocationItemsCmd);

                animalOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(animalOutput.Location.dbName, myMembershipId).Where(p => p.Form.MobilityType == PvPStatics.MobilityFull);

                animalOutput.LastUpdateTimestamp = world.LastUpdateTimestamp;

                animalOutput.NewMessageCount = DomainRegistry.Repository.FindSingle(new GetUnreadMessageCountByPlayer { OwnerId = me.Id });

                ViewBag.AnimalImgUrl = ItemStatics.GetStaticItem(animalOutput.Form.BecomesItemDbName).PortraitUrl;

                animalOutput.IsPermanent = animalOutput.YouItem.IsPermanent;

                animalOutput.StruggleChance = InanimateXPProcedures.GetStruggleChance(me);

                return View(MVC.PvP.Views.Play_Animal, animalOutput);

            }

            var output = new PlayPageViewModel();


            output.World = world;

            loadtime += "Before loading buffs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            var myBuffs = ItemProcedures.GetPlayerBuffs(me);
            loadtime += "After loading buffs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            if (myBuffs.HasSearchDiscount)
            {
                output.APSearchCost = PvPStatics.SearchAPCost - 1;
            }
            else
            {
                output.APSearchCost = PvPStatics.SearchAPCost;
            }

            loadtime += "Start get max inv. size:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.InventoryMaxSize = ItemProcedures.GetInventoryMaxSize(myBuffs);
            loadtime += "End get max inv. size:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.You = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            output.PlayerIsAtBusStop =
                DomainRegistry.Repository.FindSingle(new PlayerIsAtBusStop { playerLocation = me.dbLocationName });

            output.LastUpdateTimestamp = PvPWorldStatProcedures.GetLastWorldUpdate();

            loadtime += "Start get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(me.dbLocationName, myMembershipId).Where(p => p.Form.MobilityType == PvPStatics.MobilityFull);
            loadtime += "End get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.Location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(x => x.dbName == me.dbLocationName);
            output.Location.CovenantController = CovenantProcedures.GetLocationCovenantOwner(me.dbLocationName);

            output.Location.FriendlyName_North = LocationsStatics.GetConnectionName(output.Location.Name_North);
            output.Location.FriendlyName_East = LocationsStatics.GetConnectionName(output.Location.Name_East);
            output.Location.FriendlyName_South = LocationsStatics.GetConnectionName(output.Location.Name_South);
            output.Location.FriendlyName_West = LocationsStatics.GetConnectionName(output.Location.Name_West);

            loadtime += "Start get location logs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.LocationLog = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = me.dbLocationName, ConcealmentLevel = (int)myBuffs.Perception() });
            loadtime += "End get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get playerlogs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse();
            loadtime += "End get playerlogs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.PlayerLogImportant = output.PlayerLog.Where(l => l.IsImportant);

            loadtime += "Start get player items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayerItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer {OwnerId = me.Id});
            output.CurrentCarryWeight = DomainRegistry.Repository.FindSingle(new GetCurrentCarryWeight {PlayerId = me.Id});
            loadtime += "End get player items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get location items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.LocationItems = DomainRegistry.Repository.Find(new GetItemsAtLocationVisibleToGameMode { dbLocationName = me.dbLocationName, gameMode = me.GameMode });
            loadtime += "End get location items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            ViewBag.InventoryItemCount = output.PlayerItems.Count();
            output.UnreadMessageCount = DomainRegistry.Repository.FindSingle(new GetUnreadMessageCountByPlayer { OwnerId = me.Id });
            output.WorldStats = PlayerProcedures.GetWorldPlayerStats();
            output.AttacksMade = me.TimesAttackingThisUpdate;
            ViewBag.AttacksMade = me.TimesAttackingThisUpdate;


            ViewBag.LoadTime = loadtime;

            if (me.InDuel > 0)
            {
                var duel = DuelProcedures.GetDuel(me.InDuel);
                var playersInDuel = DuelProcedures.GetPlayerViewModelsInDuel(me.InDuel);
                var duelNames = "";
                var playersInDuelCount = playersInDuel.Count();
                var i = 1;
                foreach (var p in playersInDuel)
                {
                    // ignore if own name
                    if (p.Player.GetFullName() != me.GetFullName())
                    {
                        duelNames += p.Player.GetFullName();
                        if (i < playersInDuelCount)
                        {
                            duelNames += ", ";
                        }
                    }
                    i++;
                }
                ViewBag.Duel = duelNames;
                ViewBag.DuelId = me.InDuel;
                ViewBag.TurnsLeft = PvPStatics.MaximumDuelTurnLength - (PvPWorldStatProcedures.GetWorldTurnNumber() - duel.StartTurn);
            }

            if (me.InQuest > 0)
            {
                var quest = QuestProcedures.GetQuest(me.InQuest);
                ViewBag.QuestName = quest.Name;
            }

            return View(MVC.PvP.Views.Play, output);
        }

        public virtual ActionResult ShowOffline()
        {
            TempData["ShowOffline"] = true;
            return RedirectToAction(MVC.PvP.Play());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult NewCharacter(NewCharacterViewModel newCharacterViewModel)
        {

            ViewBag.IsRerolling = false;

            var myMembershipId = User.Identity.GetUserId();
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Your character was not created.  You can only use letters and your first and last names must be between 2 and 30 letters long.";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            if (newCharacterViewModel.StartGameMode != 0 && newCharacterViewModel.StartGameMode != 1 && newCharacterViewModel.StartGameMode != 2)
            {
                ViewBag.ErrorMessage = "That is not a valid game mode.";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            DbStaticForm staticForm = FormStatics.GetForm(newCharacterViewModel.FormSourceId);

            if (staticForm.FriendlyName == "Regular Girl")
            {
                newCharacterViewModel.Gender = PvPStatics.GenderFemale;
            }
            else if (staticForm.FriendlyName == "Regular Guy")
            {
                newCharacterViewModel.Gender = PvPStatics.GenderMale;
            }
            else
            {
                ViewBag.ErrorMessage = "That is not a valid starting form.";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            // assert that the first name is not reserved by the system
            var fnamecheck = TrustStatics.NameIsReserved(newCharacterViewModel.FirstName);
            if (!fnamecheck.IsNullOrEmpty())
            {
                ViewBag.ErrorMessage = "You can't use the first name '" + newCharacterViewModel.FirstName + "'.  It is reserved.";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            // assert that the last name is not reserved by the system
            var lnamecheck = TrustStatics.NameIsReserved(newCharacterViewModel.LastName);
            if (!lnamecheck.IsNullOrEmpty())
            {
                ViewBag.ErrorMessage = "You can't use the last name '" + newCharacterViewModel.LastName + "'.  It is reserved or else not allowed.";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            // assert player does not currently have an animate character
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            if (me != null && me.Mobility == PvPStatics.MobilityFull)
            {
                ViewBag.ErrorMessage = "You cannot create a new character right now.  You already have a fully animate character already, " + me.GetFullName() + ".";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            // assert player does not have more than 1 account already

            var iAmWhitelisted = User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist);

            if (!iAmWhitelisted && newCharacterViewModel.InanimateForm == null && PlayerProcedures.IsMyIPInUseAndAnimate(Request.GetRealUserHostAddress()))
            {

                ViewBag.ErrorMessage = "Your character was not created.  It looks like your IP address, <b>" + Request.GetRealUserHostAddress() + "</b> already has 1 animate character in this world, and the current limit is 1. ";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            var result = PlayerProcedures.SaveNewPlayer(newCharacterViewModel, myMembershipId);

            if (result != "saved")
            {
                ViewBag.ErrorMessage = "Your character was not created.  Reason:  " + result;
                ViewBag.IsRerolling = true;
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            PlayerProcedures.LogIP(Request.GetRealUserHostAddress(), myMembershipId);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult Restart()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me != null && me.Mobility == PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You are still animate.";
                return RedirectToAction(MVC.PvP.Play());
            }

            ViewBag.IsRerolling = false;
            ViewBag.OldFirstName = "";
            ViewBag.OldLastName = "";
            ViewBag.OldFormSourceId = 2;

            if (me != null)
            {
                ViewBag.IsRerolling = true;
                ViewBag.OldFirstName = me.FirstName;
                ViewBag.OldLastName = me.LastName.Split(' ')[0];
                ViewBag.OldFormSourceId = me.OriginalFormSourceId;
            }

            // find the reserved name if there is one
            if (me == null)
            {

                var reservedName = PlayerProcedures.GetPlayerReservedName(myMembershipId);
                if (reservedName != null)
                {
                    ViewBag.OldFirstName = reservedName.FullName.Split(' ')[0];
                    ViewBag.OldLastName = reservedName.FullName.Split(' ')[1];
                }
            }

            return View(MVC.PvP.Views.MakeNewCharacter);
        }

        public virtual ActionResult MoveTo(string locname)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.LogIP(Request.GetRealUserHostAddress(), myMembershipId);

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert that the player is not mind controlled and cannot move on their own
            if (me.MindControlIsActive)
            {

                var myExistingMCs = MindControlProcedures.GetAllMindControlsWithPlayer(me);

                if (MindControlProcedures.PlayerIsMindControlledWithType(me, myExistingMCs, MindControlStatics.MindControl__Movement))
                {
                    TempData["Error"] = "You try to move but discover you cannot!";
                    TempData["SubError"] = "Some other mage has partial control of your mind, disabling your ability to move on your own!";
                    return RedirectToAction(MVC.PvP.Play());
                }
                else if (!MindControlProcedures.PlayerIsMindControlledWithSomeType(me, myExistingMCs))
                {
                    // turn off mind control is the player has no more MC effects on them
                    MindControlProcedures.ClearPlayerMindControlFlagIfOn(me);
                    me.MindControlIsActive = false;
                }
            }

            var buffs = ItemProcedures.GetPlayerBuffs(me.Id);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new Move { PlayerId = me.Id, Buffs = buffs, destination = locname });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }
            
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult EnterDungeon(string entering)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.LogIP(Request.GetRealUserHostAddress(), myMembershipId);

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);


            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to enter or exit the dungeon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you enter or leave the dungeon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can enter or leave the dungeon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has enough action points
            if (me.ActionPoints < 30)
            {
                TempData["Error"] = "You need 30 action points to enter or exit the dungeon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is in PvP mode
            if (me.GameMode < (int)GameModeStatics.GameModes.PvP)
            {
                TempData["Error"] = "You must be in PvP mode in order to enter the dungeon.  It is not a safe place...";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is in a correct location to do this if in overworld
            if (!me.IsInDungeon() && (me.dbLocationName != "street_9th" && me.dbLocationName != "street_14th_north"))
            {
                TempData["Error"] = "You cannot enter the dungeon here.";
                TempData["SubError"] = "You must be at Street: Main Street and Sunnyglade Drive Intersection or Street: Main Street and E. 9th Avenue Intersection in order to enter the dungeon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not been in combat recently if trying to enter OR leave the dungeon
            var lastAttackTimeAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
            if (lastAttackTimeAgo < 30)
            {
                TempData["Error"] = "You have been in combat too recently in order to enter or leave the dungeon right now.";
                TempData["SubError"] = "You must stay out of combat for another " + (30 - lastAttackTimeAgo) + " minutes.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (entering == "true")
            {

                // give player the Vanquish spell if they don't already know it
                SkillProcedures.GiveSkillToPlayer(me.Id, PvPStatics.Dungeon_VanquishSpellSourceId);

                var dungeonLocation = LocationsStatics.GetRandomLocation_InDungeon();
                PlayerProcedures.TeleportPlayer(me, dungeonLocation, false);
                TempData["Result"] = "You slipped down a manhole, tumbling through a dark tunnel and ending up down in the otherworldly dungeon deep below Sunnyglade, both physically and dimensionally.  Be careful where you tread... danger could come from anywhere and the magic down here is likely to keep you imprisoned much longer of permanently should you find yourself defeated...";
                PlayerLogProcedures.AddPlayerLog(me.Id, "You entered the dungeon.", false);
                LocationLogProcedures.AddLocationLog(me.dbLocationName, me.GetFullName() + " slid down a manhole to the dungeon deep below.");
                LocationLogProcedures.AddLocationLog(dungeonLocation, me.GetFullName() + " fell through the a portal in the ceiling from the town above.");
            }
            else if (entering == "false")
            {
                var overworldLocation = LocationsStatics.GetRandomLocationNotInDungeon();
                PlayerProcedures.TeleportPlayer(me, overworldLocation, false);
                TempData["Result"] = "Gasping for fresh air, you use your magic to tunnel your way up and out of the hellish labrynth of the dungeon.  ";
                PlayerLogProcedures.AddPlayerLog(me.Id, "You left the dungeon.", false);
                LocationLogProcedures.AddLocationLog(me.dbLocationName, me.GetFullName() + " cast an earthmoving spell, tunneling back up to the town.");
                LocationLogProcedures.AddLocationLog(overworldLocation, me.GetFullName() + " slides out from a portal out from the dungeon.");
            }

            PlayerProcedures.ChangePlayerActionMana(30, 0, 0, me.Id);


            return RedirectToAction(MVC.PvP.Play());
        }
        
        public virtual ActionResult AttackModal(int targetId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var target = PlayerProcedures.GetPlayer(targetId);
            IEnumerable<SkillViewModel> output = new List<SkillViewModel>();

            ViewBag.Recovered = false;
            ViewBag.RecoveredMsg = "";
            ViewBag.ManaCost = AttackProcedures.GetSpellManaCost(me, target);

            // make sure a no-attack exists due to the Back On Your Feet perk

            if (EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_Back_On_Your_Feet) && target.BotId == AIStatics.ActivePlayerBotId)
            {
                ViewBag.Recovered = true;
                ViewBag.RecoveredMsg = "You can't attack as you have the <b>Back On Your Feet</b> effect, preventing you from attacking another human-controlled player.";
            }
            else if (EffectProcedures.PlayerHasEffect(target, PvPStatics.Effect_Back_On_Your_Feet))
            {
                ViewBag.Recovered = true;
                ViewBag.RecoveredMsg = "You can't attack <b>" + target.GetFullName() + "</b> since they have the <b>Back On Your Feet</b> effect, preventing human-controlled players from attacking them until the effect expires.";
            }
            else
            {
                output = SkillProcedures.GetSkillViewModelsOwnedByPlayer(me.Id).Where(s => !s.dbSkill.IsArchived);
            }

            // filter out spells that you can't use on your target
            if (FriendProcedures.PlayerIsMyFriend(me, target) || target.BotId < AIStatics.ActivePlayerBotId)
            {
                // do nothing, all spells are okay
            }

            // both players are in protection; only allow animate spells
            else if (me.GameMode == (int)GameModeStatics.GameModes.Protection && target.GameMode == (int)GameModeStatics.GameModes.Protection)
            {
                output = output.Where(s => s.MobilityType == PvPStatics.MobilityFull);
            }

            // attack or the target is in superprotection and not a friend or bot; no spells work
            else if (target.GameMode == (int)GameModeStatics.GameModes.Superprotection || (me.GameMode == (int)GameModeStatics.GameModes.Superprotection && target.BotId == AIStatics.ActivePlayerBotId))
            {
                output = output.Where(s => s.MobilityType == "NONEXISTANT");
            }

            // filter out MC spells for bots
            if (target.BotId < AIStatics.ActivePlayerBotId)
            {
                output = output.Where(s => s.MobilityType != PvPStatics.MobilityMindControl);
            }

            // only show inanimates for rat thieves
            if (target.BotId == AIStatics.MaleRatBotId || target.BotId == AIStatics.FemaleRatBotId)
            {
                output = output.Where(s => s.MobilityType == PvPStatics.MobilityInanimate);
            }

            // only show Weaken for valentine
            if (target.BotId == AIStatics.ValentineBotId)
            {
                output = output.Where(s => s.dbSkill.SkillSourceId == PvPStatics.Spell_WeakenId);
            }

            // only bimbo spell works on nerd mouse boss
            if (target.BotId == AIStatics.MouseNerdBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == BossProcedures_Sisters.BimboSpellSourceId);
            }

            // only nerd spell works on nerd bimbo boss
            if (target.BotId == AIStatics.MouseBimboBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == BossProcedures_Sisters.NerdSpellSourceId);
            }

            // Vanquish and weaken only works against dungeon demons
            if (target.BotId == AIStatics.DemonBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == PvPStatics.Dungeon_VanquishSpellSourceId || s.StaticSkill.Id == PvPStatics.Spell_WeakenId);
            }

            // Filter out Vanquish when attacking non-Dungeon Demon player
            if (target.BotId != AIStatics.DemonBotId)
            {
                output = output.Where(s => s.StaticSkill.Id != PvPStatics.Dungeon_VanquishSpellSourceId);
            }

            // Fae-In-A-Bottle only works against Narcissa
            if (target.BotId == AIStatics.FaebossBotId)
            {
                output = output.Where(s => s.StaticSkill.Id == BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId);
            }

            // Filter out Fae-In-A-Bottle when attacking non-Narcissa player
            if (target.BotId != AIStatics.FaebossBotId)
            {
                output = output.Where(s => s.StaticSkill.Id != BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId);
            }

            // only inanimate and animal spells work on minibosses, donna, and lovebringer
            if (AIStatics.IsAMiniboss(target.BotId) ||
                target.BotId == AIStatics.MotorcycleGangLeaderBotId ||
                target.BotId == AIStatics.BimboBossBotId)
            {
                output = output.Where(s => s.MobilityType == PvPStatics.MobilityInanimate || s.MobilityType == PvPStatics.MobilityPet);
            }

            ViewBag.TargetId = targetId;
            ViewBag.TargetName = target.GetFullName();
            ViewBag.BotId = target.BotId;
            return PartialView(MVC.PvP.Views.partial.AjaxAttackModal, output);
        }

        public virtual ActionResult Attack(int targetId, int spellSourceId)
        {
            var myMembershipId = User.Identity.GetUserId();
            PlayerProcedures.LogIP(Request.GetRealUserHostAddress(), myMembershipId);

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            #region validation checks



            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "attack"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player hasn't made too many attacks this update
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxAttacksPerUpdate)
            {
                TempData["Error"] = "You have attacked too much this update.";
                TempData["SubError"] = "You can only attack " + PvPStatics.MaxAttacksPerUpdate + " times per update.  Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that player has enough action points to attack
            if (me.ActionPoints < PvPStatics.AttackCost)
            {
                TempData["Error"] = "You don't have enough action points to attack.";
                TempData["SubError"] = "You will receive more action points next turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the attacker player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can attack someone.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player hasn't attacked in past second
            var secondsSinceLastAttack = Math.Abs(Math.Floor(me.LastCombatTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));

            if (secondsSinceLastAttack < 3)
            {
                TempData["Error"] = "You must wait at least three seconds between attacks.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is not too late in the round for this attack to happen
            var lastupdate = PvPWorldStatProcedures.GetLastWorldUpdate();
            var secondsAgo = Math.Abs(Math.Floor(lastupdate.Subtract(DateTime.UtcNow).TotalSeconds));

            if (secondsAgo > (TurnTimesStatics.GetTurnLengthInSeconds() - TurnTimesStatics.GetEndTurnNoAttackSeconds()) && !PvPStatics.ChaosMode)
            {
                TempData["Error"] = "It is too late into this turn to attack.";
                TempData["SubError"] = "You can't attack in the last " + TurnTimesStatics.GetEndTurnNoAttackSeconds() + " seconds of a turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (secondsAgo < TurnTimesStatics.GetStartTurnNoAttackSeconds())
            {
                TempData["Error"] = "It is too early into this turn to attack.";
                TempData["SubError"] = "You can't attack in the first " + TurnTimesStatics.GetStartTurnNoAttackSeconds() + " seconds of a turn.";
                return RedirectToAction(MVC.PvP.Play());
            }


            // assert that this player does have this skill
            var skillBeingUsed = SkillProcedures.GetSkillViewModel(spellSourceId, me.Id);
            if (skillBeingUsed == null)
            {
                TempData["Error"] = "You don't seem to have this spell.";
                TempData["SubError"] = "This spell may have run out of charges or have been forgotten.  You will need to relearn it.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var iAmWhitelisted = User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist);

            // assert player does not have more than 1 accounts already
            if (!iAmWhitelisted && PlayerProcedures.IsMyIPInUseAndAnimate(Request.GetRealUserHostAddress(), me))
            {
                TempData["Error"] = "This character looks like a multiple account, which is illegal.  This character will not be allowed to attack.";
                TempData["SubError"] = "You can only have 1 animate character in PvP mode and 1 animate character in Protection mode at a time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var targeted = PlayerProcedures.GetPlayer(targetId);

            // assert that the targeted player is not in a quest
            if (targeted.InQuest > 0)
            {
                TempData["Error"] = "Your target must finish their quest before you can attack them.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has enough mana to cast
            if ((float)me.Mana < AttackProcedures.GetSpellManaCost(me, targeted))
            {
                TempData["Error"] = "You don't have enough mana to cast this.";
                TempData["SubError"] = "You can recover mana using consumable items, meditating, or waiting for it to replenish over time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the player does not have the Back On Your Feet perk
            if (EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_Back_On_Your_Feet) && targeted.BotId == AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "The protective aura from your Back On Your Feet effect prevents this spell from working.";
                TempData["SubError"] = "You can remove this effect with a Hex-B-Gone moisturizer if you want to resume attacks on player-controlled targets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the target does not have the Back On Your Feet perk
            if (EffectProcedures.PlayerHasEffect(targeted, PvPStatics.Effect_Back_On_Your_Feet))
            {
                TempData["Error"] = "The protective aura from your target's Back On Your Feet effect prevents you from casting this spell.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target is still in the same room
            if (me.dbLocationName != targeted.dbLocationName)
            {
                TempData["Error"] = "Your target no longer seems to be here.";
                TempData["SubError"] = "Your target has probably left.  Maybe you can follow them and attack when you've caught up.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert no blacklist exists if player is in protection mode
            if (me.GameMode < (int)GameModeStatics.GameModes.PvP && BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, targeted, "attack"))
            {
                TempData["Error"] = "This player has blacklisted you or is on your own blacklist.";
                TempData["SubError"] = "You cannot attack Protection mode players who have blacklisted you.  Remove them from your blacklist or ask them to remove you from theirs.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target is not inanimate
            if (targeted.Mobility == PvPStatics.MobilityInanimate)
            {
                TempData["Error"] = "Your target is already inanimate.";
                TempData["SubError"] = "Transformation magic will have no effect on them anymore.  Someone else might have cast the final spell.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target is not an animal
            if (targeted.Mobility == PvPStatics.MobilityPet)
            {
                TempData["Error"] = "Your target is an animal.";
                TempData["SubError"] = "Transformation magic will have no effect on them anymore.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target is not offline
            if (PlayerProcedures.PlayerIsOffline(targeted))
            {
                TempData["Error"] = "This player is offline.";
                TempData["SubError"] = "Offline players can no longer be attacked.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player does not currently have a lock on their account
            if (me.FlaggedForAbuse)
            {
                TempData["Error"] = "This player has been flagged by a moderator for suspicious actions and is not allowed to attack at this time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the victim is not the own player
            if (targeted.Id == me.Id)
            {
                TempData["Error"] = "You can't cast magic on yourself.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if the spell is a curse, check that the target doesn't already have the effect
            if (skillBeingUsed.StaticSkill.GivesEffect != null)
            {
                if (EffectProcedures.PlayerHasEffect(targeted, skillBeingUsed.StaticSkill.GivesEffect))
                {
                    TempData["Error"] = "This target is already afflicted with this curse or else is still in the immune cooldown period of it.";
                    TempData["SubError"] = "You can always try again later...";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // if the spell is a form of mind control, check that the target is not a bot
            if (skillBeingUsed.MobilityType == PvPStatics.MobilityMindControl && targeted.BotId < AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "This target is immune to mind control.";
                TempData["SubError"] = "Mind control currently only works against human opponents.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if anyone is dueling, make sure they are in the combatants list
            if (me.InDuel > 0 || targeted.InDuel > 0)
            {
                if (DuelProcedures.PlayerIsNotInDuel(me, targeted))
                {
                    TempData["Error"] = "You or your target is in a duel that the other is not participating in.";
                    TempData["SubError"] = "Conclude all duels before attacks can resume.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                var duel = DuelProcedures.GetDuel(me.InDuel);
                var duelSecondsAgo = Math.Abs(Math.Floor(duel.LastResetTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));

                if (duelSecondsAgo < 20)
                {
                    TempData["Error"] = "You or your target is in a duel that the other is not participating in.";
                    TempData["SubError"] = "Conclude all duels before attacks can resume.";
                    return RedirectToAction(MVC.PvP.Play());
                }

            }

            var skillSource = SkillStatics.GetStaticSkill(spellSourceId);
            DbStaticForm futureForm = null;
            if (skillSource.FormSourceId != null)
            {
                futureForm = FormStatics.GetForm(skillSource.FormSourceId.Value);
            }

            // if the spell is a form of mind control, check that the target is not already afflicated with it
            if (me.MindControlIsActive && futureForm != null && MindControlProcedures.PlayerIsMindControlledWithType(targeted, futureForm.dbName))
            {
                TempData["Error"] = "This player is already under the influence of this type of mind control.";
                TempData["SubError"] = "You must wait for their current mind control of this kind to expire before attempting to seize control yourself.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if the spell is Vanquish, only have it work against demons
            if (skillSource.Id == PvPStatics.Dungeon_VanquishSpellSourceId && targeted.FormSourceId != PvPStatics.DungeonDemonFormSourceId)
            {
                TempData["Error"] = "Vanquish can only be cast against the Dark Demonic Guardians in the dungoen.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if the spell is Fae-in-a-Bottle, only have it work against Narcissa
            if (skillSource.Id == BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId && targeted.BotId != AIStatics.FaebossBotId)
            {
                TempData["Error"] = "This spell can only be cast against " + BossProcedures_FaeBoss.FirstName;
                return RedirectToAction(MVC.PvP.Play());
            }

            #region bot attack type checks
            // prevent low level players from taking on high level bots
            if (targeted.BotId < AIStatics.PsychopathBotId)
            {

                // disable attacks on "friendly" NPCs
                if (targeted.BotId == AIStatics.LindellaBotId ||
                    targeted.BotId == AIStatics.WuffieBotId ||
                    targeted.BotId == AIStatics.JewdewfaeBotId ||
                    targeted.BotId == AIStatics.BartenderBotId ||
                    targeted.BotId == AIStatics.LoremasterBotId)
                {
                    TempData["Error"] = "A little smile tells you it might just be a bad idea to try and attack this person...";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if (me.Level <= 3)
                {
                    TempData["Error"] = "You feel too intimdated by your target and find yourself unable to launch your spell.";
                    TempData["SubError"] = "You must gain some more experience before trying to take this target on.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                var npcAttackResult = DomainRegistry.Repository.FindSingle(new CanAttackNpcWithSpell { futureForm = futureForm, target = targeted, attacker = me, spellSourceId = skillSource.Id});

                if (npcAttackResult != String.Empty)
                {
                    TempData["SubError"] = npcAttackResult;
                    return RedirectToAction(MVC.PvP.Play());
                }

                // Valentine
                if (targeted.BotId == AIStatics.ValentineBotId)
                {

                    if (!BossProcedures_Valentine.IsAttackableInForm(me))
                    {
                        TempData["Error"] = BossProcedures_Valentine.GetWrongFormText();
                        TempData["SubError"] = "You will need to attack while in a different form.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    // only allow weakens against Valentine for now (replace with Duel spell later?)
                    if (futureForm != null)
                    {
                        TempData["Error"] = "You get the feeling this type of spell won't work against Lord Valentine.";
                        TempData["SubError"] = "Maybe a different one would do...";
                        return RedirectToAction(MVC.PvP.Play());
                    }
                }

            }
            #endregion


            // don't worry about bots
            if (targeted.BotId == AIStatics.ActivePlayerBotId)
            {

                if (me.GameMode < (int)GameModeStatics.GameModes.PvP || targeted.GameMode < (int)GameModeStatics.GameModes.PvP)
                {
                    if (FriendProcedures.PlayerIsMyFriend(me, targeted))
                    {
                        // do nothing; friends are okay to cast any spell types
                    }

                    // no inter protection/non spell casting
                    else if ((me.GameMode == (int)GameModeStatics.GameModes.PvP && targeted.GameMode < (int)GameModeStatics.GameModes.PvP) || (me.GameMode < (int)GameModeStatics.GameModes.PvP && targeted.GameMode == (int)GameModeStatics.GameModes.PvP))
                    {
                        TempData["Error"] = "You must be in the same Protection/non-Protection mode as your target in order to cast spells at them.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    // no casting spells on non-friend Protection mode players unless the target is a bot
                    else if (targeted.GameMode == (int)GameModeStatics.GameModes.Superprotection || (me.GameMode == (int)GameModeStatics.GameModes.Superprotection && targeted.BotId == AIStatics.ActivePlayerBotId))
                    {
                        TempData["Error"] = "Either you and your target is in SuperProtection mode and are not friends or bots.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    // no weaken between Protection mode players
                    else if (skillSource.Id == PvPStatics.Spell_WeakenId)
                    {
                        TempData["Error"] = "You cannot cast Weaken against protection mode players unless they are your friend.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    //  if the form is null (curse) or not fully animate, block it entirely
                    else if (futureForm.MobilityType == null || futureForm.MobilityType != PvPStatics.MobilityFull)
                    {
                        TempData["Error"] = "This player is in protection and immune from inanimate and animal spells except by their friends.";
                        return RedirectToAction(MVC.PvP.Play());
                    }
                }

            }




            #endregion

            try
            {
                TempData["Result"] = AttackProcedures.Attack(me, targeted, skillBeingUsed);

                // record into statistics
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__SpellsCast, 1);

                if (AIStatics.IsABoss(targeted.BotId))
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__BossAllAttacks, 1);
                }

                if (targeted.BotId == AIStatics.FemaleRatBotId || targeted.BotId == AIStatics.MaleRatBotId)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__BossRatThiefAttacks, 1);
                }
                else if (targeted.BotId == AIStatics.BimboBossBotId)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__BossLovebringerAttacks, 1);
                }
                else if (targeted.BotId == AIStatics.DonnaBotId)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__BossDonnaAttacks, 1);
                }
                else if (targeted.BotId == AIStatics.FaebossBotId)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__FaebossAttacks, 1);
                }
                else if (targeted.BotId == AIStatics.MouseNerdBotId || targeted.BotId == AIStatics.MouseBimboBotId)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__MouseSisterAttacks, 1);
                }
                else if (targeted.BotId == AIStatics.MotorcycleGangLeaderBotId)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__MotorcycleGangAttacks, 1);
                }
                else if (AIStatics.IsAMiniboss(targeted.BotId))
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__MinibossAttacks, 1);
                }

            }
            catch (Exception e)
            {
                TempData["Error"] = "There was a server error while carrying out your attack.  Reason:  <br><br>" + e;
            }

            AIProcedures.CheckAICounterattackRoutine(me, targeted);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult EnchantLocation()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to attempt to enchant a location.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can enchant any locations.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can drop any items or release your pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is a high enough level
            if (me.Level < 3)
            {
                TempData["Error"] = "You must be at least level 3 in order to try and enchant a location.";
                return RedirectToAction(MVC.PvP.Play());
            }


            // assert player has enough mana
            if (me.Mana < 10)
            {
                TempData["Error"] = "Not enough mana.";
                TempData["SubError"] = "You need at least 10 mana to enchant a location.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has enough AP
            if (me.ActionPoints < 3)
            {
                TempData["Error"] = "Not enough AP.";
                TempData["SubError"] = "You need at least 3 action points to enchant a location.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is in PvP mode
            if (me.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                TempData["Error"] = "You must be in PvP mode in order to enchant a location.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is in a covenant
            if (me.Covenant <= 0)
            {
                TempData["Error"] = "You must be in a covenant in order to attempt to enchant a location.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player hasn't made too many attacks this update
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxAttacksPerUpdate)
            {
                TempData["Error"] = "You have attacked too much this update.";
                TempData["SubError"] = "You can only attack " + PvPStatics.MaxAttacksPerUpdate + " times per update.  Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that player has enough action points to attack
            if (me.ActionPoints < PvPStatics.AttackCost)
            {
                TempData["Error"] = "You don't have enough action points to attack.";
                TempData["SubError"] = "You will receive more action points next turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player update is in not in progress
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the player does not have the Back On Your Feet perk
            if (EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_Back_On_Your_Feet))
            {
                TempData["Error"] = "The protective aura from your Back On Your Feet effect prevents this spell from working.";
                TempData["SubError"] = "You can remove this effect with a Hex-B-Gone moisturizer if you want to resume attacks on player-controlled targets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not in the dungeon
            var myLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
            if (myLocation.Region == "dungeon")
            {
                TempData["Error"] = "You can't enchant in the dungeon.";
                TempData["SubError"] = "You can only enchant locations in the overworld.  The magic down here is too strong.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is not too late in the round for this attack to happen
            var lastupdate = PvPWorldStatProcedures.GetLastWorldUpdate();
            var secondsAgo = Math.Abs(Math.Floor(lastupdate.Subtract(DateTime.UtcNow).TotalSeconds));

            if (secondsAgo > (TurnTimesStatics.GetTurnLengthInSeconds() - TurnTimesStatics.GetEndTurnNoAttackSeconds()) && !PvPStatics.ChaosMode)
            {
                TempData["Error"] = "It is too late into this turn to enchant.";
                TempData["SubError"] = "You can't enchant in the last " + TurnTimesStatics.GetEndTurnNoAttackSeconds() + " seconds of a turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is not too EARLY in the round for this attack to happen
            var secondsFrom = Math.Abs(Math.Floor(lastupdate.Subtract(DateTime.UtcNow).TotalSeconds));

            if (secondsAgo < TurnTimesStatics.GetStartTurnNoAttackSeconds())
            {
                TempData["Error"] = "It is too early into this turn to enchant.";
                TempData["SubError"] = "You can't enchant in the first " + TurnTimesStatics.GetStartTurnNoAttackSeconds() + " seconds of a turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the location is not a covenant's safeground
            if (CovenantProcedures.ACovenantHasASafegroundHere(me.dbLocationName))
            {
                TempData["Error"] = "This location is the safeground of another covenant.";
                TempData["SubError"] = "You cannot take over a location with a safeground established there.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player's covenant does have a safeground
            if (me.Covenant == null || !CovenantProcedures.CovenantHasSafeground((int)me.Covenant))
            {
                TempData["Error"] = "Your covenant must have established a safeground before it can enchant locations.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var myBuffs = ItemProcedures.GetPlayerBuffs(me);

            var output = CovenantProcedures.AttackLocation(me, myBuffs);

            PlayerProcedures.AddAttackCount(me);
            PlayerProcedures.ChangePlayerActionMana(3, 0, -10, me.Id);

            // record into statistics
            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TimesEnchanted, 1);

            TempData["Result"] = output;
            return RedirectToAction(MVC.PvP.Play());
        }

        /// <summary>
        /// Allow a player to use up AP, mana, and cleanse/meditate in order to attempt to restore themself to their base form.
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult SelfRestore()
        {

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert update stage is not currently in player update
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is in an okay form to do this
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to attempt to attempt to restore yourself to your base form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can attempt to attempt to restore yourself to your base form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you cattempt to attempt to restore yourself to your base form";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already in their base form
            if (me.FormSourceId == me.OriginalFormSourceId)
            {
                TempData["Error"] = "You are already in your base form!";
                return RedirectToAction(MVC.PvP.Play());
            }

            var buffs = ItemProcedures.GetPlayerBuffs(me);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new SelfRestoreToBase { PlayerId = me.Id, Buffs = buffs });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        
        public virtual ActionResult Meditate()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var mybuffs = ItemProcedures.GetPlayerBuffs(me);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new Meditate { PlayerId = me.Id, Buffs = mybuffs });
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }
        }

        public virtual ActionResult Cleanse()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var mybuffs = ItemProcedures.GetPlayerBuffs(me);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new Cleanse { PlayerId = me.Id, Buffs = mybuffs });
                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

        }

        public virtual ActionResult MySkills()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            ViewBag.TotalLearnableSkills = SkillProcedures.GetCountOfLearnableSpells();
            var mySkills = DomainRegistry.Repository.Find(new GetSkillsOwnedByPlayer {playerId = me.Id});
            return View(MVC.PvP.Views.MySkills, new MySkillsViewModel(mySkills));
        }

        public virtual ActionResult Search()
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "search"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can search your environment.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can drop any items or release your pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var mybuffs = ItemProcedures.GetPlayerBuffs(me);

            var searchCostAfterbuffs = PvPStatics.SearchAPCost;
            if (mybuffs.HasSearchDiscount)
            {
                searchCostAfterbuffs = 3;
            }

            // assert player is not in the dungeon
            if (me.IsInDungeon())
            {
                TempData["Error"] = "The constantly shifting chambers and corridors of the dungeon make searching unlikely to find anything down here.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has sufficient action points to search
            if (me.ActionPoints < searchCostAfterbuffs)
            {
                TempData["Error"] = "You don't have enough action points to search.";
                TempData["SubError"] = "Wait a while; you will receive more over time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (mybuffs.HasSearchDiscount)
            {
                PlayerProcedures.ChangePlayerActionMana(PvPStatics.SearchAPCost - 1, 0, 0, me.Id);
            }
            else
            {
                PlayerProcedures.ChangePlayerActionMana(PvPStatics.SearchAPCost, 0, 0, me.Id);
            }


            TempData["Result"] = PlayerProcedures.SearchLocation(me, me.dbLocationName, mybuffs.FindSpellsOnly);

            // write to logs
            var locationLogMessage = "<span class='playerSearchingNotification'>" + me.GetFullName() + " searched here.</span>";
            LocationLogProcedures.AddLocationLog(me.dbLocationName, locationLogMessage);
            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
            var playerLogMessage = "You searched at " + here.Name + ".";
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);

            // record into statistics
            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__SearchCount, 1);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ClearLog()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            PlayerLogProcedures.ClearPlayerLog(me.Id);
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult DismissNotifications_Ajax()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            PlayerLogProcedures.DismissImportantLogs(me.Id);
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public virtual ActionResult ViewLog()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            return View(MVC.PvP.Views.ViewLog, PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse());
        }

        public virtual ActionResult Take(int id)
        {
            var myMembershipId = User.Identity.GetUserId();

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var myBuffs = ItemProcedures.GetPlayerBuffs(me);

            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "pickup"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can pick anything up off of the ground.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can drop any items or release your pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the player is not mind controlled and cannot pick up anything on their own
            if (me.MindControlIsActive)
            {

                var myExistingMCs = MindControlProcedures.GetAllMindControlsWithPlayer(me);

                if (MindControlProcedures.PlayerIsMindControlledWithType(me, myExistingMCs, MindControlStatics.MindControl__Strip))
                {
                    TempData["Error"] = "You try to take it but find you cannot!";
                    TempData["SubError"] = "Some other mage has partial control of your mind, disabling your ability to pick anything up off the ground or tame any pets!";
                    return RedirectToAction(MVC.PvP.Play());
                }
                else if (!MindControlProcedures.PlayerIsMindControlledWithSomeType(me, myExistingMCs))
                {
                    // turn off mind control is the player has no more MC effects on them
                    var isNowFree = MindControlProcedures.ClearPlayerMindControlFlagIfOn(me);
                    me.MindControlIsActive = false;
                }
            }

            var cmd = new GetItem { ItemId = id };

            var pickup = DomainRegistry.Repository.FindSingle(cmd);

            //assert that the item is indeed at this location and on the ground
            if (pickup.dbLocationName != me.dbLocationName)
            {
                TempData["Error"] = "That item isn't in this location or else it has already been picked up.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the player is not carrying too much already UNLESS the item is a pet OR dungeon token
            if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, myBuffs) && pickup.ItemSource.ItemType != PvPStatics.ItemType_Pet && pickup.ItemSource.Id != PvPStatics.ItemType_DungeonArtifact_Id)
            {
                TempData["Error"] = "You are carrying too many items to pick this up.";
                TempData["SubError"] = "Use, drop, or wear/equip something you are carrying to make more room.  Some accessories may also allow you to carry more.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if the item is an animal, assert that the player does not already have one since pets must be automatically equipped
            if (pickup.ItemSource.ItemType == PvPStatics.ItemType_Pet && ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet) > 0)
            {
                TempData["Error"] = "You already have an animal or familiar as your pet.";
                TempData["SubError"] = "Release any existing pets you have before you can tame this one.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the item is not a consumable type or else is AND is in the same mode as the player (GameMode 2 is PvP)
            if ((pickup.PvPEnabled == 2 && me.GameMode != (int)GameModeStatics.GameModes.PvP) || (pickup.PvPEnabled == 1 && me.GameMode == (int)GameModeStatics.GameModes.PvP))
            {
                TempData["Error"] = "This item is marked as being in a different PvP mode from you.";
                TempData["SubError"] = "You are not allowed to pick up items that are not in PvP if you are not in PvP and the same for non-PvP.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var playerLogMessage = "";
            var locationLogMessage = "";
            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            // item is a dungeon artifact; immediately give the points to the player and delete it
            if (pickup.ItemSource.Id == PvPStatics.ItemType_DungeonArtifact_Id)
            {
                PlayerProcedures.GivePlayerPvPScore_NoLoser(me, PvPStatics.DungeonArtifact_Value);
                ItemProcedures.DeleteItem(pickup.Id);
                TempData["Result"] = "You pick up the artifact.  As soon as it touches your hands, it fades away, leaving you with its dark power.";
                playerLogMessage = "You picked up a <b>" + pickup.ItemSource.FriendlyName + "</b> at " + here.Name + " and absorbed its dark power into your soul.";
                locationLogMessage = me.GetFullName() + " picked up a <b>" + pickup.ItemSource.FriendlyName + "</b> here and immediately absorbed its dark powers.";

                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__DungeonArtifactsFound, 1);

                EffectProcedures.GivePerkToPlayer(PvPStatics.Dungeon_ArtifactCurse, me);

            }

            // if the item is inanimate, give the item to the player's inventory
            else if (pickup.ItemSource.ItemType != PvPStatics.ItemType_Pet)
            {
                TempData["Result"] = ItemProcedures.GiveItemToPlayer(pickup.Id, me.Id);
                playerLogMessage = "You picked up a <b>" + pickup.ItemSource.FriendlyName + "</b> at " + here.Name + " and put it into your inventory.";
                locationLogMessage = me.GetFullName() + " picked up a <b>" + pickup.ItemSource.FriendlyName + HtmlHelpers.PrintPvPIcon(pickup) + "</b> here.";
            }
            // item is an animal, equip it automatically
            else if (pickup.ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Result"] = ItemProcedures.GiveItemToPlayer(pickup.Id, me.Id);
                ItemProcedures.EquipItem(pickup.Id, true);
                playerLogMessage = "You tamed <b>" + pickup.FormerPlayer.FullName + " the " + pickup.ItemSource.FriendlyName + "</b> at " + here.Name + " and put it into your inventory.";
                locationLogMessage = me.GetFullName() + " tamed <b>" + pickup.FormerPlayer.FullName + " the " + pickup.ItemSource.FriendlyName + HtmlHelpers.PrintPvPIcon(pickup) + "</b> here.";

                var notificationMsg = me.GetFullName() + " has tamed you.  You will now follow them wherever they go and magically enhance their abilities by being their faithful companion.";
                PlayerLogProcedures.AddPlayerLog(pickup.FormerPlayer.Id, notificationMsg, true);

            }

            PlayerProcedures.AddMinutesToTimestamp(me, 15, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationLogMessage);


            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult Drop(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "drop"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can drop any items or release your pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can drop any items or release your pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var dropme = DomainRegistry.Repository.FindSingle(new GetItem {ItemId = itemId});

            // assert player does own this
            if (dropme.Owner.Id != me.Id)
            {
                TempData["Error"] = "You don't own that item.";

                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not currently wearing this UNLESS it is an animal type, since pets are always "equipped"
            if (dropme.IsEquipped && dropme.ItemSource.ItemType != PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "You can't drop this item.";
                TempData["SubError"] = "Unequip this item first if you are wearing it.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            if (here.Region != "dungeon")
            {
                // in overworld, drop at player's feet
                TempData["Result"] = ItemProcedures.DropItem(itemId);
            }
            else
            {
                // in dungeon, have it drop in a random place in the overworld
                var overworldLocation = LocationsStatics.GetRandomLocationNotInDungeon();
                var resultmsg = ItemProcedures.DropItem(itemId, overworldLocation);
                TempData["Result"] = resultmsg + "  It shimmers and falls through the dungeon floor, appearing somewhere in the town above.";
            }

            string playerLogMessage;
            string locationLogMessage;

            // animals are released
            if (dropme.ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                playerLogMessage = "You released your " + dropme.ItemSource.FriendlyName + " at " + here.Name + ".";
                locationLogMessage = me.GetFullName() + " released a <b>" + dropme.ItemSource.FriendlyName + HtmlHelpers.PrintPvPIcon(dropme) + "</b> here.";

                var notificationMsg = me.GetFullName() + " has released you.  You are now feral and may now wander the town at will until another master tames you.";
                PlayerLogProcedures.AddPlayerLog(dropme.FormerPlayer.Id, notificationMsg, true);


            }
            // everything else is dropped
            else
            {
                playerLogMessage = "You dropped a " + dropme.ItemSource.FriendlyName + " at " + here.Name + ".";
                locationLogMessage = me.FirstName + " " + me.LastName + " dropped a <b>" + dropme.ItemSource.FriendlyName + HtmlHelpers.PrintPvPIcon(dropme) + "</b> here.";
            }

            PlayerProcedures.AddMinutesToTimestamp(me, 15, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationLogMessage);

            return RedirectToAction(MVC.PvP.Play());

            // remove this item from the player's inventory

        }

        public virtual ActionResult Equip(int itemId, bool putOn)
        {
            var myMembershipId = User.Identity.GetUserId();

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "equip"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you equip or unequip any equipment.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you equip or unequip any equipment.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var item = ItemProcedures.GetItemViewModel(itemId);

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";

                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this item doesn't have the put on this turn lock
            if (item.dbItem.EquippedThisTurn)
            {
                TempData["Error"] = "You just put this on.";
                TempData["SubError"] = "You'll have to wait until next turn to take this off.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this item is not consumable
            if (item.Item.ItemType == PvPStatics.ItemType_Consumable || item.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable)
            {
                TempData["Error"] = "You can't equip or unequip consumables.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (item.Item.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "You can't equip or unequip a pet, only tame or release them.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the item is not a rune
            if (item.Item.ItemType == PvPStatics.ItemType_Rune)
            {
                TempData["Error"] = "You cannot equip runes.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (putOn)
            {

                // if item is not accessory, you can only wear one
                if (item.Item.ItemType != PvPStatics.ItemType_Accessory && (ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, item.Item.ItemType) > 0))
                {
                    TempData["Error"] = "You are already wearing a " + item.Item.ItemType + ".";
                    TempData["SubError"] = "Remove the one you are currently wearing first.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                // if item is an accessory, you can only wear two
                else if (item.Item.ItemType == PvPStatics.ItemType_Accessory && (ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, item.Item.ItemType) > 1))
                {
                    TempData["Error"] = "You are already equipped two accessories.";
                    TempData["SubError"] = "Remove at least one you are currently equipping first.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                // if item is an accessory, you can't wear two of the same thing
                if (item.Item.ItemType == PvPStatics.ItemType_Accessory && ItemProcedures.PlayerIsWearingNumberOfThisExactItem(me.Id, item.dbItem.dbName) == 1)
                {
                    TempData["Error"] = "You are already equipped with an accessory of this type.";
                    TempData["SubError"] = "You can't equip two of the same accessory at a time.";
                    return RedirectToAction(MVC.PvP.Play());
                }

            }
            else
            {

            }


            TempData["Result"] = ItemProcedures.EquipItem(itemId, putOn);

            return RedirectToAction(MVC.Item.MyInventory());

        }

        public virtual ActionResult Use(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (PvPStatics.AnimateUpdateInProgress)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "equip"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            var item = ItemProcedures.GetItemViewModel(itemId);

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn > 0)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type items next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            // assert that this item is of a consumable type (consumable or consumable-reusable)
            if (item.Item.ItemType != PvPStatics.ItemType_Consumable && item.Item.ItemType != PvPStatics.ItemType_Consumable_Reuseable)
            {
                TempData["Error"] = "You can't use that type of item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that if this item is of a reusable type that it's not on cooldown
            if (item.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable && item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "This item is still on cooldown and cannot be used again yet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if this item is a teleportation scroll, redirect to the teleportation page.
            if (item.dbItem.dbName == "item_consumeable_teleportation_scroll")
            {

                // assert that this player is not in a duel
                if (me.InDuel > 0)
                {
                    TempData["Error"] = "You must finish your duel before you use this item.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                // assert that this player is not in a quest
                if (me.InQuest > 0)
                {
                    TempData["Error"] = "You must finish your quest before you use this item.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if (me.IsInDungeon())
                {
                    var output = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region == "dungeon");
                    return View(MVC.PvP.Views.TeleportMap, output);

                }
                else
                {
                    var output = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region != "dungeon");
                    return View(MVC.PvP.Views.TeleportMap, output);
                }
            }

            // if this item is the self recaster, redirect to the animate spell listing page
            if (item.dbItem.dbName == "item_consumable_selfcaster")
            {
                return RedirectToAction(MVC.Item.SelfCast());
            }

            // if this item is a skill book, aka a tome, redirect to that page with the appropriate text
            if (item.dbItem.dbName.Contains("item_consumable_tome-"))
            {

                var cmd = new GetTomeByItem { ItemSourceId = item.Item.Id };
                var tome = DomainRegistry.Repository.FindSingle(cmd);

                var output = new SkillBookViewModel
                {
                    Text = tome.Text,
                    AlreadyRead = ItemProcedures.PlayerHasReadBook(me, item.dbItem.dbName),
                    BookId = item.dbItem.Id,
                };

                output.Text = output.Text.Replace(Environment.NewLine, "<br>");

                return View(MVC.Item.Views.SkillBook, output);
            }

            if (item.dbItem.dbName == "item_consumable_curselifter" || item.dbItem.dbName == "item_Butt_Plug_Hanna")
            {
                return RedirectToAction(MVC.Item.RemoveCurse());
            }

            if (item.dbItem.dbName == PvPStatics.ItemType_TGBomb)
            {
                try
                {
                    TempData["Result"] = DomainRegistry.Repository.Execute(new ThrowTGBomb { PlayerId = me.Id, ItemId = item.dbItem.Id });
                    return RedirectToAction(MVC.PvP.Play());
                }
                catch (DomainException e)
                {
                    TempData["Error"] = e.Message;
                    return RedirectToAction(MVC.PvP.Play());
                }

            }

            var result = ItemProcedures.UseItem(itemId, myMembershipId);

            PlayerProcedures.AddMinutesToTimestamp(me, 15, true);
            PlayerProcedures.AddItemUses(me.Id, 1);

            TempData["Result"] = result;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            PlayerLogProcedures.AddPlayerLog(me.Id, result, false);

            return RedirectToAction(MVC.Item.MyInventory());

        }

        public virtual ActionResult LookAtPlayer_FromMembershipId(string id)
        {
            var player = PlayerProcedures.GetPlayerFromMembership(id);

            if (player == null)
            {
                TempData["Error"] = "Unfortunately it seems this player does have have a character this round.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return RedirectToAction(MVC.PvP.LookAtPlayer(player.Id));
        }

        public virtual ActionResult LookAtPlayer(int id)
        {

            var playerLookedAt = PlayerProcedures.GetPlayerFormViewModel(id);

            var output = new PlayerFormItemsSkillsViewModel
            {
                PlayerForm = playerLookedAt,
                Skills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(id),
                Items = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer{OwnerId = playerLookedAt.Player.Id}).Where(i => i.IsEquipped == true),
                Bonuses = ItemProcedures.GetPlayerBuffs(playerLookedAt.Player.ToDbPlayer())
            };


            ViewBag.HasBio = SettingsProcedures.PlayerHasBio(output.PlayerForm.Player.MembershipId);
            ViewBag.HasArtistAuthorBio = SettingsProcedures.PlayerHasArtistAuthorBio(output.PlayerForm.Player.MembershipId);

            ViewBag.TimeUntilLogout = TurnTimesStatics.GetOfflineAfterXMinutes() - Math.Abs(Math.Floor(playerLookedAt.Player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (playerLookedAt.Form.MobilityType == PvPStatics.MobilityInanimate || playerLookedAt.Form.MobilityType == PvPStatics.MobilityPet)
            {


                var playerItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = playerLookedAt.Player.Id});

                if (playerLookedAt.Form.MobilityType == PvPStatics.MobilityInanimate)
                {
                    ViewBag.ImgUrl = "itemsPortraits/" + playerItem.ItemSource.PortraitUrl;
                }
                else if (playerLookedAt.Form.MobilityType == PvPStatics.MobilityPet)
                {
                    ViewBag.ImgUrl = "animalPortraits/" + playerItem.ItemSource.PortraitUrl;
                }


                ViewBag.ItemLevel = playerItem.Level;
                ViewBag.FormDescriptionItem = playerItem.ItemSource.Description;

                if (playerItem.ItemSource.ItemType == PvPStatics.ItemType_Pet)
                {
                    ViewBag.IsWorn = playerItem.Owner == null
                        ? "This creature has not been tamed as is running around feral."
                        : "This creature has been tamed and is following their master.";
                }
                else
                {
                    ViewBag.IsWorn = playerItem.Owner == null
                        ? "This item is not currently owned and is lying around available to be claimed by whoever comes across them."
                        : "This item is currently being carried and possibly worn by another player.";
                }




                return View(MVC.PvP.Views.LookAtPlayerInanimate, output);
            }
            else
            {

                ViewBag.AtLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == output.PlayerForm.Player.dbLocationName).Name;

                return View(MVC.PvP.Views.LookAtPlayer, output);
            }
        }

        public virtual ActionResult PlayerLookup(string name)
        {
            return View(MVC.PvP.Views.PlayerLookup, name);
        }

        public virtual ActionResult PlayerLookupSend(PlayerSearchViewModel results)
        {
            var result = PlayerProcedures.GetPlayersWithPartialName(results.FirstName);
            if (result != null && result.Any())
            {
                results.PlayersFound = result;
                results.FoundThem = true;
            }
            else
            {
                results.FoundThem = false;
            }
            return View(MVC.PvP.Views.PlayerLookup, results);
        }

        public virtual ActionResult InanimateAction(string actionName)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var wearer = ItemProcedures.BeingWornBy(me);
            var meItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});

            // assert that player is inanimate
            if (me.Mobility != PvPStatics.MobilityInanimate)
            {
                TempData["Error"] = "You are not inanimate";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is owned by wearer
            if (meItem.Owner.Id != wearer.Player.Id)
            {
                TempData["Error"] = "You are not currently owned by this player.";
                TempData["SubError"] = "Your former owner must have dropped you or was transformed themself.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not acted too many times already
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You don't have enough energy to physically or psychically interact with your owner right now.";
                TempData["SubError"] = "Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has submitted their captcha recently
            if (FeatureContext.IsEnabled<UseCaptcha>() &&
                DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired { UserId = me.MembershipId }))
            {
                TempData["Error"] = "Please complete this captcha on the page.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var thirdP = "";
            var firstP = "";
            var pronoun = wearer.Player.Gender == PvPStatics.GenderFemale ? "She" : "He";

            if (actionName == "rub")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, .25M, 0, wearer.Player.Id);
                thirdP = "<span class='petActionGood'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly rubbing against your skin affectionately.  You gain a tiny amount of willpower from your inanimate belonging's subtle but kind gesture.</span>";
                firstP = "You affectionately rub against your current owner, " + wearer.Player.GetFullName() + ".  " + pronoun + " gains a tiny amount of willpower from your subtle but kind gesture.";
            }

            if (actionName == "pinch")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, -.15M, 0, wearer.Player.Id);
                thirdP = "<span class='petActionBad'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly pinch your skin agitatedly.  You lose a tiny amount of willpower from your inanimate belonging's subtle but pesky gesture.</span>";
                firstP = "You agitatedly pinch against your current owner, " + wearer.Player.GetFullName() + ".  " + pronoun + " loses a tiny amount of willpower from your subtle but pesky gesture.";
            }

            if (actionName == "soothe")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, 0, .25M, wearer.Player.Id);
                thirdP = "<span class='petActionGood'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly peacefully soothe your skin.  You gain a tiny amount of mana from your inanimate belonging's subtle but kind gesture.</span>";
                firstP = "You kindly soothe a patch of your current owner, " + wearer.Player.GetFullName() + "'s skin.  " + pronoun + " gains a tiny amount of mana from your subtle but kind gesture.";
            }

            if (actionName == "zap")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, 0, -.15M, wearer.Player.Id);
                thirdP = "<span class='petActionBad'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly zap your skin.  You lose a tiny amount of mana from your inanimate belonging's subtle but pesky gesture.</span>";
                firstP = "You agitatedly zap a patch of your current owner, " + wearer.Player.GetFullName() + "'s skin.  " + pronoun + " loses a tiny amount of mana from your subtle but pesky gesture.";
            }

            PlayerProcedures.LogIP(Request.GetRealUserHostAddress(), myMembershipId);
            var leveluptext = InanimateXPProcedures.GiveInanimateXP(me.MembershipId, User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist));

            TempData["Result"] = firstP + leveluptext;
            PlayerProcedures.AddAttackCount(me);
            PlayerLogProcedures.AddPlayerLog(wearer.Player.Id, thirdP, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, firstP, true);

            ItemProcedures.UpdateSouledItem(meItem.Id);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult AnimalAction(string actionName, int targetId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is in an okay form to do this
            if (!PlayerCanPerformAction(me, "animalAction"))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that player is an animal
            if (me.Mobility != PvPStatics.MobilityPet)
            {
                TempData["Error"] = "You are not an animal or pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player hasn't made too many attacks this update
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You have interacted too much this update.";
                TempData["SubError"] = "You can only interact 1 times per update as an animal.  Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has submitted their captcha recently
            if (FeatureContext.IsEnabled<UseCaptcha>() &&
                DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired { UserId = me.MembershipId }))
            {
                TempData["Error"] = "Please complete this captcha on the page to continue with this action.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target is still in the same room
            var targeted = PlayerProcedures.GetPlayer(targetId);

            if (me.Mobility == PvPStatics.MobilityFull)
            {
                if (me.dbLocationName != targeted.dbLocationName)
                {
                    TempData["Error"] = "Your target no longer seems to be here.";
                    TempData["SubError"] = "Your target has probably left.  Maybe you can follow them and attack when you've caught up.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else if (me.Mobility == PvPStatics.MobilityPet)
            {
                var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
                if (here.dbName != targeted.dbLocationName)
                {
                    TempData["Error"] = "Your target no longer seems to be here.";
                    TempData["SubError"] = "Your target has probably left.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // assert that the target is not inanimate
            if (targeted.Mobility == PvPStatics.MobilityInanimate)
            {
                TempData["Error"] = "Your target is inanimate";
                TempData["SubError"] = "You can't interact with inanimate players.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the target is not an animal
            if (targeted.Mobility == PvPStatics.MobilityPet)
            {
                TempData["Error"] = "Your target is already an animal";
                TempData["SubError"] = "You can't interact with players turned into other animals.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player does not currently have a lock on their account
            if (me.FlaggedForAbuse)
            {
                TempData["Error"] = "This player has been flagged by a moderator for suspicious actions and is not allowed to attack at this time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the victim is not the own player
            if (targeted.Id == me.Id)
            {
                TempData["Error"] = "You can't cast magic on yourself..";
                return RedirectToAction(MVC.PvP.Play());
            }

            // all of our checks have passed, so now let's actually do the action
            PlayerProcedures.LogIP(Request.GetRealUserHostAddress(), myMembershipId);

            var result = AnimalProcedures.DoAnimalAction(actionName, me.Id, targeted.Id);
            var leveluptext = InanimateXPProcedures.GiveInanimateXP(me.MembershipId, User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist));

            TempData["Result"] = result + leveluptext;

            ItemProcedures.UpdateSouledItem(me);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult MyFriends()
        {
            var myMembershipId = User.Identity.GetUserId();
            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            var output = new FriendPageViewModel();

            var friends = FriendProcedures.GetMyFriends(myMembershipId);

            output.ConfirmedFriends = friends.Where(f => f.dbFriend.IsAccepted);

            output.RequestsForMe = friends.Where(f => !f.dbFriend.IsAccepted && (f.dbFriend.FriendMembershipId == myMembershipId));

            output.MyOutgoingRequests = friends.Where(f => !f.dbFriend.IsAccepted && (f.dbFriend.OwnerMembershipId == myMembershipId));

            return View(MVC.PvP.Views.MyFriends, output);
        }

        public virtual ActionResult AddFriend(int playerId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var friend = PlayerProcedures.GetPlayer(playerId);

            // assert no blacklist exists if player is in protection mode
            if (BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, friend, "any"))
            {
                TempData["Error"] = "This player has blacklisted you or is on your own blacklist.";
                TempData["SubError"] = "You cannot request friendship with players who have blacklisted you.  Remove them from your blacklist or ask them to remove you from theirs.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (FriendProcedures.AddFriend(friend, myMembershipId))
            {
                var message = me.GetFullName() + " has sent you a friend request.";

                if (!PlayerLogProcedures.PlayerAlreadyHasMessage(friend.Id, message))
                {
                    PlayerLogProcedures.AddPlayerLog(friend.Id, message, true);
                }
            }
            else
            {
                TempData["Error"] = "This player is already a friend.";
                TempData["SubError"] = "You cannot request friendship with an existing friend.";
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult RespondToFriendRequest(int id, string response)
        {
            var myMembershipId = User.Identity.GetUserId();
            if (response == "cancel")
            {
                FriendProcedures.CancelFriendRequest(id, myMembershipId);
            }
            else if (response == "deny")
            {
                FriendProcedures.CancelFriendRequest(id, myMembershipId);
            }
            else if (response == "defriend")
            {
                FriendProcedures.CancelFriendRequest(id, myMembershipId);
            }
            else if (response == "accept")
            {
                FriendProcedures.AcceptFriendRequest(id, myMembershipId);
            }


            return RedirectToAction(MVC.PvP.MyFriends());
        }

        private bool PlayerCanPerformAction(Player me, string actionType)
        {

            var myform = FormStatics.GetForm(me.FormSourceId);

            if (myform.MobilityType == PvPStatics.MobilityFull)
            {
                return true;
            }

            if (actionType == "move")
            {
                // inanimate
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't move.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    var meAnimal = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});
                    if (meAnimal.Owner != null)
                    {
                        TempData["Error"] = "You can't move by yourself.";
                        TempData["SubError"] = "You are an animal and are currently tamed as a pet.";
                        return false;
                    }

                }
            }
            else if (actionType == "attack")
            {
                // inanimate
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {

                    TempData["Error"] = "You can't cast any spells.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;

                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't cast any spells.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "meditate")
            {
                //inaniamte
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't meditate.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.  You're essentially already meditating... permanently..";
                    return false;
                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't meditate.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }

            else if (actionType == "cleanse")
            {
                // inanimate
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't cleanse.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't cleanse.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "search")
            {
                // inanimate
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't search.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't search.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "pickup")
            {
                // inanimate
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't pick any items up.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't pick anything up.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "drop")
            {
                // inanimate
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't drop any items.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't drop anything.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "equip")
            {
                if (myform.MobilityType == PvPStatics.MobilityInanimate)
                {
                    TempData["Error"] = "You can't equip or unequip any items.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
            }
            else if (actionType == "animalAction")
            {
                if (myform.MobilityType != PvPStatics.MobilityPet)
                {
                    TempData["Error"] = "You can't do that.";
                    TempData["SubError"] = "You are not an animal.";
                    return false;
                }
            }



            return true;
        }

        public virtual ActionResult WorldMap(string showEnchant)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Location here = null;

            IEnumerable<LocationInfo> ownerInfo = null;

            here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            if (showEnchant == "true")
            {
                ownerInfo = CovenantProcedures.GetLocationInfos();
            }

            var output = new MapViewModel
            {
                LocationInfo = ownerInfo,

            };

            if (me.IsInDungeon() && showEnchant == "false")
            {
                output.Locations = LocationsStatics.LocationList.GetLocation.Where(l => l.Region == "");
                ViewBag.IsInDungeon = true;
            }
            else if (me.IsInDungeon() && showEnchant == "true")
            {
                output.Locations = LocationsStatics.LocationList.GetLocation.Where(l => l.Region != "dungeon");
                ViewBag.IsInDungeon = false;
            }
            else
            {
                output.Locations = LocationsStatics.LocationList.GetLocation.Where(l => l.Region != "dungeon");
                ViewBag.IsInDungeon = false;
            }

            ViewBag.MapX = here.X;
            ViewBag.MapY = here.Y;
            return View(MVC.PvP.Views.WorldMap, output);
        }

        public virtual ActionResult LevelupPerk()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            ViewBag.PerksRemaining = me.UnusedLevelUpPerks;

            if (me.UnusedLevelUpPerks == 0)
            {
                TempData["Error"] = "You don't have any unused level up perks left to choose right now.  Gain another experience level for more.";
                return RedirectToAction(MVC.PvP.Play());
            }
            else
            {
                IEnumerable<DbStaticEffect> output = EffectProcedures.GetAvailableLevelupPerks(me);
                return View(MVC.PvP.Views.LevelupPerk, output);
            }


        }

        public virtual ActionResult ChoosePerk(string perk)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player does have unused perks
            if (me.UnusedLevelUpPerks < 1)
            {
                TempData["Error"] = "You don't have any unused level up perks left to choose right now.  Gain another experience level for more.";
                return RedirectToAction(MVC.PvP.Play());
            }

            //give perk to player
            TempData["Result"] = EffectProcedures.GivePerkToPlayer(perk, me);


            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult MyPerks()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var output = EffectProcedures.GetPlayerEffects2(me.Id);

            return View(MVC.PvP.Views.MyPerks, output);
        }

        public virtual ActionResult ViewEffects(int id)
        {
            var player = PlayerProcedures.GetPlayer(id);
            var output = EffectProcedures.GetPlayerEffects2(player.Id);
            ViewBag.PlayerName = player.GetFullName();

            return View(MVC.PvP.Views.ViewEffects, output);
        }

        public virtual ActionResult Teleport(string to)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // no need to assert player is mobile; inanimates and items have no inventory
            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can purchase or sell anything to Lindella.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can purchase or sell anything to Lindella.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player actually does own one of this
            if (ItemProcedures.PlayerHasNumberOfThisItem(me, "item_consumeable_teleportation_scroll") <= 0)
            {
                TempData["Error"] = "You don't have one of these.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn > 0)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            // assert player is not TPing into the dungeon from out in or vice versa
            var destinationIsInDungeon = false;
            if (to.Contains("dungeon_"))
            {
                destinationIsInDungeon = true;
            }
            if (me.IsInDungeon() != destinationIsInDungeon)
            {
                TempData["Error"] = "You can't teleport inside the dungeon from outside of it, nor can you teleport out of it from inside.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the location is a valid location
            if (!LocationsStatics.LocationList.GetLocation.Select(s => s.dbName).Contains(to))
            {
                TempData["Error"] = "That is not an eligible location to teleport to.";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = PlayerProcedures.TeleportPlayer(me, to, false);

            ItemProcedures.DeleteItemOfName(me, "item_consumeable_teleportation_scroll");

            PlayerProcedures.SetTimestampToNow(me);
            PlayerProcedures.AddItemUses(me.Id, 1);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TimesTeleported_Scroll, 1);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult FightTheTransformation()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is inanimate or an animal
            if (me.Mobility == PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You can't do this.";
                TempData["SubError"] = "You are still in an animate form.  You must be inanimate or an animal.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not acted too many times already
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You don't have enough energy to fight your transformation right now.";
                TempData["SubError"] = "Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already locked into their current form
            var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});
            if (itemMe.IsPermanent)
            {
                TempData["Error"] = "You cannot return to an animate form again.";
                TempData["SubError"] = "You have spent too long and performed too many actions as an item or animal and have lost your desire and ability to be human gain.";
                return RedirectToAction(MVC.PvP.Play());
            }


            var dungeonHalfPoints = false;

            // Give items/pets a struggle penalty if their owner isn't a bot and is in the dungeon
            if (itemMe.Owner != null)
            {
                var owner = PlayerProcedures.GetPlayer(itemMe.Owner.Id);
                if (owner.IsInDungeon())
                {
                    dungeonHalfPoints = true;
                }
            }


            TempData["Result"] = InanimateXPProcedures.ReturnToAnimate(me, dungeonHalfPoints);
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult CurseTransformOwner()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is inanimate or an animal
            if (me.Mobility != PvPStatics.MobilityInanimate && me.Mobility != PvPStatics.MobilityPet)
            {
                TempData["Error"] = "You can't do this.";
                TempData["SubError"] = "You are still in an animate form.  You must be inanimate or an animal.";
                return RedirectToAction(MVC.PvP.Play());
            }


            // assert player has not acted too many times already
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You don't have enough energy to try and transform your owner.";
                TempData["SubError"] = "Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has submitted their captcha recently
            if (FeatureContext.IsEnabled<UseCaptcha>() &&
                DomainRegistry.Repository.FindSingle(new UserCaptchaIsExpired { UserId = me.MembershipId }))
            {
                TempData["Error"] = "Please complete this captcha on the page.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});

            // assert item does have the ability to curse transform
            if (itemMe.ItemSource.CurseTFFormdbName.IsNullOrEmpty())
            {
                TempData["Error"] = "Unfortunately your new form does not have a transformation curse that it can use.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var owner = PlayerProcedures.GetPlayer(itemMe.Owner.Id);
            // assert player is owned
            if (itemMe.Owner == null)
            {
                TempData["Error"] = "You aren't owned by anyone.";
                TempData["SubError"] = "You don't currently belong to an owner and as such have nobody to curse.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert owner is not an invalid bot
            if (itemMe.Owner.BotId < AIStatics.PsychopathBotId)
            {
                TempData["Error"] = "Unfortunately it seems your owner is immune to your transformation curse!";
                TempData["SubError"] = "Only Psychopathic spellslingers and other players are susceptible to transformation curses.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert owner is animate (they always should be, but just in case...)
            if (itemMe.Owner.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "Your owner must be animate in order for you to curse transform them.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the form does exist
            var form = FormStatics.GetForm(itemMe.ItemSource.CurseTFFormSourceId.Value);
            if (form == null || form.IsUnique)
            {
                TempData["Error"] = "Unfortunately it seems that the animate form has either not yet been added to the game or is ineligible.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // all checks pass
            TempData["Result"] = InanimateXPProcedures.CurseTransformOwner(me, owner, itemMe, User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist));


            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult EscapeFromOwner()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is inanimate or an animal
            if (me.Mobility == PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You can't do this.";
                TempData["SubError"] = "You are still in an animate form.  You must be inanimate or an animal.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var inanimateMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});

            // assert that the player is owned
            if (inanimateMe.Owner == null)
            {
                TempData["Error"] = "You are not owned by anyone.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var owner = PlayerProcedures.GetPlayer(inanimateMe.Owner.Id);

            // if player is owned by a vendor, assert that the player has been in their inventory for sufficient amount of time
            if (owner.BotId == AIStatics.LindellaBotId || owner.BotId == AIStatics.WuffieBotId)
            {
                var hoursSinceSold = (int)Math.Floor(DateTime.UtcNow.Subtract(inanimateMe.LastSold).TotalHours);

                if (hoursSinceSold < PvPStatics.HoursBeforeInanimatesCanSlipFree)
                {
                    TempData["Error"] = "You cannot escape from your owner right now.";
                    TempData["SubError"] = "You must remain in the vendor's inventory for " + (PvPStatics.HoursBeforeInanimatesCanSlipFree - hoursSinceSold) + " more hours before you can slip free.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else
            {
                // assert that the owner has been sufficiently inactive only if player is not a vendor
                var hoursSinceLastActivity = -1 * (int)Math.Floor(owner.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalHours);
                if (hoursSinceLastActivity < PvPStatics.HoursBeforeInanimatesCanSlipFree)
                {
                    TempData["Error"] = "You cannot escape from your owner right now.";
                    TempData["SubError"] = "Your owner must remain inactive for " + (PvPStatics.HoursBeforeInanimatesCanSlipFree - hoursSinceLastActivity) + " more hours before you can slip free.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // don't allow items or pets to struggle while their owner is online in the dungeon
            if (owner.IsInDungeon() && !PlayerProcedures.PlayerIsOffline(owner))
            {
                TempData["Error"] = "The dark powers of the dungeon prevent you from being able to slip free while your owner is in the dungeon and online.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // all checks pass; drop item and notify owner
            ItemProcedures.DropItem(inanimateMe.Id);
            var inaniamteMePlus = ItemProcedures.GetItemViewModel(inanimateMe.Id);
            var message = me.GetFullName() + ", your " + inaniamteMePlus.Item.FriendlyName + ", slipped free due to your inactivity and can be claimed by a new owner.";
            PlayerLogProcedures.AddPlayerLog(owner.Id, message, true);

            TempData["Result"] = "You have slipped free from your owner.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ReserveName()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);



            // assert player is greater than level 3 if they are mobile
            if (me.Level < 3 && me.Mobility == PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be level 3 or greater in order to reserve a name.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if player is not mobile, see if the item they have become is at least level 3
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});
                if (itemMe.Level < 3)
                {
                    TempData["Error"] = "You must be level 3 or greater in order to reserve a name.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // strip out the roman numeral at the end of the last name if there is one
            var chunks = me.LastName.Split(' ');
            if (chunks.Any())
            {
                me.LastName = chunks[0];
            }

            IReservedNameRepository resNameRepo = new EFReservedNameRepository();

            var ghost = resNameRepo.ReservedNames.FirstOrDefault(rn => rn.MembershipId == me.MembershipId);

            if (ghost == null)
            {
                var newReservedName = new ReservedName
                {
                    FullName = me.FirstName + " " + me.LastName, // don't use GetFullName() so nickname is left out
                    MembershipId = me.MembershipId,
                    Timestamp = DateTime.UtcNow,
                };
                resNameRepo.SaveReservedName(newReservedName);
            }
            else
            {
                // check to make sure the name doesn't belong to someone else
                if (ghost.MembershipId != me.MembershipId)
                {
                    TempData["Error"] = "Unfortunately that name has already been reserved by someone else.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if (ghost.FullName == me.FirstName + " " + me.LastName)
                {
                    TempData["Result"] = "Your name has already been reserved.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                ghost.FullName = me.FirstName + " " + me.LastName;
                resNameRepo.SaveReservedName(ghost);
            }

            TempData["Result"] = "Your name has been reserved.";
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult Shout()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is mobile
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be fully animate in order to shout.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has shouts remaining
            if (me.ShoutsRemaining <= 0)
            {
                TempData["Error"] = "You do not have any shouts remaining for this turn.";
                TempData["SubError"] = "You will be able to shout more in future updates.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return View(MVC.PvP.Views.Shout);
        }

        public virtual ActionResult ShoutSend(PublicBroadcastViewModel input)
        {
            var myMembershipId = User.Identity.GetUserId();

            try
            {
                DomainRegistry.Repository.Execute(new Shout { Message = input.Message, UserId = myMembershipId });
                TempData["Result"] = "You shouted.";
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult FlagForSuspiciousActivity(int playerId)
        {
            // assert the person flagging has mod permissions
            // assert only admins can view this
            if (!User.IsInRole(PvPStatics.Permissions_Moderator) && !User.IsInRole(PvPStatics.Permissions_Admin))
            {
                return RedirectToAction(MVC.PvP.Play());
            }


            TempData["Result"] = "Player suspicious lock toggled.";
            PlayerProcedures.FlagPlayerForSuspicousActivity(playerId);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult Duel()
        {
            return RedirectToAction(MVC.Duel.Duel());
        }

        public virtual ActionResult Bus()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var canTakeBus = DomainRegistry.Repository.FindSingle(new PlayerIsAtBusStop { playerLocation = me.dbLocationName });

            if (!canTakeBus)
            {
                TempData["Error"] = "There is no bus stop here.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new BusStopsViewModel
            {
                Stops = DomainRegistry.Repository.Find(new GetBusStops { currentLocation = me.dbLocationName }).Where(b => b.Cost > 0).OrderBy(b => b.Cost),
                Player = DomainRegistry.Repository.FindSingle(new GetPlayerBusDetail { playerId = me.Id })
            };

            return View(MVC.PvP.Views.Bus, output);
        }

        public virtual ActionResult TakeBus(string destination)
        {

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            try
            {
                var output = DomainRegistry.Repository.Execute(new TakeBus { playerId = me.Id, destination = destination });
                TempData["Result"] = output;

                return RedirectToAction(MVC.PvP.Play());
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

        }

    }

}


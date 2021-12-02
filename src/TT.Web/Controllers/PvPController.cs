using FeatureSwitch;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Assets.Queries;
using TT.Domain.Concrete;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Commands;
using TT.Domain.Identity.Queries;
using TT.Domain.Items.Commands;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Messages.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Skills.Queries;
using TT.Domain.Statics;
using TT.Domain.TFEnergy.Commands;
using TT.Domain.ViewModels;
using TT.Domain.World.DTOs;
using TT.Domain.World.Queries;
using TT.Web.CustomHtmlHelpers;
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
            ViewBag.BodyClasses = $"location-{me.dbLocationName}";  // NB. This could leak location to owned items

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

            var unopenedMessageCount = DomainRegistry.Repository.FindSingle(new GetUnreadMessageCountByPlayer { OwnerId = me.Id });
            var openedUnreadMessageCount = DomainRegistry.Repository.FindSingle(new GetReadAndMarkedAsUnreadMessageCountByPlayer { OwnerId = me.Id });
            var hasNewMessages = unopenedMessageCount != 0;
            var unreadMessageCount = unopenedMessageCount + openedUnreadMessageCount;

            ItemDetail itemMe = null;
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });
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
                    Item = itemMe,
                    HasNewMessages = hasNewMessages,
                    UnreadMessageCount = unreadMessageCount,
                    PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse(),
                    StruggleChance = Math.Round(InanimateXPProcedures.GetStruggleChance(me, ItemProcedures.ItemIncursDungeonPenalty(itemMe))),
                    Message = InanimateXPProcedures.GetProspectsMessage(me)
                };
                inanimateOutput.PlayerLogImportant = inanimateOutput.PlayerLog.Where(l => l.IsImportant);
                inanimateOutput.PortraitUrl = ItemStatics.GetStaticItem(inanimateOutput.Form.ItemSourceId.Value).PortraitUrl;

                if (inanimateOutput.Item?.Owner == null)
                {
                    // Not owned
                    var loc = inanimateOutput.Item?.dbLocationName == null ? me.dbLocationName : inanimateOutput.Item.dbLocationName;
                    inanimateOutput.AtLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == loc)?.Name ?? "Unknown";
                    inanimateOutput.LocationLog = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = loc, ConcealmentLevel = 0 });
                    inanimateOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(loc, myMembershipId);

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
                    if (p.Player.Mobility == PvPStatics.MobilityFull &&
                        p.Player.GameMode != (int)GameModeStatics.GameModes.Invisible)
                    {
                        playersHere.Add(p);
                    }
                }

                inanimateOutput.PlayersHere = playersHere.OrderByDescending(p => p.Player.Level);

                inanimateOutput.HasOwnerChat = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id })
                    .Any(i => i.FormerPlayer != null &&
                              i.LastSouledTimestamp > DateTime.UtcNow.AddMinutes(-PvPStatics.Item_SoulActivityLevels_Minutes[PvPStatics.Item_SoulActivityLevels_Minutes.Count() - 1]));

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

                animalOutput.YouItem = itemMe;
                if (animalOutput.YouItem?.Owner != null)
                {
                    animalOutput.OwnedBy = PlayerProcedures.GetPlayerFormViewModel(animalOutput.YouItem.Owner.Id);

                    // move player over to owner
                    if (me.dbLocationName != animalOutput.OwnedBy.Player.dbLocationName)
                    {
                        PlayerProcedures.MovePlayer_InstantNoLog(me.Id, animalOutput.OwnedBy.Player.dbLocationName);
                    }

                }


                animalOutput.WorldStats = PlayerProcedures.GetWorldPlayerStats();

                string dbLocationName;
                if (animalOutput.OwnedBy != null)
                {
                    dbLocationName = animalOutput.OwnedBy.Player.dbLocationName;
                }
                else
                {
                    dbLocationName = me.dbLocationName;
                }

                animalOutput.Location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == dbLocationName);
                animalOutput.Location.FriendlyName_North = LocationsStatics.GetConnectionName(animalOutput.Location.Name_North);
                animalOutput.Location.FriendlyName_East = LocationsStatics.GetConnectionName(animalOutput.Location.Name_East);
                animalOutput.Location.FriendlyName_South = LocationsStatics.GetConnectionName(animalOutput.Location.Name_South);
                animalOutput.Location.FriendlyName_West = LocationsStatics.GetConnectionName(animalOutput.Location.Name_West);

                animalOutput.PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse();
                animalOutput.PlayerLogImportant = animalOutput.PlayerLog.Where(l => l.IsImportant);

                animalOutput.LocationLog = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = animalOutput.Location.dbName, ConcealmentLevel = 0 });

                var animalLocationItemsCmd = new GetItemsAtLocationVisibleToGameMode { dbLocationName = animalOutput.Location.dbName, gameMode = me.GameMode };
                animalOutput.LocationItems = DomainRegistry.Repository.Find(animalLocationItemsCmd);

                animalOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(animalOutput.Location.dbName, myMembershipId)
                                                .Where(p => p.Player.Mobility == PvPStatics.MobilityFull &&
                                                            p.Player.GameMode != (int)GameModeStatics.GameModes.Invisible);

                animalOutput.LastUpdateTimestamp = world.LastUpdateTimestamp;

                animalOutput.HasNewMessages = hasNewMessages;
                animalOutput.UnreadMessageCount = unreadMessageCount;

                animalOutput.HasOwnerChat = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id })
                    .Any(i => i.FormerPlayer != null &&
                              i.LastSouledTimestamp > DateTime.UtcNow.AddMinutes(-PvPStatics.Item_SoulActivityLevels_Minutes[PvPStatics.Item_SoulActivityLevels_Minutes.Count() - 1]));

                animalOutput.PortraitUrl = ItemStatics.GetStaticItem(animalOutput.Form.ItemSourceId.Value).PortraitUrl;

                animalOutput.IsPermanent = animalOutput.YouItem == null ? false : animalOutput.YouItem.IsPermanent;

                animalOutput.StruggleChance = Math.Round(InanimateXPProcedures.GetStruggleChance(me, ItemProcedures.ItemIncursDungeonPenalty(animalOutput.YouItem)));
                animalOutput.Message = InanimateXPProcedures.GetProspectsMessage(me);

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
            output.InventoryMaxSize = ItemProcedures.GetInventoryMaxSize(me);
            loadtime += "End get max inv. size:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.You = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            output.PlayerIsAtBusStop =
                DomainRegistry.Repository.FindSingle(new PlayerIsAtBusStop { playerLocation = me.dbLocationName });

            output.LastUpdateTimestamp = PvPWorldStatProcedures.GetLastWorldUpdate();

            loadtime += "Start get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(me.dbLocationName, myMembershipId)
                                        .Where(p => p.Player.Mobility == PvPStatics.MobilityFull &&
                                                    p.Player.GameMode != (int)GameModeStatics.GameModes.Invisible);
            loadtime += "End get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get player effects:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            var effects = EffectProcedures.GetPlayerEffects2(me.Id);
            output.Blind = effects.Where(e => e.dbEffect.EffectSourceId == CharacterPrankProcedures.BLINDED_EFFECT &&
                                              e.dbEffect.Duration > 0).Any();
            loadtime += "End get player effects:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.Location = LocationsStatics.LocationList.GetLocation.FirstOrDefault(x => x.dbName == me.dbLocationName).Clone();
            output.Location.CovenantController = CovenantProcedures.GetLocationCovenantOwner(me.dbLocationName);

            // Hide directions if player is blind
            if (output.Blind)
            {
                output.Location.Name = "Here";
                output.Location.FriendlyName_North = "North";
                output.Location.FriendlyName_East = "East";
                output.Location.FriendlyName_South = "South";
                output.Location.FriendlyName_West = "West";
                output.Location.Name_North = "north";
                output.Location.Name_East = "east";
                output.Location.Name_South = "south";
                output.Location.Name_West = "west";
            }
            else
            {
                output.Location.FriendlyName_North = LocationsStatics.GetConnectionName(output.Location.Name_North);
                output.Location.FriendlyName_East = LocationsStatics.GetConnectionName(output.Location.Name_East);
                output.Location.FriendlyName_South = LocationsStatics.GetConnectionName(output.Location.Name_South);
                output.Location.FriendlyName_West = LocationsStatics.GetConnectionName(output.Location.Name_West);
            }

            loadtime += "Start get location logs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.LocationLog = DomainRegistry.Repository.Find(new GetLocationLogsAtLocation { Location = me.dbLocationName, ConcealmentLevel = (int)myBuffs.Perception() });
            loadtime += "End get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get playerlogs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse();
            loadtime += "End get playerlogs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.PlayerLogImportant = output.PlayerLog.Where(l => l.IsImportant);

            loadtime += "Start get player items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayerItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id });
            output.CurrentCarryWeight = DomainRegistry.Repository.FindSingle(new GetCurrentCarryWeight { PlayerId = me.Id });
            loadtime += "End get player items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get mind controlled players:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            ViewBag.PlayersUnderMyMindControl = MindControlProcedures.GetAllMindControlledByPlayer(me);
            loadtime += "End get mind controlled players:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get location items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            if (me.GameMode == (int)GameModeStatics.GameModes.Invisible)
            {
                output.LocationItems = new List<PlayPageItemDetail>();
            }
            else
            {
                output.LocationItems = DomainRegistry.Repository.Find(new GetItemsAtLocationVisibleToGameMode { dbLocationName = me.dbLocationName, gameMode = me.GameMode });
            }
            loadtime += "End get location items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            ViewBag.InventoryItemCount = output.PlayerItems.Count();

            output.HasNewMessages = hasNewMessages;
            output.UnreadMessageCount = unreadMessageCount;
            output.WorldStats = PlayerProcedures.GetWorldPlayerStats();
            output.AttacksMade = me.TimesAttackingThisUpdate;
            ViewBag.AttacksMade = me.TimesAttackingThisUpdate;

            loadtime += "Start get known spells:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.LocalKnownSpells = PlayerProcedures.GetLocalKnownSpells(me);
            output.AllLocalSpells = PlayerProcedures.GetNumberOfLocalSpells(me.dbLocationName);
            loadtime += "End get known spells:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

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

            if (!iAmWhitelisted && newCharacterViewModel.InanimateForm == null && PlayerProcedures.IsMyIPInUseAndAnimate(Request.UserHostAddress))
            {

                ViewBag.ErrorMessage = "Your character was not created.  It looks like your IP address, <b>" + Request.UserHostAddress + "</b> already has 1 animate character in this world, and the current limit is 1. ";
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            if (me != null && me.Covenant > 0 && newCharacterViewModel.InanimateForm == null)
            {
                var myCov = CovenantProcedures.GetDbCovenant((int)me.Covenant);
                if (CovenantProcedures.GetPlayerCountInCovenant(myCov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount)
                {
                    TempData["Error"] = "The maximum number of animate players in your covenant has already been reached.";
                    TempData["SubError"] = "You will not be able to reroll as an animate character until there is room in the covenant or you leave it.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            var result = PlayerProcedures.SaveNewPlayer(newCharacterViewModel, myMembershipId);

            if (result != "saved")
            {
                ViewBag.ErrorMessage = "Your character was not created.  Reason:  " + result;
                ViewBag.IsRerolling = true;
                return View(MVC.PvP.Views.MakeNewCharacter);
            }

            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

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

            if (me != null)
            {
                var meItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });

                if (meItem == null)
                {
                    TempData["Error"] = "You are too confused to remember how to reroll.";
                    TempData["SubError"] = "Wait a bit.  You will soon remember who you truly are.";
                    return RedirectToAction(MVC.PvP.Play());
                }
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

            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var effects = EffectProcedures.GetPlayerEffects2(me.Id);

            var dizzy = effects.Any(e => e.dbEffect.Duration > 0 &&
                                         e.dbEffect.EffectSourceId == CharacterPrankProcedures.DIZZY_EFFECT);
            var blind = effects.Any(e => e.dbEffect.Duration > 0 &&
                                         e.dbEffect.EffectSourceId == CharacterPrankProcedures.BLINDED_EFFECT);

            string direction = null;
            var currentLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            // Dizzy players can stumble in the wrong direction
            if (dizzy)
            {
                var exits = new List<String> { currentLocation?.Name_North, currentLocation?.Name_East, currentLocation?.Name_South, currentLocation?.Name_West };

                if (blind)
                {
                    exits = new List<String> { "north", "east", "south", "west" };
                }

                var index = exits.IndexOf(locname);

                if (index != -1)
                {
                    // Roll an amount to stumble
                    var rand = new Random();
                    var roll = rand.NextDouble();

                    // Pick preferred stumble direction
                    if (roll < 0.2)
                    {
                        index++;  // 20% chance of stumbling right (i.e. clockwise)
                    }
                    else if (roll < 0.3)
                    {
                        index += 2;  // 10% chance of stumbling backward
                    }
                    else if (roll < 0.5)
                    {
                        index += 3;  // 20% chance of stumbling left (i.e. counterclockwise - we keep index positive so % plays nicely)
                    }

                    // Adjust the index to point to the nearest exit to the stumble
                    var len = exits.Count();
                    index = index % len;

                    var bias = rand.Next(2) * 2;  // Whether to prefer stumbling right or left of preferred stumble direction in case there is no route
                    locname = exits[index] ??                     // preferred stumble direction
                              exits[(index + 1 + bias) % len] ??  // right if bias is 0, left if bias is 2
                              exits[(index + 3 - bias) % len] ??  // left if bias is 0, right if bias is 2
                              exits[(index + 2) % len];           // 180 degree stumble (i.e. fall back onn the only)
                }
            }

            // Allow blinded players only to use relative directions - convert them
            if (blind)
            {
                if (locname == "north")
                {
                    direction = "North";
                    locname = currentLocation?.Name_North;
                }
                else if (locname == "east")
                {
                    direction = "East";
                    locname = currentLocation?.Name_East;
                }
                else if (locname == "south")
                {
                    direction = "South";
                    locname = currentLocation?.Name_South;
                }
                else if (locname == "west")
                {
                    direction = "West";
                    locname = currentLocation?.Name_West;
                }
                else
                {
                    locname = null;
                }

                if (locname == null)
                {
                    TempData["Error"] = "You have trouble seeing where you are going and walk into a wall.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // Access controls on joke shop
            if (locname == LocationsStatics.JOKE_SHOP)
            {
                if (JokeShopProcedures.CharacterIsBanned(me))
                {
                    TempData["Error"] = "You cannot enter the Joke Shop.";
                    TempData["SubError"] = "You are currently banned from this location.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if ((me.Id + PvPStatics.LastGameTurn) % 12 == 0)
                {
                    TempData["Error"] = "You cannot enter the Joke Shop.";
                    TempData["SubError"] = "The shop is busy right now - try again next turn.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // assert that the player is not mind controlled and cannot move on their own
            if (me.MindControlIsActive)
            {

                var myExistingMCs = MindControlProcedures.GetAllMindControlsWithPlayer(me);

                if (MindControlProcedures.PlayerIsMindControlledWithType(me, myExistingMCs, MindControlStatics.MindControl__MovementFormSourceId))
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

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new Move { PlayerId = me.Id, destination = locname, Direction = direction });
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

            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

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
            if (me.ActionPoints < 10)
            {
                TempData["Error"] = "You need 10 action points to enter or exit the dungeon.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is allowed to enter the dungeon (level, game mode, effects)
            var message = "";
            if (!me.IsInDungeon() && (!PlayerProcedures.CheckAllowedInDungeon(me, out message)))
            {
                TempData["Error"] = message;
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
            if (lastAttackTimeAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
            {
                TempData["Error"] = "You have been in combat too recently in order to enter or leave the dungeon right now.";
                TempData["SubError"] = "You must stay out of combat for another " + (TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling() - lastAttackTimeAgo) + " minutes.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (entering == "true")
            {

                // give player the Vanquish spell if they don't already know it
                SkillProcedures.GiveSkillToPlayer(me.Id, PvPStatics.Dungeon_VanquishSpellSourceId);

                var dungeonLocation = LocationsStatics.GetRandomLocation_InDungeon();
                PlayerProcedures.TeleportPlayer(me, dungeonLocation, false);
                TempData["Result"] = "You slipped down a manhole, tumbling through a dark tunnel and ending up down in the otherworldly dungeon deep below Sunnyglade, both physically and dimensionally.  Be careful where you tread... danger could come from anywhere and the magic down here is likely to keep you imprisoned much longer or permanently should you find yourself defeated...";
                PlayerLogProcedures.AddPlayerLog(me.Id, "You entered the dungeon.", false);
                LocationLogProcedures.AddLocationLog(me.dbLocationName, me.GetFullName() + " slid down a manhole to the dungeon deep below.");
                LocationLogProcedures.AddLocationLog(dungeonLocation, me.GetFullName() + " fell through the a portal in the ceiling from the town above.");
            }
            else if (entering == "false")
            {
                var overworldLocation = LocationsStatics.GetRandomLocationNotInDungeon();
                PlayerProcedures.TeleportPlayer(me, overworldLocation, false);
                TempData["Result"] = "Gasping for fresh air, you use your magic to tunnel your way up and out of the hellish labyrinth of the dungeon.  ";
                PlayerLogProcedures.AddPlayerLog(me.Id, "You left the dungeon.", false);
                LocationLogProcedures.AddLocationLog(me.dbLocationName, me.GetFullName() + " cast an earthmoving spell, tunneling back up to the town.");
                LocationLogProcedures.AddLocationLog(overworldLocation, me.GetFullName() + " slides out from a portal reaching deep into the dungeon.");
            }

            PlayerProcedures.ChangePlayerActionMana(-10, 0, 0, me.Id);


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

            // make sure a no-attack exists due to the Back On Your Feet perk

            if (target.BotId == AIStatics.ActivePlayerBotId && EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_BackOnYourFeetSourceId))
            {
                ViewBag.Recovered = true;
                ViewBag.RecoveredMsg = "You can't attack as you have the <b>Back On Your Feet</b> effect, preventing you from attacking another human-controlled player.";
            }
            else if (target.BotId == AIStatics.ActivePlayerBotId && EffectProcedures.PlayerHasEffect(target, PvPStatics.Effect_BackOnYourFeetSourceId))
            {
                ViewBag.Recovered = true;
                ViewBag.RecoveredMsg = "You can't attack <b>" + target.GetFullName() + "</b> since they have the <b>Back On Your Feet</b> effect, preventing human-controlled players from attacking them until the effect expires.";
            }
            else
            {
                output = SkillProcedures.AvailableSkills(me, target, false);
            }

            ViewBag.TargetId = targetId;
            ViewBag.TargetName = target.GetFullName();
            ViewBag.BotId = target.BotId;
            return PartialView(MVC.PvP.Views.partial.AjaxAttackModal, output);
        }

        public virtual ActionResult Attack(int targetId, int spellSourceId)
        {
            var myMembershipId = User.Identity.GetUserId();
            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            #region validation checks

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

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
            if (!iAmWhitelisted && PlayerProcedures.IsMyIPInUseAndAnimate(Request.UserHostAddress, me))
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
            if (me.Mana < PvPStatics.AttackManaCost)
            {
                TempData["Error"] = "You don't have enough mana to cast this.";
                TempData["SubError"] = "You can recover mana using consumable items, meditating, or waiting for it to replenish over time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the player does not have the Back On Your Feet perk if the target is another player
            if (targeted.BotId == AIStatics.ActivePlayerBotId && EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_BackOnYourFeetSourceId))
            {
                TempData["Error"] = "The protective aura from your Back On Your Feet effect prevents this spell from working.";
                TempData["SubError"] = "You can remove this effect with a Hex-B-Gone moisturizer or a Butt Plug if you want to resume attacks on player-controlled targets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the target does not have the Back On Your Feet perk if they are a player
            if (targeted.BotId == AIStatics.ActivePlayerBotId && EffectProcedures.PlayerHasEffect(targeted, PvPStatics.Effect_BackOnYourFeetSourceId))
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
            var blacklistExists = BlacklistProcedures.PlayersHaveBlacklistedEachOther(me, targeted, "attack");
            if ((me.GameMode < (int)GameModeStatics.GameModes.PvP && blacklistExists) || (PvPStatics.ChaosMode && blacklistExists))
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
            if (skillBeingUsed.StaticSkill.GivesEffectSourceId != null)
            {
                if (EffectProcedures.PlayerHasEffect(targeted, skillBeingUsed.StaticSkill.GivesEffectSourceId.Value))
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
            if (me.MindControlIsActive && futureForm != null && MindControlProcedures.PlayerIsMindControlledWithType(targeted, futureForm.Id))
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
                if (AIStatics.IsAFriendly(targeted.BotId))
                {
                    TempData["Error"] = "A little smile tells you it might just be a bad idea to try and attack this person...";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if (me.Level <= 3)
                {
                    TempData["Error"] = "You feel too intimidated by your target and find yourself unable to launch your spell.";
                    TempData["SubError"] = "You must gain some more experience before trying to take this target on.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                var npcAttackResult = DomainRegistry.Repository.FindSingle(new CanAttackNpcWithSpell { futureForm = futureForm, target = targeted, attacker = me, spellSourceId = skillSource.Id });

                if (npcAttackResult != String.Empty)
                {
                    TempData["SubError"] = npcAttackResult;
                    return RedirectToAction(MVC.PvP.Play());
                }

                // Valentine
                if (targeted.BotId == AIStatics.ValentineBotId)
                {

                    //The Krampus should not be attackable while other bosses are active.
                    if (PvPWorldStatProcedures.IsAnyBossActive())
                    {
                        TempData["Error"] = "You cannot attack the Krampus right now.";
                        TempData["SubError"] = "It seems there are more pressing matters right now.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    if (!BossProcedures_Valentine.IsAttackableInForm(me))
                    {
                        TempData["Error"] = BossProcedures_Valentine.GetWrongFormText();
                        TempData["SubError"] = "You will need to attack while in a different form.";
                        return RedirectToAction(MVC.PvP.Play());
                    }

                    // only allow weakens against Valentine for now (replace with Duel spell later?)
                    if (futureForm != null)
                    {
                        TempData["Error"] = "You get the feeling this type of spell won't work against Lady Krampus.";
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
                var result = "";
                if (me.dbLocationName == LocationsStatics.JOKE_SHOP)
                {
                    result = JokeShopProcedures.Attack(me, targeted, skillBeingUsed);
                }

                if (result.IsNullOrEmpty())
                {
                    result = AttackProcedures.AttackSequence(me, targeted, skillBeingUsed);
                }

                TempData["Result"] = result;
            }
            catch (Exception e)
            {
                TempData["Error"] = "There was a server error while carrying out your attack.  Reason:  <br><br>" + e;
            }

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

            var iAmWhitelisted = User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist);

            // assert player does not have more than 1 accounts already
            if (!iAmWhitelisted && PlayerProcedures.IsMyIPInUseAndAnimate(Request.UserHostAddress, me))
            {
                TempData["Error"] = "This character looks like a multiple account, which is illegal.  This character will not be allowed to enchant.";
                TempData["SubError"] = "You can only have 1 animate character in PvP mode and 1 animate character in Protection mode at a time.";
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
            if (EffectProcedures.PlayerHasEffect(me, PvPStatics.Effect_BackOnYourFeetSourceId))
            {
                TempData["Error"] = "The protective aura from your Back On Your Feet effect prevents this spell from working.";
                TempData["SubError"] = "You can remove this effect with a Hex-B-Gone moisturizer or a Butt Plug if you want to resume attacks on player-controlled targets.";
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

            if (me.dbLocationName == LocationsStatics.JOKE_SHOP)
            {
                TempData["Error"] = "An otherworldly field prevents your enchantment having any effect on the Joke Shop.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is not too late in the turn for this attack to happen
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
            PlayerProcedures.ChangePlayerActionMana(-3, 0, -10, me.Id);

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
                TempData["Error"] = "You must be animate in order to attempt to restore yourself to your base form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can attempt to restore yourself to your base form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can attempt to restore yourself to your base form";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already in their base form
            if (me.FormSourceId == me.OriginalFormSourceId && me.FirstName == me.OriginalFirstName && me.LastName == me.OriginalLastName)
            {
                TempData["Error"] = "You are already in your base form!";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (me.dbLocationName == LocationsStatics.JOKE_SHOP)
            {
                var result = JokeShopProcedures.SelfRestore(me);

                if (!result.IsNullOrEmpty())
                {
                    TempData["Result"] = result;
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            var buffs = ItemProcedures.GetPlayerBuffs(me);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new SelfRestoreToBase { PlayerId = me.Id, Buffs = buffs });
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
                newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
                playerRepo.SavePlayer(newMe);
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

            if (me.dbLocationName == LocationsStatics.JOKE_SHOP)
            {
                var result = JokeShopProcedures.Meditate(me);

                if (!result.IsNullOrEmpty())
                {
                    TempData["Result"] = result;
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

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

            if (me.dbLocationName == LocationsStatics.JOKE_SHOP)
            {
                var result = JokeShopProcedures.Cleanse(me);

                if (!result.IsNullOrEmpty())
                {
                    TempData["Result"] = result;
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

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
            var mySkills = DomainRegistry.Repository.Find(new GetSkillsOwnedByPlayer { playerId = me.Id });

            bool withLoremaster = false;
            var lorekeeper = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);
            if (lorekeeper != null)
            {
                withLoremaster = me.dbLocationName == lorekeeper.dbLocationName;
            }
            ViewBag.CanBuySpells = withLoremaster;

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
                PlayerProcedures.ChangePlayerActionMana(-(PvPStatics.SearchAPCost - 1), 0, 0, me.Id);
            }
            else
            {
                PlayerProcedures.ChangePlayerActionMana(-PvPStatics.SearchAPCost, 0, 0, me.Id);
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

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

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

                if (MindControlProcedures.PlayerIsMindControlledWithType(me, myExistingMCs, MindControlStatics.MindControl__StripFormSourceId))
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

            if (pickup == null)
            {
                TempData["Error"] = "That item has been already been consumed or does not exist.";
                return RedirectToAction(MVC.PvP.Play());
            }

            //assert that the item is indeed at this location and on the ground
            if (pickup.dbLocationName != me.dbLocationName)
            {
                TempData["Error"] = "That item isn't in this location or else it has already been picked up.";
                return RedirectToAction(MVC.PvP.Play());
            }

            //assert that the item is not soulbound to someone else
            if (pickup.SoulboundToPlayer != null && pickup.SoulboundToPlayer.Id != me.Id)
            {
                TempData["Error"] = "This item is soulbound to another player and cannot be picked up by anyone else.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item is not embedded, such as a 'ghost rune'
            if (pickup.EmbeddedOnItem != null)
            {
                TempData["Error"] = "This rune is embedded in another item and cannot be picked up.<br />\n" +
                    "<em>Please report this on <a href=\"https://discord.gg/z66CYzX\">Discord</a> " +
                    $"or by <a href=\"{Url.Action(MVC.Report.Question())}\">asking a question</a>, citing <strong>item ID {pickup.Id}.</strong></em>";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the player is not carrying too much already UNLESS the item is a pet OR dungeon token
            if (ItemProcedures.PlayerIsCarryingTooMuch(me, true) && pickup.ItemSource.ItemType != PvPStatics.ItemType_Pet && pickup.ItemSource.Id != PvPStatics.ItemType_DungeonArtifact_Id)
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

                EffectProcedures.GivePerkToPlayer(PvPStatics.Dungeon_ArtifactCurseEffectSourceId, me);

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
                var name = (pickup.FormerPlayer == null) ? "a " + pickup.ItemSource.FriendlyName
                                                         : pickup.FormerPlayer.FullName + " the " + pickup.ItemSource.FriendlyName;
                playerLogMessage = "You tamed <b>" + name + "</b> at " + here.Name + " and put it into your inventory.";
                locationLogMessage = me.GetFullName() + " tamed <b>" + name + HtmlHelpers.PrintPvPIcon(pickup) + "</b> here.";

                if (pickup.FormerPlayer != null)
                {
                    var notificationMsg = me.GetFullName() + " has tamed you.  You will now follow them wherever they go and magically enhance their abilities by being their faithful companion.";
                    PlayerLogProcedures.AddPlayerLog(pickup.FormerPlayer.Id, notificationMsg, true);
                }

            }

            PlayerProcedures.SetTimestampToNow(me);
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

            var dropme = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = itemId });

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
                return RedirectToAction(MVC.Item.MyInventory());
            }

            var here = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
            var result = "";

            if (here.Region == "dungeon")
            {
                // in dungeon, have it drop in a random place in the overworld
                var overworldLocation = LocationsStatics.GetRandomLocationNotInDungeon();
                var resultmsg = ItemProcedures.DropItem(itemId, overworldLocation);
                result = resultmsg + "  It shimmers and falls through the dungeon floor, appearing somewhere in the town above.";
            }
            else if (here.dbName == LocationsStatics.JOKE_SHOP)
            {
                result = JokeShopProcedures.Drop(me, itemId);
            }

            if (result.IsNullOrEmpty())
            {
                // in overworld, drop at player's feet
                result = ItemProcedures.DropItem(itemId);
            }

            TempData["Result"] = result;

            string playerLogMessage;
            string locationLogMessage;

            // animals are released
            if (dropme.ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                playerLogMessage = "You released your " + dropme.ItemSource.FriendlyName + " at " + here.Name + ".";
                locationLogMessage = me.GetFullName() + " released a <b>" + dropme.ItemSource.FriendlyName + HtmlHelpers.PrintPvPIcon(dropme) + "</b> here.";

                if (dropme.FormerPlayer != null)
                {
                    var notificationMsg = me.GetFullName() + " has released you.  You are now feral and may now wander the town at will until another master tames you.";
                    PlayerLogProcedures.AddPlayerLog(dropme.FormerPlayer.Id, notificationMsg, true);
                }


            }
            // everything else is dropped
            else
            {
                playerLogMessage = "You dropped a " + dropme.ItemSource.FriendlyName + " at " + here.Name + ".";
                locationLogMessage = me.FirstName + " " + me.LastName + " dropped a <b>" + dropme.ItemSource.FriendlyName + HtmlHelpers.PrintPvPIcon(dropme) + "</b> here.";
            }

            PlayerProcedures.SetTimestampToNow(me);
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationLogMessage);

            return RedirectToAction(MVC.Item.MyInventory());

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

            // assert that the player is not in combat if they are trying to swap a consumable item.
            var lastAttackTimeAgo = Math.Abs(Math.Floor(me.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalSeconds));
            var secondsInCombat = 3 * TurnTimesStatics.GetTurnLengthInSeconds();
            if (lastAttackTimeAgo < secondsInCombat &&
                    (item.Item.ItemType == PvPStatics.ItemType_Consumable ||
                     item.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable))
            {
                var minutesRemaining = Math.Ceiling((secondsInCombat - lastAttackTimeAgo) / 60);
                TempData["Error"] = "You cannot swap consumable items during combat";
                TempData["SubError"] = $"You'll have to wait {minutesRemaining} more minute(s) until you are out of combat before you can do that.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (putOn)
            {
                // Disallows the player from equipping multiple items of the same type
                if (ItemProcedures.PlayerIsWearingNumberOfThisExactItem(me.Id, item.dbItem.ItemSourceId) == 1)
                {
                    TempData["Error"] = "You already have a " + item.Item.FriendlyName + " equipped.";
                    TempData["SubError"] = "You can't equip two of the same item.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                // If the item is a consumable or a reusable consumable, allow the player to wear a maximum of three
                if ((item.Item.ItemType == PvPStatics.ItemType_Consumable || item.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable) &&
                     ItemProcedures.PlayerTotalConsumableCount(me.Id) > 2)
                {
                    TempData["Error"] = "You have already equipped three consumables.";
                    TempData["SubError"] = "You must remove at least one consumable before you can equip another.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if (item.Item.ItemType != PvPStatics.ItemType_Pet &&
                    item.Item.ItemType != PvPStatics.ItemType_Consumable &&
                    item.Item.ItemType != PvPStatics.ItemType_Consumable_Reuseable)
                {
                    // Count items per type (non-accessory empty slots count as 1)
                    var numHats = Math.Max(1, ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Hat));
                    var numShirts = Math.Max(1, ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Shirt));
                    var numUndershirts = Math.Max(1, ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Undershirt));
                    var numPants = Math.Max(1, ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pants));
                    var numUnderpants = Math.Max(1, ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Underpants));
                    var numShoes = Math.Max(1, ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Shoes));
                    var numAccessories = ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Accessory);

                    var numItemsEquipped = numHats + numShirts + numUndershirts + numPants + numUnderpants + numShoes + numAccessories;

                    if (numItemsEquipped >= 8 && (item.Item.ItemType == PvPStatics.ItemType_Accessory || ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, item.Item.ItemType) > 0))
                    {
                        TempData["Error"] = "There is no room to equip this item because all your accessory slots are in use.";
                        TempData["SubError"] = "You must remove at least one item of the same type before you can equip another.";
                        return RedirectToAction(MVC.PvP.Play());
                    }
                }
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
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type items next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            //assert that the item is not soulbound to someone else
            if (item.dbItem.SoulboundToPlayerId != null && item.dbItem.SoulboundToPlayerId != me.Id)
            {
                TempData["Error"] = "This item is soulbound to another player and cannot be picked up by anyone else.";
                return RedirectToAction(MVC.PvP.Play());
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

            // assert that if the item is a consumable, it is equipped
            if (item.Item.ItemType == PvPStatics.ItemType_Consumable && !item.dbItem.IsEquipped)
            {
                TempData["Error"] = "You cannot use an item you do not have equipped.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that if the item is a reusable consumable, it is equipped
            if (item.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable && !item.dbItem.IsEquipped)
            {
                TempData["Error"] = "You cannot use an item you do not have equipped.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if this item is a teleportation scroll, redirect to the teleportation page.
            if (item.dbItem.ItemSourceId == ItemStatics.TeleportationScrollItemSourceId)
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
                    var model = new TT.Domain.ViewModels.TeleportMapViewModel
                    {
                        ItemId = itemId,
                        Destinations = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region == "dungeon")
                    };

                    return View(MVC.PvP.Views.TeleportMap, model);
                }
                else
                {
                    var model = new TT.Domain.ViewModels.TeleportMapViewModel
                    {
                        ItemId = itemId,
                        Destinations = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.dbName != LocationsStatics.JOKE_SHOP && l.Region != "dungeon")
                    };

                    return View(MVC.PvP.Views.TeleportMap, model);
                }
            }

            // if this item is the self recaster, redirect to the animate spell listing page
            if (item.dbItem.ItemSourceId == ItemStatics.AutoTransmogItemSourceId)
            {
                return RedirectToAction(MVC.Item.SelfCast(itemId));
            }

            // if this item is a skill book, aka a tome, redirect to that page with the appropriate text
            if (item.Item.ConsumableSubItemType != null && item.Item.ConsumableSubItemType == (int)ItemStatics.ConsumableSubItemTypes.Tome)
            {

                var cmd = new GetTomeByItem { ItemSourceId = item.Item.Id };
                var tome = DomainRegistry.Repository.FindSingle(cmd);

                var output = new SkillBookViewModel
                {
                    Text = tome.Text,
                    AlreadyRead = ItemProcedures.PlayerHasReadBook(me, item.dbItem.ItemSourceId),
                    BookId = item.dbItem.Id,
                };

                output.Text = output.Text.Replace(Environment.NewLine, "<br>");

                return View(MVC.Item.Views.SkillBook, output);
            }

            if (item.dbItem.ItemSourceId == ItemStatics.CurseLifterItemSourceId || item.dbItem.ItemSourceId == ItemStatics.ButtPlugItemSourceId)
            {
                return RedirectToAction(MVC.Item.RemoveCurse(item.dbItem.Id));
            }

            if (item.dbItem.ItemSourceId == ItemStatics.TgSplashOrbItemSourceId)
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

            var (success, result) = ItemProcedures.UseItem(itemId, myMembershipId);

            if (success == true)
            {
                PlayerProcedures.SetTimestampToNow(me);
                PlayerProcedures.AddItemUses(me.Id, 1);
            }

            TempData["Result"] = result;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            var playerMessage = item.Item.UsageMessage_Player;
            if (string.IsNullOrEmpty(playerMessage))
            {
                PlayerLogProcedures.AddPlayerLog(me.Id, result, false);
            }
            else
            {
                PlayerLogProcedures.AddPlayerLog(me.Id, $"{playerMessage}<br /><b>{result}</b>", item.dbItem.FormerPlayerId != null);
            }

            if (item.dbItem.FormerPlayerId != null)
            {
                var itemMessage = item.Item.UsageMessage_Item;
                var context = "<b>Your owner just used you!</b>";
                itemMessage = string.IsNullOrEmpty(itemMessage) ? context : $"{itemMessage}<br />{context}";
                PlayerLogProcedures.AddPlayerLog((int)item.dbItem.FormerPlayerId, itemMessage, true);
            }

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

            if (playerLookedAt == null)
            {
                TempData["Result"] = "This character is not active in the current round.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new PlayerFormItemsSkillsViewModel
            {
                PlayerForm = playerLookedAt,
                Skills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(id),
                Items = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = playerLookedAt.Player.Id }).Where(i => i.IsEquipped == true),
                Bonuses = ItemProcedures.GetPlayerBuffs(playerLookedAt.Player.ToDbPlayer()),
                ShowInventory = !AIStatics.IsAFriendly(playerLookedAt.Player.BotId),
                PlayerUserStrikesDetail = DomainRegistry.Repository.FindSingle(new GetPlayerUserStrikes { UserId = playerLookedAt.Player.MembershipId })
            };


            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var lookedAtPlayerId = playerLookedAt.Player.MembershipId;
            ViewBag.IsMe = myMembershipId == lookedAtPlayerId;

            ViewBag.HasBio = SettingsProcedures.PlayerHasBio(lookedAtPlayerId);
            ViewBag.HasArtistAuthorBio = SettingsProcedures.PlayerHasArtistAuthorBio(lookedAtPlayerId);
            ViewBag.TimeUntilLogout = TurnTimesStatics.GetOfflineAfterXMinutes() - Math.Abs(Math.Floor(playerLookedAt.Player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if ((playerLookedAt.Player.Mobility == PvPStatics.MobilityInanimate || playerLookedAt.Player.Mobility == PvPStatics.MobilityPet) && playerLookedAt.Player.InQuest == 0)
            {
                var playerItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = playerLookedAt.Player.Id });
                ViewBag.ImgUrl = HtmlHelpers.GetImagePath(playerLookedAt, false);
                ViewBag.ItemId = playerItem == null ? -1 : playerItem.Id;
                ViewBag.ItemLevel = playerItem == null ? playerLookedAt.Player.Level : playerItem.Level;
                ViewBag.IsEquipped = playerItem != null && playerItem.IsEquipped;
                ViewBag.FormDescriptionItem = playerItem == null ? "This player is lost, forgotten by the world and everyone in it." : playerItem.ItemSource.Description;
                ViewBag.ItemSkills = playerItem == null ? null : SkillStatics.GetItemSpecificSkills(playerItem.ItemSource.Id).ToList();
                ViewBag.IsConsumable = (playerItem?.ItemSource.ItemType == PvPStatics.ItemType_Consumable_Reuseable) ||
                                       (playerItem?.ItemSource.ItemType == PvPStatics.ItemType_Consumable);

                MvcHtmlString consumableEffect = new MvcHtmlString("");
                if (playerItem?.ItemSource.GivesEffectSourceId != null)
                {
                    consumableEffect = HtmlHelpers.GetEffectFriendlyName(playerItem.ItemSource.GivesEffectSourceId.Value);
                }
                ViewBag.ConsumableEffect = consumableEffect;

                var owner = (playerItem?.FormerPlayer == null) ? null : ItemProcedures.BeingWornBy(playerItem.FormerPlayer.Id);
                var ownedByMe = owner != null && owner.Player.MembershipId == myMembershipId;

                ViewBag.Owner = owner;
                ViewBag.OwnedByMe = ownedByMe;
                ViewBag.Unowned = (owner == null) || owner.Player.BotId == AIStatics.WuffieBotId || owner.Player.BotId == AIStatics.SoulbinderBotId;
                ViewBag.SameLocation = owner == null
                    ? playerItem?.dbLocationName == me.dbLocationName
                    : owner.Player.dbLocationName == me.dbLocationName;

                if (ownedByMe)
                {
                    var world = DomainRegistry.Repository.FindSingle(new GetWorld());
                    var secondsSinceUpdate = Math.Abs(Math.Floor(world.LastUpdateTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));
                    ViewBag.SecondsUntilUpdate = TurnTimesStatics.GetTurnLengthInSeconds() - (int)secondsSinceUpdate;
                }

                ViewBag.PlayerInteractionsRemain = me.ItemsUsedThisTurn < PvPStatics.MaxItemUsesPerUpdate;
                ViewBag.ItemInteractionsRemain = playerItem?.FormerPlayer != null &&
                    playerItem.FormerPlayer.ItemsUsedThisTurn < PvPStatics.MaxItemUsesPerUpdate &&
                    playerItem.FormerPlayer.TimesAttackingThisUpdate < PvPStatics.MaxActionsPerUpdate;

                if (playerItem?.ItemSource.ItemType == PvPStatics.ItemType_Pet)
                {
                    ViewBag.WornMessage = playerItem?.Owner == null
                        ? "This creature has not been tamed as is running around feral."
                        : "This creature has been tamed and is following their master.";
                }
                else
                {
                    ViewBag.WornMessage = playerItem?.Owner == null
                        ? "This item is not currently owned and is lying around available to be claimed by whoever comes across them."
                        : "This item is currently being carried and possibly worn by another player.";
                }

                ViewBag.ErrorMessage = TempData["Error"];
                ViewBag.SubErrorMessage = TempData["SubError"];
                ViewBag.Result = TempData["Result"];

                return View(MVC.PvP.Views.LookAtPlayerInanimate, output);
            }
            else
            {

                ViewBag.AtLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == output.PlayerForm.Player.dbLocationName).Name;
                ViewBag.FormSkills = SkillStatics.GetFormSpecificSkills(playerLookedAt.Player.FormSourceId).ToList();

                ViewBag.ErrorMessage = TempData["Error"];
                ViewBag.SubErrorMessage = TempData["SubError"];
                ViewBag.Result = TempData["Result"];

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
            var meItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });

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
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxActionsPerUpdate)
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
                PlayerProcedures.ChangePlayerActionMana(0, .25M, 0, wearer.Player.Id, false);
                thirdP = "<span class='petActionGood'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly rubbing against your skin affectionately.  You gain a tiny amount of willpower from your inanimate belonging's subtle but kind gesture.</span>";
                firstP = "You affectionately rub against your current owner, " + wearer.Player.GetFullName() + ".  " + pronoun + " gains a tiny amount of willpower from your subtle but kind gesture.";
            }
            else if (actionName == "pinch")
            {
                PlayerProcedures.ChangePlayerActionMana(0, -.15M, 0, wearer.Player.Id, false);
                thirdP = "<span class='petActionBad'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly pinch your skin agitatedly.  You lose a tiny amount of willpower from your inanimate belonging's subtle but pesky gesture.</span>";
                firstP = "You agitatedly pinch against your current owner, " + wearer.Player.GetFullName() + ".  " + pronoun + " loses a tiny amount of willpower from your subtle but pesky gesture.";
            }
            else if (actionName == "soothe")
            {
                PlayerProcedures.ChangePlayerActionMana(0, 0, .25M, wearer.Player.Id, false);
                thirdP = "<span class='petActionGood'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly peacefully soothe your skin.  You gain a tiny amount of mana from your inanimate belonging's subtle but kind gesture.</span>";
                firstP = "You kindly soothe a patch of your current owner, " + wearer.Player.GetFullName() + "'s skin.  " + pronoun + " gains a tiny amount of mana from your subtle but kind gesture.";
            }
            else if (actionName == "zap")
            {
                PlayerProcedures.ChangePlayerActionMana(0, 0, -.15M, wearer.Player.Id, false);
                thirdP = "<span class='petActionBad'>You feel " + me.GetFullName() + ", currently your " + meItem.ItemSource.FriendlyName + ", ever so slightly zap your skin.  You lose a tiny amount of mana from your inanimate belonging's subtle but pesky gesture.</span>";
                firstP = "You agitatedly zap a patch of your current owner, " + wearer.Player.GetFullName() + "'s skin.  " + pronoun + " loses a tiny amount of mana from your subtle but pesky gesture.";
            }
            else
            {
                TempData["Error"] = "You cannot perform that action.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);
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
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxActionsPerUpdate)
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

            // assert that the action is valid
            if (actionName != "snarl" && actionName != "lick" && actionName != "nuzzle" && actionName != "headbutt")
            {
                TempData["Error"] = "You cannot perform that action.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // all of our checks have passed, so now let's actually do the action
            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

            var result = AnimalProcedures.DoAnimalAction(actionName, me.Id, targeted.Id);
            var leveluptext = InanimateXPProcedures.GiveInanimateXP(me.MembershipId, User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist));

            TempData["Result"] = result + leveluptext;

            ItemProcedures.UpdateSouledItem(me);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult OwnerAction(string actionName, int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can interact with your pet and items.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can interact with your pet and items.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var item = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = itemId });
            var itemFormerPlayer = item?.FormerPlayer;
            var itemPlayer = itemFormerPlayer == null ? null : PlayerProcedures.GetPlayer(itemFormerPlayer.Id);

            if (itemPlayer == null)
            {
                TempData["Error"] = "Cannot find that player";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (itemPlayer.BotId != AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You can only interact with souled items and pets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that item is inanimate
            if (itemPlayer.Mobility != PvPStatics.MobilityInanimate && itemPlayer.Mobility != PvPStatics.MobilityPet)
            {
                TempData["Error"] = "You can only interact with inanimate and pet players.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var posession = itemPlayer.Mobility == PvPStatics.MobilityPet ? "pet" : "item";

            // assert player owns item/pet
            if (item.Owner.Id != me.Id)
            {
                TempData["Error"] = $"You do not currently own this {posession}.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = $"Wriggle as you might, you find you cannot interact with your {posession} until you are fully animate again!";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = $"You don't have enough time to devote to your {posession} right now.";
                TempData["SubError"] = "Try again in a few moments.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item/pet has not been interacted with this turn
            if (itemPlayer.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = $"This {posession} is getting a lot of attention and is nearly worn out!  They need to be left alone to recover.";
                TempData["SubError"] = "Try again in a few moments.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item has not acted too many times already
            if ((actionName == "hush" || actionName == "restrain") && itemPlayer.TimesAttackingThisUpdate >= PvPStatics.MaxActionsPerUpdate)
            {
                TempData["Error"] = $"Your {posession} has already acted this turn.";
                TempData["SubError"] = "Maybe you can block their actions next turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var inanimXpRepo = new EFInanimateXPRepository();
            var xp = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == itemPlayer.Id);

            // check item has performed an action
            if (xp == null)
            {
                TempData["Error"] = $"Your {posession} is currently unresponsive.";

                if (itemPlayer.Mobility == PvPStatics.MobilityPet)
                {
                    TempData["SubError"] = "Perhaps you should train them to be more obedient.";
                }
                else
                {
                    TempData["SubError"] = "Perhaps you should remind them who's in charge?";
                }

                return RedirectToAction(MVC.PvP.Play());
            }

            // Generate RP messages for owner and item/pet
            var secondP = "";
            var firstP = "";
            var their = me.Gender == PvPStatics.GenderFemale ? "her" : "his";
            var Their = me.Gender == PvPStatics.GenderFemale ? "Her" : "His";
            var didStuffTo = $"{actionName}ed";
            var block = false;
            var boost = false;

            if (itemPlayer.Mobility == PvPStatics.MobilityInanimate && actionName == "flaunt")
            {
                secondP = $"Your proud owner, {me.GetFullName()}, confidently flaunts you for everyone to see!";
                firstP = $"You proudly flaunt {item.FormerPlayer.FullName}, your {item.ItemSource.FriendlyName}, for everyone to see!";
                boost = true;
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (itemPlayer.Mobility == PvPStatics.MobilityInanimate && actionName == "shun")
            {
                secondP = $"Your embarrassed owner, {me.GetFullName()}, is disappointed in you and shuns you in favor of {their} other items.";
                firstP = $"You disappointedly shun {item.FormerPlayer.FullName}, your {item.ItemSource.FriendlyName}.";
                didStuffTo = "shunned";
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (itemPlayer.Mobility == PvPStatics.MobilityInanimate && actionName == "hush")
            {
                secondP = $"Your distracted owner, {me.GetFullName()}, is tired of your noise and sternly hushes you, preventing you from acting this turn.";
                firstP = $"You sternly hush {item.FormerPlayer.FullName}, your {item.ItemSource.FriendlyName}, preventing them from acting this turn.";
                block = true;
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (itemPlayer.Mobility == PvPStatics.MobilityPet && actionName == "praise")
            {
                secondP = $"Your delighted owner, {me.GetFullName()}, heaps praise upon you and rewards you for being such a good {posession}!";
                firstP = $"You heap praise upon {item.FormerPlayer.FullName}, your {item.ItemSource.FriendlyName}!";
                didStuffTo = "praised";
                boost = true;
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (itemPlayer.Mobility == PvPStatics.MobilityPet && actionName == "scold")
            {
                secondP = $"Your angry owner, {me.GetFullName()}, bitterly scolds you in front of everyone as punishment for your misbehavior!";
                firstP = $"You bitterly scold {item.FormerPlayer.FullName}, your {item.ItemSource.FriendlyName}, for everyone to see!";
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (itemPlayer.Mobility == PvPStatics.MobilityPet && actionName == "restrain")
            {
                secondP = $"Your irritated owner, {me.GetFullName()}, is fed up with your yapping and forcibly restrains you, preventing you from acting this turn.";
                firstP = $"You forcibly restrain {item.FormerPlayer.FullName}, your {item.ItemSource.FriendlyName}, preventing them from acting this turn.";
                block = true;
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else
            {
                TempData["Error"] = "You cannot perform that action.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (block)
            {
                // Drain item of all their actions this turn
                PlayerProcedures.SetAttackCount(itemPlayer, PvPStatics.MaxActionsPerUpdate);
            }
            else
            {
                // Roll chance to gain/lose a turn's worth of XP, owner TF or struggle.
                // Owner cannot level up/lock the item.
                var rand = new Random();
                if (rand.NextDouble() < 0.2)  // 1 XP per turn average for single account item (5 x 20%)
                {
                    var currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

                    if (boost && xp.LastActionTurnstamp > 0)
                    {
                        secondP += $"  {Their} actions gives you a helpful little boost!";
                        firstP += "  Your action gives them a helpful little boost!";
                        xp.LastActionTurnstamp--;
                    }
                    else if (!boost && xp.LastActionTurnstamp < currentGameTurn)
                    {
                        secondP += $"  {Their} actions hinder your progress and leave you feeling disheartened.";
                        firstP += "  Your action hinders their progress and leaves them feeling disheartened.";
                        xp.LastActionTurnstamp++;
                    }

                    inanimXpRepo.SaveInanimateXP(xp);
                }
            }

            // Format item's message
            if (boost)
            {
                secondP = $"<span class='petActionGood'>{secondP}</span>";
            }
            else
            {
                secondP = $"<span class='petActionBad'>{secondP}</span>";
            }

            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

            // Subtract cost (item use for both players)
            PlayerProcedures.AddItemUses(me.Id, 1);
            PlayerProcedures.AddItemUses(itemPlayer.Id, 1);

            // Bring player online
            PlayerProcedures.SetTimestampToNow(me);

            TempData["Result"] = firstP;
            PlayerLogProcedures.AddPlayerLog(me.Id, firstP, false);
            PlayerLogProcedures.AddPlayerLog(itemPlayer.Id, secondP, true);

            if (!block)
            {
                LocationLogProcedures.AddLocationLog(me.dbLocationName, $"{me.GetFullName()} {didStuffTo} {itemPlayer.GetFullName()}, a {item.ItemSource.FriendlyName}, here.");
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult FeralAction(string actionName, int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can interact with this pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can interact with this pet.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var pet = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = itemId });
            var petFormerPlayer = pet?.FormerPlayer;
            var petPlayer = petFormerPlayer == null ? null : PlayerProcedures.GetPlayer(petFormerPlayer.Id);

            if (petPlayer == null)
            {
                TempData["Error"] = "Cannot find that player";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (petPlayer.BotId != AIStatics.ActivePlayerBotId)
            {
                TempData["Error"] = "You can only interact with souled pets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that item is a pet
            if (petPlayer.Mobility != PvPStatics.MobilityPet)
            {
                TempData["Error"] = "You can only interact with pet players.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert pet is unowned or on a vendor
            if (pet.Owner != null && pet.Owner.BotId != AIStatics.WuffieBotId && pet.Owner.BotId != AIStatics.SoulbinderBotId)
            {
                TempData["Error"] = "This pet is currently owned by another player.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "Wriggle as you might, you find you cannot interact with this pet until you are fully animate again!";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert pet and player are in the same place
            if (pet.Owner != null)
            {
                var vendor = PlayerProcedures.GetPlayer(pet.Owner.Id);
                if (me.dbLocationName != vendor.dbLocationName)
                {
                    TempData["Error"] = "You can only interact with this pet when you are both in the same location";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else if (me.dbLocationName != pet.dbLocationName)
            {
                TempData["Error"] = "You can only interact with this pet when you are both in the same location";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You don't have enough time to play with the this pet right now.";
                TempData["SubError"] = "Try again in a few moments.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert pet has not been interacted with this turn
            if (petPlayer.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "This pet has been very busy this turn and needs a rest.";
                TempData["SubError"] = "Try again in a few moments.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert pet has not acted too many times already
            if (actionName == "tranquilize" && petPlayer.TimesAttackingThisUpdate >= PvPStatics.MaxActionsPerUpdate)
            {
                TempData["Error"] = "This pet has already acted this turn.";
                TempData["SubError"] = "Maybe you can block their actions next turn.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var inanimXpRepo = new EFInanimateXPRepository();
            var xp = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == petPlayer.Id);

            // check pet has performed an action
            if (xp == null)
            {
                TempData["Error"] = "This pet is not currently responding to strangers.";
                TempData["SubError"] = "Maybe you could ask them for a nuzzle?";
                return RedirectToAction(MVC.PvP.Play());
            }

            // Generate RP messages for player and pet
            var secondP = "";
            var firstP = "";
            var Their = me.Gender == PvPStatics.GenderFemale ? "Her" : "His";
            var didStuffTo = "";
            var block = false;
            var boost = false;

            if (petPlayer.Mobility == PvPStatics.MobilityPet && actionName == "pat")
            {
                secondP = $"A friendly passer-by called {me.GetFullName()} greets you with gentle pat!";
                firstP = $"You gently pat {pet.FormerPlayer.FullName}, a {pet.ItemSource.FriendlyName}!";
                didStuffTo = "patted";
                boost = true;
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (petPlayer.Mobility == PvPStatics.MobilityPet && actionName == "shoo")
            {
                secondP = $"You get on the nerves of {me.GetFullName()}, who agitatedly shoos you away!";
                firstP = $"You shoo away {pet.FormerPlayer.FullName}, a {pet.ItemSource.FriendlyName}!";
                didStuffTo = "shooed away";
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else if (petPlayer.Mobility == PvPStatics.MobilityPet && actionName == "tranquilize")
            {
                secondP = $"{me.GetFullName()} fires a tranquilizer dart at you in an attempt to subdue you.  When it hits you find yourself unable to perform any more actions this turn.";
                firstP = $"You fire a dart at {pet.FormerPlayer.FullName} the {pet.ItemSource.FriendlyName}, incapacitating them for the rest of this turn.";
                didStuffTo = "tranquilized";
                block = true;
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__ItemPetInteractions, 1);
            }
            else
            {
                TempData["Error"] = "You cannot perform that action.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (block)
            {
                // Drain pet of all their actions this turn
                PlayerProcedures.SetAttackCount(petPlayer, PvPStatics.MaxActionsPerUpdate);
            }
            else
            {
                // Roll chance to gain/lose a turn's worth of XP, owner TF or struggle.
                // Animate player cannot level up/lock the pet.
                var rand = new Random();
                if (rand.NextDouble() < 0.2)  // 1 XP per turn average for single account item (5 x 20%)
                {
                    var currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

                    if (boost && xp.LastActionTurnstamp > 0)
                    {
                        secondP += $"  {Their} actions gives you a helpful little boost!";
                        firstP += "  Your action gives them a helpful little boost!";
                        xp.LastActionTurnstamp--;
                    }
                    else if (!boost && xp.LastActionTurnstamp < currentGameTurn)
                    {
                        secondP += $"  {Their} actions hinder your progress and leave you feeling disheartened.";
                        firstP += "  Your action hinders their progress and leaves them feeling disheartened.";
                        xp.LastActionTurnstamp++;
                    }

                    inanimXpRepo.SaveInanimateXP(xp);
                }
            }

            // Format pet's message
            if (boost)
            {
                secondP = $"<span class='petActionGood'>{secondP}</span>";
            }
            else
            {
                secondP = $"<span class='petActionBad'>{secondP}</span>";
            }

            PlayerProcedures.LogIP(Request.UserHostAddress, myMembershipId);

            // Subtract cost (item use for both players)
            PlayerProcedures.AddItemUses(me.Id, 1);
            PlayerProcedures.AddItemUses(petPlayer.Id, 1);

            // Bring player online
            PlayerProcedures.SetTimestampToNow(me);

            TempData["Result"] = firstP;
            PlayerLogProcedures.AddPlayerLog(me.Id, firstP, false);
            PlayerLogProcedures.AddPlayerLog(petPlayer.Id, secondP, true);

            if (!block)
            {
                LocationLogProcedures.AddLocationLog(me.dbLocationName, $"{me.GetFullName()} {didStuffTo} {petPlayer.GetFullName()}, a {pet.ItemSource.FriendlyName}, here.");
            }

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

                    var sentMessage = "You have sent a friend request to " + friend.FirstName + " " + friend.LastName + "!";
                    PlayerLogProcedures.AddPlayerLog(me.Id, sentMessage, false);
                    TempData["Result"] = sentMessage;
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

            if (me.Mobility == PvPStatics.MobilityFull)
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
                    var meAnimal = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });
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

            // Don't reveal location to blind players
            if (EffectProcedures.PlayerHasActiveEffect(me, CharacterPrankProcedures.BLINDED_EFFECT))
            {
                ViewBag.MapX = 999;
                ViewBag.MapY = 999;
            }

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

        public virtual ActionResult ChoosePerk(int effectSourceId)
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
            TempData["Result"] = EffectProcedures.GivePerkToPlayer(effectSourceId, me);


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

        public virtual ActionResult Teleport(int itemId, string to)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // no need to assert player is mobile; inanimates and items have no inventory
            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can teleport.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that this player is not in a duel
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can teleport.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var item = ItemProcedures.GetItemViewModel(itemId);

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is equipped
            if (!item.dbItem.IsEquipped)
            {
                TempData["Error"] = "You cannot use an item you do not have equipped.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert this item is a teleportation scroll
            if (item.dbItem.ItemSourceId != ItemStatics.TeleportationScrollItemSourceId)
            {
                TempData["Error"] = "You cannot teleport with an item that is not a teleportation scroll.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
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

            if (to == LocationsStatics.JOKE_SHOP)
            {
                TempData["Error"] = "You cannot teleport to the Joke Shop.";
                TempData["SubError"] = "There is a powerful magical shield around this place.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert the location is a valid location
            if (!LocationsStatics.LocationList.GetLocation.Select(s => s.dbName).Contains(to))
            {
                TempData["Error"] = "That is not an eligible location to teleport to.";
                return RedirectToAction(MVC.PvP.Play());
            }

            TempData["Result"] = PlayerProcedures.TeleportPlayer(me, to, false);

            ItemProcedures.DeleteItem(itemId);

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
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxActionsPerUpdate)
            {
                TempData["Error"] = "You don't have enough energy to fight your transformation right now.";
                TempData["SubError"] = "Wait a bit.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already locked into their current form
            var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });
            if (itemMe != null && itemMe.IsPermanent)
            {
                TempData["Error"] = "You cannot return to an animate form again.";
                TempData["SubError"] = "You have spent too long and performed too many actions as an item or animal and have lost your desire and ability to be human gain.";
                return RedirectToAction(MVC.PvP.Play());
            }

            bool dungeonPenalty = ItemProcedures.ItemIncursDungeonPenalty(itemMe);

            TempData["Result"] = InanimateXPProcedures.ReturnToAnimate(me, dungeonPenalty);
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
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxActionsPerUpdate)
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

            var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });

            // assert item does have the ability to curse transform
            if (itemMe.ItemSource.CurseTFFormSourceId == null && (PvPStatics.DefaultTFCurseForms == null || PvPStatics.DefaultTFCurseForms.IsEmpty()))
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

            if (itemMe.ItemSource.CurseTFFormSourceId != null)
            {
                // item form curse - assert that the form does exist
                var form = FormStatics.GetForm(itemMe.ItemSource.CurseTFFormSourceId.Value);
                if (form == null || form.IsUnique)
                {
                    TempData["Error"] = "Unfortunately it seems that the animate form has either not yet been added to the game or is ineligible.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else
            {
                // theme form curse
                if (!PvPStatics.ChaosMode)
                {
                    TempData["Error"] = "This curse is only available when the game is in Chaos mode.";
                    return RedirectToAction(MVC.PvP.Play());
                }

                if (!DomainRegistry.Repository.FindSingle(new IsChaosChangesEnabled { UserId = owner.MembershipId }))
                {
                    TempData["Error"] = "You cannot currently attempt to transform your owner.";
                    TempData["SubError"] = "Your owner can opt in to this feature by enabling Chaos changes on their account.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }

            // all checks pass
            TempData["Result"] = InanimateXPProcedures.CurseTransformOwner(me, owner, itemMe, User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist));
            PlayerLogProcedures.AddPlayerLog(me.Id, (string)TempData["Result"], true);

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

            var inanimateMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });

            // assert that the player is owned
            if (inanimateMe.Owner == null)
            {
                TempData["Error"] = "You are not owned by anyone.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var owner = PlayerProcedures.GetPlayer(inanimateMe.Owner.Id);

            if (owner.BotId == AIStatics.LindellaBotId || owner.BotId == AIStatics.WuffieBotId)
            {
                // if player is owned by a vendor, assert that the player has been in their inventory for sufficient amount of time
                var hoursSinceSold = (int)Math.Floor(DateTime.UtcNow.Subtract(inanimateMe.LastSold).TotalHours);

                if (hoursSinceSold < (PvPStatics.HoursBeforeInanimatesCanSlipFree / 2))
                {
                    TempData["Error"] = "You cannot escape from your owner right now.";
                    TempData["SubError"] = "You must remain in the vendor's inventory for " + ((PvPStatics.HoursBeforeInanimatesCanSlipFree / 2) - hoursSinceSold) + " more hours before you can slip free.";
                    return RedirectToAction(MVC.PvP.Play());
                }
            }
            else if (owner.BotId == AIStatics.SoulbinderBotId)
            {
                // if player is being looked after by the soulbinder, allow time for their true owner to collect them
                var hoursSinceSold = (int)Math.Floor(DateTime.UtcNow.Subtract(inanimateMe.LastSold).TotalHours);

                if (hoursSinceSold < PvPStatics.HoursBeforeInanimatesCanSlipFree)
                {
                    TempData["Error"] = "Your soulbinding prevents you escaping right now.";
                    TempData["SubError"] = "You must remain in the soulbinder's inventory for " + (PvPStatics.HoursBeforeInanimatesCanSlipFree - hoursSinceSold) + " more hours before you can slip free.";
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
            DomainRegistry.Repository.Execute(new RemoveSoulbindingOnItem { ItemId = inanimateMe.Id });

            ItemProcedures.DropItem(inanimateMe.Id);
            var message = $"{me.GetFullName()}, your {inanimateMe.ItemSource.FriendlyName}, slipped free due to your inactivity and can be claimed by a new owner.";
            PlayerLogProcedures.AddPlayerLog(owner.Id, message, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, $"You slip free of your former owner, {owner.GetFullName()}", false);

            TempData["Result"] = "You have slipped free from your owner.";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ReserveName()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is greater than level 3 if they are mobile
            if (me.Level < 3 && me.Mobility == PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be level 3 or greater in order to reserve a name.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // if player is not mobile, see if the item they have become is at least level 3
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                var itemMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = me.Id });
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

            if (DomainRegistry.Repository.FindSingle(new IsAccountLockedOut { userId = me.MembershipId }))
            {
                return RedirectToAction(MVC.PvP.Play());
            }

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

            // assert player has not been hushed
            if (EffectProcedures.PlayerHasActiveEffect(me, CharacterPrankProcedures.HUSHED_EFFECT))
            {
                TempData["Error"] = "You have been hushed by a powerful mage and cannot currently shout!";
                TempData["SubError"] = "You will be able to shout again when the effect has worn off.";
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


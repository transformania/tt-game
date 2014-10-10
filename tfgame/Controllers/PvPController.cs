using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;

namespace tfgame.Controllers
{
    [InitializeSimpleMembership]
    public class PvPController : Controller
    {
        //
        // GET: /PvP/

        public ActionResult Play()
        {


            string loadtime = "";
            Stopwatch updateTimer = new Stopwatch();
            updateTimer.Start();

            // load up the covenant bindings into memory
            if (CovenantDictionary.IdNameFlagLookup.Count() == 0)
            {
                CovenantProcedures.LoadCovenantDictionary();
            }

            int myMembershipId = WebSecurity.CurrentUserId;

            ViewBag.MyMembershipId = myMembershipId;
            ViewBag.MaxLogSize = PvPStatics.MaxLogMessagesPerLocation;

            // assert that the player is logged in; otherwise ask them to do so
            if (myMembershipId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            // if the player is logged in but has no character, go to a setup screen
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me == null)
            {
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }

            if (Session["ContributionId"] == null)
            {
                Session["ContributionId"] = -1;
            }

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            PvPWorldStat WorldStat = PvPWorldStatProcedures.GetWorldStats();

            ViewBag.UpdateInProgress = false;

            double secondsSinceUpdate = Math.Abs(Math.Floor(WorldStat.LastUpdateTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));

            // if it has been long enough since last update, force an update to occur
            if (secondsSinceUpdate > 605 && WorldStat.WorldIsUpdating == false)
            {
                UpdateWorld("oogabooga99");
            }

            // turn off world update toggle if it's simply been too long
            if (secondsSinceUpdate > 90 && PvPStatics.AnimateUpdateInProgress == true)
            {
                PvPStatics.AnimateUpdateInProgress = false;
            }

            if (WorldStat.WorldIsUpdating == true && secondsSinceUpdate < 90)
            {
                ViewBag.UpdateInProgress = true;
            }

            PvPWorldStat PvPWorldStat = PvPWorldStatProcedures.GetWorldStats();

            // load the update date into memory
            PvPStatics.LastGameUpdate = PvPWorldStat.GameNewsDate;

            ViewBag.WorldTurnNumber = PvPWorldStat.TurnNumber;

            // player is inanimate, load up the inanimate endgame page
            if (me.Mobility == "inanimate")
            {
                GameOverViewModel inanimateOutput = new GameOverViewModel();

                inanimateOutput.Player = me;
                inanimateOutput.Form = FormStatics.GetForm(me.Form);
                inanimateOutput.Item = ItemStatics.GetStaticItem(inanimateOutput.Form.BecomesItemDbName);

                inanimateOutput.IsPermanent = ItemProcedures.GetItemByVictimName(me.FirstName, me.LastName).IsPermanent;

                inanimateOutput.AtLocation = ItemProcedures.PlayerIsItemAtLocation(me);
                inanimateOutput.NewMessageCount = MessageProcedures.GetMessageCountData(me).NewMessagesCount;

                if (inanimateOutput.AtLocation == null)
                {
                    inanimateOutput.WornBy = ItemProcedures.BeingWornBy(me);
                }

                inanimateOutput.StruggleChance = InanimateXPProcedures.GetStruggleChance(me);

                if (inanimateOutput.WornBy != null)
                {
                    List<LocationLog> actionsHere = LocationLogProcedures.GetLocationLogsAtLocation(inanimateOutput.WornBy.Player.dbLocationName).ToList();
                    List<LocationLog> validActionsHere = new List<LocationLog>();
                    foreach (LocationLog log in actionsHere) {
                        if (!log.Message.Contains("entered from") && !log.Message.Contains("left toward"))
                        {
                            validActionsHere.Add(log);
                        }
                    }
                    inanimateOutput.LocationLog = validActionsHere;
                    inanimateOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(inanimateOutput.WornBy.Player.dbLocationName);
                    
                }
                else
                {
                    inanimateOutput.LocationLog = LocationLogProcedures.GetLocationLogsAtLocation(me.dbLocationName);
                    inanimateOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(me.dbLocationName);
                }

                List<PlayerFormViewModel> playersHere = new List<PlayerFormViewModel>();
                foreach (PlayerFormViewModel p in inanimateOutput.PlayersHere) {
                    if (!PlayerProcedures.PlayerIsOffline(p.Player) && p.Player.Mobility == "full") {
                        playersHere.Add(p);
                    }
                }

                inanimateOutput.PlayersHere = playersHere.OrderByDescending(p => p.Player.Level);
                
                


                return View("Play_Inanimate", inanimateOutput);
            }

            // player is an animal, load up the inanimate endgame page
            if (me.Mobility == "animal")
            {
                GameOverViewModelAnimal animalOutput = new GameOverViewModelAnimal();
                animalOutput.You = me;

                animalOutput.Form = FormStatics.GetForm(me.Form);

                try { 
                    animalOutput.OwnedBy = PlayerProcedures.GetPlayerFormViewModel(me.IsPetToId);
                }
                catch
                {

                }
                animalOutput.WorldStats = PlayerProcedures.GetWorldPlayerStats();

                animalOutput.Location = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
                animalOutput.Location.FriendlyName_North = LocationsStatics.GetConnectionName(animalOutput.Location.Name_North);
                animalOutput.Location.FriendlyName_East = LocationsStatics.GetConnectionName(animalOutput.Location.Name_East);
                animalOutput.Location.FriendlyName_South = LocationsStatics.GetConnectionName(animalOutput.Location.Name_South);
                animalOutput.Location.FriendlyName_West = LocationsStatics.GetConnectionName(animalOutput.Location.Name_West);

                animalOutput.LocationLog = LocationLogProcedures.GetLocationLogsAtLocation(animalOutput.Location.dbName).Reverse();

                animalOutput.LocationItems = ItemProcedures.GetAllItemsAtLocation(animalOutput.Location.dbName, me);

                animalOutput.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(animalOutput.Location.dbName).Where(p => p.Form.MobilityType == "full");

                animalOutput.LastUpdateTimestamp = PvPWorldStatProcedures.GetLastWorldUpdate();

                animalOutput.NewMessageCount = MessageProcedures.GetMessageCountData(me).NewMessagesCount;

                ViewBag.AnimalImgUrl = ItemStatics.GetStaticItem(animalOutput.Form.BecomesItemDbName).PortraitUrl;

                animalOutput.IsPermanent = ItemProcedures.GetItemByVictimName(me.FirstName, me.LastName).IsPermanent;

                animalOutput.StruggleChance = InanimateXPProcedures.GetStruggleChance(me);

                return View("Play_Animal", animalOutput);

            }

            PlayPageViewModel output = new PlayPageViewModel();

            output.PvPWorldStat = PvPWorldStat;

            loadtime += "Before loading buffs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            BuffBox myBuffs = ItemProcedures.GetPlayerBuffs(me);
            loadtime += "After loading buffs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            if (myBuffs.HasSearchDiscount == true)
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

            output.LastUpdateTimestamp = PvPWorldStatProcedures.GetLastWorldUpdate();

            loadtime += "Start get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayersHere = PlayerProcedures.GetPlayerFormViewModelsAtLocation(me.dbLocationName).Where(p => p.Form.MobilityType == "full");
            loadtime += "End get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            // output.Location = Locations.GetLocation()

            output.Location = LocationsStatics.GetLocation.FirstOrDefault(x => x.dbName == me.dbLocationName);
            output.Location.FriendlyName_North = LocationsStatics.GetConnectionName(output.Location.Name_North);
            output.Location.FriendlyName_East = LocationsStatics.GetConnectionName(output.Location.Name_East);
            output.Location.FriendlyName_South = LocationsStatics.GetConnectionName(output.Location.Name_South);
            output.Location.FriendlyName_West = LocationsStatics.GetConnectionName(output.Location.Name_West);

            loadtime += "Start get location logs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.LocationLog = LocationLogProcedures.GetLocationLogsAtLocation(me.dbLocationName).Reverse();
            loadtime += "End get players here:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get player logs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayerLog = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse();
            loadtime += "End get player logs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            output.PlayerLogImportant = output.PlayerLog.Where(l => l.IsImportant == true);

            //loadtime += "Start get player spells:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            //output.Skills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(me.Id);
            //loadtime += "End get player logs:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get player items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.PlayerItems = ItemProcedures.GetAllPlayerItems(me.Id);
            loadtime += "End get player items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            loadtime += "Start get location items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";
            output.LocationItems = ItemProcedures.GetAllItemsAtLocation(output.Location.dbName, me);
            loadtime += "End get location items:  " + updateTimer.ElapsedMilliseconds.ToString() + "<br>";

            ViewBag.InventoryItemCount = output.PlayerItems.Count();
            output.MessageCounts = MessageProcedures.GetMessageCountData(me);
            output.WorldStats = PlayerProcedures.GetWorldPlayerStats();
            output.AttacksMade = me.TimesAttackingThisUpdate;
            ViewBag.AttacksMade = me.TimesAttackingThisUpdate;


            ViewBag.LoadTime = loadtime; 
            


            return View(output);
        }

         [Authorize]
        public ActionResult NewCharacter(NewCharacterViewModel player)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.ValidationMessage = "Your character was not created.  You can only use letters and your first and last names must be between 2 and 30 letters long." ;
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }

            if (player.FormName.Contains("woman_"))
            {
                player.Gender = "female";
            }
            else
            {
                player.Gender = "male";
            }

            // assert that neither the first name nor last name is reserved
            string fnamecheck = TrustStatics.NameIsReserved(player.FirstName);
            if (fnamecheck != "")
            {
                ViewBag.ValidationMessage = "You can't use the first name '" + player.FirstName + "'.  It is reserved.";
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }

            // assert that neither the first name nor last name is reserved
            string lnamecheck = TrustStatics.NameIsReserved(player.LastName);
            if (lnamecheck != "")
            {
                ViewBag.ValidationMessage = "You can't use the last name '" + player.LastName + "'.  It is reserved or else not allowed.";
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }
            
            // assert player does not currently have an animate player
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            if (me != null && me.Mobility == "full")
            {
                ViewBag.ValidationMessage = "You cannot create a new character right now.  You already have a fully animate character already, " + me.FirstName + " " + me.LastName + ".";
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }

            // assert player does not have more than 1 account already
            if (TrustStatics.AccountCanHaveMultis(WebSecurity.CurrentUserId) == false && PlayerProcedures.IsMyIPInUseAndAnimate(Request.UserHostAddress) == true)
            {

                ViewBag.ValidationMessage = "Your character was not created.  It looks like your IP address, <b>" + Request.UserHostAddress + "</b> already has 1 animate character in this world, and the current limit is 1. ";
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }

            string result = PlayerProcedures.SaveNewPlayer(player);

            if (result != "saved")
            {
                ViewBag.ValidationMessage = "Your character was not created.  Reason:  " + result;
                return View("~/Views/PvP/MakeNewCharacter.cshtml");
            }

            PlayerProcedures.LogIP(Request.UserHostAddress);

            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult Restart()
        {
        
            return View("~/Views/PvP/MakeNewCharacter.cshtml");
        }

         [Authorize]
        public ActionResult MoveTo(string locname)
        {

            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction("Play");
            }

            PlayerProcedures.LogIP(Request.UserHostAddress);

            Player me = PlayerProcedures.GetPlayerFromMembership();

            // assert that this player is mobile
            if (PlayerCanPerformAction(me, "move") == false)
            {
                return RedirectToAction("Play");
            }

            Location currentLocation = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
            Location nextLocation = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == locname);

            // assert this location does have a connection to the next one
            if (currentLocation.Name_North != locname && currentLocation.Name_East != locname && currentLocation.Name_South != locname && currentLocation.Name_West != locname)
            {
                TempData["Error"] = "You can't go there from here.";
                return RedirectToAction("Play");
            }

            BuffBox buffs = ItemProcedures.GetPlayerBuffs(me);

            // assert that this player has sufficient action points for this move
            if (me.ActionPoints < PvPStatics.LocationMoveCost - buffs.MoveActionPointDiscount())
            {

                TempData["Error"] = "You don't have enough action points to move.";
                TempData["SubError"] = "Wait a while; you will receive more action points every ten minutes.";
                return RedirectToAction("Play");
            }

            // assert that this player is not carrying too much
            if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 1, buffs) == true)
            {
                TempData["Error"] = "You are carrying too much to move.";
                TempData["SubError"] = "Reduce the amount of items you are carrying to be able to move again.";
                return RedirectToAction("Play");
            }

             // assert that the player has not attacked too recently to move
            double lastAttackTimeAgo = Math.Abs(Math.Floor(me.LastCombatTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));
            if (lastAttackTimeAgo < 45)
            {
                TempData["Error"] = "You are resting from a recent attack.";
                TempData["SubError"] = "You must wait " + (45-lastAttackTimeAgo) + " more seconds before moving.";
                return RedirectToAction("Play");
            }

            if (me.Mobility == "animal")
            {
                ItemProcedures.MoveAnimalItem(me, nextLocation.dbName);

                TempData["Result"] = "You move to " + nextLocation.Name + ".";
                string leavingMessage = me.FirstName + " " + me.LastName + " (feral) left toward " + nextLocation.Name;
                string enteringMessage = me.FirstName + " " + me.LastName + " (feral) entered from " + currentLocation.Name;
                LocationLogProcedures.AddLocationLog(me.dbLocationName, leavingMessage);
                LocationLogProcedures.AddLocationLog(locname, enteringMessage);
                return RedirectToAction("Play");
            }

            decimal sneakChance = buffs.SneakPercent();
            decimal stumbleChance = -1*buffs.EvasionPercent();
             decimal moveAPdiscount = buffs.MoveActionPointDiscount();

             Random die = new Random();
            double sneakroll = die.NextDouble() * 100;
            if (sneakroll < (double)sneakChance)
            {
                TempData["Result"] = "You silently move to " + nextLocation.Name + ".";
            }
            else
            {

                string msg = "";

                // if the attacker's evasion negation is too low, add in a chance of the spell totally missing.
                if (stumbleChance > 0)
                {
                    Random rand = new Random();
                    double roll = rand.NextDouble() * 200;
                    if (roll < (double)stumbleChance)
                    {
                        msg = "Due to your poor evasion and clumbsiness you trip and fall, wasting some energy.  ";
                        PlayerProcedures.ChangePlayerActionMana(1, 0, 0, me.Id);
                    }

                }

                TempData["Result"] = msg + "You move to " + nextLocation.Name + ".";
                string leavingMessage = me.FirstName + " " + me.LastName + " left toward " + nextLocation.Name;
                string enteringMessage = me.FirstName + " " + me.LastName + " entered from " + currentLocation.Name;
                LocationLogProcedures.AddLocationLog(me.dbLocationName, leavingMessage);
                LocationLogProcedures.AddLocationLog(locname, enteringMessage);
            }

            PlayerProcedures.MovePlayer(locname, moveAPdiscount);

         



            string playerLogMessage = "You moved from <b>" + currentLocation.Name + "</b> to <b>" + nextLocation.Name + "</b>.";
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);


            return RedirectToAction("Play");
        }

        [Authorize]
         public ActionResult AttackModal(int targetId)
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
             Player target = PlayerProcedures.GetPlayer(targetId);
            IEnumerable<SkillViewModel2> output = SkillProcedures.GetSkillViewModelsOwnedByPlayer(me.Id);
            ViewBag.TargetId = targetId;
            ViewBag.TargetName = target.FirstName + " " + target.LastName;
             return PartialView("partial/AjaxAttackModal", output);
         }

         [Authorize]
        public ActionResult Attack(int targetId, string attackName)
        {

            PlayerProcedures.LogIP(Request.UserHostAddress);

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            LogBox logs = new LogBox();

            #region validation checks

            // assert player is logged in
            if (WebSecurity.CurrentUserId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "attack") == false)
            {
                return RedirectToAction("Play");
            }

            // assert player hasn't made too many attacks this update
            if (me.TimesAttackingThisUpdate >= PvPStatics.MaxAttacksPerUpdate)
            {
                TempData["Error"] = "You have attacked too much this update.";
                TempData["SubError"] = "You can only attack " + PvPStatics.MaxAttacksPerUpdate + " times per update.  Wait a bit.";
                return RedirectToAction("Play");
            }

            // assert that player has enough action points to attack
            if (me.ActionPoints < PvPStatics.AttackCost)
            {
                TempData["Error"] = "You don't have enough action points to attack.";
                TempData["SubError"] = "You will receive more action points next turn.";
                return RedirectToAction("Play");
            }

            // assert that it is not too late in the round for this attack to happen
            DateTime lastupdate = PvPWorldStatProcedures.GetLastWorldUpdate();
            double secondsAgo = Math.Abs(Math.Floor(lastupdate.Subtract(DateTime.UtcNow).TotalSeconds));

            if (secondsAgo > 570 && PvPStatics.ChaosMode == false)
            {
                TempData["Error"] = "It is too late into this turn to attack.";
                TempData["SubError"] = "You must attack within the first 9.5 minutes of a round.";
                return RedirectToAction("Play");
            }

            // assert that it is not too EARLY in the round for this attack to happen
            double secondsFrom = Math.Abs(Math.Floor(lastupdate.Subtract(DateTime.UtcNow).TotalSeconds));

            if (secondsAgo < 90)
            {
                TempData["Error"] = "It is too early into this turn to attack.";
                TempData["SubError"] = "You must wait at least 90 seconds after the round update has started to attack.";
                return RedirectToAction("Play");
            }


            // assert that this player does have this skill
            SkillViewModel2 skillBeingUsed = SkillProcedures.GetSkillViewModel(attackName, me.Id);
            if (skillBeingUsed == null)
            {
                TempData["Error"] = "You don't seem to have this spell.";
                TempData["SubError"] = "This spell may have run out of charges or have been forgotten.  You will need to relearn it.";
                return RedirectToAction("Play");
            }

            // assert player does not have more than 1 accounts already
            if (TrustStatics.AccountCanHaveMultis(WebSecurity.CurrentUserId) == false && PlayerProcedures.IsMyIPInUseAndAnimate(Request.UserHostAddress, me) == true)
            {
                TempData["Error"] = "This character looks like a multiple account, which is illegal.  This character will not be allowed to attack.";
                TempData["SubError"] = "You can only have 1 animate character in PvP mode and 1 animate character in non-PvP mode at a time.";
                return RedirectToAction("Play");
            }

            // assert player has enough mana to cast
            if (me.Mana < skillBeingUsed.Skill.ManaCost)
            {
                TempData["Error"] = "You don't have enough mana to cast this.";
                TempData["SubError"] = "You can recover mana using potions, meditating, or simply waiting for it to replenish over time.";
                return RedirectToAction("Play");
            }

            // assert that the target is still in the same room
            Player targeted = PlayerProcedures.GetPlayer(targetId);

            if (me.dbLocationName != targeted.dbLocationName)
            {
                TempData["Error"] = "Your target no longer seems to be here.";
                TempData["SubError"] = "Your target has probably left.  Maybe you can follow them and attack when you've caught up.";
                return RedirectToAction("Play");
            }

            // assert that the target is not inanimate
            if (targeted.Mobility == "inanimate")
            {
                TempData["Error"] = "Your target is already inanimate.";
                TempData["SubError"] = "Transformation magic will have no effect on them anymore.  Someone else might have cast the final spell.";
                return RedirectToAction("Play");
            }

            // assert that the target is not an animal
            if (targeted.Mobility == "animal")
            {
                TempData["Error"] = "Your target is an animal.";
                TempData["SubError"] = "Transformation magic will have no effect on them anymore.";
                return RedirectToAction("Play");
            }

             // assert that the target is not offline
            if (PlayerProcedures.PlayerIsOffline(targeted)==true)
            {
                TempData["Error"] = "This player is offline.";
                TempData["SubError"] = "Offline players can no longer be attacked.";
                return RedirectToAction("Play");
            }

            // assert that this player does not currently have a lock on their account
            if (me.FlaggedForAbuse == true)
            {
                TempData["Error"] = "This player has been flagged by a moderator for suspicious actions and is not allowed to attack at this time.";
                return RedirectToAction("Play");
            }

            // assert that the victim is not the own player
            if (targeted.Id == me.Id)
            {
                TempData["Error"] = "You can't cast magic on yourself.";
                return RedirectToAction("Play");
            }

            // if the spell is a curse, check that the target doesn't already have the effect
            if (skillBeingUsed.Skill.GivesEffect != null)
            {
                if (EffectProcedures.PlayerHasEffect(targeted, skillBeingUsed.Skill.GivesEffect) == true)
                {
                    TempData["Error"] = "This target is already afflicted with this curse or else is still in the immune cooldown period of it.";
                    TempData["SubError"] = "You can always try again later...";
                return RedirectToAction("Play");
                }
            }

          

            DbStaticSkill skill = SkillStatics.GetStaticSkill(attackName);
            DbStaticForm futureForm = FormStatics.GetForm(skill.FormdbName);

            // prevent low level players from taking on high level bots
            if (targeted.MembershipId <= -4)
            {
                if (me.Level <= 3)
                {
                    TempData["Error"] = "You feel too intimdated by your target and find yourself unable to launch your spell.";
                    TempData["SubError"] = "You must gain some more experience before trying to take this target on.";
                    return RedirectToAction("Play");
                }

                // Donna
                if (targeted.MembershipId == -4)
                {
                    if (futureForm == null || futureForm.MobilityType == "full")
                    {
                        TempData["Error"] = "You get the feeling this type of spell won't work against Donna.";
                        TempData["SubError"] = "Maybe a different one would do...";
                        return RedirectToAction("Play");
                    }
                }

                // Valentine
                if (targeted.MembershipId == -5)
                {
                    // only allow weakens against Valentine for now (replace with Duel spell later?)
                    if (futureForm != null)
                    {
                        TempData["Error"] = "You get the feeling this type of spell won't work against Lord Valentine.";
                        TempData["SubError"] = "Maybe a different one would do...";
                        return RedirectToAction("Play");
                    }
                }

            }


             // don't worry about bots
            if (targeted.MembershipId > 0)
            {

                if (me.InPvP == true || targeted.InPvP == true)
                {
                    if (FriendProcedures.PlayerIsMyFriend(me, targeted) == true)
                    {
                        // do nothing; friends are okay to cast any spell types
                    }

                    // no inter protection/non spell casting
                    else if (me.InPvP != targeted.InPvP)
                    {

                        TempData["Error"] = "You must be in the same Protection/non-Protection mode as your target in order to cast spells at them.";
                        return RedirectToAction("Play");

                    }

                    //  if the form is null (curse) or not fully animate, block it entirely
                    else if (futureForm.MobilityType == null || futureForm.MobilityType != "full")
                    {
                        TempData["Error"] = "This player is in protection and immune from inanimate and animal spells except by their friends.";
                        return RedirectToAction("Play");
                    }
                }

            }

          
           

            #endregion

            try { 
                TempData["Result"] = AttackProcedures.Attack(me, targeted, skillBeingUsed);
            }
            catch (Exception e)
            {
                TempData["Error"] = "There was a server error while carrying out your attack.  Reason:  <br><br>" + e.ToString() + ".<br><br>If this error persists please report this bug at on the game forums, found at http://luxianne.com/forum/viewforum.php?f=5&sid=3eeb3207fe885b88851edcb7d964eb94.  When posting this bug please including information on which spell you were casting, whether you or your target was in protection mode, and if possible how close to a turn update this attack was made.";

              //  PlayerProcedures.AddAttackCount(me, -1);
               // PlayerProcedures.ChangePlayerActionMana(4, 0, skillBeingUsed.Skill.ManaCost, me.Id);
            }


            AIProcedures.CheckAICounterattackRoutine(me, targeted);

           

            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult Meditate()
        {

            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction("Play");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player has sufficient action points to meditate
            if (me.ActionPoints < PvPStatics.MeditateCost)
            {
                TempData["Error"] = "You don't have enough action points to meditate.";
                TempData["SubError"] = "";
                return RedirectToAction("Play");
            }

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "meditate") == false)
            {
                return RedirectToAction("Play");
            }

            // assert player has not cleansed and meditated too much this turn
            if (me.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
            {
                TempData["Error"] = "You have cleansed and meditate the maximum number of times this update.";
                TempData["SubError"] = "Next turn you will be able to cleanse again.";
                return RedirectToAction("Play");
            }

            BuffBox mybuffs = ItemProcedures.GetPlayerBuffs(me);

            TempData["Result"] = PlayerProcedures.Meditate(me, mybuffs);


            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult Cleanse()
        {

            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction("Play");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player has sufficient action points to meditate
            if (me.ActionPoints < PvPStatics.CleanseCost)
            {
                TempData["Error"] = "You don't have enough action points to cleanse.";
                TempData["SubError"] = "Wait a while for more.";
                return RedirectToAction("Play");
            }

            BuffBox mybuffs = ItemProcedures.GetPlayerBuffs(me);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "cleanse") == false)
            {

                return RedirectToAction("Play");
            }

            //assert player has enough mana
            if (me.Mana < PvPStatics.CleanseManaCost)
            {
                TempData["Error"] = "You don't have enough mana to cleanse.";
                return RedirectToAction("Play");
            }

            // assert player has not cleansed and meditated too much this turn
            if (me.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
            {
                TempData["Error"] = "You have cleansed and meditate the maximum number of times this update.";
                TempData["SubError"] = "Next turn you will be able to cleanse again.";
                return RedirectToAction("Play");
            }

            TempData["Result"] = PlayerProcedures.Cleanse(me, mybuffs);



            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult MySkills()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            return View("MySkills", SkillProcedures.GetSkillViewModelsOwnedByPlayer(me.Id));
        }

         [Authorize]
        public ActionResult Search()
        {

            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction("Play");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "search") == false)
            {
                return RedirectToAction("Play");
            }

            BuffBox mybuffs = ItemProcedures.GetPlayerBuffs(me);

            decimal searchCostAfterbuffs = PvPStatics.SearchAPCost;
            if (mybuffs.HasSearchDiscount)
            {
                searchCostAfterbuffs = 3;
            }
            // assert player has sufficient action points to search
            if (me.ActionPoints < searchCostAfterbuffs)
            {
                TempData["Error"] = "You don't have enough action points to search.";
                TempData["SubError"] = "Wait a while; you will receive more over time.";
                return RedirectToAction("Play");
            }

            if (mybuffs.HasSearchDiscount == true)
            {
                PlayerProcedures.ChangePlayerActionMana(PvPStatics.SearchAPCost-1, 0, 0, me.Id);
            }
            else
            {
                PlayerProcedures.ChangePlayerActionMana(PvPStatics.SearchAPCost, 0, 0, me.Id);
            }

            
            TempData["Result"] = PlayerProcedures.SearchLocation(me, me.dbLocationName);

            // write to logs
            string locationLogMessage = "<span style='color:  purple'>" + me.FirstName + " " + me.LastName + " searched here.</span>";
            LocationLogProcedures.AddLocationLog(me.dbLocationName, locationLogMessage);
            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
            string playerLogMessage = "You searched at " + here.Name + ".";
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);

            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult ClearLog()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            PlayerLogProcedures.ClearPlayerLog(me.Id);
            return RedirectToAction("Play");
        }

        public ActionResult DismissNotifications()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            PlayerLogProcedures.DismissImportantLogs(me.Id);
            return RedirectToAction("Play");
        }

        public ActionResult ViewLog()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            return View("ViewLog", PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse());
        }

         [Authorize]
        public ActionResult MyInventory()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (me.MembershipId == WebSecurity.CurrentUserId)
            {
                ViewBag.BelongsToPlayer = "block";
            }
            else
            {
                ViewBag.BelongsToPlayer = "none";
            }


            InventoryBonusesViewModel output = new InventoryBonusesViewModel
            {
                Items = ItemProcedures.GetAllPlayerItems(me.Id),
                Bonuses = ItemProcedures.GetPlayerBuffs(me),
            };

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View("Inventory", output);
        }

         [Authorize]
        public ActionResult Take(int id)
        {

            // assert player is logged in
            if (WebSecurity.CurrentUserId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            BuffBox myBuffs = ItemProcedures.GetPlayerBuffs(me);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "pickup") == false)
            {
                return RedirectToAction("Play");
            }

            ItemViewModel pickup = ItemProcedures.GetItemViewModel(id);



            //assert that the item is indeed at this location and on the ground
            if (pickup.dbItem.dbLocationName != me.dbLocationName)
            {
                TempData["Error"] = "That item isn't in this location or else it has already been picked up.";
                return RedirectToAction("Play");
            }

            // assert that the player is not carrying too much already UNLESS the item is a pet
            if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, myBuffs)==true && pickup.Item.ItemType!=PvPStatics.ItemType_Pet) {
                TempData["Error"] = "You are carrying too many items to pick this up.";
                TempData["SubError"] = "Use, drop, or wear/equip something you are carrying to make more room.  Some accessories may also allow you to carry more.";
                return RedirectToAction("Play");
            }

            // if the item is an animal, assert that the player does not already have one since pets must be automatically equipped
            if (pickup.Item.ItemType == PvPStatics.ItemType_Pet && ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet) > 0)
            {
                TempData["Error"] = "You already have an animal or familiar as your pet.";
                TempData["SubError"] = "Release any existing pets you have before you can tame this one.";
                return RedirectToAction("Play");
            }

            // assert the item is not a consumeable type or else is AND is in the same mode as the player
            if (pickup.Item.ItemType == "consumable" && pickup.dbItem.PvPEnabled != me.InPvP)
            {
                TempData["Error"] = "This item is marked as being in a different PvP mode from you.";
                TempData["SubError"] = "You are not allowed to pick up consumable-type items that are not in PvP if you are not in PvP and the same for non-PvP.";
                return RedirectToAction("Play");
            }

            string playerLogMessage = "";
            string locationLogMessage = "";
            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);


            // if the item is inanimate, the item to the player's inventory
            if (pickup.Item.ItemType!=PvPStatics.ItemType_Pet) {
                TempData["Result"] = ItemProcedures.GiveItemToPlayer(pickup.dbItem.Id, me.Id);
                playerLogMessage = "You picked up a <b>" + pickup.Item.FriendlyName + "</b> at " + here.Name + " and put it into your inventory.";
                locationLogMessage = me.FirstName + " " + me.LastName + " picked up a <b>" + pickup.Item.FriendlyName + "</b> here.";
                
            }
                // item is an animal, equip it automatically
            else if (pickup.Item.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Result"] = ItemProcedures.GiveItemToPlayer(pickup.dbItem.Id, me.Id);
                ItemProcedures.EquipItem(pickup.dbItem.Id, true);
                AnimalProcedures.ChangeOwner(pickup.dbItem, me.Id);
                playerLogMessage = "You tamed <b>" + pickup.dbItem.VictimName + " the " + pickup.Item.FriendlyName + "</b> at " + here.Name + " and put it into your inventory.";
               // PlayerLogProcedures.AddPlayerLog()
                locationLogMessage = me.FirstName + " " + me.LastName + " tamed <b>" + pickup.dbItem.VictimName + " the " + pickup.Item.FriendlyName + "</b> here.";

                Player personAnimal = PlayerProcedures.GetPlayerWithExactName(pickup.dbItem.VictimName);

                string notificationMsg = me.FirstName + " " + me.LastName + " has tamed you.  You will now follow them wherever they go and magically enhance their abilities by being their faithful companion.";
                PlayerLogProcedures.AddPlayerLog(personAnimal.Id, notificationMsg, true);

            }




            PlayerProcedures.AddMinutesToTimestamp(me, 15, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationLogMessage);


            return RedirectToAction("Play");

        }

        public ActionResult Drop(int itemId)
        {

            // assert player is logged in
            if (WebSecurity.CurrentUserId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "drop") == false)
            {
                return RedirectToAction("Play");
            }

            ItemViewModel dropme = ItemProcedures.GetItemViewModel(itemId);

            // assert player does own this
            if (dropme.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";

                return RedirectToAction("Play");
            }

            // assert player is not currently wearing this UNLESS it is an animal type, since pets are always "equipped"
            if (dropme.dbItem.IsEquipped == true && dropme.Item.ItemType!=PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "You can't drop this item.";
                TempData["SubError"] = "Unequip this item first if you are wearing it.";
                return RedirectToAction("Play");
            }


            TempData["Result"] = ItemProcedures.DropItem(itemId, me.dbLocationName);

            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            string playerLogMessage;
            string locationLogMessage;

            // animals are released
            if (dropme.Item.ItemType == PvPStatics.ItemType_Pet)
            {
                playerLogMessage = "You released your " + dropme.Item.FriendlyName + " at " + here.Name + ".";
                locationLogMessage = me.FirstName + " " + me.LastName + " released a <b>" + dropme.Item.FriendlyName + "</b> here.";

                Player personAnimal = PlayerProcedures.GetPlayerWithExactName(dropme.dbItem.VictimName);

                string notificationMsg = me.FirstName + " " + me.LastName + " has released you.  You are now feral and may now wander the town at will until another master tames you.";
                PlayerLogProcedures.AddPlayerLog(personAnimal.Id, notificationMsg, true);

               
            }
            // everything else is dropped
            else
            {
                playerLogMessage = "You dropped a " + dropme.Item.FriendlyName + " at " + here.Name + ".";
                locationLogMessage = me.FirstName + " " + me.LastName + " dropped a <b>" + dropme.Item.FriendlyName + "</b> here.";
            }

            PlayerProcedures.AddMinutesToTimestamp(me, 15, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, playerLogMessage, false);
            LocationLogProcedures.AddLocationLog(here.dbName, locationLogMessage);

            return RedirectToAction("Play");

            // remove this item from the player's inventory

        }

         [Authorize]
        public ActionResult Equip(int itemId, bool putOn)
        {
            // assert player is logged in
            if (WebSecurity.CurrentUserId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "equip") == false)
            {
                return RedirectToAction("Play");
            }

            ItemViewModel item = ItemProcedures.GetItemViewModel(itemId);

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";

                return RedirectToAction("Play");
            }

            // assert that this item doesn't have the put on this turn lock
            if (item.dbItem.EquippedThisTurn == true)
            {
                TempData["Error"] = "You just put this on.";
                TempData["SubError"] = "You'll have to wait until next turn to take this off.";
                return RedirectToAction("Play");
            }

            if (putOn == true)
            {

                // if item is not accessory, you can only wear one
                if (item.Item.ItemType != PvPStatics.ItemType_Accessory && (ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, item.Item.ItemType) > 0))
                {
                    TempData["Error"] = "You are already wearing a " + item.Item.ItemType + ".";
                    TempData["SubError"] = "Remove the one you are currently wearing first.";
                    return RedirectToAction("Play");
                }

                // if item is an accessory, you can only wear two
                else if (item.Item.ItemType == PvPStatics.ItemType_Accessory && (ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, item.Item.ItemType) > 1))
                {
                    TempData["Error"] = "You are already equipped two accessories.";
                    TempData["SubError"] = "Remove at least one you are currently equipping first.";
                    return RedirectToAction("Play");
                }

                // if item is an accessory, you can't wear two of the same thing
                if (item.Item.ItemType == PvPStatics.ItemType_Accessory && ItemProcedures.PlayerIsWearingNumberOfThisExactItem(me.Id, item.dbItem.dbName) == 1) {
                    TempData["Error"] = "You are already equipped with an accessory of this type.";
                    TempData["SubError"] = "You can't equip two of the same accessory at a time.";
                    return RedirectToAction("Play");
                }

            }
            else
            {

            }


            TempData["Result"] = ItemProcedures.EquipItem(itemId, putOn);

            return RedirectToAction("MyInventory");

        }

         [Authorize]
        public ActionResult Use(int itemId)
        {

            if (PvPStatics.AnimateUpdateInProgress == true)
            {
                TempData["Error"] = "Player update portion of the world update is still in progress.";
                TempData["SubError"] = "Try again a bit later when the update has progressed farther along.";
                return RedirectToAction("Play");
            }

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "equip") == false)
            {
                return RedirectToAction("Play");
            }

            ItemViewModel item = ItemProcedures.GetItemViewModel(itemId);

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";
                return RedirectToAction("Play");
            }

            // assert that this item is of a consumeable type
            if (item.Item.ItemType.Contains("consumeable"))
            {
                TempData["Error"] = "You can't use that type of item.";
                return RedirectToAction("Play");
            }

            // if this item is a teleportation scroll, redirect to the teleportation page.
            if (item.dbItem.dbName == "item_consumeable_teleportation_scroll")
            {
                IEnumerable<Location> output = LocationsStatics.GetLocation.Where(l => l.dbName != "");
                return View("TeleportMap", output);
            }
         
            string result = ItemProcedures.UseItem(itemId);

            PlayerProcedures.AddMinutesToTimestamp(me, 15, true);

            TempData["Result"] = result;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            PlayerLogProcedures.AddPlayerLog(me.Id, result, false);

            return RedirectToAction("MyInventory");

        }

        public ActionResult LookAtPlayer(int id)
        {

            PlayerFormViewModel playerLookedAt = PlayerProcedures.GetPlayerFormViewModel(id);

            PlayerFormItemsSkillsViewModel output = new PlayerFormItemsSkillsViewModel
            {
                PlayerForm = playerLookedAt,
                Skills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(id),
                Items = ItemProcedures.GetAllPlayerItems(id).Where(i => i.dbItem.IsEquipped == true),
                Bonuses = ItemProcedures.GetPlayerBuffs(playerLookedAt.Player.ToDbPlayer())
            };


            ViewBag.HasBio = SettingsProcedures.PlayerHasBio(output.PlayerForm.Player.MembershipId);

            ViewBag.TimeUntilLogout = 60 - Math.Abs(Math.Floor(playerLookedAt.Player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            if (playerLookedAt.Form.MobilityType == "inanimate" || playerLookedAt.Form.MobilityType == "animal")
            {
                

                Item playerItem = ItemProcedures.GetItemByVictimName(playerLookedAt.Player.FirstName, playerLookedAt.Player.LastName);
                DbStaticItem playerItemStatic = ItemStatics.GetStaticItem(playerItem.dbName);

                if (playerLookedAt.Form.MobilityType == "inanimate")
                {
                    ViewBag.ImgUrl = "itemsPortraits/" + playerItemStatic.PortraitUrl;
                }
                else if (playerLookedAt.Form.MobilityType == "animal")
                {
                    ViewBag.ImgUrl = "animalPortraits/" + playerItemStatic.PortraitUrl;
                }
                

                ViewBag.ItemLevel = playerItem.Level;
                ViewBag.FormDescriptionItem = playerItemStatic.Description;

                if (playerItemStatic.ItemType == PvPStatics.ItemType_Pet)
                {
                    if (playerItem.OwnerId != -1)
                    {
                        ViewBag.IsWorn = "This creature has been tamed and is following their master.";
                    }
                    else
                    {
                        ViewBag.IsWorn = "This creature has not been tamed as is running around feral.";
                    }
                }
                else
                {

                    if (playerItem.OwnerId != -1)
                    {
                        ViewBag.IsWorn = "This item is currently being carried and possibly worn by another player.";
                    }
                    else
                    {
                        ViewBag.IsWorn = "This item is not currently owned and is lying around available to be claimed by whoever comes across them.";
                    }
                }

            
              
                
                return View("LookAtPlayerInanimate", output);
            }
            else {

                ViewBag.AtLocation = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == output.PlayerForm.Player.dbLocationName).Name;

                return View("LookAtPlayer", output);
            }
        }

        public ActionResult LookAtPlayerItem(string vicname)
        {
            Player victim = PlayerProcedures.GetPlayerWithExactName(vicname);
            return RedirectToAction("LookAtPlayer", new RouteValueDictionary(
    new { controller = "PvP", action = "LookAtPlayer", id = victim.Id }));
        }

         [Authorize]
        public ActionResult MyMessages()
        {
            // this might fix some odd log-off message interception oddities... maybe?
            int myMembershipId = WebSecurity.CurrentUserId;
            if (myMembershipId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

           Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
           MessageBag output = MessageProcedures.GetPlayerMessages(me);

            // if you are inanimate and are being worn, grab the data on who is wearing you

           if (me.Mobility == "inanimate")
           {
               try
               {
                   PlayerFormViewModel personWearingMe = ItemProcedures.BeingWornBy(me);
                   output.WearerId = personWearingMe.Player.Id;
                   output.WearerName = personWearingMe.Player.FirstName + " " + personWearingMe.Player.LastName;
               }
               catch
               {

               }
           
           }

           ViewBag.Result = TempData["Result"];
            return View("MyMessages", output);
        }

         [Authorize]
        public ActionResult DeleteMessage(int messageId)
        {

            // assert player owns message
            if (MessageProcedures.PlayerOwnsMessage(messageId) == false)
            {
                TempData["Error"] = "You can't delete this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction("Play");
            }

            MessageProcedures.DeleteMessage(messageId);
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            MessageBag output = MessageProcedures.GetPlayerMessages(me);
            return RedirectToAction("MyMessages");
        }

         [Authorize]
        public ActionResult DeleteAllMessages()
        {

            // assert player is logged in
            if (WebSecurity.CurrentUserId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            MessageProcedures.DeleteAllMessages();
            return RedirectToAction("MyMessages");
        }

         [Authorize]
        public ActionResult ReadMessage(int messageId)
        {
            // assert player owns message
            if (MessageProcedures.PlayerOwnsMessage(messageId) == false)
            {
                TempData["Error"] = "You can't read this message.";
                TempData["SubError"] = "It wasn't sent to you.";
                return RedirectToAction("Play");
            }
            MessageViewModel output = MessageProcedures.GetMessageAndMarkAsRead(messageId);
           // ViewBag.IsReplyable = PlayerProcedures.GetPlayer(output)

            return View("ReadMessage", output);
        }


        public ActionResult PlayerLookup(string name)
        {
            PlayerSearchViewModel output = new PlayerSearchViewModel();
            return View("PlayerLookup", name);
        }

        public ActionResult PlayerLookupSend(PlayerSearchViewModel results)
        {

            Player result = PlayerProcedures.GetPlayerWithExactName(results.FirstName + " " + results.LastName);

            if (result != null)
            {
                results.ExactPlayer = result;
                results.FoundThem = true;
            }
            else
            {
                results.FoundThem = false;
            }

            return View("PlayerLookup", results);
        }

         [Authorize]
        public ActionResult Write(int playerId, int responseTo)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            Message output = new Message();
            output.SenderId = me.Id;
            output.ReceiverId = playerId;
            Player sendingTo = PlayerProcedures.GetPlayer(output.ReceiverId);
            ViewBag.SendingToName = sendingTo.FirstName + " " + sendingTo.LastName;

            if (responseTo != -1)
            {

                MessageViewModel msgRepliedTo = MessageProcedures.GetMessage(responseTo);

                // assert the letter being replied to is yours
                if (msgRepliedTo.dbMessage.ReceiverId != me.Id)
                {
                    TempData["Result"] = "You can't reply to this message since the original was not sent to you.";
                    return RedirectToAction("MyMessages");
                }
                else
                {
                    ViewBag.RespondingToMsg = msgRepliedTo.dbMessage.MessageText;
                }

            }

            return View("Write", output);
        }

         [Authorize]
        public ActionResult SendMessage(Message input)
        {
            if (input.MessageText == null || input.MessageText == "")
            {
                ViewBag.ErrorMessage = "You need to write something to send to this person.";
                return View("Write", input);
               
            }

            if (input.MessageText.Length> 1000)
            {
                ViewBag.ErrorMessage = "Your message is too long.";
                return RedirectToAction("Write", input);
            }
            MessageProcedures.AddMessage(input);
            TempData["Result"] = "Your message has been sent.";
            return RedirectToAction("MyMessages");
        }

         [Authorize]
        public ActionResult InanimateAction(string actionName)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            PlayerFormViewModel mePlus = PlayerProcedures.GetPlayerFormViewModel(me.Id);
            PlayerFormViewModel wearer = ItemProcedures.BeingWornBy(me);
            Item meDbItem = ItemProcedures.GetItemByVictimName(me.FirstName, me.LastName);
            DbStaticItem meItem = ItemStatics.GetStaticItem(meDbItem.dbName);

            // assert that player is inanimate
            if (me.Mobility != "inanimate")
            {
                TempData["Error"] = "You are not inanimate";
                return RedirectToAction("Play");
            }

            // assert item is owned by wearer
            if (meDbItem.OwnerId != wearer.Player.Id)
            {
                TempData["Error"] = "You are not currently owned by this player.";
                TempData["SubError"] = "Your former owner must have dropped you or was transformed themself.";
                return RedirectToAction("Play");
            }
          
            // assert player has not acted too many times already
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You don't have enough energy to physically or psychically interact with your owner right now.";
                TempData["SubError"] = "Wait a bit.";
                return RedirectToAction("Play");
            }

            string thirdP = "";
            string firstP = "";
            string pronoun = wearer.Player.Gender == "female" ? "She" : "He";

            if (actionName == "rub")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, .25M, 0, wearer.Player.Id);
                thirdP = "<span style='color: blue'>You feel " + me.FirstName + " " + me.LastName + ", currently your " + meItem.FriendlyName + ", every so slightly rubbing against your skin affectionately.  You gain a tiny amount of willpower from your inanimate belonging's subtle but kind gesture.</blue>";
                firstP = "You affectionately rub against your current owner, " + wearer.Player.FirstName + " " + wearer.Player.LastName + ".  " + pronoun + " gains a tiny amount of willpower from your subtle but kind gesture.";
            }

            if (actionName == "pinch")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, -.15M, 0, wearer.Player.Id);
                thirdP = "<red>You feel " + me.FirstName + " " + me.LastName + ", currently your " + meItem.FriendlyName + ", every so slightly pinch your skin agitatedly.  You lose a tiny amount of willpower from your inanimate belonging's subtle but pesky gesture.</red>";
                firstP = "You agitatedly pinch against your current owner, " + wearer.Player.FirstName + " " + wearer.Player.LastName + ".  " + pronoun + " loses a tiny amount of willpower from your subtle but pesky gesture.";
            }

            if (actionName == "soothe")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, 0, .25M, wearer.Player.Id);
                thirdP = "<blue>You feel " + me.FirstName + " " + me.LastName + ", currently your " + meItem.FriendlyName + ", every so slightly peacefully soothe your skin.  You gain a tiny amount of mana from your inanimate belonging's subtle but kind gesture.</blue>";
                firstP = "You kindly soothe a patch of your current owner, " + wearer.Player.FirstName + " " + wearer.Player.LastName + "'s skin.  " + pronoun + " gains a tiny amount of mana from your subtle but kind gesture.";
            }

            if (actionName == "zap")
            {
                PlayerProcedures.ChangePlayerActionManaNoTimestamp(0, 0, -.15M, wearer.Player.Id);
                thirdP = "<red>You feel " + me.FirstName + " " + me.LastName + ", currently your " + meItem.FriendlyName + ", every so slightly zap your skin.  You lose a tiny amount of mana from your inanimate belonging's subtle but pesky gesture.</red>";
                firstP = "You agitatedly zap a patch of your current owner, " + wearer.Player.FirstName + " " + wearer.Player.LastName + "'s skin.  " + pronoun + " loses a tiny amount of mana from your subtle but pesky gesture.";
            }

            PlayerProcedures.LogIP(Request.UserHostAddress);
            string leveluptext = InanimateXPProcedures.GiveInanimateXP(me.Id);

            TempData["Result"] = firstP + leveluptext;
            PlayerProcedures.AddAttackCount(me);
            PlayerLogProcedures.AddPlayerLog(wearer.Player.Id, thirdP, true);
            PlayerLogProcedures.AddPlayerLog(me.Id, firstP, true);

            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult AnimalAction(string actionName, int targetId)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is in an okay form to do this
            if (PlayerCanPerformAction(me, "animalAction") == false)
            {
                return RedirectToAction("Play");
            }

            // assert that player is an animal
            if (me.Mobility != "animal")
            {
                TempData["Error"] = "You are not an animal or pet.";
                return RedirectToAction("Play");
            }

            // assert player hasn't made too many attacks this update
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You have interacted too much this update.";
                TempData["SubError"] = "You can only interact 1 times per update as an animal.  Wait a bit.";
                return RedirectToAction("Play");
            }

            // assert that the target is still in the same room
            Player targeted = PlayerProcedures.GetPlayer(targetId);

            if (me.Mobility == "full")
            {
                if (me.dbLocationName != targeted.dbLocationName)
                {
                    TempData["Error"] = "Your target no longer seems to be here.";
                    TempData["SubError"] = "Your target has probably left.  Maybe you can follow them and attack when you've caught up.";
                    return RedirectToAction("Play");
                }
            }
            else if (me.Mobility == "animal")
            {
                Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);
                if (here.dbName != targeted.dbLocationName)
                {
                    TempData["Error"] = "Your target no longer seems to be here.";
                    TempData["SubError"] = "Your target has probably left.";
                    return RedirectToAction("Play");
                }
            }

            // assert that the target is not inanimate
            if (targeted.Mobility == "inanimate")
            {
                TempData["Error"] = "Your target is inanimate";
                TempData["SubError"] = "You can't interact with inanimate players.";
                return RedirectToAction("Play");
            }

            // assert that the target is not an animal
            if (targeted.Mobility == "animal")
            {
                TempData["Error"] = "Your target is already an animal";
                TempData["SubError"] = "You can't interact with players turned into other animals.";
                return RedirectToAction("Play");
            }

            // assert that this player does not currently have a lock on their account
            if (me.FlaggedForAbuse == true)
            {
                TempData["Error"] = "This player has been flagged by a moderator for suspicious actions and is not allowed to attack at this time.";
                return RedirectToAction("Play");
            }

            // assert that the victim is not the own player
            if (targeted.Id == me.Id)
            {
                TempData["Error"] = "You can't cast magic on yourself..";
                return RedirectToAction("Play");
            }

            // all of our checks have passed, so now let's actually do the action
            PlayerProcedures.LogIP(Request.UserHostAddress);

            string result = AnimalProcedures.DoAnimalAction(actionName, me.Id, targeted.Id);
            string leveluptext = InanimateXPProcedures.GiveInanimateXP(me.Id);

            TempData["Result"] = result + leveluptext;

            return RedirectToAction("Play");
        }

        public ActionResult HowToPlay()
        {
            return View("~/Views/PvP/HowToPlay.cshtml");
        }
        public ActionResult GameNews()
        {
            return View("~/Views/PvP/GameNews.cshtml");
        }

        public ActionResult GameNews_Archive()
        {
            return View("~/Views/PvP/GameNews_Archive.cshtml");
        }

        public ActionResult GameNews_PlannedFeatures()
        {
            return View("~/Views/PvP/GameNews_PlannedFeatures.cshtml");
        }

         [Authorize]
        public ActionResult Contribute(int Id)
        {

            IContributionRepository contributionRepo = new EFContributionRepository();

            int currentUserId = WebSecurity.CurrentUserId;

            IEnumerable<Contribution> myContributions = contributionRepo.Contributions.Where(c => c.OwnerMembershipId == currentUserId);
            IEnumerable<Contribution> proofreading = null;

            // add the rest of the submitted contributions if the player is a proofread
            if (TrustStatics.PlayerIsProofreader(WebSecurity.CurrentUserId) == true)
            {
                proofreading = contributionRepo.Contributions.Where(c => c.AdminApproved == true && c.ProofreadingCopy == true);
            }

            Contribution contribution;

            if (Id != -1)
            {
                try
                {
                   // contribution = myContributions.FirstOrDefault(c => c.Id == Id);
                    contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == Id);
                    ViewBag.Result = "Load successful.";

                    // assert player owns this
                    if (contribution.OwnerMembershipId != currentUserId && !TrustStatics.PlayerIsProofreader(WebSecurity.CurrentUserId))
                    {
                        TempData["Error"] = "This contribution does not belong to your account.";
                        return RedirectToAction("Play");
                    }

                    // if this player is a proofreader and this contribution is not marked as ready for proofreading, tell the editor to go to the proofreading version instead.
                    if (TrustStatics.PlayerIsProofreader(WebSecurity.CurrentUserId) == true && contribution.ProofreadingCopy == false)
                    {
                        Contribution contributionProofed = contributionRepo.Contributions.FirstOrDefault(c => c.OwnerMembershipId == contribution.OwnerMembershipId && c.ProofreadingCopy == true && c.Skill_FriendlyName == contribution.Skill_FriendlyName);
                        if (contributionProofed != null)
                        {
                            TempData["Error"] = "There is already a proofreading version of this available.  Please load that instead.";
                            return RedirectToAction("Play");
                        }

                        

                    }

                    // save the proofreading lock on this contribution
                    if (contribution.ProofreadingCopy == true) { 
                         contribution.ProofreadingLockIsOn = true;
                         contribution.CheckedOutBy = WebSecurity.CurrentUserName;
                         contribution.CreationTimestamp = DateTime.UtcNow;
                         contributionRepo.SaveContribution(contribution);
                    }

                }
                catch
                {
                    contribution = new Contribution();
                    contribution.OwnerMembershipId = WebSecurity.CurrentUserId;
                }
            }
            else
            {
                contribution = new Contribution();
            }

            ViewBag.Result = TempData["Result"];

            ViewBag.OtherContributions = myContributions;
            ViewBag.Proofreading = proofreading;

            BalanceBox bbox = new BalanceBox();
            bbox.LoadBalanceBox(contribution);
            decimal balance = bbox.GetBalance();
            ViewBag.BalanceScore = balance;

            return View(contribution);
        }

        public ActionResult ContributeBalanceCalculator()
        {
            return View("~/Views/PvP/BalanceCalculator.cshtml");
        }

         public ActionResult ContributeBalanceCalculator2(int id)
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (TrustStatics.PlayerIsProofreader(me.MembershipId) == false && contribution.OwnerMembershipId != me.MembershipId)
            {
                TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                return RedirectToAction("Play");
            }
            else
            {

            }

            return View("~/Views/PvP/BalanceCalculator2.cshtml", contribution);
        }

         public ActionResult ContributeBalanceCalculatorSend(Contribution input)
         {
             IContributionRepository contributionRepo = new EFContributionRepository();
             Contribution SaveMe = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.Id);

             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

             if (TrustStatics.PlayerIsProofreader(me.MembershipId) == false && SaveMe.OwnerMembershipId != me.MembershipId)
             {
                 TempData["Error"] = "That does not belong to you and you are not a proofreader.";
                 return RedirectToAction("Play");
             }
             else
             {
                 SaveMe.HealthBonusPercent = input.HealthBonusPercent;
                 SaveMe.ManaBonusPercent = input.ManaBonusPercent;
                 SaveMe.ExtraSkillCriticalPercent = input.ExtraSkillCriticalPercent;
                 SaveMe.HealthRecoveryPerUpdate = input.HealthRecoveryPerUpdate;
                 SaveMe.ManaRecoveryPerUpdate = input.ManaRecoveryPerUpdate;
                 SaveMe.SneakPercent = input.SneakPercent;
                 SaveMe.EvasionPercent = input.EvasionPercent;
                 SaveMe.EvasionNegationPercent = input.EvasionNegationPercent;
                 SaveMe.MeditationExtraMana = input.MeditationExtraMana;
                 SaveMe.CleanseExtraHealth = input.CleanseExtraHealth;
                 SaveMe.MoveActionPointDiscount = input.MoveActionPointDiscount;
                 SaveMe.SpellExtraTFEnergyPercent = input.SpellExtraTFEnergyPercent;
                 SaveMe.SpellExtraHealthDamagePercent = input.SpellExtraHealthDamagePercent;
                 SaveMe.CleanseExtraTFEnergyRemovalPercent = input.CleanseExtraTFEnergyRemovalPercent;
                 SaveMe.SpellMisfireChanceReduction = input.SpellMisfireChanceReduction;
                 SaveMe.SpellHealthDamageResistance = input.SpellHealthDamageResistance;
                 SaveMe.SpellTFEnergyDamageResistance = input.SpellTFEnergyDamageResistance;
                 SaveMe.ExtraInventorySpace = input.ExtraInventorySpace;

                 SaveMe.History += "Bonus values edited by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";

                 contributionRepo.SaveContribution(SaveMe);

                 TempData["Result"] = "Contribution stats saved.";
                 return RedirectToAction("Play");

             }
             return RedirectToAction("Play");
         }

         [Authorize]
        public ActionResult ContributeGraphicsNeeded()
        {
            IContributionRepository contributionRepo = new EFContributionRepository();
            IEnumerable<Contribution> output = contributionRepo.Contributions.Where(c => c.IsReadyForReview == true && c.AdminApproved == true && c.IsLive == false && c.ProofreadingCopy == true);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View("ContributeGraphicsNeeded", output);
        }

         [Authorize]
        public ActionResult ContributeSetGraphicStatus(int id)
        {

            if (!TrustStatics.Artists.Contains(WebSecurity.CurrentUserId))
            {
                TempData["Result"] = "You don't have permissions to do that.  If you are an artist and are interested in contributing artwork, please contact the administrator, Judoo.";
                return RedirectToAction("ContributeGraphicsNeeded");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution cont = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            ContributionStatusViewModel output = new ContributionStatusViewModel
            {
                ContributionId = id,
                Status = cont.AssignedToArtist,
            };

            return View("ContributeSetGraphicStatus", output);
        }

         [Authorize]
        public ActionResult ContributeSetGraphicStatusSubmit(ContributionStatusViewModel input)
        {
            if (!TrustStatics.Artists.Contains(WebSecurity.CurrentUserId))
            {
                TempData["Result"] = "You don't have permissions to do that.  If you are an artist and are interested in contributing artwork, please contact the administrator, Judoo.";
                return RedirectToAction("ContributeGraphicsNeeded");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution cont = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.ContributionId);

            cont.AssignedToArtist = input.Status;

            contributionRepo.SaveContribution(cont);

            TempData["Result"] = "Status saved!";
            return RedirectToAction("ContributeGraphicsNeeded");

        }

        [HttpPost]
        public ActionResult SendContribution(Contribution input)
        {

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution SaveMe;

            Session["ContributionId"] = input.Id;

            SaveMe = contributionRepo.Contributions.FirstOrDefault(c => c.Id == input.Id);
            if (SaveMe == null)
            {
                SaveMe = new Contribution();
                SaveMe.OwnerMembershipId = WebSecurity.CurrentUserId;
            }


            if (input.Id != -1)
            {

                // submitter is original author, ID stays the same and do NOT mark as proofreading version
                if (SaveMe != null && SaveMe.OwnerMembershipId == WebSecurity.CurrentUserId)
                {
                    SaveMe.Id = input.Id;
                    //SaveMe.ProofreadingCopy = false;
                }

                // submitter is not original author.  Do more logic...
                else if (SaveMe != null && SaveMe.OwnerMembershipId != WebSecurity.CurrentUserId)
                {
                    // this is a poorfreading copy.  Keep Id the same and keep it marked as a proofreading copy IF the editor is a proofreader
                    if (SaveMe.ProofreadingCopy == true && TrustStatics.PlayerIsProofreader(WebSecurity.CurrentUserId))
                    {
                        SaveMe.Id = input.Id;
                        //SaveMe.ProofreadingCopy = true;
                    }
                    else
                    {
                        TempData["Result"] = "You do not have the authorization to edit this.  If you are a proofreader, make sure to load up the proofreading version instead.";
                        return RedirectToAction("Play");
                    }

                    // this is not yet a proofreading copy.  Recreate and mark it as a proofreading copy IF the editor is a proofreader
                   // else if (TrustStatics.PlayerIsProofreader(WebSecurity.CurrentUserId))
                   // {
                   //     SaveMe = new Contribution();
                    //    SaveMe.ProofreadingCopy = true;
                     //   SaveMe.AdminApproved = true;
                    //    SaveMe.OwnerMembershipId = input.OwnerMembershipId;
                  //  }
                  //  else
                   // {
//
                   // }
                }
   

            }

            // unlock the proofreading flag since it has been saved
            input.ProofreadingLockIsOn = false;
            SaveMe.ProofreadingLockIsOn = false;
            SaveMe.CheckedOutBy = "";
            
            SaveMe.IsReadyForReview = input.IsReadyForReview;
            SaveMe.IsLive = input.IsLive;

            SaveMe.Skill_FriendlyName = input.Skill_FriendlyName;
            SaveMe.Skill_FormFriendlyName = input.Skill_FormFriendlyName;
            SaveMe.Skill_Description = input.Skill_Description;
            SaveMe.Skill_ManaCost = input.Skill_ManaCost;
            SaveMe.Skill_TFPointsAmount = input.Skill_TFPointsAmount;
            SaveMe.Skill_HealthDamageAmount = input.Skill_HealthDamageAmount;
            SaveMe.Skill_LearnedAtRegion = input.Skill_LearnedAtRegion;
            SaveMe.Skill_LearnedAtLocationOrRegion = input.Skill_LearnedAtLocationOrRegion;
            SaveMe.Skill_DiscoveryMessage = input.Skill_DiscoveryMessage;

            SaveMe.Form_FriendlyName = input.Form_FriendlyName;
            SaveMe.Form_Description = input.Form_Description;
            SaveMe.Form_TFEnergyRequired = input.Form_TFEnergyRequired;
            SaveMe.Form_Gender = input.Form_Gender;
            SaveMe.Form_MobilityType = input.Form_MobilityType;
            SaveMe.Form_BecomesItemDbName = input.Form_BecomesItemDbName;
            SaveMe.Form_Bonuses = input.Form_Bonuses;

            SaveMe.Form_TFMessage_20_Percent_1st = input.Form_TFMessage_20_Percent_1st;
            SaveMe.Form_TFMessage_40_Percent_1st = input.Form_TFMessage_40_Percent_1st ;
            SaveMe.Form_TFMessage_60_Percent_1st = input.Form_TFMessage_60_Percent_1st ;
            SaveMe.Form_TFMessage_80_Percent_1st = input.Form_TFMessage_80_Percent_1st ;
            SaveMe.Form_TFMessage_100_Percent_1st = input.Form_TFMessage_100_Percent_1st ;
            SaveMe.Form_TFMessage_Completed_1st = input.Form_TFMessage_Completed_1st ;

            SaveMe.Form_TFMessage_20_Percent_1st_M = input.Form_TFMessage_20_Percent_1st_M ;
            SaveMe.Form_TFMessage_40_Percent_1st_M = input.Form_TFMessage_40_Percent_1st_M ;
            SaveMe.Form_TFMessage_60_Percent_1st_M = input.Form_TFMessage_60_Percent_1st_M ;
            SaveMe.Form_TFMessage_80_Percent_1st_M = input.Form_TFMessage_80_Percent_1st_M ;
            SaveMe.Form_TFMessage_100_Percent_1st_M = input.Form_TFMessage_100_Percent_1st_M ;
            SaveMe.Form_TFMessage_Completed_1st_M = input.Form_TFMessage_Completed_1st_M ;

            SaveMe.Form_TFMessage_20_Percent_1st_F = input.Form_TFMessage_20_Percent_1st_F ;
            SaveMe.Form_TFMessage_40_Percent_1st_F = input.Form_TFMessage_40_Percent_1st_F ;
            SaveMe.Form_TFMessage_60_Percent_1st_F = input.Form_TFMessage_60_Percent_1st_F ;
            SaveMe.Form_TFMessage_80_Percent_1st_F = input.Form_TFMessage_80_Percent_1st_F ;
            SaveMe.Form_TFMessage_100_Percent_1st_F = input.Form_TFMessage_100_Percent_1st_F ;
            SaveMe.Form_TFMessage_Completed_1st_F = input.Form_TFMessage_Completed_1st_F ;

            SaveMe.Form_TFMessage_20_Percent_3rd = input.Form_TFMessage_20_Percent_3rd;
            SaveMe.Form_TFMessage_40_Percent_3rd = input.Form_TFMessage_40_Percent_3rd ;
            SaveMe.Form_TFMessage_60_Percent_3rd = input.Form_TFMessage_60_Percent_3rd ;
            SaveMe.Form_TFMessage_80_Percent_3rd = input.Form_TFMessage_80_Percent_3rd ;
            SaveMe.Form_TFMessage_100_Percent_3rd = input.Form_TFMessage_100_Percent_3rd ;
            SaveMe.Form_TFMessage_Completed_3rd = input.Form_TFMessage_Completed_3rd ;

            SaveMe.Form_TFMessage_20_Percent_3rd_M = input.Form_TFMessage_20_Percent_3rd_M ;
            SaveMe.Form_TFMessage_40_Percent_3rd_M = input.Form_TFMessage_40_Percent_3rd_M ;
            SaveMe.Form_TFMessage_60_Percent_3rd_M = input.Form_TFMessage_60_Percent_3rd_M ;
            SaveMe.Form_TFMessage_80_Percent_3rd_M = input.Form_TFMessage_80_Percent_3rd_M ;
            SaveMe.Form_TFMessage_100_Percent_3rd_M = input.Form_TFMessage_100_Percent_3rd_M ;
            SaveMe.Form_TFMessage_Completed_3rd_M = input.Form_TFMessage_Completed_3rd_M ;

            SaveMe.Form_TFMessage_20_Percent_3rd_F = input.Form_TFMessage_20_Percent_3rd_F ;
            SaveMe.Form_TFMessage_40_Percent_3rd_F = input.Form_TFMessage_40_Percent_3rd_F ;
            SaveMe.Form_TFMessage_60_Percent_3rd_F = input.Form_TFMessage_60_Percent_3rd_F ;
            SaveMe.Form_TFMessage_80_Percent_3rd_F = input.Form_TFMessage_80_Percent_3rd_F ;
            SaveMe.Form_TFMessage_100_Percent_3rd_F = input.Form_TFMessage_100_Percent_3rd_F ;
            SaveMe.Form_TFMessage_Completed_3rd_F = input.Form_TFMessage_Completed_3rd_F ;


            SaveMe.Item_FriendlyName = input.Item_FriendlyName ;
            SaveMe.Item_Description = input.Item_Description ;
            SaveMe.Item_ItemType = input.Item_ItemType ;
            SaveMe.Item_UseCooldown = input.Item_UseCooldown ;
            SaveMe.Item_Bonuses = input.Item_Bonuses ;

            //SaveMe.HealthBonusPercent = input.HealthBonusPercent;
            //SaveMe.ManaBonusPercent = input.ManaBonusPercent;
            //SaveMe.ExtraSkillCriticalPercent = input.ExtraSkillCriticalPercent;
            //SaveMe.HealthRecoveryPerUpdate = input.HealthRecoveryPerUpdate;
            //SaveMe.ManaRecoveryPerUpdate = input.ManaRecoveryPerUpdate;
            //SaveMe.SneakPercent = input.SneakPercent;
            //SaveMe.EvasionPercent = input.EvasionPercent;
            //SaveMe.EvasionNegationPercent = input.EvasionNegationPercent;
            //SaveMe.MeditationExtraMana = input.MeditationExtraMana;
            //SaveMe.CleanseExtraHealth = input.CleanseExtraHealth;
            //SaveMe.MoveActionPointDiscount = input.MoveActionPointDiscount;
            //SaveMe.SpellExtraTFEnergyPercent = input.SpellExtraTFEnergyPercent;
            //SaveMe.SpellExtraHealthDamagePercent = input.SpellExtraHealthDamagePercent;
            //SaveMe.CleanseExtraTFEnergyRemovalPercent = input.CleanseExtraTFEnergyRemovalPercent;
            //SaveMe.SpellMisfireChanceReduction = input.SpellMisfireChanceReduction;
            //SaveMe.SpellHealthDamageResistance = input.SpellHealthDamageResistance;
            //SaveMe.SpellTFEnergyDamageResistance = input.SpellTFEnergyDamageResistance;
            //SaveMe.ExtraInventorySpace = input.ExtraInventorySpace;

            SaveMe.SubmitterName = input.SubmitterName;
            SaveMe.SubmitterUrl = input.SubmitterUrl;
            SaveMe.AdditionalSubmitterNames = input.AdditionalSubmitterNames;
            SaveMe.Notes = input.Notes;
            SaveMe.NeedsToBeUpdated = input.NeedsToBeUpdated;

            SaveMe.AssignedToArtist = input.AssignedToArtist;

            if (input.ImageURL != null && input.ImageURL != "" && WebSecurity.CurrentUserId == 69)
            {
                SaveMe.ImageURL = input.ImageURL;
            }

            SaveMe.CreationTimestamp = DateTime.UtcNow;

            if (SaveMe.ProofreadingCopy == true)
            {
                SaveMe.History += "Edited by " + WebSecurity.CurrentUserName + " on " + DateTime.UtcNow + ".<br>";
            }

            contributionRepo.SaveContribution(SaveMe);

  

            TempData["Result"] = "Contribution Saved!";
            return RedirectToAction("Play");
        }

        public ActionResult SendContributionUndoLock(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (TrustStatics.PlayerIsProofreader(me.MembershipId) == false)
            {
                TempData["Error"] = "You must be a proofreader in order to do this.";
                return RedirectToAction("Play");
            }

            IContributionRepository contributionRepo = new EFContributionRepository();
            Contribution contribution = contributionRepo.Contributions.FirstOrDefault(c => c.Id == id);

            if (contribution.ProofreadingCopy == false)
            {
                TempData["Error"] = "This is not a proofreading copy.";
                return RedirectToAction("Play");
            }

            contribution.ProofreadingLockIsOn = false;
            contribution.CheckedOutBy = "";
            contributionRepo.SaveContribution(contribution);

            return RedirectToAction("Play");
        }

        public ActionResult MyFriends()
        {
            if (WebSecurity.CurrentUserId == -1)
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            FriendPageViewModel output = new FriendPageViewModel();

            IEnumerable<FriendPlayerViewModel> friends = FriendProcedures.GetMyFriends();

            output.ConfirmedFriends = friends.Where(f => f.dbFriend.IsAccepted == true);

            output.RequestsForMe = friends.Where(f => f.dbFriend.IsAccepted == false && (f.dbFriend.FriendMembershipId == WebSecurity.CurrentUserId));

            output.MyOutgoingRequests = friends.Where(f => f.dbFriend.IsAccepted == false && (f.dbFriend.OwnerMembershipId == WebSecurity.CurrentUserId));

            return View("MyFriends", output);
        }

        public ActionResult AddFriend(int playerId)
        {
            Player friend = PlayerProcedures.GetPlayer(playerId);
            FriendProcedures.AddFriend(friend);

            return RedirectToAction("Play");
        }

        public ActionResult RespondToFriendRequest(int id, string response)
        {

            if (response == "cancel")
            {
                FriendProcedures.CancelFriendRequest(id);
            }
            else if (response == "deny")
            {
                FriendProcedures.CancelFriendRequest(id);
            }
            else if (response == "defriend")
            {
                FriendProcedures.CancelFriendRequest(id);
            }
            else if (response == "accept")
            {
                FriendProcedures.AcceptFriendRequest(id);
            }

            
            return RedirectToAction("MyFriends");
        }

        private bool PlayerCanPerformAction(Player me, string actionType)
        {

            DbStaticForm myform = FormStatics.GetForm(me.Form);

            if (myform.MobilityType == "full")
            {
                return true;
            }

            if (actionType == "move")
            {
                // inanimate
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't move.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (me.IsPetToId != -1)
                {
                    TempData["Error"] = "You can't move by yourself.";
                    TempData["SubError"] = "You are an animal and are currently tamed as a pet.";
                    return false;
                }
            }
            else if (actionType == "attack")
            {
                // inanimate
                if (myform.MobilityType == "inanimate")
                {

                    TempData["Error"] = "You can't cast any spells.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;

                }
                // animal
                if (myform.MobilityType=="animal")
                {
                    TempData["Error"] = "You can't cast any spells.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "meditate")
            {
                //inaniamte
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't meditate.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.  You're essentially already meditating... permanently..";
                    return false;
                }
                // animal
                if (myform.MobilityType == "animal")
                {
                    TempData["Error"] = "You can't meditate.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }

            else if (actionType == "cleanse")
            {
                // inanimate
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't cleanse.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == "animal")
                {
                    TempData["Error"] = "You can't cleanse.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "search")
            {
                // inanimate
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't search.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == "animal")
                {
                    TempData["Error"] = "You can't search.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "pickup")
            {
                // inanimate
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't pick any items up.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == "animal")
                {
                    TempData["Error"] = "You can't pick anything up.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "drop")
            {
                // inanimate
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't drop any items.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
                // animal
                if (myform.MobilityType == "animal")
                {
                    TempData["Error"] = "You can't drop anything.";
                    TempData["SubError"] = "You are an animal.";
                    return false;
                }
            }
            else if (actionType == "equip")
            {
                if (myform.MobilityType == "inanimate")
                {
                    TempData["Error"] = "You can't equip or unequip any items.";
                    TempData["SubError"] = "You are currently stuck as an inanimate object.";
                    return false;
                }
            }
            else if (actionType == "animalAction")
            {
                if (myform.MobilityType != "animal")
                {
                    TempData["Error"] = "You can't do that.";
                    TempData["SubError"] = "You are not an animal.";
                    return false;
                }
            }



            return true;
        }

        private string ParseForGender(string gender, string message)
        {
           
            return message;
        }

         [Authorize]
        public ActionResult WorldMap()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            Location here = null;

            here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == me.dbLocationName);

            ViewBag.MapX = here.X;
            ViewBag.MapY = here.Y;
            return View("WorldMap");
        }

         [Authorize]
        public ActionResult Leaderboard()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            ViewBag.MyName = me.FirstName + " " + me.LastName;
            return View("Leaderboard", PlayerProcedures.GetLeadingPlayers__XP(100));
        }

         public ActionResult PvPLeaderboard()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
             ViewBag.MyName = me.FirstName + " " + me.LastName;
             return View(PlayerProcedures.GetLeadingPlayers__PvP(100));
         }

         public ActionResult ItemLeaderboard()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
             ViewBag.MyName = me.FirstName + " " + me.LastName;
             IEnumerable<SimpleItemLeaderboardViewModel> output = ItemProcedures.GetLeadingItemsSimple(100);
             return View(output);
         }

         [Authorize]
        public ActionResult Chat(string room)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            if (me == null || me.MembershipId == -1 || me.FirstName=="" || me.LastName=="")
            {
                return View("~/Views/PvP/LoginRequired.cshtml");
            }

            //if (me.FirstName)

            TempData["MyName"] = me.FirstName + " " + me.LastName;

            if (me.MembershipId == 69)
            {
                TempData["MyName"] = "Judoo (admin)";
            }

            else if (me.MembershipId == 3490)
            {
                TempData["MyName"] = "Mizuho (dev)";
            }

            if (room == "global")
            {
                return View("Chats/CHat_Global");
            }
            else if (room == "rp1")
            {
                return View("Chats/Chat_RP1");
            }
            else if (room == "rp2")
            {
                return View("Chats/Chat_RP2");
            }
            else if (room == "rp3")
            {
                return View("Chats/Chat_RP3");
            }

            return View();
        }

 

         public ActionResult ChatLog(string room, string filter)
         {
             ViewBag.Room = room;
             return View("Chats/ChatLog", ChatLogProcedures.GetChatLogs(room, filter));
         }

         public ActionResult ChatCommands()
         {
             return View("Chats/ChatCommands");
         }

         [Authorize]
        public ActionResult LevelupPerk()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            ViewBag.PerksRemaining = me.UnusedLevelUpPerks;

            if (me.UnusedLevelUpPerks == 0)
            {
                TempData["Error"] = "You don't have any unused level up perks left to choose right now.  Gain another experience level for more.";
                return RedirectToAction("Play");
            }
            else
            {
                IEnumerable<StaticEffect> output = EffectProcedures.GetAvailableLevelupPerks(me);
                return View(output);
            }

           
        }

         [Authorize]
        public ActionResult ChoosePerk(string perk)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player does have unused perks
            if (me.UnusedLevelUpPerks < 1)
            {
                TempData["Error"] = "You don't have any unused level up perks left to choose right now.  Gain another experience level for more.";
                return RedirectToAction("Play");
            }

            //give perk to player
            TempData["Result"] = EffectProcedures.GivePerkToPlayer(perk, me);
            

            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult MyPerks()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            IEnumerable<EffectViewModel> output = EffectProcedures.GetPlayerEffectViewModels(me);

            return View(output);
        }

         [Authorize]
        public ActionResult Settings()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            ViewBag.InProtection = me.InPvP;

            ViewBag.TimeUntilLogout = 60-Math.Abs(Math.Floor(me.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes));

            return View(me);
        }

        public ActionResult EnableRP()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            TempData["Result"] = PlayerProcedures.SetRPFlag(me, true);
            return RedirectToAction("Play");
        }

        public ActionResult DisableRP()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            TempData["Result"] = PlayerProcedures.SetRPFlag(me, false);
            return RedirectToAction("Play");
        }

        public ActionResult EnablePvP()
        {
            //Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            //// get this player's covenant, if any, and see if it is non-PvP.  If so, make them leave first.
            //if (me.Covenant > 0)
            //{
            //    Covenant myCov = CovenantProcedures.GetDbCovenant(me.Covenant);
            //    if (myCov.IsPvP == false)
            //    {
            //        TempData["Error"] = "Before you can enter PvP mode, you must leave your current non-PvP covenant.";
            //        return RedirectToAction("Play");
            //    }
            //}

            //TempData["Result"] = PlayerProcedures.SetPvPFlag(me);
            return RedirectToAction("Play");
        }

        [Authorize]
        public ActionResult EnterProtection()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (PlayerProcedures.PlayerIsOffline(me) == false && me.Mobility == "full")
            {
                TempData["Error"] = "You must be offline in order to enter or leave protection mode while animate.";
                return RedirectToAction("Play");
            }

            // assert it is not too early for the player to change into/out of protection mode
            PlayerExtra playerExtra = PlayerExtraProcedures.GetPlayerExtra(me);
            int turnNo = PvPWorldStatProcedures.GetWorldTurnNumber();
            if (playerExtra.ProtectionToggleTurnsRemaining > 0)
            {
                TempData["Error"] = "You cannot enter protection mode right now.";
                TempData["SubError"] = "You must wait for " + playerExtra.ProtectionToggleTurnsRemaining + " more updates to complete while your character is online in order to leave protection mode.";
                return RedirectToAction("Play");
            }

            PlayerProcedures.SetPvPFlag(me, true);
            PlayerExtraProcedures.SetNextProtectionToggleTurn(me);
            EffectProcedures.GivePerkToPlayer("help_entered_PvP", me);

            TempData["Result"] = "You are now in protection mode.  You cannot be hit by inanimate or animal spells nor cast them, except against those on your friends list.";
            return RedirectToAction("Play");
        }

        [Authorize]
        public ActionResult LeaveProtection()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (PlayerProcedures.PlayerIsOffline(me) == false && me.Mobility == "full")
            {
                TempData["Error"] = "You must be offline in order to enter or leave protection mode while animate.";
                return RedirectToAction("Play");
            }

           // assert it is not too early for the player to change into/out of protection mode
            PlayerExtra playerExtra = PlayerExtraProcedures.GetPlayerExtra(me);
            int turnNo = PvPWorldStatProcedures.GetWorldTurnNumber();
            if (playerExtra.ProtectionToggleTurnsRemaining > 0)
            {
                TempData["Error"] = "You cannot leave protection mode right now.";
                TempData["SubError"] = "You must wait until turn " + playerExtra.ProtectionToggleTurnsRemaining + " while your character is online in order to leave protection mode.";
                return RedirectToAction("Play");
            }

            PlayerProcedures.SetPvPFlag(me, false);
            PlayerExtraProcedures.SetNextProtectionToggleTurn(me);

            TempData["Result"] = "You are no longer in protection mode.";
            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult Teleport(string to)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // no need to assert player is mobile; inanimates and items have no inventory

            // assert player actually does own one of this
            if (ItemProcedures.PlayerHasNumberOfThisItem(me, "item_consumeable_teleportation_scroll") <= 0)
            {
                TempData["Error"] = "You don't have one of these.";
                return RedirectToAction("Play");
            }

            TempData["Result"] = PlayerProcedures.TeleportPlayer(me, to);

            ItemProcedures.DeleteItemOfName(me, "item_consumeable_teleportation_scroll");

            return RedirectToAction("Play");
        }

         [Authorize]
        public ActionResult FightTheTransformation()
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is not in PvP mode
            //if (me.InPvP == true)
            //{
            //    TempData["Error"] = "You cannot return to an animate form in PvP mode.";
            //    TempData["SubError"] = "You fought in this magical battle for keeps.  And now you're being kept.  Maybe in a few years when all this blows over your owner will change you back...";
            //    return RedirectToAction("Play");
            //}

            // assert player is inanimate or an animal
            if (me.Mobility == "full")
            {
                TempData["Error"] = "You can't do this.";
                TempData["SubError"] = "You are still in an animate form.  You must be inanimate or an animal.";
                return RedirectToAction("Play");
            }

            // assert player has not acted too many times already
            if (me.TimesAttackingThisUpdate >= 1)
            {
                TempData["Error"] = "You don't have enough energy to fight your transformation right now.";
                TempData["SubError"] = "Wait a bit.";
                return RedirectToAction("Play");
            }

             // assert player is not already locked into their current form
            Item itemMe = ItemProcedures.GetItemByVictimName(me.FirstName, me.LastName);
            if (itemMe.IsPermanent == true)
            {
                TempData["Error"] = "You cannot return to an animate form again.";
                TempData["SubError"] = "You have spent too long and performed too many actions as an item or animal and have lost your desire and ability to be human gain.";
                return RedirectToAction("Play");
            }

            TempData["Result"] = InanimateXPProcedures.ReturnToAnimate(me);
            return RedirectToAction("Play");
        }

        [Authorize]
         public ActionResult TalkWithJewdewfae()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            Player fae = PlayerProcedures.GetPlayerFromMembership(-6);

            // assert player is mobile
             if (me.Mobility != "full")
             {
                 TempData["Error"] = "You must be fully animate in order to talk with Jewdewfae.";
                 return RedirectToAction("Play");
             }

            // assert player is in same location as jewdewfae
             if (me.dbLocationName != fae.dbLocationName)
             {
                 TempData["Error"] = "You must be in the same location as Jewfewfae in order to talk with her.";
                 return RedirectToAction("Play");
             }

             FairyChallengeBag output = tfgame.Procedures.BossProcedures.BossProcedures_Fae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

             ViewBag.IsInWrongForm = false;

             if (me.Form != output.RequiredForm)
             {
                 ViewBag.IsInWrongForm = true;
             }

             if (me.ActionPoints < 5)
             {
                 ViewBag.IsTired = true;
             }

             ViewBag.ShowSuccess = false;

             ViewBag.HadRecentInteraction = false;
             if (tfgame.Procedures.BossProcedures.BossProcedures_Fae.PlayerHasHadRecentInteraction(me, fae))
             {
                 ViewBag.HadRecentInteraction = true;
             }

             return View(output);
         }

        [Authorize]
        public ActionResult PlayWithJewdewfae()
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            Player fae = PlayerProcedures.GetPlayerFromMembership(-6);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be fully animate in order to talk with Jewdewfae.";
                return RedirectToAction("Play");
            }

            // assert player is in same location as jewdewfae
            if (me.dbLocationName != fae.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Jewfewfae in order to talk with her.";
                return RedirectToAction("Play");
            }

            // assert player has enough AP
            if (me.ActionPoints < 5)
            {
                TempData["Error"] = "You need 5 action points to play with Jewdewfae.";
                return RedirectToAction("Play");
            }

            // assert player has not already interacted this location
            if (tfgame.Procedures.BossProcedures.BossProcedures_Fae.PlayerHasHadRecentInteraction(me, fae))
            {
                TempData["Error"] = "You have already interacted with Jewdewfae here.";
                TempData["SubError"] = "Wait for her to move somewhere else.";
                return RedirectToAction("Play");
            }

            FairyChallengeBag output = tfgame.Procedures.BossProcedures.BossProcedures_Fae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

            if (me.Form == output.RequiredForm)
            {
                decimal xpGained = tfgame.Procedures.BossProcedures.BossProcedures_Fae.AddInteraction(me);
                PlayerProcedures.GiveXP(me.Id, xpGained);
                PlayerProcedures.ChangePlayerActionMana(5, 0, 0, me.Id);
                ViewBag.XPGain = xpGained;
                ViewBag.ShowSuccess = true;
                ViewBag.HadRecentInteraction = false;
                return View("TalkWithJewdewfae", output);
            }
            else
            {
                TempData["Error"] = "You are not in the correct form to play with Jewdewfae right now.";
                return RedirectToAction("Play");
            }

        }

         [Authorize]
        public ActionResult TradeWithMerchant(string filter)
        {

            if (filter == null || filter == "")
            {
                filter = "clothes";
            }

            // assert that player is logged in
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            ViewBag.MyMoney = Math.Floor(me.Money);

            // assert player is logged in
            if (me.MembershipId < 0)
            {
                TempData["Error"] = "You need to log in.";
                return RedirectToAction("Play");
            }

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play");
            }

            // assert that the player has room in their inventory
            //if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffs(me))) {
            //    TempData["Error"] = "You are carrying too many items to purchase a new one.";
            //    TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
            //    return RedirectToAction("Play");
            //}

            ViewBag.NoRoom = ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffs(me));

            ViewBag.DisableLinks = "true";

            IEnumerable<ItemViewModel> output = null;
            if (filter == "hat")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Hat);
            }
            else if (filter == "shirt")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Shirt);
            }
            else if (filter == "undershirt")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Undershirt);
            }
            else if (filter == "pants")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Pants);
            }
            else if (filter == "underpants")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Underpants);
            }
            else if (filter == "shoes")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Shoes);
            }
            else if (filter == "accessory")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Accessory);
            }
            else if (filter == "consumable_reusable")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable);
            }
            else if (filter == "consumables")
            {
                output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == "consumable");
            }
             

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

         [Authorize]
        public ActionResult Purchase(int id)
        {
            // assert that player is logged in
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is logged in
            if (me.MembershipId < 0)
            {
                TempData["Error"] = "You need to log in.";
                return RedirectToAction("Play");
            }

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play");
            }

            ItemViewModel purchased = ItemProcedures.GetItemViewModel(id);

            // assert that the item is in fact owned by the merchant
            if (purchased.dbItem.OwnerId != merchant.Id)
            {
                TempData["Error"] = "Lindella does not own this item.";
                return RedirectToAction("TradeWithMerchant");
            }

            decimal cost = ItemProcedures.GetCostOfItem(purchased, "buy");

            // assert that the player has enough money for this
            if (me.Money < cost)
            {
                TempData["Error"] = "You can't afford this right now.";
                TempData["SubError"] = "Try finding some more Arpeyjis from searching or take them off of players who you have turned inanimate or an animal.";
                return RedirectToAction("Play");
            }

            // assert that the player has room in their inventory
            if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffs(me)))
            {
                TempData["Error"] = "You are carrying too many items to purchase a new one.";
                TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
                return RedirectToAction("Play");
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);
            ItemProcedures.GiveItemToPlayer_Nocheck(purchased.dbItem.Id, me.Id);

            TempData["Result"] = "You have purchased a " + purchased.Item.FriendlyName + " from Lindella.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }

         [Authorize]
        public ActionResult SellList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            

            // assert player is logged in
            if (me.MembershipId < 0)
            {
                TempData["Error"] = "You need to log in.";
                return RedirectToAction("Play");
            }

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play");
            }

            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(me.Id).Where(i => i.Item.ItemType != PvPStatics.ItemType_Pet && i.dbItem.IsEquipped == false);
            return View(output);
        }

         [Authorize]
        public ActionResult Sell(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is logged in
            if (me.MembershipId < 0)
            {
                TempData["Error"] = "You need to log in.";
                return RedirectToAction("Play");
            }

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play");
            }

            ItemViewModel itemBeingSold = ItemProcedures.GetItemViewModel(id);

            // assert that player does own this
            if (itemBeingSold.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own this item.";
                return RedirectToAction("Play");
            }

            // assert that the item is not an animal type
            if (itemBeingSold.Item.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "Unfortunately Lindella does not purchase or sell pets or animals.";
                return RedirectToAction("Play");
            }

            ItemProcedures.GiveItemToPlayer_Nocheck(itemBeingSold.dbItem.Id, merchant.Id);
            decimal cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);


            TempData["Result"] = "You sold your " + itemBeingSold.Item.FriendlyName + " to Lindella for " + (int)cost +  " Arpeyjis.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }

         [Authorize]
         public ActionResult ReserveName()
         {
             Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            

             // assert player is greater than level 3 if they are mobile
             if (me.Level < 3 && me.Mobility == "full")
             {
                 TempData["Error"] = "You must be level 3 or greater in order to reserve a name.";
                 return RedirectToAction("Play");
             }

             // if player is not mobile, see if the item they have become is at least level 3
             if (me.Mobility != "full")
             {
                 Item itemMe = ItemProcedures.GetItemByVictimName(me.FirstName, me.LastName);
                 if (itemMe.Level < 3)
                 {
                     TempData["Error"] = "You must be level 3 or greater in order to reserve a name.";
                     return RedirectToAction("Play");
                 }
             }

             IReservedNameRepository resNameRepo = new EFReservedNameRepository();

             ReservedName ghost = resNameRepo.ReservedNames.FirstOrDefault(rn => rn.MembershipId == me.MembershipId);

             if (ghost == null)
             {
                 ReservedName newReservedName = new ReservedName
                 {
                     FullName = me.FirstName + " " + me.LastName,
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
                     return RedirectToAction("Play");
                 }

                 if (ghost.FullName == me.FirstName + " " + me.LastName)
                 {
                     TempData["Result"] = "Your name has already been reserved.";
                     return RedirectToAction("Play");
                 }

                 ghost.FullName = me.FirstName + " " + me.LastName;
                 resNameRepo.SaveReservedName(ghost);
             }

             TempData["Result"] = "Your name has been reserved.";
             return RedirectToAction("Play");

         }

        public ActionResult OldLeadboards(string round)
        {
            return View("~/Views/PvP/RoundLeaderboards/Alpha_" + round + ".cshtml");
        }


        private List<DropDownListItem> getGenderChoiceList()
        {
            List<DropDownListItem> genderChoiceList = new List<DropDownListItem>();
            DropDownListItem male = new DropDownListItem
            {
                ID = "male",
                Desc = "male"
            };
            DropDownListItem female = new DropDownListItem
            {
                ID = "female",
                Desc = "female"
            };

            genderChoiceList.Add(male);
            genderChoiceList.Add(female);
            return genderChoiceList;
        }

        private class DropDownListItem
        {
            public string ID { get; set; }
            public string Desc { get; set; }
        }

        public ActionResult UpdateWorld(string password)
        {

            
            // localhost:64397/PvP/UpdateWorld/?password=oogabooga99


            if (password == null || password != "oogabooga99")
            {
                TempData["Result"] = "WRONG PASSWORD";
                return RedirectToAction("Play");
            }

            DateTime lastupdate = PvPWorldStatProcedures.GetLastWorldUpdate();
            double minutesAgo = Math.Abs(Math.Floor(lastupdate.Subtract(DateTime.UtcNow).TotalMinutes));

          //  if (minutesAgo < 10 && WebSecurity.CurrentUserId != 69)
            if (minutesAgo < 10)
            {
                TempData["Result"] = "You can't update the world again yet--it is too soon.";
                return RedirectToAction("Play");
            }

            PvPWorldStat worldStats = PvPWorldStatProcedures.GetWorldStats();

            
            int turnNo = worldStats.TurnNumber;


            if (turnNo < PvPStatics.RoundDuration)
            {

                IServerLogRepository serverLogRepo = new EFServerLogRepository();
                ServerLog log = new ServerLog
                {
                    TurnNumber = turnNo,
                    StartTimestamp = DateTime.UtcNow,
                    FinishTimestamp = DateTime.UtcNow,
                    Errors = 0,
                    FullLog = "",
                };
                log.AddLog("Started new log for turn " + turnNo + ".");
                Stopwatch updateTimer = new Stopwatch();
                updateTimer.Start();

                PvPStatics.AnimateUpdateInProgress = true;

                PvPWorldStatProcedures.UpdateWorldTurnCounter();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started loading animate players");
                IPlayerRepository playerRepo = new EFPlayerRepository();
                //List<Player> players_Animate = playerRepo.Players.Where(p => p.Mobility != "inanimate").ToList();
                //List<Player> players_Animate_to_Save = new List<Player>();

                IEffectRepository effectRepo = new EFEffectRepository();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started loading effects");
                List<Effect> temporaryEffects = effectRepo.Effects.Where(e => e.IsPermanent == false).ToList();
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished loading effects");
                List<Effect> effectsToDelete = new List<Effect>();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating effects");
                foreach (Effect e in temporaryEffects)
                {
                    e.Duration--;
                    e.Cooldown--;

                    if (e.Duration < 0)
                    {
                        e.Duration = 0;
                    }

                    if (e.Cooldown <= 0)
                    {
                        effectsToDelete.Add(e);
                    }
                    else
                    {
                        effectRepo.SaveEffect(e);
                    }
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating effects");

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting expired effects");
                foreach (Effect e in effectsToDelete)
                {
                    effectRepo.DeleteEffect(e.Id);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired effects");

                #region playerExtra / protection cooldown loop
                IPlayerExtraRepository playerExtraRepo = new EFPlayerExtraRepository();
                List<PlayerExtra> extrasToIncrement = playerExtraRepo.PlayerExtras.ToList();
                List<PlayerExtra> extrasToIncrement_SaveList = new List<PlayerExtra>();
                List<PlayerExtra> extrasToIncrement_DeleteList = new List<PlayerExtra>();
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating protection change cooldown (" + extrasToIncrement.Count + ")");

                foreach (PlayerExtra e in extrasToIncrement) {
                    Player owner = PlayerProcedures.GetPlayer(e.PlayerId);
                    if (PlayerProcedures.PlayerIsOffline(owner))
                    {
                        extrasToIncrement_SaveList.Add(e);
                    }
                }

                foreach (PlayerExtra e in extrasToIncrement_SaveList)
                {
                    if (e.ProtectionToggleTurnsRemaining > 0)
                    {
                        e.ProtectionToggleTurnsRemaining--;
                        playerExtraRepo.SavePlayerExtra(e);
                    }
                    else if (e.ProtectionToggleTurnsRemaining <= 0)
                    {
                        extrasToIncrement_DeleteList.Add(e);
                    }
                }

                foreach (PlayerExtra e in extrasToIncrement_DeleteList)
                {
                    playerExtraRepo.DeletePlayerExtra(e.Id);
                }

                #endregion

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating protection change cooldown.");

                //log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating animate players (" + players_Animate.Count + ")");

                #region main player loop

                using (var context = new StatsContext())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand("UPDATE [Stats].[dbo].[Players] SET TimesAttackingThisUpdate = 0, CleansesMeditatesThisRound = 0, ActionPoints = ActionPoints + 10 WHERE Mobility='full' " +

                        "UPDATE [Stats].[dbo].[Players] SET ActionPoints_Refill = ActionPoints_Refill + (ActionPoints % 120 / 2) WHERE ActionPoints >= 120 AND Mobility='full'" + 

                        "UPDATE [Stats].[dbo].[Players] SET ActionPoints = 120 WHERE ActionPoints > 120 AND Mobility='full'" + 
                        
                        "UPDATE [Stats].[dbo].[Players] SET ActionPoints_Refill = 360 WHERE ActionPoints_Refill > 360 AND Mobility='full'" + 
                        
                        "UPDATE [Stats].[dbo].[Players] SET ActionPoints = ActionPoints + 20, ActionPoints_Refill = ActionPoints_Refill - 20 WHERE ActionPoints <= 100 AND ActionPoints_Refill >= 20 AND Mobility='full'");

                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  ANIMATE SQL UPDATE SUCCEEDED!");
                    }
                    catch (Exception e)
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ANIMATE SQL UPDATE FAILED.  Reason:  " + e.ToString());
                    }
                }

                DateTime timeCutoff = DateTime.UtcNow.AddHours(-8);

                List<int> playerIdsToSave = playerRepo.Players.Where(p => p.Mobility == "full" && p.LastActionTimestamp > timeCutoff).Select(p => p.Id).ToList();

                foreach (int i in playerIdsToSave)
                {

                    Player player = playerRepo.Players.FirstOrDefault(p => p.Id == i);

                    // skip players who have no done anything in the past 24 hours
                    double hoursSinceLastAction = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalHours));
                    if (hoursSinceLastAction > 24)
                    {
                        continue;
                    }

                    //// if the player is already sitting at max AP, throw it in their reserves instead
                    //if (player.ActionPoints == PvPStatics.MaximumStoreableActionPoints)
                    //{
                    //    player.ActionPoints_Refill += PvPStatics.APRestoredPerUpdate/2;

                    //    if (player.ActionPoints_Refill > PvPStatics.MaximumStoreableActionPoints_Refill)
                    //    {
                    //        player.ActionPoints_Refill = PvPStatics.MaximumStoreableActionPoints_Refill;
                    //    }

                    //}
                    //else
                    //{
                    //    // give the usual base 10
                    //    player.ActionPoints += PvPStatics.APRestoredPerUpdate;

                    //    if (player.ActionPoints > PvPStatics.MaximumStoreableActionPoints)
                    //    {
                    //        player.ActionPoints = PvPStatics.MaximumStoreableActionPoints;
                    //    }

                    //    // Now consider the bonus.
                    //    // the limiting factor is the difference between current AP and max AP
                    //    decimal possibleBonus = PvPStatics.MaximumStoreableActionPoints - player.ActionPoints;

                    //    // the limiting factor may be ALL of the reserved AP
                    //    if (player.ActionPoints_Refill < possibleBonus)
                    //    {
                    //        possibleBonus = player.ActionPoints_Refill;
                    //    }

                    //    // the limiting factor may be the maximum bonus
                    //    if (possibleBonus > 20)
                    //    {
                    //        possibleBonus = 20;
                    //    }

                    //    player.ActionPoints += possibleBonus;
                    //    player.ActionPoints_Refill -= possibleBonus;
                    //}
                    

                    //player.TimesAttackingThisUpdate = 0;
                    //player.CleansesMeditatesThisRound = 0;

                    BuffBox buffs = ItemProcedures.GetPlayerBuffs(player);
                    player.Health += buffs.HealthRecoveryPerUpdate();
                    player.Mana += buffs.ManaRecoveryPerUpdate();


                    // THIS IS A DUPLICATION OF THE READJUST MAXES PROCEDURES.

                    player.MaxHealth = PvPStatics.XP__HealthManaBaseByLevel[player.Level] * (1.0M + (buffs.HealthBonusPercent() / 100.0M));
                    player.MaxMana = PvPStatics.XP__HealthManaBaseByLevel[player.Level] * (1.0M + (buffs.ManaBonusPercent() / 100.0M));

                    if (player.MaxHealth < 1)
                    {
                        player.MaxHealth = 1;
                    }

                    if (player.MaxMana < 1)
                    {
                        player.MaxMana = 1;
                    }


                    if (player.Health > player.MaxHealth)
                    {
                        player.Health = player.MaxHealth;
                    }
                    if (player.Mana > player.MaxMana)
                    {
                        player.Mana = player.MaxMana;
                    }
                    if (player.Health < 0)
                    {
                        player.Health = 0;
                    }
                    if (player.Mana < 0)
                    {
                        player.Mana = 0;
                    }

                    // for inbetween round testing
                    //if (PvPStatics.ChaosMode == true)
                    //{
                    //    player.Mana = player.MaxMana;
                    //    player.ActionPoints = 120;
                    //    player.TimesAttackingThisUpdate = -999;
                    //    player.ActionPoints_Refill = 360;
                    //}

                   // players_Animate_to_Save.Add(player);

                }

                //foreach (Player player in players_Animate_to_Save)
                //{
                //    playerRepo.SavePlayer(player);
                //}

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating animate players (" + playerIdsToSave.Count + ")");

#endregion main player loop

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating inanimate/animal players");

                List<Player> players_Inanimate_Animal = playerRepo.Players.Where(p => (p.Mobility == "inanimate" || p.Mobility == "animal") && p.MembershipId > 0).ToList();

                //////////////////////////

                foreach (Player player in players_Inanimate_Animal)
                {
                    if (player.TimesAttackingThisUpdate != 0)
                    {
                        player.TimesAttackingThisUpdate = 0;

                        if (PvPStatics.ChaosMode == true)
                        {
                            player.TimesAttackingThisUpdate = -999;
                        }

                        playerRepo.SavePlayer(player);
                    }
                }

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating inanimate/animal players (" + players_Inanimate_Animal.Count + ")");

                PvPStatics.AnimateUpdateInProgress = false;


                // bump down the timer on all items that are reuseable consuemables
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating items on cooldown");
                IItemRepository itemsRepo = new EFItemRepository();
                List<Item> itemsToUpdate = itemsRepo.Items.Where(i => i.TurnsUntilUse > 0).ToList();

                foreach (Item item in itemsToUpdate)
                {
                    item.TurnsUntilUse--;
                }

                foreach (Item item in itemsToUpdate)
                {
                    itemsRepo.SaveItem(item);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating items on cooldown");

                // we need to find the merchant to get her ID
    
               int merchantId = -999;

                try {
                    Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);
                    merchantId = merchant.Id;
                }
                catch
                {
                   // 
                }

                // delete all consumeable type items that have been sitting around on the ground for too long
                List<Item> possibleToDelete = itemsRepo.Items.Where(i => (i.dbLocationName != "" && i.OwnerId == -1) || (i.OwnerId == merchantId)).ToList();
                List<Item> deleteItems = new List<Item>();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting expired consumables");
                foreach (Item item in possibleToDelete)
                {
                    DbStaticItem temp = ItemStatics.GetStaticItem(item.dbName);
                    if (temp.ItemType != "consumable")
                    {
                        continue;
                    }
                    double droppedMinutesAgo = Math.Abs(Math.Floor(item.TimeDropped.Subtract(DateTime.UtcNow).TotalMinutes));
                    if (droppedMinutesAgo > PvPStatics.MinutesToDroppedItemDelete)
                    {
                        deleteItems.Add(item);
                    }
                }

                foreach (Item item in deleteItems)
                {
                    itemsRepo.DeleteItem(item.Id);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired consumables");

                // allow all items that have been recently equipped to be taken back off
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started resetting items that have been recently equipped");
                List<Item> recentlyEquipped = itemsRepo.Items.Where(i => i.EquippedThisTurn==true).ToList();

                foreach (Item item in recentlyEquipped)
                {
                    item.EquippedThisTurn = false;
                    itemsRepo.SaveItem(item);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished resetting items that have been recently equipped");

                serverLogRepo.SaveServerLog(log);
                log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                

                TempData["Result"] = "WORLD UPDATED";

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Lindella actions");
                serverLogRepo.SaveServerLog(log);
                AIProcedures.RunAIMerchantActions();

                log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Lindella actions");

                #region bosses
                try {
                    // run boss logic if one is active
                    if (worldStats.Boss_Donna == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Donna actions");
                        serverLogRepo.SaveServerLog(log);
                        AIProcedures.RunDonnaActions();
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Donna actions");
                    }
                }
                catch (Exception e)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  DONNA ERROR:  " + e.InnerException.ToString());
                }

                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Valentine == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Valentine actions");
                        serverLogRepo.SaveServerLog(log);
                        tfgame.Procedures.BossProcedures.BossProcedures_Valentine.RunValentineActions();
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Valentine actions");
                    }
                }
                catch (Exception e)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Valentine ERROR:  " + e.InnerException.ToString());
                }

                #endregion bosses

                // psychopaths
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started psychopath actions");
                serverLogRepo.SaveServerLog(log);
                AIProcedures.RunPsychopathActions();
                log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished psychopath actions");

                PvPWorldStatProcedures.UpdateWorldTurnCounter_UpdateDone();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started stored procedure maintenance");
                using (var context = new StatsContext())
                {
                    context.Database.ExecuteSqlCommand("ClearOldLocationLogs");
                    context.Database.ExecuteSqlCommand("ClearOldMessages");
                    context.Database.ExecuteSqlCommand("ClearValuelessTFEnergies");
                    //context.Database.ExecuteSqlCommand("resetAnimalInanimateTimesAttacking");
                    context.Database.ExecuteSqlCommand("DELETE FROM [Stats].[dbo].[PlayerLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                    context.Database.ExecuteSqlCommand("DELETE FROM [Stats].[dbo].[ChatLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                    context.Database.ExecuteSqlCommand("DELETE FROM [Stats].[dbo].[AIDirectives] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE()) AND DoNotRecycleMe = 0");
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished stored procedure maintenance");
                log.FinishTimestamp = DateTime.UtcNow;
                serverLogRepo.SaveServerLog(log);
                

            }

            return RedirectToAction("Play");
            // return View("UpdateWorld.cshtml","PvPAdmin", output);

        }

        public ActionResult FlagForSuspiciousActivity(int playerId)
        {
            // assert the person flagging has mod permissions
            if (PlayerProcedures.AccountIsTrusted(WebSecurity.CurrentUserId) == false)
            {
                TempData["Result"] = "You don't have permissions to do that.";
                return RedirectToAction("Play");
            }

          
            TempData["Result"] = "Player suspicious lock toggled.";
            PlayerProcedures.FlagPlayerForSuspicousActivity(playerId);

            return RedirectToAction("Play");

        }

        public ActionResult ClientUpdateCheck()
        {
            int num = PvPWorldStatProcedures.GetWorldTurnNumber();
            return Content(num.ToString(), "text/html");
        }

        private string GetIP()
        {
            string ip = Request.ServerVariables.Get("HTTP_REMOTE_USER");
            if (ip != "")
            {
                ip = Request.ServerVariables.Get("REMOTE_ADDR");
            }

            return ip;
        }

        public ActionResult Duel()
        {
            return View("../Duel/Duel.cshtml");
        }

      


    }

}


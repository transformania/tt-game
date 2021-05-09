using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Items.Services;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Skills.Queries;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Domain.ViewModels.NPCs;

namespace TT.Web.Controllers
{

    [Authorize]
    public partial class NPCController : Controller
    {

        // This controller should handle all interactions with NPC characters, ie Lindella, Wuffie, and Jewdewfae, and any others

        public virtual ActionResult TradeWithMerchant(string filter)
        {

            if (filter.IsNullOrEmpty())
            {
                filter = "clothes";
            }

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LindellaBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }


            ViewBag.MyMoney = Math.Floor(me.Money);

            var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);

            ViewBag.DisableLinks = "true";

            var output = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayerOfType { OwnerId = merchant.Id, ItemType = filter })
                .Where(i => i.EmbeddedOnItem == null);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];
            if (me.GameMode == (int)GameModeStatics.GameModes.PvP)
            {
                return View(MVC.NPC.Views.TradeWithMerchant, output.Where(i => i.PvPEnabled == 2 || i.PvPEnabled == -1));
            }
            else
            {
                return View(MVC.NPC.Views.TradeWithMerchant, output.Where(i => i.PvPEnabled == 1 || i.PvPEnabled == -1));
            }
        }

        public virtual ActionResult Purchase(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LindellaBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);

            var purchased = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = id });

            // assert that the item is in fact owned by the merchant
            if (purchased.Owner.Id != merchant.Id)
            {
                TempData["Error"] = "Lindella does not own this item.";
                return RedirectToAction(MVC.NPC.TradeWithMerchant());
            }

            // assert this item is not already embedded on an item
            if (purchased.EmbeddedOnItem != null)
            {
                TempData["Error"] = "You cannot purchase an already-embedded rune.";
                return RedirectToAction(MVC.NPC.TradeWithMerchant());
            }

            // assert this item is not a pet
            if (purchased.ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = $"You cannot purchase pets from {merchant.FirstName}.";
                return RedirectToAction(MVC.NPC.TradeWithMerchant());
            }

            var cost = ItemProcedures.GetCostOfItem(purchased, "buy");

            // assert that the player has enough money for this
            if (me.Money < cost)
            {
                TempData["Error"] = "You can't afford this right now.";
                TempData["SubError"] = "Try finding some more Arpeyjis from searching or take them off of players who you have turned inanimate or an animal.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the player has room in their inventory
            if (ItemProcedures.PlayerIsCarryingTooMuch(me, true))
            {
                TempData["Error"] = "You are carrying too many items to purchase a new one.";
                TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item is in the same game mode as the player, if the item's game mode is locked
            if ((purchased.PvPEnabled == (int)GameModeStatics.GameModes.PvP && me.GameMode != (int)GameModeStatics.GameModes.PvP || purchased.PvPEnabled == (int)GameModeStatics.GameModes.Protection && me.GameMode == (int)GameModeStatics.GameModes.PvP))
            {
                TempData["Error"] = "This item is the wrong mode.";
                TempData["SubError"] = "You cannot buy this item. It does not match your gameplay mode.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetProfit, -(float)cost);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetLoss, (float)cost);

            ItemProcedures.GiveItemToPlayer(purchased.Id, me.Id);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me);

            TempData["Result"] = "You have purchased a " + purchased.ItemSource.FriendlyName + " from Lindella.";
            return RedirectToAction(MVC.NPC.TradeWithMerchant(PvPStatics.ItemType_Shirt));
        }

        public virtual ActionResult SellList()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LindellaBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // show the permanent and consumable items the player is carrying
            var output = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id })
                .Where(i => i.ItemSource.ItemType != PvPStatics.ItemType_Pet &&
                !i.IsEquipped &&
                i.SoulboundToPlayer == null &&
                (i.IsPermanent || i.ItemSource.ItemType == PvPStatics.ItemType_Consumable || i.ItemSource.ItemType == PvPStatics.ItemType_Rune));

            return View(MVC.NPC.Views.SellList, output);
        }

        public virtual ActionResult Sell(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LindellaBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);

            var itemBeingSold = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = id });

            // assert that player does own this
            if (itemBeingSold.Owner.Id != me.Id)
            {
                TempData["Error"] = "You do not own this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item is not an animal type
            if (itemBeingSold.ItemSource.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "Unfortunately Lindella does not purchase or sell pets or animals.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item is either permanent or consumable
            if (!itemBeingSold.IsPermanent &&
                itemBeingSold.ItemSource.ItemType != PvPStatics.ItemType_Consumable &&
                itemBeingSold.ItemSource.ItemType != PvPStatics.ItemType_Rune)
            {
                TempData["Error"] = "Unfortunately Lindella will not purchase items that may later struggle free anymore.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is not soulbound
            if (itemBeingSold.SoulboundToPlayer != null)
            {
                TempData["Error"] = "You can't sell Lindella any soulbould items.";
                return RedirectToAction(MVC.PvP.Play());
            }

            ItemProcedures.GiveItemToPlayer(itemBeingSold.Id, merchant.Id);
            var cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetProfit, (float)cost);
            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetLoss, -(float)cost);

            TempData["Result"] = $"You sold your {itemBeingSold.ItemSource.FriendlyName} to Lindella for {cost:0} Arpeyjis.";
            return RedirectToAction(MVC.NPC.TradeWithMerchant(PvPStatics.ItemType_Shirt));
        }

        public virtual ActionResult TradeWithPetMerchant(int offset = 0)
        {

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());


            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.WuffieBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);

            ViewBag.Wuffie = true;
            ViewBag.DisableReleaseLink = true;

            var playerGameMode = me.GameMode;
            if (playerGameMode == (int)GameModeStatics.GameModes.Superprotection)
            {
                playerGameMode = (int)GameModeStatics.GameModes.Protection;
            }

            var pets = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = merchant.Id })
                                                .Where(i => i.ItemSource.ItemType == PvPStatics.ItemType_Pet &&
                                                            (i.PvPEnabled == (int)GameModeStatics.GameModes.Any ||
                                                             i.PvPEnabled == (int)playerGameMode));

            var output = new WuffieTradeViewModel
            {
                Paginator = new Paginator(pets.Count(), PvPStatics.PaginationPageSize),
                Money = (int)Math.Floor(me.Money),
            };
            output.Paginator.CurrentPage = offset + 1;
            output.Pets = pets.Skip(output.Paginator.GetSkipCount()).Take(output.Paginator.PageSize);

            var petAmount = ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet);

            if (petAmount == 0)
            {
                output.PlayerHasPet = false;
            }
            else
            {
                output.PlayerHasPet = true;
            }


            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.NPC.Views.TradeWithPetMerchant, output);
        }

        public virtual ActionResult PurchasePet(int id)
        {
            // assert that player is logged in
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.WuffieBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);

            var purchased = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = id });

            // assert that the item is in fact owned by the merchant
            if (purchased.Owner.Id != merchant.Id)
            {
                TempData["Error"] = "Wüffie does not own this pet.";
                return RedirectToAction(MVC.NPC.TradeWithPetMerchant());
            }


            if (purchased.PvPEnabled != (int)GameModeStatics.GameModes.Any)
            {
                var playerGameMode = me.GameMode;

                if (playerGameMode == (int)GameModeStatics.GameModes.Superprotection)
                {
                    playerGameMode = (int)GameModeStatics.GameModes.Protection;
                }

                if (purchased.PvPEnabled != playerGameMode)
                {
                    TempData["Error"] = "You cannot purchase a pet in this game mode.";
                    return RedirectToAction(MVC.NPC.TradeWithPetMerchant());
                }
            }

            var cost = ItemProcedures.GetCostOfItem(purchased, "buy");

            // assert that the player has enough money for this
            if (me.Money < cost)
            {
                TempData["Error"] = "You can't afford this right now.";
                TempData["SubError"] = "Try finding some more Arpeyjis from searching or take them off of players who you have turned inanimate or an animal.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the player does not already have a pet
            if (ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet) > 0)
            {
                TempData["Error"] = "You already have a pet.";
                TempData["SubError"] = "You can only keep one pet at a time.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // checks have passed.  Transfer the item, update abilities and stats
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);


            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetProfit, (float)-cost);
            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetLoss, (float)cost);

            ItemProcedures.GiveItemToPlayer(purchased.Id, me.Id);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me);

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
            newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
            playerRepo.SavePlayer(newMe);

            TempData["Result"] = "You have purchased a " + purchased.ItemSource.FriendlyName + " from Wüffie.";
            return RedirectToAction(MVC.NPC.TradeWithPetMerchant());
        }

        public virtual ActionResult SellPetList()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.WuffieBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // show the permanent and consumable items the player is carrying
            var output = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id })
                .Where(i => i.ItemSource.ItemType == PvPStatics.ItemType_Pet &&
                            i.IsEquipped);

            return View(MVC.NPC.Views.SellPetList, output);
        }

        public virtual ActionResult SellPet(int id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.WuffieBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);
            var itemBeingSold = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = id });

            // assert that player does own this
            if (itemBeingSold.Owner.Id != me.Id)
            {
                TempData["Error"] = "You do not own this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item is only an animal type
            if (itemBeingSold.ItemSource.ItemType != PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "Unfortunately Wüffie only buys and sells pets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item is permanent
            if (!itemBeingSold.IsPermanent)
            {
                TempData["Error"] = "Unfortunately Wüffie will not purchase pets that may later struggle free anymore.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert pet is not soulbound
            if (itemBeingSold.SoulboundToPlayer != null)
            {
                TempData["Error"] = "You can't sell Wüffie any soulbould pets.";
                return RedirectToAction(MVC.PvP.Play());
            }

            ItemProcedures.GiveItemToPlayer(itemBeingSold.Id, merchant.Id);
            var cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me);

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
            newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
            playerRepo.SavePlayer(newMe);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetProfit, (float)cost);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetLoss, (float)-cost);

            TempData["Result"] = $"You sold your {itemBeingSold.ItemSource.FriendlyName} to Wüffie for {cost:0} Arpeyjis.";
            return RedirectToAction(MVC.NPC.TradeWithPetMerchant());
        }

        public virtual ActionResult MoveVictimSend(int id, string to)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            var errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__MovementFormSourceId);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction(MVC.MindControl.MindControlList());
            }

            // assert that the victim has enough AP for the journey
            var apCost = MindControlProcedures.GetAPCostToMove(victim, to);
            if (victim.ActionPoints < apCost)
            {
                TempData["Error"] = "Your victim does not have enough action points to move there.";
                TempData["SubError"] = "Wait for your victim to regenerate more.";
                return RedirectToAction(MVC.MindControl.MindControlList());
            }

            // assert that the location is not the same as current
            if (victim.dbLocationName == to)
            {
                TempData["Error"] = "Your victim is already in this location.";
                return RedirectToAction(MVC.MindControl.MindControlList());
            }

            // assert that the player has not attacked too recently to move
            var lastAttackTimeAgo = Math.Abs(Math.Floor(victim.LastCombatTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));
            if (lastAttackTimeAgo < 30)
            {
                TempData["Error"] = "Your victim is resting from a recent attack.";
                TempData["SubError"] = "You must wait " + (30 - lastAttackTimeAgo) + " more seconds your victim will be able to move.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the victim is not carrying too much to move
            if (ItemProcedures.PlayerIsCarryingTooMuch(victim))
            {
                TempData["Error"] = "Your victim is carrying too much to be able to move.";
                return RedirectToAction(MVC.PvP.Play());
            }


            // assert player is not TPing into the dungeon from out in or vice versa
            var destinationIsInDungeon = to.Contains("dungeon_");
            if (victim.IsInDungeon() != destinationIsInDungeon)
            {
                TempData["Error"] = "You can't order your victim to move into the dungeon from outside of it or the other way around.";
                return RedirectToAction(MVC.PvP.Play());
            }


            // success; move the victim.
            PlayerProcedures.MovePlayerMultipleLocations(victim, to, apCost, false);
            MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__MovementFormSourceId);

            var attackerMessage = "You commanded " + victim.GetFullName() + " to move to " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == to).Name + ", using " + apCost + " of their action points in the process.";
            PlayerLogProcedures.AddPlayerLog(me.Id, attackerMessage, false);
            TempData["Result"] = attackerMessage;

            var victimMessage = me.GetFullName() + " commanded you to move to " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == to).Name + ", using " + apCost + " of your action points in the process!";
            PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__MindControlCommandsIssued, 1);

            return RedirectToAction(MVC.MindControl.MindControlList());
        }

        public virtual ActionResult TalkToBartender(string question)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var bartender = PlayerProcedures.GetPlayerFromBotId(AIStatics.BartenderBotId);

            // update timestamp (so that he can heal naturally)
            PlayerProcedures.SetTimestampToNow(bartender);
            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.BartenderBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            if (question == "none")
            {

                if (me.Gender == PvPStatics.GenderMale)
                {
                    ViewBag.Speech = "\"Greetings, sir " + me.GetFullName() + "!  How may I assist you today?\"";
                }
                else if (me.Gender == PvPStatics.GenderFemale)
                {
                    ViewBag.Speech = "\"Greetings, madam " + me.GetFullName() + "!  How may I assist you today?\"";
                }

            }
            else if (question == "lindella")
            {
                var lindella = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
                if (lindella != null)
                {
                    var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == lindella.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Lindella is at <b>" + temp.Name + "</b>.  She is always moving about town to find new customers, but she tends to stick to the streets because of her wagon.  Is there anything else I can assist you with?\"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Lindella at this time.  Is there anything else I can assist you with?";
                }
            }

            else if (question == "wuffie")
            {
                var wuffie = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);
                if (wuffie != null)
                {
                    var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == wuffie.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Wüffie is at <b>" + temp.Name + "</b>.  She moves around every now and then to graze her animals and pets, so you'll typically see her where there is plenty of grass.  Is there anything else I can assist you with?\"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Wüffie at this time.  Is there anything else I can assist you with?";
                }
            }
            else if (question == "jewdewfae")
            {
                var jewdewfae = PlayerProcedures.GetPlayerFromBotId(AIStatics.JewdewfaeBotId);
                if (jewdewfae != null)
                {
                    var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == jewdewfae.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Jewdewfae is at <b>" + temp.Name + "</b>.  She is always looking for people to play with, and unlike many of her more mischevious peers, she won't do it by turning you into a statue for a hundred years!  Probably.  Is there anything else I can assist you with?\"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Jewdewfae at this time.  Is there anything else I can assist you with?";
                }
            }
            else if (question == "lorekeeper")
            {
                var lorekeeper = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);
                if (lorekeeper != null)
                {
                    var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == lorekeeper.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, " + lorekeeper.GetFullName() + " is at <b>" + temp.Name + "</b>.  Poor chap, how far he's fallen from his glory days.  Why don't you go and talk to him?  He'll be willing to teach you a spell or two or sell you some books if you're looking to increase your knowledge.";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on " + lorekeeper.GetFullName() + " at this time.  Is there anything else I can assist you with?";
                }
            }
            else if (question == "soulbinder")
            {
                var soulbinder = PlayerProcedures.GetPlayerFromBotId(AIStatics.SoulbinderBotId);
                if (soulbinder != null)
                {
                    var temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == soulbinder.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Karin is at <b>" + temp.Name + "</b>. Nothing but trouble, that demoness... But I have to admit, I admire her entrepreneurial spirit! \"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Karin at this time.  Is there anything else I can assist you with?";
                }
            }
            else if (question == "psychos")
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var psychoCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull && (b.OriginalFormSourceId == AIProcedures.Psycho1FId || b.OriginalFormSourceId == AIProcedures.Psycho1MId));
                var fierceCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull && (b.OriginalFormSourceId == AIProcedures.Psycho3FId || b.OriginalFormSourceId == AIProcedures.Psycho3MId));
                var wrathfulCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull && (b.OriginalFormSourceId == AIProcedures.Psycho5FId || b.OriginalFormSourceId == AIProcedures.Psycho5MId));
                var loathfulCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull && (b.OriginalFormSourceId == AIProcedures.Psycho7FId || b.OriginalFormSourceId == AIProcedures.Psycho7MId));
                var soullessCount = playerRepo.Players.Count(b => b.BotId == AIStatics.PsychopathBotId && b.Mobility == PvPStatics.MobilityFull && (b.OriginalFormSourceId == AIProcedures.Psycho9FId || b.OriginalFormSourceId == AIProcedures.Psycho9MId));

                if (psychoCount + fierceCount + wrathfulCount + loathfulCount + soullessCount > 0)
                {
                    var output = "\"This city's never truly safe. But well, let's see here... </br>";

                    if (psychoCount > 0)
                    {
                        output += "Folks have mentioned seeing a large brawl between no less than <b>" + psychoCount + " psychopaths</b> out on the streets. </br>";
                    }
                    if (fierceCount > 0)
                    {
                        output += "I'd guess that about <b>" + fierceCount + " fierce psychopaths</b> are roaming about, based on the constant commotion. </br>";
                    }
                    if (wrathfulCount > 0)
                    {
                        output += "At least <b>" + wrathfulCount + "</b> separate times today <b>wrathful psyhopaths</b> tried to break in here.</br>";
                    }
                    if (loathfulCount > 0)
                    {
                        output += "There's also been a few mentions of <b>loathful psychopaths</b> prowling about... at least <b>" + loathfulCount + "</b> of them, I'd wager.</br>";
                    }
                    if (soullessCount > 0)
                    {
                        output += "Some alarming reports came in about <b>" + soullessCount + " soulless psychopaths</b> out there. Those truly are deranged monsters, I'd recommend to steer well clear if you see one.</br>";
                    }
                    output += "And well, that just about sums it up. Watch your step out there. Anything else I can assist you with?\"";

                    ViewBag.Speech = output;
                }
                else
                {
                    ViewBag.Speech = "It's a marvel, but the streets are really peaceful today. Is there anything else I can assist you with?";
                }
            }
            else if (question == "miniboss")
            {
                var output = "\"";
                var sororityMother = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossSororityMotherId);
                var popGoddess = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossPopGoddessId);
                var possessedMaid = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossPossessedMaidId);
                var seamstress = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossSeamstressId);
                var groundsKeeper = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossGroundskeeperId);
                var exchangeProfessor = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossExchangeProfessorId);
                var fiendishFarmhand = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossFiendishFarmhandId);
                var lazyLifeguard = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossLazyLifeguardId);
                var plushAngel = PlayerProcedures.GetAnimatePlayerFromBotId(AIStatics.MinibossPlushAngelId);

                if (sororityMother != null)
                {
                    output += "There was quite a stir at the <b>Sorority House</b>, from what I know. Something about their management being a little overprotective.</br>";
                }
                if (popGoddess != null)
                {
                    output += "The <b>concert hall</b> is abuzz right now - the townsfolk are flocking in to see some kind of big star...</br>";
                }
                if (possessedMaid != null)
                {
                    output += "There's a rumor of a maid spirit claiming another victim in <b>the estate</b>. The poor sap needs to be freed from their misery if that is the case.</br>";
                }
                if (seamstress != null)
                {
                    output += "Word is, a dangerous seamstress is gathering a congregation at <b>one of the clothing stores</b> around town. She has a way with words, so I urge you to be careful with her gospel. </br>";
                }
                if (groundsKeeper != null)
                {
                    output += "<b>Sunnyglade Park</b> is not safe to tread right now - the groundskeeper is on patrol and not to be trifled with.</br>";
                }
                if (exchangeProfessor != null)
                {
                    output += "An exchange professor is in town - I don't know much about her, but the rumors seem to link her to a string of recent disappearances around the <b>research lab</b>.</br>";
                }
                if (fiendishFarmhand != null)
                {
                    output += "Something's afoot at the <b>farm</b> - I've heard troubling whispers that those who come by don't return, while farm's pens are growing ever fuller. </br>";
                }
                if (lazyLifeguard != null)
                {
                    output += "The lifeguard is on post at the <b>pool</b>, though that doesn't really amount to much with that slacker. She'd rather just turn the patrons into pool toys than do her job. Unacceptable conduct for any establishment, if you ask me.</br>";
                }
                if (plushAngel != null)
                {
                    output += "If you see a plush angel on the street, it is better to leave it alone. It is just looking to make some friends.";
                }

                if (output.Length < 3)
                {
                    output += "I've not heard of any notable figures prowling about, sorry. Is there anything else I can assist you with?\"";
                }
                else
                {
                    output += "And that about sums it up. Is there anything else I can assist you with?\"";
                }
                ViewBag.Speech = output;

            }
            else if (question == "boss")
            {
                var stats = PvPWorldStatProcedures.GetWorldStats();
                var output = "";

                if (stats.Boss_Thief == AIStatics.ACTIVE)
                {
                    output += "\"There are a pair of rat thieves from the Seekshadow guild going about the town brashly mugging any inhabitants with enough Arpeyjis in their wallet.  Keep an eye out for them, and if possible make sure you don't carry too much money on you at once.  Careful, if you manage to defeat one, the other will not be too happy and will relentlessly pursue anyone who possesses the other.\"<br><br>";
                }
                if (stats.Boss_Donna == AIStatics.ACTIVE)
                {
                    output += "\"There is a powerful sorceress about the town, a relative of the witchling Circine Milton of whom you may have seen about town.  She won't attack you unprovoked, but know that she holds a grudge, and once she has you in her sights... well, let's just say don't be surprised to join the livestock down at the Milton Ranch.  Unless she's on the hunt for someone who has tried to fight her, you can find her in her bedroom at the Milton Ranch.  Be careful however, when Donna is near defeat, her magic amplifies and she will attack anyone and everyone in sight, regardless of how innocent they are!\"<br><br>";
                }
                if (stats.Boss_Bimbo == AIStatics.ACTIVE)
                {
                    output += "\"There is a woman named Lady Lovebringer, a renowned scientist who has recently arrived at the town.  I have heard she carries a powerful virus that can transform anyone who catches it into voluptuous women who spread the virus even further.  Those who have been infected will shortly transform into a bimbo and may find themselves attacking any other bystanders against their own will.  Luckily the Center for Transformation Control and Prevention are airdropping cures which will make you immune from the virus for a while, so keep an eye open for them lying around on the ground!\"<br><br>";
                }
                if (stats.Boss_Valentine == AIStatics.ACTIVE)
                {
                    /*
                    output += "\"A powerful vampire lord named Valentine is in town, letting people try to test their skills against him.  He won't leave the castle at all, so you can safely disengage him as you please; he holds no grudges.  However, it's best not to be standing near him for too long; he seeks to turn vampires out of the population here, and if he grows tired of you as a vampire he will try to turn you into a sleek sword for his personal collection instead!\"<br><br>";
                    */
                    output += "\"Rumor has it that a powerful demon named Krampus is hiding somewhere in town. People are saying that they are up to no good, spreading havoc where they go. Countless spell casters have met a terrible fate at the hand of this ravenous demon. You might want to steer clear if you can help it, or you might end up as something a little more festive instead.\"<br><br>";

                }
                if (stats.Boss_Sisters == AIStatics.ACTIVE)
                {
                    output += "\"A pair of feuding sisters is about town trying to turn the other into a form, physically and mentally, into something more desirable.  Perhaps you can take a side and help the issue to be resolved... after all, nobody likes it when family fights.\"<br><br>";
                }
                if (stats.Boss_Faeboss == AIStatics.ACTIVE)
                {
                    output += "\"A disgruntled fae from the Winston grove is going about the town, transforming those she meets into what she believes to be more suitable forms.  If you don't play along, she may well try to turn you into a flower or keep you as her own personal pet.  I've been told the only way to fight is to capture her and imprison her in a jar.  Perhaps questing in the back of Words of Wisdom will help you.  Best of luck, friend!\"<br><br>";
                }
                if (stats.Boss_MotorcycleGang == AIStatics.ACTIVE)
                {
                    output += $"\"A biker gang has rolled into town recently, searching for new recruits.  It's led by a ferocious woman named {BossProcedures_MotorcycleGang.BossFirstName} {BossProcedures_MotorcycleGang.BossLastName} who will forcibly convert any followers on the streets she comes across!  And those who won't follow, she'll gladly take home as a trophy.  The more followers she has, the stronger she'll be, so you might want to be careful about letting her ranks grow--and careful, once you're part of the gang, it's hard to get back out!\"";
                }
                if (output.IsNullOrEmpty())
                {
                    output += "\"I do not know of anything strange going about Sunnyglade right now.  Well, stranger than usual, anyway.  Is there anything else I can assist you with?\"";
                }
                ViewBag.Speech = output;

            }
            else if (question == "quests")
            {
                var output = "\"Looking for an adventure, eh?  You may want to go take a look at these locations...\"<br><br>";
                var quests = QuestProcedures.GetAllAvailableQuestsForPlayer(me, PvPWorldStatProcedures.GetWorldTurnNumber());

                foreach (var q in quests)
                {
                    var Loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == q.Location);

                    // just in case a location is misnamed, skip over it
                    if (Loc != null)
                    {
                        output += "\"<b>" + q.Name + "</b> is available for you at <b>" + Loc.Name + "</b>.\"<br><br>";
                    }
                }

                if (!quests.Any())
                {
                    output += "\"Oh, it seems there's nothing available for you right now.\"<br><br>";
                }

                output += "\"Is there anything else I can assist you with?\"";

                ViewBag.Speech = output;

            }
            else if (question == "bounties")
            {
                var bounties = BountyProcedures.OutstandingBounties();
                if (bounties == null || bounties.IsEmpty())
                {
                    ViewBag.Speech = "\"I've not heard of anyone in town currently being sought by the authorities.\"";
                }
                else
                {
                    string summary = "";

                    foreach (var bounty in bounties.OrderByDescending(b => b.CurrentReward))
                    {
                        summary += $"<li>There is a reward of up to <b>{bounty.CurrentReward} arpeyjis</b> to any PvP player who turns <b>{bounty.PlayerName}</b> into a <b>{bounty.Form?.FriendlyName}</b> by the end of turn {bounty.ExpiresTurn - 1}.</li>";
                    }
                    ViewBag.Speech = $"There are a number of wanted posters all over town.  The ones I remember are:<ul class=\"listdots\">{summary}</ul>";
                }
            }
            else if (question == "challenge")
            {
                var challenge = ChallengeProcedures.CurrentChallenge(me);
                if (challenge == null || challenge.ByEndOfTurn < PvPWorldStatProcedures.GetWorldTurnNumber())
                {
                    var message = "You haven't yet been set any challenges.";
                    var whyNotTry = "";

                    if (JokeShopProcedures.IsJokeShopActive())
                    {
                        whyNotTry = "  If you're after a challenge then try seeking out the Cursed Joke Shop.  Many people believe it isn't real, others swear blind they've been there.  One thing's for sure, if it does exist it's a place unlike any other in Sunnyglade and you should be very careful what you do there!";
                    }

                    ViewBag.Speech = $"\"{message}{whyNotTry}\"";
                }
                else
                {
                    string summary = "I hear you have been set a challenge by some angry spirits.<br /><br />What you need to do is:<ul class=\"listdots\">";
                    var progress = 0.0m;

                    foreach (var part in challenge.Parts)
                    {
                        var (done, parts) = part.Progress(me);

                        var progressString = "";
                        if (done != parts && parts > 1)
                        {
                            progressString = (parts == 100) ? $" ({done}%)" : $" ({done}/{parts})";
                        }

                        var color = part.Satisfied(me) ? "good" : "bad";
                        summary += $"<li>{part.Description}<br /><b class=\"{color}\" >{part.Status(me)}</b>{progressString}</li>";
                        progress += Math.Min(1m, done / ((decimal)parts));
                    }

                    if (challenge.Parts.Count() > 0)
                    {
                        progress /= challenge.Parts.Count();
                    }

                    summary += $"</ul>If you succeed you will be handsomely rewarded with <b>{challenge.Reward}</b>.<br /><br/>";

                    if (!challenge.Penalty.IsNullOrEmpty())
                    {
                        summary += $"But if you fail, those mean spirits will inflict a <b>penalty of {challenge.Penalty}</b> upon you!<br /><br />";
                    }

                    summary += $"To claim victory in this challenge you must present yourself for judgement at the Joke Shop <b>by the end of turn {challenge.ByEndOfTurn}</b>, about {challenge.GetTimeLeft()} from now.<br /><br />";

                    if (challenge.Satisfied(me))
                    {
                        summary += "It looks to me like you're most of the way there - just make sure you can get to the Joke Shop in time and be careful not to let your score slip.";
                    }
                    else if (progress > 0.75m)
                    {
                        summary += "You're making good progress, but you still have work to do before those spirits will be satisifed.";
                    }
                    else if (progress > 0.4m)
                    {
                        summary += "I reckon you're about half way done, but you've still got a lot to do before that deadline.";
                    }
                    else
                    {
                        summary += "In my opinion you still have a lot of work to do before you have any hope of claiming that reward.";
                    }

                    ViewBag.Speech = $"{summary}<br /><br />Good luck - I think you might need it!";
                }
            }

            return View(MVC.NPC.Views.TalkToBartender);
        }

        public virtual ActionResult TalkWithJewdewfae()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var fae = PlayerProcedures.GetPlayerFromBotId(AIStatics.JewdewfaeBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.JewdewfaeBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new JewdewfaeEncounterViewModel();
            output.JewdewfaeEncounter = BossProcedures_Jewdewfae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

            output.JewdewfaeEncounter.IntroText = output.JewdewfaeEncounter.IntroText.Replace("[", "<").Replace("]", ">");
            output.JewdewfaeEncounter.CorrectFormText = output.JewdewfaeEncounter.CorrectFormText.Replace("[", "<").Replace("]", ">");
            output.JewdewfaeEncounter.FailureText = output.JewdewfaeEncounter.FailureText.Replace("[", "<").Replace("]", ">");

            output.IsInWrongForm = false;

            if (me.FormSourceId != output.JewdewfaeEncounter.RequiredFormSourceId)
            {
                output.IsInWrongForm = true;
            }

            if (me.ActionPoints < 5)
            {
                output.IsTired = true;
            }

            output.ShowSuccess = false;

            output.HadRecentInteraction = false;
            if (BossProcedures_Jewdewfae.PlayerHasHadRecentInteraction(me, fae))
            {
                output.HadRecentInteraction = true;
            }

            return View(MVC.NPC.Views.TalkWithJewdewfae, output);
        }

        public virtual ActionResult PlayWithJewdewfae()
        {

            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var fae = PlayerProcedures.GetPlayerFromBotId(AIStatics.JewdewfaeBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.JewdewfaeBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has enough AP
            if (me.ActionPoints < 5)
            {
                TempData["Error"] = "You need 5 action points to play with Jewdewfae.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already interacted this location
            if (BossProcedures_Jewdewfae.PlayerHasHadRecentInteraction(me, fae))
            {
                TempData["Error"] = "You have already interacted with Jewdewfae here.";
                TempData["SubError"] = "Wait for her to move somewhere else.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new JewdewfaeEncounterViewModel();
            output.JewdewfaeEncounter = BossProcedures_Jewdewfae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

            output.JewdewfaeEncounter.IntroText = output.JewdewfaeEncounter.IntroText.Replace("[", "<").Replace("]", ">");
            output.JewdewfaeEncounter.CorrectFormText = output.JewdewfaeEncounter.CorrectFormText.Replace("[", "<").Replace("]", ">");
            output.JewdewfaeEncounter.FailureText = output.JewdewfaeEncounter.FailureText.Replace("[", "<").Replace("]", ">");

            if (me.FormSourceId == output.JewdewfaeEncounter.RequiredFormSourceId)
            {
                var xpGained = BossProcedures_Jewdewfae.AddInteraction(me);
                PlayerProcedures.GiveXP(me, xpGained);
                PlayerProcedures.ChangePlayerActionMana(-5, 0, 0, me.Id);
                output.XPGain = xpGained;
                output.ShowSuccess = true;
                output.HadRecentInteraction = false;

                var newSpells = ListifyHelper.Listify(SkillProcedures.GiveRandomFindableSkillsToPlayer(me, 3), true);

                string spellsLearned;
                if (newSpells.Length > 0)
                {
                    spellsLearned = $"Jewdewfae's magic also teaches you the following spells:  {newSpells}.";
                }
                else
                {
                    spellsLearned = "Unfortunately Jewdewfae can't teach you any spells that you don't already know.";
                }

                output.SpellsLearned = spellsLearned;

                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__JewdewfaeEncountersCompleted, 1);

                return View(MVC.NPC.Views.TalkWithJewdewfae, output);
            }
            else
            {
                TempData["Error"] = "You are not in the correct form to play with Jewdewfae right now.";
                return RedirectToAction(MVC.PvP.Play());
            }

        }

        public virtual ActionResult TalkToCandice()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var bimbo = PlayerProcedures.GetPlayerFromBotId(AIStatics.MouseBimboBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.MouseBimboBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert bimbo is still in base form
            if (bimbo.FormSourceId != BossProcedures_Sisters.BimboBossFormSourceId)
            {
                TempData["Error"] = BossProcedures_Sisters.BimboBossFirstName + " seems to be too distracted with her recent change to want to talk to you.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return View(MVC.NPC.Views.TalkToCandice);

        }

        public virtual ActionResult TalkToAdrianna()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var nerd = PlayerProcedures.GetPlayerFromBotId(AIStatics.MouseNerdBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.MouseNerdBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert nerd is still in base form
            if (nerd.FormSourceId != BossProcedures_Sisters.NerdBossFormSourceId)
            {
                TempData["Error"] = BossProcedures_Sisters.NerdBossFirstName + " seems to be too distracted with her recent change to want to talk to you.";
                return RedirectToAction(MVC.PvP.Play());
            }

            return View(MVC.NPC.Views.TalkToAdrianna);

        }

        public virtual ActionResult TalkToLorekeeper()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LoremasterBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // transfer all of the books Lindella owns over to Lorekeeper
            BossProcedures_Loremaster.TransferBooksFromLindellaToLorekeeper(loremaster);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.NPC.Views.TalkToLorekeeper);

        }

        public virtual ActionResult LorekeeperPurchaseBook()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LoremasterBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var output = new LorekeeperBookListViewModel
            {
                Items = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = loremaster.Id }),
                MyMoney = Math.Floor(me.Money)
            };

            ViewBag.Lorekeeper = true; // has to stay Viewbag to give access to partial view

            return View(MVC.NPC.Views.LorekeeperPurchaseBook, output);

        }

        public virtual ActionResult LorekeeperPurchaseBookSend(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LoremasterBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert lorekeeper owns this book
            var purchased = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = id });
            if (purchased.Owner.Id != loremaster.Id)
            {
                TempData["Error"] = "You can't purchse this as " + loremaster.GetFullName() + " does not own it.";
                return RedirectToAction(MVC.NPC.TalkToLorekeeper());
            }


            var cost = ItemProcedures.GetCostOfItem(purchased, "buy");

            // assert that the player has enough money for this
            if (me.Money < cost)
            {
                TempData["Error"] = "You can't afford this right now.";
                TempData["SubError"] = "Try finding some more Arpeyjis from searching or take them off of players who you have turned inanimate or an animal.";
                return RedirectToAction(MVC.NPC.TalkToLorekeeper());
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);

            ItemProcedures.GiveItemToPlayer(purchased.Id, me.Id);

            TempData["Result"] = $"You purchased {purchased.ItemSource.FriendlyName} from {loremaster.GetFullName()} for {cost:0} Arpeyjis.";
            return RedirectToAction(MVC.NPC.TalkToLorekeeper());

        }

        public virtual ActionResult LorekeeperLearnSpell(string filter)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LoremasterBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var output =
                Domain.DomainRegistry.Repository.Find(new GetSkillsPurchaseableByPlayer { MobilityType = filter, playerId = me.Id });

            ViewBag.Filter = filter;
            ViewBag.Money = Math.Floor(me.Money);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.NPC.Views.LorekeeperLearnSpell, output);
        }

        public virtual ActionResult LorekeeperLearnSpellSend(int spellSourceId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.LoremasterBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has enough money to buy a spell
            if (me.Money < PvPStatics.LorekeeperSpellPrice)
            {
                TempData["Error"] = "You don't have enough Arpeyjis to pay " + loremaster.GetFullName() + " to teach you any spells right now.";
                TempData["SubError"] = "You need " + PvPStatics.LorekeeperSpellPrice + " Arpeyjs to be taught a spell.";
                return RedirectToAction(MVC.NPC.TalkToLorekeeper());
            }

            // assert player does not already have that spell
            var spellViewModel = SkillProcedures.GetSkillViewModel_NotOwned(spellSourceId);

            var playerExistingSpells = SkillProcedures.GetSkillsOwnedByPlayer(me.Id);

            if (playerExistingSpells.Select(s => s.SkillSourceId).Contains(spellSourceId))
            {
                TempData["Error"] = "You already know that spell.";
                return RedirectToAction(MVC.NPC.TalkToLorekeeper());
            }

            // assert spells is learnable
            if ((spellViewModel.StaticSkill.LearnedAtLocation.IsNullOrEmpty() && spellViewModel.StaticSkill.LearnedAtRegion.IsNullOrEmpty()) || !spellViewModel.StaticSkill.IsPlayerLearnable)
            {
                TempData["Error"] = "You cannot learn that spell.";
                return RedirectToAction(MVC.NPC.TalkToLorekeeper());
            }

            // all checks passed; give the player the spell
            SkillProcedures.GiveSkillToPlayer(me.Id, spellViewModel.StaticSkill.Id);
            PlayerProcedures.GiveMoneyToPlayer(me, -PvPStatics.LorekeeperSpellPrice);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LorekeeperSpellsLearned, 1);

            TempData["Result"] = loremaster.GetFullName() + " taught you " + spellViewModel.StaticSkill.FriendlyName + " for " + PvPStatics.LorekeeperSpellPrice + " Arpeyjis.";
            return RedirectToAction(MVC.NPC.TalkToLorekeeper());
        }

        public virtual ActionResult TalkToValentine(string question)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var valentine = PlayerProcedures.GetPlayerFromBotId(AIStatics.ValentineBotId);

            //Why would the Krampus talk to someone when there are more important things that need doing?
            if (PvPWorldStatProcedures.IsAnyBossActive())
            {
                TempData["Error"] = "The Krampus appears to be ignoring you for the moment.";
                TempData["SubError"] = "Perhaps you should go pester someone else for now.";
                return RedirectToAction(MVC.PvP.Play());
            }

            try
            {
                DomainRegistry.Repository.FindSingle(
                    new CanInteractWith { BotId = AIStatics.ValentineBotId, PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            var responseText = "";

            // assert that the question is valid depending on Valentine's stance
            var stance = BossProcedures_Valentine.GetStance();
            if (question != "none")
            {
                BossProcedures_Valentine.TalkToAndCastSpell(me, valentine);
                var tftext = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse().ToList();
                var lastlog = tftext.FirstOrDefault(f => f.IsImportant);

                if (lastlog != null)
                {
                    // if the log is too old, presumably the player already got transformed, so don't show that text again.  Yeah, this is so hacky...
                    var secDiff = Math.Abs(Math.Floor(lastlog.Timestamp.Subtract(DateTime.UtcNow).TotalSeconds));
                    if (secDiff < 3)
                    {
                        responseText = lastlog.Message;
                    }
                    else
                    {
                        responseText = valentine.GetFullName() + " ignores you.";
                    }
                }
                else
                {
                    responseText = valentine.GetFullName() + " ignores you.";
                }

            }

            ViewBag.stance = stance;

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];

            ViewBag.Result = responseText;

            return View(MVC.NPC.Views.TalkToValentine);

        }

        public virtual ActionResult TalkToSoulbinder()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var npc = PlayerProcedures.GetPlayerFromBotId(AIStatics.SoulbinderBotId);
            var output = new TalkToSoulbinderViewModel
            {
                Items = DomainRegistry.Repository.Find(new GetPlayerItemsThatCanBeSoulbound { OwnerId = me.Id }).ToList(),
                AllSoulboundItems = DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = me.Id })
            };
            output.NPCOwnedSoulboundItems = output.AllSoulboundItems.Where(i => i.Owner != null && i.Owner.Id == npc.Id);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.NPC.Views.TalkToSoulbinder, output);
        }

        public virtual ActionResult SoulbindItemList()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var npc = PlayerProcedures.GetPlayerFromBotId(AIStatics.SoulbinderBotId);
            var output = new TalkToSoulbinderViewModel
            {
                Items = DomainRegistry.Repository.Find(new GetPlayerItemsThatCanBeSoulbound { OwnerId = me.Id }).ToList(),
                Money = (int)me.Money,
                AllSoulboundItems = DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = me.Id })
            };
            output.NPCOwnedSoulboundItems = output.AllSoulboundItems.Where(i => i.Owner != null && i.Owner.Id == npc.Id);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(MVC.NPC.Views.SoulbindItemList, output);
        }

        public virtual ActionResult SoulbindItem(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new SoulbindItemToPlayer { ItemId = itemId, OwnerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            return RedirectToAction(MVC.NPC.TalkToSoulbinder());
        }

        public virtual ActionResult RetrieveSoulboundItems()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new RetrieveSoulboundItems { PlayerId = me.Id });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction(MVC.PvP.Play());
            }

            return RedirectToAction(MVC.NPC.TalkToSoulbinder());
        }

        [HttpGet]
        public virtual ActionResult SoulboundRename(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            var item = DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = me.Id }).FirstOrDefault(i => i.FormerPlayer.Id == id);

            if (item == null)
            {
                TempData["Error"] = "This player is not soul bound to you";
                return RedirectToAction(MVC.PvP.Play());
            }

            var itemSoulboundCount = DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = me.Id }).Count();
            var price = PriceCalculator.GetPriceToSoulbindNextItem(itemSoulboundCount) / 10;

            if (me.Money < price)
            {
                TempData["Error"] = "You do not have enough money to rename this item.";
                TempData["SubError"] = "You need " + price + " arpeyjis to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.GiveMoneyToPlayer(me, -price);

            var output = new PlayerNameViewModel
            {
                OwnerMoney = me.Money,
                SoulboundCount = itemSoulboundCount,
                Id = item.FormerPlayer.Id,
                NewFirstName = item.FormerPlayer.FirstName,
                NewLastName = item.FormerPlayer.LastName
            };

            return View(MVC.NPC.Views.SoulboundRename, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SoulboundRename(PlayerNameViewModel input)
        {

            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var player = playerRepo.Players.FirstOrDefault(p => p.Id == input.Id);

            var item = DomainRegistry.Repository.Find(new GetItemsSoulboundToPlayer { OwnerId = me.Id }).Where(i => i.FormerPlayer.Id == input.Id).FirstOrDefault();

            // assert that the first name is not reserved by the system
            var fnamecheck = TrustStatics.NameIsReserved(input.NewFirstName);
            if (!fnamecheck.IsNullOrEmpty())
            {
                ModelState.AddModelError("NewFirstName", "You can't use the first name '" + input.NewFirstName + "'.  It is reserved or else not allowed.");
            }


            // assert that the last name is not reserved by the system
            var lnamecheck = TrustStatics.NameIsReserved(input.NewLastName);
            if (!lnamecheck.IsNullOrEmpty())
            {
                ModelState.AddModelError("NewLastName", "You can't use the last name '" + input.NewLastName + "'.  It is reserved or else not allowed.");
            }

            IReservedNameRepository resNameRepo = new EFReservedNameRepository();
            var resName = resNameRepo.ReservedNames.FirstOrDefault(r => r.FullName == input.NewFirstName + " " + input.NewLastName);

            if (resName != null && resName.MembershipId != player.MembershipId)
            {
                ModelState.AddModelError("", "This name has been reserved by a different player.  Choose another.");
            }


            var ghost = playerRepo.Players.FirstOrDefault(p => p.FirstName == input.NewFirstName && p.LastName == input.NewLastName);

            if (ghost != null)
            {
                ModelState.AddModelError("", "A character of this name already exists.  Choose another.");
            }
            else
            {
                var originalGhost = playerRepo.Players.FirstOrDefault(p => p.OriginalFirstName == input.NewFirstName && p.OriginalLastName == input.NewLastName && p.BotId == AIStatics.ActivePlayerBotId);

                if (originalGhost != null && originalGhost.MembershipId != player.MembershipId)
                {
                    ModelState.AddModelError("", "An existing character is eligible to reclaim this name.  Choose another.");
                }
            }


            if (item == null)
            {
                TempData["Error"] = "This player is not soul bound to you";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (!string.IsNullOrEmpty(input.NewFirstName) && input.NewFirstName != player.FirstName)
            {
                player.FirstName = input.NewFirstName;
            }

            if (!string.IsNullOrEmpty(input.NewLastName) && input.NewLastName != player.LastName)
            {
                player.LastName = input.NewLastName;
            }

            if (!ModelState.IsValid)
            {
                return View(MVC.NPC.Views.SoulboundRename, input);
            }

            playerRepo.SavePlayer(player);

            PlayerLogProcedures.AddPlayerLog(me.Id, "<b>You have renamed your soulbound object: </b>" + input.NewFirstName + " " + input.NewLastName, true);
            PlayerLogProcedures.AddPlayerLog(player.Id, "<b>Your owner has renamed you to </b>" + input.NewFirstName + " " + input.NewLastName, true);

            return RedirectToAction(MVC.PvP.Play());
        }
    }
}

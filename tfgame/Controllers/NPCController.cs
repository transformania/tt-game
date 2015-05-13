using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using tfgame.dbModels.Models;
using tfgame.Filters;
using tfgame.Procedures;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;

namespace tfgame.Controllers
{
    [InitializeSimpleMembership]
    public class NPCController : Controller
    {

        // This controller should handle all interactions with NPC characters, ie Lindella, Wuffie, and Jewdewfae, and any others

        public const int MovementControlLimit = 2;

        public ActionResult Index()
        {
            return View();
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


            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the player has room in their inventory
            //if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffsSQL(me))) {
            //    TempData["Error"] = "You are carrying too many items to purchase a new one.";
            //    TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
            //    return RedirectToAction("Play", "PvP");
            //}

            ViewBag.NoRoom = ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffsSQL(me));

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
            if (me.GameMode == 2)
            {
                return (View(output.Where(i => i.dbItem.PvPEnabled == 2 || i.dbItem.PvPEnabled == -1)));
            }
            else
            {
                return (View(output.Where(i => i.dbItem.PvPEnabled == 1 || i.dbItem.PvPEnabled == -1)));
            }
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
                return RedirectToAction("Play", "PvP");
            }

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
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
                return RedirectToAction("Play", "PvP");
            }

            // assert that the player has room in their inventory
            if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffsSQL(me)))
            {
                TempData["Error"] = "You are carrying too many items to purchase a new one.";
                TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the item is in the same game mode as the player, if the item's game mode is locked
            if ((purchased.dbItem.PvPEnabled == 2 && me.GameMode != 2 || purchased.dbItem.PvPEnabled == 1 && me.GameMode == 2))
            {
                TempData["Error"] = "This item is the wrong mode.";
                TempData["SubError"] = "You cannot buy this item. It does not match your gameplay mode.";
                return RedirectToAction("Play", "PvP");
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaCostsAmount, (float)cost)
             ).Start();

            ItemProcedures.GiveItemToPlayer_Nocheck(purchased.dbItem.Id, me.Id);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me.Id);

            TempData["Result"] = "You have purchased a " + purchased.Item.FriendlyName + " from Lindella.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }

        [Authorize]
        public ActionResult SellList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }

            // show the permanent and consumable items the player is carrying
            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(me.Id).Where(i => i.Item.ItemType != PvPStatics.ItemType_Pet && i.dbItem.IsEquipped == false && (i.dbItem.IsPermanent == true || i.Item.ItemType == "consumable"));
            return View(output);
        }

        [Authorize]
        public ActionResult Sell(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-3);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Lindella.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Lindella in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }

            ItemViewModel itemBeingSold = ItemProcedures.GetItemViewModel(id);

            // assert that player does own this
            if (itemBeingSold.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own this item.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the item is not an animal type
            if (itemBeingSold.Item.ItemType == PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "Unfortunately Lindella does not purchase or sell pets or animals.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the item is either permanent or consumable
            if (itemBeingSold.dbItem.IsPermanent == false && itemBeingSold.Item.ItemType != "consumable")
            {
                TempData["Error"] = "Unfortunately Lindella will not purchase items that may later struggle free anymore.";
                return RedirectToAction("Play", "PvP");
            }

            ItemProcedures.GiveItemToPlayer_Nocheck(itemBeingSold.dbItem.Id, merchant.Id);
            decimal cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaProfitsAmount, (float)cost)
             ).Start();


            TempData["Result"] = "You sold your " + itemBeingSold.Item.FriendlyName + " to Lindella for " + (int)cost + " Arpeyjis.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }

        [Authorize]
        public ActionResult TradeWithPetMerchant()
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);
            ViewBag.MyMoney = Math.Floor(me.Money);

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-10);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Wüffie.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Wüffie in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }

            int petAmount = ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet);

            if (petAmount == 0) 
            { 
                ViewBag.PlayerHasPet = false;
            }
            else
            {
                ViewBag.PlayerHasPet = true;
            }

            ViewBag.Wuffie = true;

            ViewBag.DisableReleaseLink = true;
            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Pet);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

        [Authorize]
        public ActionResult PurchasePet(int id)
        {
            // assert that player is logged in
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-10);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Wüffie.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Wüffie in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }

            ItemViewModel purchased = ItemProcedures.GetItemViewModel(id);

            // assert that the item is in fact owned by the merchant
            if (purchased.dbItem.OwnerId != merchant.Id)
            {
                TempData["Error"] = "Wüffie does not own this pet.";
                return RedirectToAction("TradeWithMerchant");
            }

            decimal cost = ItemProcedures.GetCostOfItem(purchased, "buy");

            // assert that the player has enough money for this
            if (me.Money < cost)
            {
                TempData["Error"] = "You can't afford this right now.";
                TempData["SubError"] = "Try finding some more Arpeyjis from searching or take them off of players who you have turned inanimate or an animal.";
                return RedirectToAction("Play","PvP");
            }

            // assert that the player does not already have a pet
            if (ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet) > 0)
            {
                TempData["Error"] = "You already have a pet.";
                TempData["SubError"] = "You can only keep one pet at a time.";
                return RedirectToAction("Play", "PvP");
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);


            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieCostsAmount, (float)cost)
             ).Start();

            ItemProcedures.GiveItemToPlayer_Nocheck(purchased.dbItem.Id, me.Id);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me.Id);




            TempData["Result"] = "You have purchased a " + purchased.Item.FriendlyName + " from Wüffie.";
            return RedirectToAction("TradeWithPetMerchant");
        }

        [Authorize]
        public ActionResult SellPetList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-10);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Wüffie.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Wüffie
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Wüffie in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }


           

            // show the permanent and consumable items the player is carrying
            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(me.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Pet && i.dbItem.IsEquipped == true && i.dbItem.IsPermanent == true);
            return View(output);
        }


        [Authorize]
        public ActionResult SellPet(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(WebSecurity.CurrentUserId);

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromMembership(-10);

            // assert the merchant is animate
            if (merchant.Mobility != "full")
            {
                TempData["Error"] = "You cannot trade with Wüffie.  She has been turned into an item or animal.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same location as Lindella
            if (me.dbLocationName != merchant.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Wüffie in order to trade with her.";
                TempData["SubError"] = "She may have moved since beginning this transaction.";
                return RedirectToAction("Play", "PvP");
            }

            ItemViewModel itemBeingSold = ItemProcedures.GetItemViewModel(id);

            // assert that player does own this
            if (itemBeingSold.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own this item.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the item is only an animal type
            if (itemBeingSold.Item.ItemType != PvPStatics.ItemType_Pet)
            {
                TempData["Error"] = "Unfortunately Wüffie only buys and sells pets.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that the item is either permanent or consumable
            if (itemBeingSold.dbItem.IsPermanent == false)
            {
                TempData["Error"] = "Unfortunately Wüffie will not purchase pets that may later struggle free anymore.";
                return RedirectToAction("Play", "PvP");
            }

            ItemProcedures.GiveItemToPlayer_Nocheck(itemBeingSold.dbItem.Id, merchant.Id);
            decimal cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieProfitsAmount, (float)cost)
             ).Start();

            TempData["Result"] = "You sold your " + itemBeingSold.Item.FriendlyName + " to Wüffie for " + (int)cost + " Arpeyjis.";
            return RedirectToAction("TradeWithPetMerchant");
        }

        [Authorize]
        public ActionResult MindControlList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();

            MindControlProcedures.ClearPlayerMindControlFlagIfOn(me);

            ViewBag.MyId = me.Id;

            IEnumerable<MindControlViewModel> output = MindControlProcedures.GetAllMindControlVMsWithPlayer(me);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);
        }

        [Authorize]
        public ActionResult MoveVictim(int id)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Movement);
            if (errorsBox.HasError == true)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            ViewBag.Victim = victim;
            ViewBag.Buffs = ItemProcedures.GetPlayerBuffsSQL(victim);

            if (victim.IsInDungeon() == true)
            {
                IEnumerable<Location> output = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region == "dungeon");
                return View(output);
            }
            else
            {
                IEnumerable<Location> output = LocationsStatics.LocationList.GetLocation.Where(l => l.dbName != "" && l.Region != "dungeon");
                return View(output);
            } 
        }


        [Authorize]
        public ActionResult StripVictim(int id)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Strip);
            if (errorsBox.HasError == true)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            List<ItemViewModel> victimItems = ItemProcedures.GetAllPlayerItems(victim.Id).ToList();

            if (victimItems.Count() > 0)
            {
                double max = victimItems.Count();
                Random rand = new Random();
                double num = rand.NextDouble();

                int index = Convert.ToInt32(Math.Floor(num * max));
                ItemViewModel itemToDrop = victimItems.ElementAt(index);

                MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__Strip);

                string attackerMessage = "";

                if (itemToDrop.Item.ItemType != PvPStatics.ItemType_Pet)
                {
                    attackerMessage = "You commanded " + victim.GetFullName() + " to drop something.  They let go of a " + itemToDrop.Item.FriendlyName + " that they were carrying.";
                } else {
                    attackerMessage = "You commanded " + victim.GetFullName() + " to drop something.  They released their pet " + itemToDrop.Item.FriendlyName + " that they had tamed.";
                }

                PlayerLogProcedures.AddPlayerLog(me.Id, attackerMessage, false);
                TempData["Result"] = attackerMessage;

                string victimMessage = "";

                if (itemToDrop.Item.ItemType != PvPStatics.ItemType_Pet)
                {
                     victimMessage =  me.GetFullName() + " commanded you to to drop something. You had no choice but to go of a " + itemToDrop.Item.FriendlyName + " that you were carrying.";
                }
                else
                {
                    victimMessage = me.GetFullName() + " commanded you to drop something. You had no choice but to release your pet " + itemToDrop.Item.FriendlyName + " that you had tamed.";
                }

                PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);

                ItemProcedures.DropItem(itemToDrop.dbItem.Id, victim.dbLocationName);

                string locationLogMessage = victim.GetFullName() + " was forced to drop their <b>" + itemToDrop.Item.FriendlyName + "</b> by someone mind controlling them.";
                LocationLogProcedures.AddLocationLog(victim.dbLocationName, locationLogMessage);


            }
            else
            {
                TempData["Error"] = "It seems " + victim.GetFullName() + " was not carrying or wearing anything to drop!";
            }

            return RedirectToAction("Play","PvP");
        }

        [Authorize]
        public ActionResult DeMeditateVictim(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Meditate);
            if (errorsBox.HasError == true)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            BuffBox buffs = ItemProcedures.GetPlayerBuffsSQL(victim);
            string result = PlayerProcedures.DeMeditate(victim, me, buffs);

            MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__Meditate);


            TempData["Result"] = "You force " + victim.GetFullName() + " to meditate while filling their mind with nonsense instead of relaxation, lowering their mana instead of increasing it!";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult MoveVictimSend(int id, string to)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Movement);
            if (errorsBox.HasError == true)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            BuffBox victimBuffs = ItemProcedures.GetPlayerBuffsSQL(victim);
            // assert that the victim has enough AP for the journey
            decimal apCost = MindControlProcedures.GetAPCostToMove(victimBuffs, victim.dbLocationName, to);
            if (victim.ActionPoints < apCost)
            {
                TempData["Error"] = "Your victim does not have enough action points to move there.";
                TempData["SubError"] = "Wait for your victim to regenerate more.";
                return RedirectToAction("MindControlList");
            }

            // assert that the location is not the same as current
            if (victim.dbLocationName == to)
            {
                TempData["Error"] = "Your victim is already in this location.";
                return RedirectToAction("MindControlList");
            }

            // assert that the player has not attacked too recently to move
            double lastAttackTimeAgo = Math.Abs(Math.Floor(victim.LastCombatTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));
            if (lastAttackTimeAgo < 45)
            {
                TempData["Error"] = "Your victim is resting from a recent attack.";
                TempData["SubError"] = "You must wait " + (45 - lastAttackTimeAgo) + " more seconds your victim will be able to move.";
                return RedirectToAction("Play");
            }

            // assert that the victim is not carrying too much to move
            if (ItemProcedures.PlayerIsCarryingTooMuch(victim.Id, 0, victimBuffs))
            {
                TempData["Error"] = "Your victim is carrying too much to be able to move.";
                return RedirectToAction("Play");
            }


            // assert player is not TPing into the dungeon from out in or vice versa
            bool destinationIsInDungeon = false;
            if (to.Contains("dungeon_"))
            {
                destinationIsInDungeon = true;
            }
            if (victim.IsInDungeon() != destinationIsInDungeon)
            {
                TempData["Error"] = "You can't order your victim to move into the dungeon from outside of it or the other way around.";
                return RedirectToAction("Play");
            }


            // success; move the victim.
            PlayerProcedures.MovePlayerMultipleLocations(victim, to, apCost);
            MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__Movement);

            string attackerMessage = "You commanded " + victim.GetFullName() + " to move to " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == to).Name + ", using " + apCost + " of their action points in the process.";
            PlayerLogProcedures.AddPlayerLog(me.Id, attackerMessage, false);
            TempData["Result"] = attackerMessage;

           string victimMessage = me.GetFullName() + " commanded you to move to " + LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == to).Name + ", using " + apCost + " of your action points in the process!";
           PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);

            return RedirectToAction("MindControlList");
        }

        [Authorize]
        public ActionResult TalkToBartender(string question)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player bartender = PlayerProcedures.GetPlayerFromMembership();

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with Rusty.";
                return RedirectToAction("Play", "PvP");
            }

            // assert bartender is mobile
            if (bartender.Mobility != "full")
            {
                TempData["Error"] = "Rusty must be animate in order to chat with you.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is animate
            if (me.dbLocationName != bartender.dbLocationName)
            {
                TempData["Error"] = "You cannot chat with Rusty as you are not in the same location as him.";
                return RedirectToAction("Play", "PvP");
            }

            if (question == "none")
            {

                if (me.Gender == "male") {
                    ViewBag.Speech = "\"Greetings, sir " + me.GetFullName() + "!  How may I assist you today?\"";
                } else if (me.Gender == "female") {
                    ViewBag.Speech = "\"Greetings, madam " + me.GetFullName() + "!  How may I assist you today?\"";
                }
                
            }
            else if (question == "lindella")
            {
                Player lindella = PlayerProcedures.GetPlayerFromMembership(AIProcedures.LindellaMembershipId);
                if (lindella != null)
                {
                    Location temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == lindella.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Lindella is at <b>" + temp.Name + "</b>.  She is always moving about town to find new customers, but she tends to stick to the streets because of her wagon.  Is there anything else I can assist you with?\"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Lindella at this time.  Is there anything else I can assist you with?";
                }
            }

            else if (question == "wuffie")
            {
                Player wuffie = PlayerProcedures.GetPlayerFromMembership(AIProcedures.WuffieMembershipId);
                if (wuffie != null)
                {
                    Location temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == wuffie.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Wüffie is at <b>" + temp.Name + "</b>.  She moves around every now and then to graze her animals and pets, so you'll typically see her where there is plenty of grass.  Is there anything else I can assist you with?\"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Wüffie at this time.  Is there anything else I can assist you with?";
                }
            }
            else if (question == "jewdewfae")
            {
                Player jewdewfae = PlayerProcedures.GetPlayerFromMembership(AIProcedures.JewdewfaeMembershipId);
                if (jewdewfae != null)
                {
                    Location temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == jewdewfae.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, Jewdewfae is at <b>" + temp.Name + "</b>.  She is always looking for people to play with, and unlike many of her more mischevious peers, she won't do it by turning you into a statue for a hundred years!  Probably.  Is there anything else I can assist you with?\"";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on Jewdewfae at this time.  Is there anything else I can assist you with?";
                }
            }
            else if (question == "boss")
            {
                PvPWorldStat stats = PvPWorldStatProcedures.GetWorldStats();
                string output = "";


                if (stats.Boss_Thief == "active")
                {
                    output += "\"There are a pair of rat thieves from the Seekshadow guild going about the town brashly mugging any inhabitants with enough Arpeyjis in their wallet.  Keep an eye out for them, and if possible make sure you don't carry too much money on you at once.  Careful, if you manage to defeat one, the other will not be too happy and will relentlessly pursue anyone who possesses the other.\"<br><br>";
                }
                if (stats.Boss_Donna == "active") {
                    output += "\"There is a powerful sorceress about the town, a relative of the witchling Circine Milton of whom you may have seen about town.  She won't attack you unprovoked, but know that she holds a grudge, and once she has you in her sights... well, let's just say don't be surprised to join the livestock down at the Milton Ranch.  Unless she's on the hunt for someone who has tried to fight her, you can find her in her bedroom at the Milton Ranch.  Be careful however, when Donna is near defeat, her magic amplifies and she will attack anyone and everyone in sight, regardless of how innocent they are!\"<br><br>";

                } if (stats.Boss_Bimbo == "active") {
                    output += "\"There is a woman named Lady Lovebringer, a renowned scientist who has recently arrived at the town.  I have heard she carries a powerful virus that can transform anyone who catches it into voluptuous women who spread the virus even further.  Those who have been infected will shortly transform into a bimbo and may find themselves attacking any other bystanders against their own will.  Luckily the Center for Transformation Control and Prevention are airdropping cures which will make you immune from the virus for a while, so keep an eye open for them lying around on the ground!\"<br><br>";
                

                } if (stats.Boss_Valentine == "active") {
                    output += "\"A powerful vampire lord named Valentine is in town, letting people try to test their skills against him.  He won't leave the castle at all, so you can safely disengage him as you please; he holds no grudges.  However, it's best not to be standing near him for too long; he seeks to turn vampires out of the population here, and if he grows tired of you as a vampire he will try to turn you into a sleek sword for his personal collection instead!\"<br><br>";

                } if (stats.Boss_Sisters == "active") {
                    output += "\"A pair of feuding sisters is about town trying to turn the other into a form, physically and mentally, into something more desirable.  Perhaps you can take a side and help the issue to be resolved... after all, nobody likes it when family fights.\"<br><br>";
                }

                if (output == "")
                {
                    output += "\"I do not know of anything strange going about Sunnyglade right now.  Well, stranger than usual, anyway.  Is there anything else I can assist you with?\"";
                }

                ViewBag.Speech = output;

            }

            return View();
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
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in same location as jewdewfae
            if (me.dbLocationName != fae.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Jewfewfae in order to talk with her.";
                return RedirectToAction("Play", "PvP");
            }

            JewdewfaeEncounter output = tfgame.Procedures.BossProcedures.BossProcedures_Fae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

            output.IntroText = output.IntroText.Replace("[", "<").Replace("]", ">");
            output.CorrectFormText = output.CorrectFormText.Replace("[", "<").Replace("]", ">");
            output.FailureText = output.FailureText.Replace("[", "<").Replace("]", ">");

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
                return RedirectToAction("Play","PvP");
            }

            // assert player is in same location as jewdewfae
            if (me.dbLocationName != fae.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as Jewfewfae in order to talk with her.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player has enough AP
            if (me.ActionPoints < 5)
            {
                TempData["Error"] = "You need 5 action points to play with Jewdewfae.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player has not already interacted this location
            if (tfgame.Procedures.BossProcedures.BossProcedures_Fae.PlayerHasHadRecentInteraction(me, fae))
            {
                TempData["Error"] = "You have already interacted with Jewdewfae here.";
                TempData["SubError"] = "Wait for her to move somewhere else.";
                return RedirectToAction("Play", "PvP");
            }

            JewdewfaeEncounter output = tfgame.Procedures.BossProcedures.BossProcedures_Fae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

            output.IntroText = output.IntroText.Replace("[", "<").Replace("]", ">");
            output.CorrectFormText = output.CorrectFormText.Replace("[", "<").Replace("]", ">");
            output.FailureText = output.FailureText.Replace("[", "<").Replace("]", ">");

            if (me.Form == output.RequiredForm)
            {
                decimal xpGained = tfgame.Procedures.BossProcedures.BossProcedures_Fae.AddInteraction(me);
                PlayerProcedures.GiveXP(me.Id, xpGained);
                PlayerProcedures.ChangePlayerActionMana(5, 0, 0, me.Id);
                ViewBag.XPGain = xpGained;
                ViewBag.ShowSuccess = true;
                ViewBag.HadRecentInteraction = false;

                new Thread(() =>
                     StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__JewdewfaeEncountersCompleted, 1)
                 ).Start();

                return View("TalkWithJewdewfae", output);
            }
            else
            {
                TempData["Error"] = "You are not in the correct form to play with Jewdewfae right now.";
                return RedirectToAction("Play", "PvP");
            }

        }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
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
            //if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffs(me))) {
            //    TempData["Error"] = "You are carrying too many items to purchase a new one.";
            //    TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
            //    return RedirectToAction("Play", "PvP");
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
            if (ItemProcedures.PlayerIsCarryingTooMuch(me.Id, 0, ItemProcedures.GetPlayerBuffs(me)))
            {
                TempData["Error"] = "You are carrying too many items to purchase a new one.";
                TempData["SubError"] = "You need to free up a space in your inventory before purchasing something from Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);
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
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim);
            if (errorsBox.HasError == true)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            ViewBag.Victim = victim;
            ViewBag.Buffs = ItemProcedures.GetPlayerBuffs(victim);

            IEnumerable<Location> output = LocationsStatics.GetLocation.Where(l => l.dbName != "");

            return View(output);
        }

        [Authorize]
        public ActionResult MoveVictimSend(int id, string to)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership();
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim);
            if (errorsBox.HasError == true)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            // assert that the victim has enough AP for the journey
            decimal apCost = MindControlProcedures.GetAPCostToMove(ItemProcedures.GetPlayerBuffs(victim), victim.dbLocationName, to);
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


            // success; move the victim.
            PlayerProcedures.MovePlayerMultipleLocations(victim, to, apCost);
            MindControlProcedures.AddCommandUsedToMindControl(me, victim);

            string attackerMessage = "You commanded " + victim.GetFullName() + " to move to " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == to).Name + ", using " + apCost + " of their action points in the process.";
            PlayerLogProcedures.AddPlayerLog(me.Id, attackerMessage, false);
            TempData["Result"] = attackerMessage;

           string victimMessage = me.GetFullName() + " commanded you to move to " + LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == to).Name + ", using " + apCost + " of your action points in the process!";
           PlayerLogProcedures.AddPlayerLog(victim.Id, victimMessage, true);

            return RedirectToAction("MindControlList");
        }
	}
}
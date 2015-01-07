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

        // This controller should handle all interactions with NPC characters, ie Lindella, Wuf, and Jewdewfae, and any others


        [InitializeSimpleMembership]
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

            // show the permanent and consumable items the player is carrying
            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(me.Id).Where(i => i.Item.ItemType != PvPStatics.ItemType_Pet && i.dbItem.IsEquipped == false && (i.dbItem.IsPermanent == true || i.Item.ItemType == "consumable"));
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

            // assert that the item is either permanent or consumable
            if (itemBeingSold.dbItem.IsPermanent == false && itemBeingSold.Item.ItemType != "consumable")
            {
                TempData["Error"] = "Unfortunately Lindella will not purchase items that may later struggle free anymore.";
                return RedirectToAction("Play");
            }

            ItemProcedures.GiveItemToPlayer_Nocheck(itemBeingSold.dbItem.Id, merchant.Id);
            decimal cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);


            TempData["Result"] = "You sold your " + itemBeingSold.Item.FriendlyName + " to Lindella for " + (int)cost + " Arpeyjis.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }
	}
}
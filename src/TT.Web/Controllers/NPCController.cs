using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    public class NPCController : Controller
    {

        // This controller should handle all interactions with NPC characters, ie Lindella, Wuffie, and Jewdewfae, and any others

        public const int MovementControlLimit = 2;

        [Authorize]
        public ActionResult TradeWithMerchant(string filter)
        {

            if (filter.IsNullOrEmpty())
            {
                filter = "clothes";
            }

            // assert that player is logged in
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            ViewBag.MyMoney = Math.Floor(me.Money);


            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can purchase or sell anything to Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can purchase or sell anything to Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-3);

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
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can purchase or sell anything to Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a duel
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can purchase or sell anything to Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-3);

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
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetProfit, -(float)cost)
             ).Start();

            new Thread(() =>
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetLoss, (float)cost)
            ).Start();

            ItemProcedures.GiveItemToPlayer_Nocheck(purchased.dbItem.Id, me.Id);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me);

            TempData["Result"] = "You have purchased a " + purchased.Item.FriendlyName + " from Lindella.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }

        [Authorize]
        public ActionResult SellList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-3);

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
            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(me.Id).Where(i => i.Item.ItemType != PvPStatics.ItemType_Pet && !i.dbItem.IsEquipped && (i.dbItem.IsPermanent || i.Item.ItemType == "consumable"));
            return View(output);
        }

        [Authorize]
        public ActionResult Sell(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Lindella.";
                return RedirectToAction("Play", "PvP");
            }


            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can purchase or sell anything to Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a duel
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can purchase or sell anything to Lindella.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-3);

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
            if (!itemBeingSold.dbItem.IsPermanent && itemBeingSold.Item.ItemType != "consumable")
            {
                TempData["Error"] = "Unfortunately Lindella will not purchase items that may later struggle free anymore.";
                return RedirectToAction("Play", "PvP");
            }

            ItemProcedures.GiveItemToPlayer_Nocheck(itemBeingSold.dbItem.Id, merchant.Id);
            decimal cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetProfit, (float)cost)
             ).Start();

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LindellaNetLoss, -(float)cost)
             ).Start();


            TempData["Result"] = "You sold your " + itemBeingSold.Item.FriendlyName + " to Lindella for " + (int)cost + " Arpeyjis.";
            return RedirectToAction("TradeWithMerchant", new { filter = PvPStatics.ItemType_Shirt });
        }

        [Authorize]
        public ActionResult TradeWithPetMerchant(int offset = 0)
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-10);

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

            

            ViewBag.Wuffie = true;
            ViewBag.DisableReleaseLink = true;

            IEnumerable<ItemViewModel> pets = ItemProcedures.GetAllPlayerItems(merchant.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Pet);

            WuffieTradeViewModel output = new WuffieTradeViewModel
            {
                Paginator = new Paginator(pets.Count(), PvPStatics.PaginationPageSize),
                Money = (int)Math.Floor(me.Money),
            };
            output.Paginator.CurrentPage = offset + 1;
            output.Pets = pets.Skip(output.Paginator.GetSkipCount()).Take(output.Paginator.PageSize);

            int petAmount = ItemProcedures.PlayerIsWearingNumberOfThisType(me.Id, PvPStatics.ItemType_Pet);

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

            return View(output);
        }

        [Authorize]
        public ActionResult PurchasePet(int id)
        {
            // assert that player is logged in
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can interact with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can interact with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-10);

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
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetProfit, (float)-cost)
             ).Start();

            new Thread(() =>
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetLoss, (float)cost)
            ).Start();

            ItemProcedures.GiveItemToPlayer_Nocheck(purchased.dbItem.Id, me.Id);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(me);




            TempData["Result"] = "You have purchased a " + purchased.Item.FriendlyName + " from Wüffie.";
            return RedirectToAction("TradeWithPetMerchant");
        }

        [Authorize]
        public ActionResult SellPetList()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-10);

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
            IEnumerable<ItemViewModel> output = ItemProcedures.GetAllPlayerItems(me.Id).Where(i => i.Item.ItemType == PvPStatics.ItemType_Pet && i.dbItem.IsEquipped && i.dbItem.IsPermanent);
            return View(output);
        }


        [Authorize]
        public ActionResult SellPet(int id)
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to trade with Wüffie.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must conclude your duel before you can interact with Wuffie.";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must conclude your quest before you can interact with Wuffie.";
                return RedirectToAction("Play", "PvP");
            }

            Player merchant = PlayerProcedures.GetPlayerFromBotId(-10);

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
            if (!itemBeingSold.dbItem.IsPermanent)
            {
                TempData["Error"] = "Unfortunately Wüffie will not purchase pets that may later struggle free anymore.";
                return RedirectToAction("Play", "PvP");
            }

            ItemProcedures.GiveItemToPlayer_Nocheck(itemBeingSold.dbItem.Id, merchant.Id);
            decimal cost = ItemProcedures.GetCostOfItem(itemBeingSold, "sell");
            PlayerProcedures.GiveMoneyToPlayer(me, cost);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetProfit, (float)cost)
             ).Start();

            new Thread(() =>
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieNetLoss, (float)-cost)
            ).Start();

            TempData["Result"] = "You sold your " + itemBeingSold.Item.FriendlyName + " to Wüffie for " + (int)cost + " Arpeyjis.";
            return RedirectToAction("TradeWithPetMerchant");
        }

        [Authorize]
        public ActionResult MindControlList()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Movement);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            ViewBag.Victim = victim;
            ViewBag.Buffs = ItemProcedures.GetPlayerBuffs(victim);

            if (victim.IsInDungeon())
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Strip);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            List<ItemViewModel> victimItems = ItemProcedures.GetAllPlayerItems(victim.Id).ToList();

            if (victimItems.Any())
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Meditate);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            BuffBox buffs = ItemProcedures.GetPlayerBuffs(victim);
            string result = PlayerProcedures.DeMeditate(victim, me, buffs);

            MindControlProcedures.AddCommandUsedToMindControl(me, victim, MindControlStatics.MindControl__Meditate);


            TempData["Result"] = "You force " + victim.GetFullName() + " to meditate while filling their mind with nonsense instead of relaxation, lowering their mana instead of increasing it!";
            return RedirectToAction("Play", "PvP");
        }

        [Authorize]
        public ActionResult MoveVictimSend(int id, string to)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player victim = PlayerProcedures.GetPlayer(id);

            // run generic MC checks
            ErrorBox errorsBox = MindControlProcedures.AssertBasicMindControlConditions(me, victim, MindControlStatics.MindControl__Movement);
            if (errorsBox.HasError)
            {
                TempData["Error"] = errorsBox.Error;
                TempData["SubError"] = errorsBox.SubError;
                return RedirectToAction("MindControlList");
            }

            BuffBox victimBuffs = ItemProcedures.GetPlayerBuffs(victim);
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
                return RedirectToAction("Play", "PvP");
            }

            // assert that the victim is not carrying too much to move
            if (ItemProcedures.PlayerIsCarryingTooMuch(victim.Id, 0, victimBuffs))
            {
                TempData["Error"] = "Your victim is carrying too much to be able to move.";
                return RedirectToAction("Play", "PvP");
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
                return RedirectToAction("Play", "PvP");
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
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player bartender = PlayerProcedures.GetPlayerFromBotId(AIStatics.BartenderBotId);

            // update timestamp (so that he can heal naturally)
            PlayerProcedures.SetTimestampToNow(bartender);

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

                if (me.Gender == PvPStatics.GenderMale) {
                    ViewBag.Speech = "\"Greetings, sir " + me.GetFullName() + "!  How may I assist you today?\"";
                } else if (me.Gender == PvPStatics.GenderFemale) {
                    ViewBag.Speech = "\"Greetings, madam " + me.GetFullName() + "!  How may I assist you today?\"";
                }
                
            }
            else if (question == "lindella")
            {
                Player lindella = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
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
                Player wuffie = PlayerProcedures.GetPlayerFromBotId(AIStatics.WuffieBotId);
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
                Player jewdewfae = PlayerProcedures.GetPlayerFromBotId(AIStatics.JewdewfaeBotId);
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
            else if (question == "lorekeeper")
            {
                Player lorekeeper = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);
                if (lorekeeper != null)
                {
                    Location temp = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == lorekeeper.dbLocationName);
                    ViewBag.Speech = "\"The last I have heard, " + lorekeeper.GetFullName() + " is at <b>" + temp.Name + "</b>.  Poor chap, how far he's fallen from his glory days.  Why don't you go and talk to him?  He'll be willing to teach you a spell or two or sell you some books if you're looking to increase your knowledge.";
                }
                else
                {
                    ViewBag.Speech = "Unfortunately I do not have any information on " + lorekeeper.GetFullName() + " at this time.  Is there anything else I can assist you with?";
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
                if (stats.Boss_Donna == "active")
                {
                    output += "\"There is a powerful sorceress about the town, a relative of the witchling Circine Milton of whom you may have seen about town.  She won't attack you unprovoked, but know that she holds a grudge, and once she has you in her sights... well, let's just say don't be surprised to join the livestock down at the Milton Ranch.  Unless she's on the hunt for someone who has tried to fight her, you can find her in her bedroom at the Milton Ranch.  Be careful however, when Donna is near defeat, her magic amplifies and she will attack anyone and everyone in sight, regardless of how innocent they are!\"<br><br>";
                }
                if (stats.Boss_Bimbo == "active")
                {
                    output += "\"There is a woman named Lady Lovebringer, a renowned scientist who has recently arrived at the town.  I have heard she carries a powerful virus that can transform anyone who catches it into voluptuous women who spread the virus even further.  Those who have been infected will shortly transform into a bimbo and may find themselves attacking any other bystanders against their own will.  Luckily the Center for Transformation Control and Prevention are airdropping cures which will make you immune from the virus for a while, so keep an eye open for them lying around on the ground!\"<br><br>";
                }
                if (stats.Boss_Valentine == "active")
                {
                    output += "\"A powerful vampire lord named Valentine is in town, letting people try to test their skills against him.  He won't leave the castle at all, so you can safely disengage him as you please; he holds no grudges.  However, it's best not to be standing near him for too long; he seeks to turn vampires out of the population here, and if he grows tired of you as a vampire he will try to turn you into a sleek sword for his personal collection instead!\"<br><br>";

                }
                if (stats.Boss_Sisters == "active")
                {
                    output += "\"A pair of feuding sisters is about town trying to turn the other into a form, physically and mentally, into something more desirable.  Perhaps you can take a side and help the issue to be resolved... after all, nobody likes it when family fights.\"<br><br>";
                }
                if (stats.Boss_Faeboss == "active")
                {
                    output += "\"A disgruntled fae from the Winston grove is going about the town, transforming those she meets into what she believes to be more suitable forms.  If you don't play along, she may well try to turn you into a flower or keep you as her own personal pet.  I've been told the only way to fight is to capture her and imprison her in a jar.  Perhaps questing in the back of Words of Wisdom will help you.  Best of luck, friend!\"<br><br>";
                }
                if (output.IsNullOrEmpty())
                {
                    output += "\"I do not know of anything strange going about Sunnyglade right now.  Well, stranger than usual, anyway.  Is there anything else I can assist you with?\"";
                }
                ViewBag.Speech = output;

            }
            else if (question == "quests")
            {
                string output = "\"Looking for an adventure, eh?  You may want to go take a look at these locations...\"<br><br>";
                IEnumerable<QuestStart> quests = QuestProcedures.GetAllAvailableQuestsForPlayer(me, PvPWorldStatProcedures.GetWorldTurnNumber());

                foreach (QuestStart q in quests)
                {
                    var Loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == q.Location);

                    // just in case a location is misnamed, skip over it
                    if (Loc != null)
                    {
                        output += "\"<b>" +  q.Name + "</b> is available for you at <b>" + Loc.Name + "</b>.\"<br><br>";
                    }
                }

                if (!quests.Any())
                {
                    output += "\"Oh, it seems there's nothing available for you right now.\"<br><br>";
                }

                output += "\"Is there anything else I can assist you with?\"";

                ViewBag.Speech = output;

            }

            return View();
        }

        [Authorize]
        public ActionResult TalkWithJewdewfae()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            Player fae = PlayerProcedures.GetPlayerFromBotId(-6);

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

            JewdewfaeEncounter output = TT.Domain.Procedures.BossProcedures.BossProcedures_Jewdewfae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

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
            if (TT.Domain.Procedures.BossProcedures.BossProcedures_Jewdewfae.PlayerHasHadRecentInteraction(me, fae))
            {
                ViewBag.HadRecentInteraction = true;
            }

            return View(output);
        }

        [Authorize]
        public ActionResult PlayWithJewdewfae()
        {

            Player me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            Player fae = PlayerProcedures.GetPlayerFromBotId(-6);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be fully animate in order to talk with Jewdewfae.";
                return RedirectToAction("Play","PvP");
            }

            // assert that this player is not in a duel
            if (me.InDuel > 0)
            {
                TempData["Error"] = "You must finish your duel before you can play with Jewdewfae.";
                return RedirectToAction("Play", "PvP");
            }

            // assert that this player is not in a quest
            if (me.InQuest > 0)
            {
                TempData["Error"] = "You must finish your quest before you can play with Jewdewfae.";
                return RedirectToAction("Play", "PvP");
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
            if (TT.Domain.Procedures.BossProcedures.BossProcedures_Jewdewfae.PlayerHasHadRecentInteraction(me, fae))
            {
                TempData["Error"] = "You have already interacted with Jewdewfae here.";
                TempData["SubError"] = "Wait for her to move somewhere else.";
                return RedirectToAction("Play", "PvP");
            }

            JewdewfaeEncounter output = TT.Domain.Procedures.BossProcedures.BossProcedures_Jewdewfae.GetFairyChallengeInfoAtLocation(fae.dbLocationName);

            output.IntroText = output.IntroText.Replace("[", "<").Replace("]", ">");
            output.CorrectFormText = output.CorrectFormText.Replace("[", "<").Replace("]", ">");
            output.FailureText = output.FailureText.Replace("[", "<").Replace("]", ">");

            if (me.Form == output.RequiredForm)
            {
                decimal xpGained = TT.Domain.Procedures.BossProcedures.BossProcedures_Jewdewfae.AddInteraction(me);
                PlayerProcedures.GiveXP(me, xpGained);
                PlayerProcedures.ChangePlayerActionMana(5, 0, 0, me.Id);
                ViewBag.XPGain = xpGained;
                ViewBag.ShowSuccess = true;
                ViewBag.HadRecentInteraction = false;

                string spellsLearned = "Jewdewfae's magic also teaches you the following spells:  " + SkillProcedures.GiveRandomFindableSkillsToPlayer(me, 3);
                ViewBag.SpellsLearned = spellsLearned;

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

        [Authorize]
        public ActionResult TalkToCandice()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player bimbo = PlayerProcedures.GetPlayerFromBotId(AIStatics.MouseBimboBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + BossProcedures_Sisters.BimboBossFirstName + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as Candice
            if (me.dbLocationName != bimbo.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + BossProcedures_Sisters.BimboBossFirstName + " in order to talk with her.";
                return RedirectToAction("Play", "PvP");
            }

            // assert bimbo is still in base form
            if (bimbo.Form != BossProcedures_Sisters.BimboBossForm)
            {
                TempData["Error"] = BossProcedures_Sisters.BimboBossFirstName + " seems to be too distracted with her recent change to want to talk to you.";
                return RedirectToAction("Play", "PvP");
            }

            return View();

        }

        [Authorize]
        public ActionResult TalkToAdrianna()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player nerd = PlayerProcedures.GetPlayerFromBotId(AIStatics.MouseNerdBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + BossProcedures_Sisters.NerdBossFirstName + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as Candice
            if (me.dbLocationName != nerd.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + BossProcedures_Sisters.NerdBossFirstName + " in order to talk with her.";
                return RedirectToAction("Play", "PvP");
            }

            // assert nerd is still in base form
            if (nerd.Form != BossProcedures_Sisters.NerdBossForm)
            {
                TempData["Error"] = BossProcedures_Sisters.NerdBossFirstName + " seems to be too distracted with her recent change to want to talk to you.";
                  return RedirectToAction("Play", "PvP");
            }

            return View();

        }

        [Authorize]
        public ActionResult TalkToLorekeeper()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + loremaster.GetFullName() + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as loremaster
            if (me.dbLocationName != loremaster.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + loremaster.GetFullName() +" in order to talk with him.";
                return RedirectToAction("Play", "PvP");
            }

            // transfer all of the books Lindella owns over to Lorekeeper
            BossProcedures_Loremaster.TransferBooksFromLindellaToLorekeeper(loremaster);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View();

        }

        [Authorize]
        public ActionResult LorekeeperPurchaseBook()
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + loremaster.GetFullName() + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as loremaster
            if (me.dbLocationName != loremaster.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + loremaster.GetFullName() + " in order to talk with him.";
                return RedirectToAction("Play", "PvP");
            }

            IEnumerable<ItemViewModel> inventory = ItemProcedures.GetAllPlayerItems(loremaster.Id);

            ViewBag.MyMoney = Math.Floor(me.Money);
            ViewBag.Lorekeeper = true;

            return View(inventory);

        }

        [Authorize]
        public ActionResult LorekeeperPurchaseBookSend(int id)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + loremaster.GetFullName() + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as loremaster
            if (me.dbLocationName != loremaster.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + loremaster.GetFullName() + " in order to talk with him.";
                return RedirectToAction("Play", "PvP");
            }

            // assert lorekeeper owns this book
            ItemViewModel purchased = ItemProcedures.GetItemViewModel(id);
            if (purchased.dbItem.OwnerId != loremaster.Id)
            {
                TempData["Error"] = "You can't purchse this as " + loremaster.GetFullName() + " does not own it.";
                return RedirectToAction("TalkToLorekeeper", "NPC");
            }


            decimal cost = ItemProcedures.GetCostOfItem(purchased, "buy");

            // assert that the player has enough money for this
            if (me.Money < cost)
            {
                TempData["Error"] = "You can't afford this right now.";
                TempData["SubError"] = "Try finding some more Arpeyjis from searching or take them off of players who you have turned inanimate or an animal.";
                return RedirectToAction("TalkToLorekeeper", "NPC");
            }

            // checks have passed.  Transfer the item
            PlayerProcedures.GiveMoneyToPlayer(me, -cost);

            //new Thread(() =>
            //     StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__WuffieCostsAmount, (float)cost)
            // ).Start();

            ItemProcedures.GiveItemToPlayer_Nocheck(purchased.dbItem.Id, me.Id);

            TempData["Result"] = "You purchased " + purchased.Item.FriendlyName + " from " + loremaster.GetFullName() + " for " + cost + " Arpeyjis.";
            return RedirectToAction("TalkToLorekeeper", "NPC");

        }

        [Authorize]
        public ActionResult LorekeeperLearnSpell(string filter)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + loremaster.GetFullName() + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as loremaster
            if (me.dbLocationName != loremaster.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + loremaster.GetFullName() + " in order to talk with him.";
                return RedirectToAction("Play", "PvP");
            }

            IEnumerable<string> knownSkillsStrings = SkillProcedures.GetSkillViewModelsOwnedByPlayer(me.Id).Select(s => s.Skill).Select(s => s.dbName);

            IEnumerable<DbStaticSkill> allSkills = SkillProcedures.GetAllLearnableSpells();

            // filter based on mobility type
            if (filter.IsNullOrEmpty() || filter == "animate")
            {
                allSkills = allSkills.Where(s => s.MobilityType == "full");
            }
            else if (filter == "inanimate")
            {
                allSkills = allSkills.Where(s => s.MobilityType == "inanimate");
            }
            else if (filter == "animal")
            {
                allSkills = allSkills.Where(s => s.MobilityType == "animal");
            }
            else if (filter == "other")
            {
                allSkills = allSkills.Where(s => s.MobilityType == "curse" || s.MobilityType == "mindcontrol");
            } 

            List<DbStaticSkill> output = new List<DbStaticSkill>();


            // TODO:  this can probably done through LINQ or a better SQL query
            foreach (DbStaticSkill s in allSkills)
            {
                if (!knownSkillsStrings.Contains(s.dbName))
                {
                    output.Add(s);
                }
            }

            ViewBag.Money = Math.Floor(me.Money);

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            return View(output);

        }

        [Authorize]
        public ActionResult LorekeeperLearnSpellSend(string spell)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player loremaster = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

            // assert player is animate
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to learn spells from " + loremaster.GetFullName() + ".";
                    return RedirectToAction("TalkToLorekeeper", "NPC");
            }

            // assert player is in the same place as loremaster
            if (me.dbLocationName != loremaster.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + loremaster.GetFullName() + " in order to talk with him.";
                     return RedirectToAction("TalkToLorekeeper", "NPC");
            }

            // assert player has enough money to buy a spell
            if (me.Money < PvPStatics.LorekeeperSpellPrice)
            {
                TempData["Error"] = "You don't have enough Arpeyjis to pay " + loremaster.GetFullName() + " to teach you any spells right now.";
                TempData["SubError"] = "You need " + PvPStatics.LorekeeperSpellPrice + " Arpeyjs to be taught a spell.";
                     return RedirectToAction("TalkToLorekeeper", "NPC");
            }

            // assert player does not already have that spell
            SkillViewModel spellViewModel = SkillProcedures.GetSkillViewModel_NotOwned(spell);

            IEnumerable<Skill> playerExistingSpells = SkillProcedures.GetSkillsOwnedByPlayer(me.Id);

            if (playerExistingSpells.Select(s => s.Name).Contains(spell))
            {
                TempData["Error"] = "You already know that spell.";
                     return RedirectToAction("TalkToLorekeeper", "NPC");
            }

            // assert spells is learnable
            if ((spellViewModel.Skill.LearnedAtLocation.IsNullOrEmpty() && spellViewModel.Skill.LearnedAtRegion.IsNullOrEmpty()) || !spellViewModel.Skill.IsPlayerLearnable)
            {
                TempData["Error"] = "You cannot learn that spell.";
                     return RedirectToAction("TalkToLorekeeper", "NPC");
            }

            // all checks passed; give the player the spell
            SkillProcedures.GiveSkillToPlayer(me.Id, spellViewModel.Skill.dbName);
            PlayerProcedures.GiveMoneyToPlayer(me, -PvPStatics.LorekeeperSpellPrice);

            new Thread(() =>
                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LorekeeperSpellsLearned,1 )
            ).Start();

            TempData["Result"] = loremaster.GetFullName() + " taught you " + spellViewModel.Skill.FriendlyName + " for " + PvPStatics.LorekeeperSpellPrice + " Arpeyjis.";
            return RedirectToAction("LorekeeperLearnSpell", "NPC");

        }

        [Authorize]
        public ActionResult TalkToValentine(string question)
        {
            string myMembershipId = User.Identity.GetUserId();
            Player me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            Player valentine = PlayerProcedures.GetPlayerFromBotId(AIStatics.ValentineBotId);

            // assert player is mobile
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You must be animate in order to chat with " + valentine.GetFullName() + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is not dueling
            if (me.Mobility != "full")
            {
                TempData["Error"] = "You should conclude your current duel before talking to " + valentine.GetFullName() + ".";
                return RedirectToAction("Play", "PvP");
            }

            // assert player is in the same place as Valenti8ne
            if (me.dbLocationName != valentine.dbLocationName)
            {
                TempData["Error"] = "You must be in the same location as " + valentine.GetFullName() + " in order to talk with him.";
                return RedirectToAction("Play", "PvP");
            }

            string responseText = "";

            // assert that the question is valid depending on Valentine's stance
            string stance = BossProcedures_Valentine.GetStance();
            if (question != "none")
            {
                BossProcedures_Valentine.TalkToAndCastSpell(me, valentine);
                List<PlayerLog> tftext = PlayerLogProcedures.GetAllPlayerLogs(me.Id).Reverse().ToList();
                PlayerLog lastlog = tftext.FirstOrDefault(f => f.IsImportant);

                if (lastlog != null)
                {
                    // if the log is too old, presumably the player already got transformed, so don't show that text again.  Yeah, this is so hacky...
                    double secDiff = Math.Abs(Math.Floor(lastlog.Timestamp.Subtract(DateTime.UtcNow).TotalSeconds));
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

            return View("TalkToValentine");

        }
    }
}
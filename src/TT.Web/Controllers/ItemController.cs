using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TT.Domain;
using TT.Domain.Exceptions;
using TT.Domain.Items.Commands;
using TT.Domain.Items.Queries;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Skills.Queries;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Web.Controllers
{
    [Authorize]
    public partial class ItemController : Controller
    {

        public virtual ActionResult MyInventory()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            if (me.MembershipId == myMembershipId)
            {
                ViewBag.BelongsToPlayer = "block";
            }
            else
            {
                ViewBag.BelongsToPlayer = "none";
            }


            var output = new InventoryBonusesViewModel
            {
                Items = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id }).Where(i => i.EmbeddedOnItem == null),
                Bonuses = ItemProcedures.GetPlayerBuffs(me),
                Health = me.Health,
                MaxHealth = me.MaxHealth,
                Mana = me.Mana,
                MaxMana = me.MaxMana,
                CurrentCarryCount = DomainRegistry.Repository.FindSingle( new GetCurrentCarryWeight { PlayerId = me.Id})
            };

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            ViewBag.ShowDetailLinks = true;
            ViewBag.ItemsUsedThisTurn = me.ItemsUsedThisTurn;


            return View(MVC.PvP.Views.Inventory, output);
        }

        public virtual ActionResult SelfCast()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player owns at least one of the type of item needed
            var itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.dbName == "item_consumable_selfcaster");
            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

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

            var output = DomainRegistry.Repository.Find(new GetSkillsOwnedByPlayer { playerId = me.Id }).Where(s => s.SkillSource.MobilityType == PvPStatics.MobilityFull);

            return View(MVC.Item.Views.SelfCast, output);
        }

        public virtual ActionResult SelfCastSend(string spell)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction(MVC.PvP.Play());
            }

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

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn > 0)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            // assert player owns at least one of the type of item needed
            var itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.dbName == "item_consumable_selfcaster");
            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player does own this skill
            var skill = SkillProcedures.GetSkillViewModel(spell, me.Id);
            if (skill == null)
            {
                TempData["Error"] = "You do not own this spell.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert desired form is animate
            if (skill.MobilityType != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "The target form must be an animate form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already in the form of the spell
            if (me.Form == skill.Skill.FormdbName)
            {
                TempData["Error"] = "You are already in the target form of that spell, so doing this would do you no good.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.InstantChangeToForm(me, skill.Skill.FormdbName);
            ItemProcedures.DeleteItemOfName(me, itemToUse.dbItem.dbName);

            PlayerProcedures.SetTimestampToNow(me);
            PlayerProcedures.AddItemUses(me.Id, 1);

            var form = FormStatics.GetForm(skill.Skill.FormdbName);
            TempData["Result"] = "You use a " + itemToUse.Item.FriendlyName + ", your spell bouncing through the device for a second before getting flung back at you and hitting you square in the chest, instantly transforming you into a " + form.FriendlyName + "!";

            new Thread(() =>
                  StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TransmogsUsed, 1)
            ).Start();

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult RemoveCurse()
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player owns at least one of the type of item needed
            var itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.dbName == "item_consumable_curselifter" || i.dbItem.dbName == "item_Butt_Plug_Hanna");
            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IEnumerable<EffectViewModel2> output = EffectProcedures.GetPlayerEffects2(me.Id).Where(e => e.Effect.IsRemovable && e.dbEffect.Duration > 0).ToList();
            ViewBag.itemToUseId = itemToUse.dbItem.Id;

            return View(MVC.Item.Views.RemoveCurse, output);
        }

        public virtual ActionResult RemoveCurseSend(string curse, int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player owns this item
            var itemToUse = ItemProcedures.GetItemViewModel(id);
            if (itemToUse == null || itemToUse.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item can remove curses and is not any old item
            if (itemToUse.dbItem.dbName != "item_consumable_curselifter" && itemToUse.dbItem.dbName != "item_Butt_Plug_Hanna")
            {
                TempData["Error"] = "This item cannot remove curses.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var curseToRemove = EffectStatics.GetEffect(curse);

            // assert this curse is removable
            if (!curseToRemove.IsRemovable)
            {
                TempData["Error"] = "This curse is too strong to be lifted.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // back on your feet curse/buff -- just delete outright
            if (curseToRemove.dbName == EffectProcedures.BackOnYourFeetEffect)
            {
                EffectProcedures.RemovePerkFromPlayer(curseToRemove.dbName, me);
            }

            // regular curse; set duration to 0 but keep cooldown
            else
            {
                EffectProcedures.SetPerkDurationToZero(curseToRemove.dbName, me);
            }

            // if the item is a consumable type, delete it.  Otherwise reset its cooldown
            if (itemToUse.Item.ItemType == PvPStatics.ItemType_Consumable)
            {
                ItemProcedures.DeleteItem(itemToUse.dbItem.Id);
            }
            // else if (itemToUse.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable)
            else
            {
                ItemProcedures.ResetUseCooldown(itemToUse);
            }


            TempData["Result"] = "You have successfully removed the curse <b>" + curseToRemove.FriendlyName + "</b> from your body!";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ReadSkillBook(int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "This curse is too strong to be lifted.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var book = ItemProcedures.GetItemViewModel(id);

            // assert player owns this book
            if (book.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own this book.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // make sure that this is actually a book
            if (!book.dbItem.dbName.Contains("item_consumable_tome-"))
            {
                TempData["Error"] = "You can't read that item!";
                TempData["SubError"] = "It's not a book.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player hasn't already read this book
            if (ItemProcedures.PlayerHasReadBook(me, book.dbItem.dbName))
            {
                TempData["Error"] = "You have already absorbed the knowledge from this book and can learn nothing more from it.";
                TempData["SubError"] = "Perhaps a friend could use this tome more than you right now.";
                return RedirectToAction(MVC.PvP.Play());
            }

            ItemProcedures.DeleteItem(book.dbItem.Id);
            ItemProcedures.AddBookReading(me, book.dbItem.dbName);
            PlayerProcedures.GiveXP(me, 35);

            new Thread(() =>
                 StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LoreBooksRead, 1)
             ).Start();

            TempData["Result"] = "You read your copy of " + book.Item.FriendlyName + ", absorbing its knowledge for 35 XP.  The tome slips into thin air so it can provide its knowledge to another mage in a different time and place.";
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult ShowItemDetails(int id)
        {
            var output = DomainRegistry.Repository.FindSingle(new GetItem {ItemId = id});
            return PartialView(MVC.Item.Views.partial.ItemDetails, output);
        }

        public virtual ActionResult ShowStatsTable()
        {
            return View(MVC.Item.Views.ShowStatsTable);
        }

        public virtual ActionResult AttachRuneList(int runeId)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var output = new AttachRuneListViewModel();
            output.items = DomainRegistry.Repository.Find(new GetItemsThatCanGetRunes { OwnerId = me.Id });
            output.rune = DomainRegistry.Repository.FindSingle(new GetItemRune { ItemId = runeId});

            return View(MVC.Item.Views.AttachRuneList, output);
        }

        public virtual ActionResult AttachRune(int runeId, int itemId)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new EmbedRune { ItemId = itemId, PlayerId = me.Id, RuneId = runeId });
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult UnembedRunesList()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());
            var output = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = me.Id }).Where(i => i.Runes.Any());

            return View(MVC.Item.Views.UnembedRunesList, output);
        }

        public virtual ActionResult UnattachRune(int Id)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new UnembedRune { PlayerId = me.Id, ItemId = Id});
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.Item.MyInventory());
        }

        public virtual ActionResult UnembedRunes()
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new UnembedAllRunes { PlayerId = me.Id});
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.Item.MyInventory());
        }


    }
}
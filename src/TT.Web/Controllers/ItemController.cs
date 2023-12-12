using System;
using System.Collections.Generic;
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
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Legacy.Services;
using TT.Domain.Players.Queries;
using TT.Domain.Procedures;
using TT.Domain.Skills.Queries;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using TT.Web.ViewModels;

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
                CurrentCarryCount = DomainRegistry.Repository.FindSingle( new GetCurrentCarryWeight { PlayerId = me.Id}),
                MaxInventorySize = ItemProcedures.GetInventoryMaxSize(me)
            };

            ViewBag.ErrorMessage = TempData["Error"];
            ViewBag.SubErrorMessage = TempData["SubError"];
            ViewBag.Result = TempData["Result"];

            ViewBag.ShowDetailLinks = true;
            ViewBag.ItemsUsedThisTurn = me.ItemsUsedThisTurn;


            return View(MVC.PvP.Views.Inventory, output);
        }
        public virtual ActionResult SelfRewardCast(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var item = ItemProcedures.GetItemViewModel(itemId);

            //asert item has not been used.
            if (item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

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

            // assert this item is the proper tome
            if (item.dbItem.ItemSourceId != ItemStatics.TomeRewards)
            {
                TempData["Error"] = "You cannot change form with that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
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

            var formRepo = new EFDbStaticFormRepository();
            var rewardForms = formRepo.DbStaticForms.Where(f => f.FriendlyName.Contains("*")).ToArray();

            // Shuffle forms so players don't always pick the first one
            var rand = new Random();
            for (var backstop = rewardForms.Length; backstop > 1; backstop--)
            {
                var dest = backstop - 1;
                var src = rand.Next(0, backstop);
                var temp = rewardForms[dest];
                rewardForms[dest] = rewardForms[src];
                rewardForms[src] = temp;
            }

            var model = new SelfCastViewModel
            {
                ItemId = itemId,
                Forms = rewardForms
            };

            return View(MVC.Item.Views.SelfRewardCast, model);
        }

        public virtual ActionResult SelfRewardCastSend(int formId, int itemId)
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
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            var item = ItemProcedures.GetItemViewModel(itemId);

            //asert item has not been used.
            if (item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

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

            // assert this item is a transmog
            if (item.dbItem.ItemSourceId != ItemStatics.TomeRewards)
            {
                TempData["Error"] = "You cannot change form with that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var form = FormStatics.GetForm(formId);

            if (form == null)
            {
                TempData["Error"] = "That form does not exist.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (!form.FriendlyName.Contains("*"))
            {
                TempData["Error"] = "That is not a selectable reward form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert desired form is animate
            if (form.MobilityType != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "The target form must be an animate form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already in the form of the spell
            if (me.FormSourceId == form.Id)
            {
                TempData["Error"] = "You are already in the target form of that spell, so doing this would do you no good.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.InstantChangeToForm(me, formId);
            ItemProcedures.ResetUseCooldown(item);
            PlayerProcedures.SetTimestampToNow(me);
            PlayerProcedures.AddItemUses(me.Id, 1);
            
            TempData["Result"] = "You read your copy of '<b>" + item.Item.FriendlyName + "</b>', absorbing its knowledge to take on the form it calls '<b>" + form.FriendlyName + "</b>'.The tome slips into thin air so it can provide its knowledge to another mage in a different time and place.";

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult SelfCast(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var item = ItemProcedures.GetItemViewModel(itemId);

            //asert item has not been used.
            if (item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

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

            // assert this item is a defective transmog
            if (item.dbItem.ItemSourceId == ItemStatics.DefectiveTransmogItemSourceId)
            {
                var rand = new Random();

                var forms = JokeShopProcedures.InanimateForms();

                if (forms.IsEmpty())
                {
                    TempData["Error"] = "Huh. It looks like something went wrong with that one.";
                    ItemProcedures.ResetUseCooldown(item);
                    return RedirectToAction(MVC.PvP.Play());
                }

                var randForm = rand.Next(forms.Count());
                FormDetail form = forms.ElementAt(randForm);

                PlayerProcedures.InstantChangeToForm(me, form.FormSourceId);
                ItemProcedures.ResetUseCooldown(item);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                var target = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
                target.Mobility = PvPStatics.MobilityFull;
                playerRepo.SavePlayer(target);

                var mobileTarget = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
                mobileTarget.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(mobileTarget));
                playerRepo.SavePlayer(mobileTarget);

                CharacterPrankProcedures.GiveEffect(me, JokeShopProcedures.ROOT_EFFECT, 3);

                PlayerProcedures.SetTimestampToNow(me);
                PlayerProcedures.AddItemUses(me.Id, 1);

                StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TransmogsUsed, 1);

                TempData["Error"] = "Uh oh. Something seems off...";
                TempData["SubError"] = "You've been pranked! You're going to need some time to recover.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert this item is a transmog
            if (item.dbItem.ItemSourceId != ItemStatics.AutoTransmogItemSourceId)
            {
                TempData["Error"] = "You cannot change form with that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
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

            var model = new SelfCastViewModel
            {
                ItemId = itemId,
                Skills = DomainRegistry.Repository.Find(new GetSkillsOwnedByPlayer { playerId = me.Id }).Where(s => s.SkillSource.MobilityType == PvPStatics.MobilityFull)
            };

            return View(MVC.Item.Views.SelfCast, model);
        }

        public virtual ActionResult SelfCastSend(int itemId, int skillSourceId)
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
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            var item = ItemProcedures.GetItemViewModel(itemId);

            //asert item has not been used.
            if (item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

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

            // assert this item is a transmog
            if (item.dbItem.ItemSourceId != ItemStatics.AutoTransmogItemSourceId)
            {
                TempData["Error"] = "You cannot change form with that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player does own this skill
            var skill = SkillProcedures.GetSkillViewModel(skillSourceId, me.Id);
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
            if (me.FormSourceId == skill.StaticSkill.FormSourceId)
            {
                TempData["Error"] = "You are already in the target form of that spell, so doing this would do you no good.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.InstantChangeToForm(me, skill.StaticSkill.FormSourceId.Value);
            ItemProcedures.ResetUseCooldown(item);

            PlayerProcedures.SetTimestampToNow(me);
            PlayerProcedures.AddItemUses(me.Id, 1);

            var form = FormStatics.GetForm(skill.StaticSkill.FormSourceId.Value);
            TempData["Result"] = "You use a " + item.Item.FriendlyName + ", your spell bouncing through the device for a second before getting flung back at you and hitting you square in the chest, instantly transforming you into a " + form.FriendlyName + "!";

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__TransmogsUsed, 1);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult ItemCast(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var item = ItemProcedures.GetItemViewModel(itemId);
            var itemPlayer = PlayerProcedures.GetPlayer(item.dbItem.FormerPlayerId);

            //asert item has not been used.
            if (item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is a player
            if (itemPlayer.MembershipId == null)
            {
                TempData["Error"] = "You can only change items that still hold a soul.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is equipped
            if (!item.dbItem.IsEquipped)
            {
                TempData["Error"] = "You cannot change an item you do not have equipped.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is locked
            if (!item.dbItem.IsPermanent)
            {
                TempData["Error"] = "You cannot change an item that is not locked.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item consents to soulbinding
            if (!item.dbItem.ConsentsToSoulbinding)
            {
                TempData["Error"] = "You cannot change an item that has not consented to being soulbound.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is soulbound
            if (item.dbItem.SoulboundToPlayerId == null)
            {
                TempData["Error"] = "You cannot change an item that is not soulbound.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is a friend
            if (!FriendProcedures.MemberIsMyFriend(me.MembershipId, itemPlayer.MembershipId))
            {
                TempData["Error"] = "You should probably be on friendly terms with them first.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var model = new SelfCastViewModel
            {
                ItemId = itemId,
                Skills = DomainRegistry.Repository.Find(new GetSkillsOwnedByPlayer { playerId = me.Id }).Where(s => s.SkillSource.MobilityType == itemPlayer.Mobility),
                Item = item,
                ItemPlayer = itemPlayer,
                OwnerMoney = me.Money
            };

            return View(MVC.Item.Views.ItemCast, model);
        }

        public virtual ActionResult ItemCastSend(int itemId, int skillSourceId, int itemSourceId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var item = ItemProcedures.GetItemViewModel(itemId);
            var itemPlayer = PlayerProcedures.GetPlayer(item.dbItem.FormerPlayerId);

            //asert item has not been used.
            if (item.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player does own this
            if (item.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is a player
            if (itemPlayer.MembershipId == null)
            {
                TempData["Error"] = "You can only change items that still hold a soul.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is locked
            if (!item.dbItem.IsPermanent)
            {
                TempData["Error"] = "You cannot change an item that is not locked.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item consents to soulbinding
            if (!item.dbItem.ConsentsToSoulbinding)
            {
                TempData["Error"] = "You cannot change an item that has not consented to being soulbound.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is soulbound
            if (item.dbItem.SoulboundToPlayerId == null)
            {
                TempData["Error"] = "You cannot change an item that is not soulbound.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert item is a friend
            if (!FriendProcedures.MemberIsMyFriend(me.MembershipId, itemPlayer.MembershipId))
            {
                TempData["Error"] = "You should probably be on friendly terms with them first.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player does own this skill
            var skill = SkillProcedures.GetSkillViewModel(skillSourceId, me.Id);
            if (skill == null)
            {
                TempData["Error"] = "You do not own this spell.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert desired form mobility matches the player item's current mobility
            if (skill.MobilityType != itemPlayer.Mobility)
            {
                TempData["Error"] = "The target form must be the same as the item's form.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert form id matches skill
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            var getItem = itemRepo.DbStaticItems.FirstOrDefault(i => i.Id == itemSourceId);
            var formItem = ItemProcedures.GetFormFromItem(getItem);
            var form = FormStatics.GetForm(skill.StaticSkill.FormSourceId.Value);

            // assert form id matches skill
            if (formItem == null)
            {
                TempData["Error"] = "There seems to be something wrong with that form!";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert form names match
            if (formItem.FriendlyName != form.FriendlyName)
            {
                TempData["Error"] = "Those forms don't seem to match.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player is not already in the form of the spell
            if (itemPlayer.FormSourceId == skill.StaticSkill.FormSourceId)
            {
                TempData["Error"] = "Your item is already in the target form of that spell, so doing this would do you no good.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has enough ARP
            var cost = 300;
            if (me.Money < cost)
            {
                TempData["Error"] = "You don't have enough arpeyjis to pay for a reshape!";
                return RedirectToAction(MVC.PvP.Play());
            }

            DomainRegistry.Repository.Execute(new SoulbindChangeForm
            {
                PlayerId = itemPlayer.Id,
                FormSourceId = skill.StaticSkill.FormSourceId.Value,
                ItemId = itemId,
                ItemSource = itemSourceId,
                OwnerId = me.Id
            });

            var message = "You reshape your item, " + itemPlayer.FirstName + " " + itemPlayer.LastName + ", instantly transforming them into a " + form.FriendlyName + "!";
            TempData["Result"] = message;
            PlayerLogProcedures.AddPlayerLog(me.Id, message, true);
            PlayerLogProcedures.AddPlayerLog(itemPlayer.Id, "You can feel your form beginning to change as it is instantly transformed into a " + form.FriendlyName + "!", true);

            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult SetName(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);

            var output = new SetNicknameViewModel
            {
                OriginalFirstName = me.OriginalFirstName,
                OriginalLastName = me.OriginalLastName,
                HasSelfRenamed = me.HasSelfRenamed,
                ItemId = itemId,
            };

            return View(MVC.Item.Views.SetName, output);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SetNameSend(SetNicknameViewModel input)
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
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable type item next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            var fnamecheck = TrustStatics.NameIsReserved(input.OriginalFirstName);
            if (!fnamecheck.IsNullOrEmpty())
            {
                ModelState.AddModelError("OriginalFirstName", "You can't use the first name '" + input.OriginalFirstName + "'.  It is reserved or else not allowed.");
            }

            // assert that the last name is not reserved by the system
            var lnamecheck = TrustStatics.NameIsReserved(input.OriginalLastName);
            if (!lnamecheck.IsNullOrEmpty())
            {
                ModelState.AddModelError("OriginalLastName", "You can't use the last name '" + input.OriginalLastName + "'.  It is reserved or else not allowed.");
            }

            IReservedNameRepository resNameRepo = new EFReservedNameRepository();
            var resName = resNameRepo.ReservedNames.FirstOrDefault(r => r.FullName == input.OriginalFirstName + " " + input.OriginalLastName);

            if (resName != null && resName.MembershipId != me.MembershipId)
            {
                ModelState.AddModelError("", "This name has been reserved by a different player.  Choose another.");
            }

            IItemRepository itemRepo = new EFItemRepository();
            var itemPlus = ItemProcedures.GetItemViewModel(input.ItemId);

            //asert item has not been used.
            if (itemPlus.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player does own this
            if (itemPlus.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You don't own that item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that it is equipped
            if (!itemPlus.dbItem.IsEquipped)
            {
                TempData["Error"] = "You cannot use an item you do not have equipped.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert this item is a rejuvenative lotion
            if (itemPlus.dbItem.ItemSourceId != ItemStatics.OtherRestoreItemSourceId)
            {
                TempData["Error"] = "You cannot do that with this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            Random rand = new Random();
            var randomForm = Array.Empty<int>();

            if (!ModelState.IsValid)
            {
                return View(MVC.Item.Views.SetName, input);
            }

            if (input.Personalities == null)
            {
                TempData["Error"] = "Nothing seems to happen when you use the lotion.";
                TempData["SubError"] = "You must select a personality.";
                return RedirectToAction(MVC.PvP.Play());
            }

            switch (input.Personalities.ToString())
            {
                case "BIMBOS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.BIMBOS).ToArray();
                    break;
                case "CATS_AND_NEKOS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.CATS_AND_NEKOS).ToArray();
                    break;
                case "CHRISTMAS_FORMS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.CHRISTMAS_FORMS).ToArray();
                    break;
                case "DOGS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DOGS).ToArray();
                    break;
                case "DRONES":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DRONES).ToArray();
                    break;
                case "EASTER_FORMS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.EASTER_FORMS).ToArray();
                    break;
                case "FAIRIES":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.FAIRIES).ToArray();
                    break;
                case "GHOSTS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.GHOSTS).ToArray();
                    break;
                case "HALLOWEEN_FORMS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.HALLOWEEN_FORMS).ToArray();
                    break;
                case "MAIDS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MAIDS).ToArray();
                    break;
                case "MANA_FORMS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MANA_FORMS).ToArray();
                    break;
                case "MISCHIEVOUS_FORMS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MISCHIEVOUS_FORMS).ToArray();
                    break;
                case "RODENTS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.RODENTS).ToArray();
                    break;
                case "ROMANTIC_FORMS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.ROMANTIC_FORMS).ToArray();
                    break;
                case "SHEEP":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.SHEEP).ToArray();
                    break;
                case "STRIPPERS":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.STRIPPERS).ToArray();
                    break;
                case "THIEVES":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.THIEVES).ToArray();
                    break;
                case "TREES":
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.TREES).ToArray();
                    break;
                default:
                    randomForm = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.BIMBOS).ToArray();
                    break;
            }

            var formSourceId = randomForm[rand.Next(randomForm.Count())];

            if (PlayerProcedures.CheckHasSelfRenamed(me))
            {
                input.OriginalFirstName = NameService.GetRandomFirstName();
                input.OriginalLastName = NameService.GetRandomLastName();

                PlayerProcedures.SetOriginalNameAndBase(input.OriginalFirstName, input.OriginalLastName, myMembershipId, formSourceId);
                TFEnergyProcedures.CleanseTFEnergies(me, 25);
                PlayerLogProcedures.AddPlayerLog(me.Id, "You can feel your form beginning to change as it is instantly transformed by the " + itemPlus.Item.FriendlyName + "!", true);
                LocationLogProcedures.AddLocationLog(me.dbLocationName, me.FirstName + " " + me.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                
                ItemProcedures.ResetUseCooldown(itemPlus);

                TempData["Error"] = "Your mind wanders as you apply the lotion.";
                TempData["SubError"] = "You recall your name being " + input.OriginalFirstName + " " + input.OriginalLastName + ".";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (input.OriginalFirstName == null || input.OriginalLastName == null)
            {
                TempData["Error"] = "Nothing seems to happen when you use the lotion.";
                TempData["SubError"] = "Your name may not be blank.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (input.OriginalFirstName.Length < 2 || input.OriginalLastName.Length < 2)
            {
                TempData["Error"] = "Nothing seems to happen when you use the lotion.";
                TempData["SubError"] = "Names must be at least 2 characters.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if ((input.OriginalFirstName != null && input.OriginalFirstName.Length > 30) || (input.OriginalLastName != null && input.OriginalLastName.Length > 30))
            {
                TempData["Error"] = "Nothing seems to happen when you use the lotion.";
                TempData["SubError"] = "First or last names must be no longer than 30 characters.";
                return RedirectToAction(MVC.PvP.Play());
            }

            PlayerProcedures.SetOriginalNameAndBase(input.OriginalFirstName, input.OriginalLastName, myMembershipId, formSourceId);
            TFEnergyProcedures.CleanseTFEnergies(me, 25);
            PlayerLogProcedures.AddPlayerLog(me.Id, "You can feel your form beginning to change as it is instantly transformed by the " + itemPlus.Item.FriendlyName + "!", true);
            LocationLogProcedures.AddLocationLog(me.dbLocationName, me.FirstName + " " + me.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
            ItemProcedures.ResetUseCooldown(itemPlus);

            TempData["Result"] = "You seem to recall your name being " + input.OriginalFirstName + " " + input.OriginalLastName + ".";
            return RedirectToAction(MVC.PvP.Play());
        }

        public virtual ActionResult RemoveCurse(int itemId)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            // assert player owns at least one of the type of item needed
            var itemToUse = ItemProcedures.GetAllPlayerItems(me.Id).FirstOrDefault(i => i.dbItem.Id == itemId);

            //asert item has not been used.
            if (itemToUse.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (itemToUse == null)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            if (itemToUse.dbItem.ItemSourceId != ItemStatics.CurseLifterItemSourceId && itemToUse.dbItem.ItemSourceId != ItemStatics.ButtPlugItemSourceId)
            {
                TempData["Error"] = "This type of item cannot lift curses.";
                return RedirectToAction(MVC.PvP.Play());
            }

            IEnumerable<EffectViewModel2> effects = EffectProcedures.GetPlayerEffects2(me.Id).Where(e => e.Effect.IsRemovable && e.dbEffect.Duration > 0).ToList();
           
            RemoveCurseViewModel output = new RemoveCurseViewModel
            {
                Effects = effects,
                Item = itemToUse
            };

            return View(MVC.Item.Views.RemoveCurse, output);
        }

        public virtual ActionResult RemoveCurseSend(int curseEffectSourceId, int id)
        {
            var myMembershipId = User.Identity.GetUserId();
            var me = PlayerProcedures.GetPlayerFromMembership(myMembershipId);
            // assert player is animate
            if (me.Mobility != PvPStatics.MobilityFull)
            {
                TempData["Error"] = "You must be animate in order to use this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            var itemToUse = ItemProcedures.GetItemViewModel(id);

            //asert item has not been used.
            if (itemToUse.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player owns this item
            if (itemToUse == null || itemToUse.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own the item needed to do this.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert that the item can remove curses and is not any old item
            if (itemToUse.dbItem.ItemSourceId != ItemStatics.CurseLifterItemSourceId && itemToUse.dbItem.ItemSourceId != ItemStatics.ButtPlugItemSourceId)
            {
                TempData["Error"] = "This item cannot remove curses.";
                return RedirectToAction(MVC.PvP.Play());
            }

            var curseToRemove = EffectStatics.GetDbStaticEffect(curseEffectSourceId);

            // assert this curse is removable
            if (!curseToRemove.IsRemovable)
            {
                TempData["Error"] = "This curse is too strong to be lifted.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // back on your feet curse/buff -- just delete outright
            if (curseToRemove.Id == PvPStatics.Effect_BackOnYourFeetSourceId)
            {
                EffectProcedures.RemovePerkFromPlayer(curseToRemove.Id, me);
            }

            // regular curse; set duration to 0 but keep cooldown
            else
            {
                EffectProcedures.SetPerkDurationToZero(curseToRemove.Id, me);
            }

            // if the item is a consumable type, it will be deleted once its cooldown expires
            ItemProcedures.ResetUseCooldown(itemToUse);

            PlayerProcedures.AddItemUses(me.Id, 1);

            var result = $"You have successfully removed the curse <b>{curseToRemove.FriendlyName}</b> from your body!";
            TempData["Result"] = result;

            var playerMessage = itemToUse.Item.UsageMessage_Player;
            if (string.IsNullOrEmpty(playerMessage))
            {
                PlayerLogProcedures.AddPlayerLog(me.Id, result, false);
            }
            else
            {
                PlayerLogProcedures.AddPlayerLog(me.Id, $"{playerMessage}<br />{result}", itemToUse.dbItem.FormerPlayerId != null);
            }

            if (itemToUse.dbItem.ItemSourceId == ItemStatics.ButtPlugItemSourceId && itemToUse.dbItem.FormerPlayerId != null)
            {
                var itemMessage = itemToUse.Item.UsageMessage_Item;
                var context = $"Your owner just used you to remove the curse <b>{curseToRemove.FriendlyName}</b>! Doesn't that make you feel all warm and tingly?";
                itemMessage = string.IsNullOrEmpty(itemMessage) ? context : $"{itemMessage}<br />{context}";
                PlayerLogProcedures.AddPlayerLog((int)itemToUse.dbItem.FormerPlayerId, itemMessage, true);
            }

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

            // assert player has not already used an item this turn
            if (me.ItemsUsedThisTurn >= PvPStatics.MaxItemUsesPerUpdate)
            {
                TempData["Error"] = "You've already used an item this turn.";
                TempData["SubError"] = "You will be able to use another consumable next turn.";
                return RedirectToAction(MVC.Item.MyInventory());
            }

            var book = ItemProcedures.GetItemViewModel(id);

            //asert item has not been used.
            if (book.dbItem.TurnsUntilUse > 0)
            {
                TempData["Error"] = "You have already used this item.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player owns this book
            if (book.dbItem.OwnerId != me.Id)
            {
                TempData["Error"] = "You do not own this book.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // make sure that this is actually a book
            if (book.Item.ConsumableSubItemType != (int)ItemStatics.ConsumableSubItemTypes.Tome)
            {
                TempData["Error"] = "You can't read that item!";
                TempData["SubError"] = "It's not a book.";
                return RedirectToAction(MVC.PvP.Play());
            }

            // assert player hasn't already read this book
            if (ItemProcedures.PlayerHasReadBook(me, book.dbItem.ItemSourceId))
            {
                TempData["Error"] = "You have already absorbed the knowledge from this book and can learn nothing more from it.";
                TempData["SubError"] = "Perhaps a friend could use this tome more than you right now.";
                return RedirectToAction(MVC.PvP.Play());
            }

            ItemProcedures.ResetUseCooldown(book);
            ItemProcedures.AddBookReading(me, book.dbItem.ItemSourceId);
            PlayerProcedures.GiveXP(me, 35);
            PlayerProcedures.AddItemUses(me.Id, 1);

            StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__LoreBooksRead, 1);

            TempData["Result"] = "You read your copy of " + book.Item.FriendlyName + ", absorbing its knowledge for 35 XP.  The tome slips into thin air so it can provide its knowledge to another mage in a different time and place.";
            return RedirectToAction(MVC.PvP.Play());

        }

        public virtual ActionResult ShowItemDetails(int id)
        {
            var item = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = id });
            var skills = SkillStatics.GetItemSpecificSkills(item.ItemSource.Id);

            var output = new ItemDetailsModel { Item = item, Skills = skills };

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
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
                newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
                playerRepo.SavePlayer(newMe);
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.Item.MyInventory());
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
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
                newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
                playerRepo.SavePlayer(newMe);
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
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newMe = playerRepo.Players.FirstOrDefault(p => p.Id == me.Id);
                newMe.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMe));
                playerRepo.SavePlayer(newMe);
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction(MVC.Item.MyInventory());
        }

        public virtual ActionResult SetSoulbindingConsent(bool isConsenting)
        {
            var me = PlayerProcedures.GetPlayerFromMembership(User.Identity.GetUserId());

            try
            {
                TempData["Result"] = DomainRegistry.Repository.Execute(new SetSoulbindingConsent { PlayerId = me.Id, IsConsenting = isConsenting});
            }
            catch (DomainException e)
            {
                TempData["Error"] = e.Message;
            }

            IItemRepository itemRep = new EFItemRepository();
            var inanimateMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});

            if (inanimateMe.Owner != null)
            {
                var formRepo = new EFDbStaticFormRepository();
                var form = formRepo.DbStaticForms.FirstOrDefault(f => f.Id == me.FormSourceId);

                if (isConsenting)
                {
                    PlayerLogProcedures.AddPlayerLog(inanimateMe.Owner.Id, $"{me.GetFullName()}, your {form.FriendlyName}, has agreed to let you soulbind them!", true);
                }
                else
                {
                    PlayerLogProcedures.AddPlayerLog(inanimateMe.Owner.Id, $"{me.GetFullName()}, your {form.FriendlyName}, has withdrawn their soulbinding consent.", false);
                }
            }

            if (isConsenting)
            {
                PlayerLogProcedures.AddPlayerLog(me.Id, $"You have consented to soulbinding.", false);
            }
            else
            {
                PlayerLogProcedures.AddPlayerLog(me.Id, $"You have withdrawn soulbinding consent.", false);
            }

            return RedirectToAction(MVC.PvP.Play());
        }

    }
}
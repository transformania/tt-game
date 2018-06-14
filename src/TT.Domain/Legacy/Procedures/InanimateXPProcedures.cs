using System;
using System.Linq;
using System.Threading;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Statics;

namespace TT.Domain.Procedures
{
    public static class InanimateXPProcedures
    {

        public static decimal GetStruggleChance(Player player)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            decimal output = -6 * player.Level;
            var myItemXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

            if (myItemXP != null)
            {
                return myItemXP.TimesStruggled;
            }
            else
            {
                return output;
            }
        }

        public static string GiveInanimateXP(string membershipId, bool isWhitelist)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            IItemRepository itemRep = new EFItemRepository();

            // get the current level of this player based on what item they are
            var me = PlayerProcedures.GetPlayerFromMembership(membershipId);
            var inanimateMeHack = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = me.Id});
            var inanimateMe = itemRep.Items.FirstOrDefault(i => i.Id == inanimateMeHack.Id); // TODO: Replace with proper Command

            var currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

            decimal xpGain = 0;

            // get the number of inanimate accounts under this IP
            IPlayerRepository playerRepo = new EFPlayerRepository();
            decimal playerCount = playerRepo.Players.Count(p => p.IpAddress == me.IpAddress && (p.Mobility == PvPStatics.MobilityInanimate || p.Mobility == PvPStatics.MobilityPet) && p.BotId == AIStatics.ActivePlayerBotId);

            if (playerCount == 0 || isWhitelist)
            {
                playerCount = 1;
            }
            xpGain = xpGain / playerCount;

            var xp = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == me.Id);

            if (xp == null)
            {
                xp = new InanimateXP
                {
                    OwnerId = me.Id,
                    Amount = xpGain / playerCount,
                    TimesStruggled = -6 * me.Level,
                    LastActionTimestamp = DateTime.UtcNow,
                    LastActionTurnstamp = currentGameTurn - 1,
                };

                if (me.Mobility == PvPStatics.MobilityInanimate)
                {
                    new Thread(() =>
                        StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__InanimateXPEarned, (float)xpGain)
                    ).Start();
                }
                else if (me.Mobility == PvPStatics.MobilityPet)
                {
                    new Thread(() =>
                        StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__PetXPEarned, (float)xpGain)
                    ).Start();
                }


            }
            else
            {

                //double timeBonus = Math.Floor(Math.Abs(Math.Floor(xp.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalMinutes)) / 10);
                double timeBonus = currentGameTurn - xp.LastActionTurnstamp;

                if (timeBonus > InanimateXPStatics.ItemMaxTurnsBuildup)
                {
                    timeBonus = InanimateXPStatics.ItemMaxTurnsBuildup;
                }

                if (timeBonus < 0)
                {
                    timeBonus = 0;
                }



                xpGain += Convert.ToDecimal(timeBonus) * InanimateXPStatics.XPGainPerInanimateAction;


                xpGain = xpGain / playerCount;

                if (me.Mobility == PvPStatics.MobilityInanimate)
                {
                    new Thread(() =>
                        StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__InanimateXPEarned, (float)xpGain)
                    ).Start();
                }
                else if (me.Mobility == PvPStatics.MobilityPet)
                {
                    new Thread(() =>
                        StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__PetXPEarned, (float)xpGain)
                    ).Start();
                }

                xp.Amount += xpGain;
                xp.TimesStruggled -= 2 * Convert.ToInt32(timeBonus);
                xp.LastActionTimestamp = DateTime.UtcNow;
                xp.LastActionTurnstamp = currentGameTurn;
            }

            var resultMessage = "  ";



            if (xp.Amount >= Convert.ToDecimal(ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level)))
            {
                xp.Amount -= Convert.ToDecimal(ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level));
                inanimateMe.Level++;
                itemRep.SaveItem(inanimateMe);

                resultMessage += "  You have gained " + xpGain + " xp.  <b>Congratulations, you have gained a level!  Your owner will be so proud...</b>";

                var wearerMessage = "<span style='color: darkgreen'>" + me.FirstName + " " + me.LastName + ", currently your " + ItemStatics.GetStaticItem(inanimateMe.dbName).FriendlyName + ", has gained a level!  Treat them kindly and they might keep helping you out...</span>";


                // now we need to change the owner's max health or mana based on this leveling
                if (inanimateMe.OwnerId > 0)
                {
                    var inanimateMePlus = ItemProcedures.GetItemViewModel(inanimateMe.Id);

                    if (inanimateMePlus.Item.HealthBonusPercent != 0.0M || inanimateMePlus.Item.ManaBonusPercent != 0.0M)
                    {

                        var myowner = playerRepo.Players.FirstOrDefault(p => p.Id == inanimateMe.OwnerId);

                        var healthChange = PvPStatics.Item_LevelBonusModifier * inanimateMePlus.Item.HealthBonusPercent;
                        var manaChange = PvPStatics.Item_LevelBonusModifier * inanimateMePlus.Item.ManaBonusPercent;

                        myowner.MaxHealth += healthChange;
                        myowner.MaxMana += manaChange;

                        if (myowner.MaxHealth < 1)
                        {
                            myowner.MaxHealth = 1;
                        }

                        if (myowner.MaxMana < 1)
                        {
                            myowner.MaxMana = 1;
                        }

                        if (myowner.Health > myowner.MaxHealth)
                        {
                            myowner.Health = myowner.MaxHealth;
                        }

                        if (myowner.Mana > myowner.Mana)
                        {
                            myowner.Mana = myowner.Mana;
                        }

                        playerRepo.SavePlayer(myowner);

                    }

                }


                PlayerLogProcedures.AddPlayerLog((int)inanimateMe.OwnerId, wearerMessage, true);
            }
            else
            {
                resultMessage = "  You have gained " + xpGain + " xp.  (" + xp.Amount + "/" + ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level) + ") to next level.";
            }

            inanimXpRepo.SaveInanimateXP(xp);

            // lock the player into their fate if their inanimate XP gets too high
            if (xp.TimesStruggled <= -100 && xp.TimesStruggled > -160 && !inanimateMe.IsPermanent)
            {
                resultMessage += "  Careful, if you keep doing this you may find yourself stuck in your current form forever...";
            }

            if (xp.TimesStruggled <= -160 && !inanimateMe.IsPermanent)
            {
                inanimateMe.IsPermanent = true;
                itemRep.SaveItem(inanimateMe);
                resultMessage += "  <b>You find the last of your old human self slip away as you permanently embrace your new form.</b>";
            }

            return resultMessage;


        }

        public static string ReturnToAnimate(Player player, bool dungeonHalfPoints)
        {


            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            IItemRepository itemRepo = new EFItemRepository();

            var inanimXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

            var currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

            if (inanimXP == null)
            {
                inanimXP = new InanimateXP
                {
                    OwnerId = player.Id,
                    Amount = 0,

                    // set the initial times struggled proportional to how high of a level the player is
                    TimesStruggled = -6 * player.Level,
                    LastActionTimestamp = DateTime.UtcNow,
                    LastActionTurnstamp = currentGameTurn - 1,

                };
            }

            double strugglebonus = currentGameTurn - inanimXP.LastActionTurnstamp;

            if (strugglebonus > InanimateXPStatics.ItemMaxTurnsBuildup)
            {
                strugglebonus = InanimateXPStatics.ItemMaxTurnsBuildup;
            }

            if (strugglebonus < 0)
            {
                strugglebonus = 0;
            }

            if (PvPStatics.ChaosMode)
            {
                strugglebonus = 100;
            }

            // increment the player's attack count.  Also decrease their player XP some.
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate++;

            var strugglesMade = Convert.ToDouble(inanimXP.TimesStruggled);

            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            // if player is in dungeon, make struggling chance much lower
            if (dungeonHalfPoints)
            {
                roll = roll * 3;
            }

            var dbPlayerItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = player.Id});

            if (dbPlayerItem.Owner != null)
            {
                var owner = PlayerProcedures.GetPlayer(dbPlayerItem.Owner.Id);
                dbPlayer.dbLocationName = owner.dbLocationName;
            }

            var itemPlus = ItemStatics.GetStaticItem(dbPlayerItem.dbName);

            if (roll < strugglesMade)
            {

                // assert that the covenant the victim is in is not too full to accept them back in
                if (dbPlayer.Covenant > 0)
                {
                    var victimCov = CovenantProcedures.GetCovenantViewModel((int)dbPlayer.Covenant).dbCovenant;
                    if (victimCov != null && CovenantProcedures.GetPlayerCountInCovenant(victimCov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount)
                    {
                        return "Although you had enough energy to break free from your body as a " + itemPlus.FriendlyName + " and restore your regular body, you were unfortunately not able to break free because there is no more room in your covenant for any more animate mages.";
                    }
                }


                // if the item has an owner, notify them via a message.
                if (dbPlayerItem.Owner != null)
                {
                    var message = player.FirstName + " " + player.LastName + ", your " + itemPlus.FriendlyName + ", successfully struggles against your magic and reverses their transformation.  You can no longer claim them as your property, not unless you manage to turn them back again...";
                    PlayerLogProcedures.AddPlayerLog(dbPlayerItem.Owner.Id, message, true);
                }

                // change the player's form and mobility
                DomainRegistry.Repository.Execute(new ChangeForm
                {
                    PlayerId = dbPlayer.Id,
                    FormName = dbPlayer.OriginalForm
                });

                dbPlayer.ActionPoints = PvPStatics.MaximumStoreableActionPoints;
                dbPlayer.ActionPoints_Refill = PvPStatics.MaximumStoreableActionPoints_Refill;
                dbPlayer.CleansesMeditatesThisRound = PvPStatics.MaxCleansesMeditatesPerUpdate;
                dbPlayer.TimesAttackingThisUpdate = PvPStatics.MaxAttacksPerUpdate;

                // don't let the player spawn in the dungeon if they are not in PvP mode
                if (dbPlayer.GameMode < GameModeStatics.PvP && dbPlayer.IsInDungeon())
                {
                    dbPlayer.dbLocationName = LocationsStatics.GetRandomLocationNotInDungeon();
                }

                dbPlayer = PlayerProcedures.ReadjustMaxes(dbPlayer, ItemProcedures.GetPlayerBuffs(dbPlayer));
                dbPlayer.Health = dbPlayer.MaxHealth / 3;
                dbPlayer.Mana = dbPlayer.MaxHealth / 3;
                playerRepo.SavePlayer(dbPlayer);


                // drop any runes embedded on the player-item, or return them to the former owner's inventory
                DomainRegistry.Repository.Execute(new UnbembedRunesOnItem {ItemId = dbPlayerItem.Id});

                // delete the item or animal that this player had turned into
                itemRepo.DeleteItem(dbPlayerItem.Id);

                // delete the inanimate XP item
                inanimXpRepo.DeleteInanimateXP(inanimXP.Id);

                // give the player the recovery buff
                EffectProcedures.GivePerkToPlayer(PvPStatics.Effect_Back_On_Your_Feet, dbPlayer);

                var msg = "You have managed to break free from your form as " + itemPlus.FriendlyName + " and occupy an animate body once again!";

                if (PvPStatics.ChaosMode)
                {
                    msg += " [CHAOS MODE:  struggle value overriden to 5% per struggle.";
                }

                PlayerLogProcedures.AddPlayerLog(dbPlayer.Id, msg, false);

                new Thread(() =>
                        StatsProcedures.AddStat(dbPlayer.MembershipId, StatsProcedures.Stat__SuccessfulStruggles, 1)
                    ).Start();

                return msg;

            }

            // failure to break free; increase time struggles
            else
            {
                // raise the probability of success for next time somewhat proportion to how many turns they missed
                inanimXP.TimesStruggled += Convert.ToInt32(strugglebonus);
                inanimXP.LastActionTimestamp = DateTime.UtcNow;
                inanimXP.LastActionTurnstamp = currentGameTurn;
                inanimXpRepo.SaveInanimateXP(inanimXP);

                playerRepo.SavePlayer(dbPlayer);

                if (dbPlayerItem.Owner != null)
                {
                    var message = player.FirstName + " " + player.LastName + ", your " + itemPlus.FriendlyName + ", struggles but fails to return to an animate form.  [Recovery chance Recovery chance::  " + inanimXP.TimesStruggled + "%]";
                    PlayerLogProcedures.AddPlayerLog(dbPlayerItem.Owner.Id, message, true);
                }

                PlayerLogProcedures.AddPlayerLog(dbPlayer.Id, "You struggled to return to a human form.", false);

                return "Unfortunately you are not able to struggle free from your form as " + itemPlus.FriendlyName + ".  Keep trying and you might succeed later... [Recovery chance next struggle:  " + inanimXP.TimesStruggled + "%]";
            }
        }

        public static string CurseTransformOwner(Player player, Player owner, ItemDetail playerItem, bool isWhitelist)
        {
            var rand = new Random();
            var roll = rand.NextDouble() * 100;


            IInanimateXPRepository inanimateXpRepo = new EFInanimateXPRepository();
            var xp = inanimateXpRepo.InanimateXPs.FirstOrDefault(x => x.OwnerId == player.Id);
            var gameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

            // assign the player inanimate XP based on turn building
            if (xp == null)
            {
                xp = new InanimateXP
                {
                    OwnerId = player.Id,
                    Amount = 0,
                    TimesStruggled = -6 * player.Level,
                    LastActionTimestamp = DateTime.UtcNow,
                    LastActionTurnstamp = gameTurn - 1,
                };
            }

            double chanceOfSuccess = (gameTurn - xp.LastActionTurnstamp);

            ITFMessageRepository tfMessageRepo = new EFTFMessageRepository();
            var tf = tfMessageRepo.TFMessages.FirstOrDefault(t => t.FormDbName == playerItem.ItemSource.CurseTFFormdbName);

            var ownerMessage = "";
            var playerMessage = "";



            // success; owner is transformed!
            if (roll < chanceOfSuccess)
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newForm = FormStatics.GetForm(playerItem.ItemSource.CurseTFFormdbName);

                if (newForm.MobilityType == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new ChangeForm
                    {
                        PlayerId = playerItem.Owner.Id,
                        FormName = playerItem.ItemSource.CurseTFFormdbName
                    });

                    var dbOwner = playerRepo.Players.FirstOrDefault(p => p.Id == playerItem.Owner.Id);
                    dbOwner.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(dbOwner));
                    dbOwner.Mana -= dbOwner.MaxMana * .5M;
                    dbOwner.NormalizeHealthMana();
                    playerRepo.SavePlayer(dbOwner);

                    if (playerItem.Owner.Gender == PvPStatics.GenderMale && !tf.CursedTF_Succeed_M.IsNullOrEmpty())
                    {
                        ownerMessage = tf.CursedTF_Succeed_M;
                    }
                    else if (playerItem.Owner.Gender == PvPStatics.GenderFemale && !tf.CursedTF_Succeed_F.IsNullOrEmpty())
                    {
                        ownerMessage = tf.CursedTF_Succeed_F;
                    }
                    else if (!tf.CursedTF_Succeed.IsNullOrEmpty())
                    {
                        ownerMessage = tf.CursedTF_Succeed;
                    }

                    playerMessage = "Your subtle transformation curse overwhelms your owner, transforming them into a " + newForm.FriendlyName + "!";
                    PlayerLogProcedures.AddPlayerLog(playerItem.Owner.Id, ownerMessage, true);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, "<b> " + owner.GetFullName() + " is suddenly transformed by " + playerItem.FormerPlayer.FullName + " the " + playerItem.ItemSource.FriendlyName + ", one of their belongings!</b>");
                }
            }


            // fail; owner is not transformed
            else
            {
                if (owner.Gender == PvPStatics.GenderMale && !tf.CursedTF_Fail_M.IsNullOrEmpty())
                {
                    ownerMessage = tf.CursedTF_Fail_M;
                }
                else if (owner.Gender == PvPStatics.GenderFemale && !tf.CursedTF_Fail_F.IsNullOrEmpty())
                {
                    ownerMessage = tf.CursedTF_Fail_F;
                }
                else if (!tf.CursedTF_Fail.IsNullOrEmpty())
                {
                    ownerMessage = tf.CursedTF_Fail;
                }

                playerMessage = "Unfortunately your subtle transformation curse fails to transform your owner.";
                PlayerLogProcedures.AddPlayerLog(owner.Id, ownerMessage, true);
            }
            
            PlayerProcedures.AddAttackCount(player);
            return playerMessage + GiveInanimateXP(player.MembershipId, isWhitelist);
        }
    }
}
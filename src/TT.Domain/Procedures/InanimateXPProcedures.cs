using System;
using System.Linq;
using System.Threading;
using System.Web;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class InanimateXPProcedures
    {

        public static void MakeInanimateXP(Player player)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            decimal struggle = -12 * player.Level;

            InanimateXP makeme = new InanimateXP
            {
                Amount = 0,
                LastActionTimestamp = DateTime.UtcNow,
                LastActionTurnstamp = PvPWorldStatProcedures.GetWorldTurnNumber(),
                TimesStruggled = Convert.ToInt32(struggle),
                OwnerId = player.Id,
            };

            inanimXpRepo.SaveInanimateXP(makeme);

        }

        public static decimal GetStruggleChance(Player player)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            decimal output = -6 * player.Level;
            InanimateXP myItemXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

            if (myItemXP != null)
            {
                return myItemXP.TimesStruggled;
            }
            else
            {
                return output;
            }
        }

        public static string GiveInanimateXP(string membershipId)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            IItemRepository itemRep = new EFItemRepository();

            // get the current level of this player based on what item they are
            Player me = PlayerProcedures.GetPlayerFromMembership(membershipId);
            Item inanimateMe = itemRep.Items.FirstOrDefault(i => i.VictimName == me.FirstName + " " + me.LastName);

            int currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

            decimal xpGain = 0;

            // get the number of inanimate accounts under this IP
            IPlayerRepository playerRepo = new EFPlayerRepository();
            decimal playerCount = playerRepo.Players.Where(p => p.IpAddress == me.IpAddress && (p.Mobility == "inanimate" || p.Mobility == "animal") && p.BotId == AIStatics.ActivePlayerBotId).Count();
            //TODO: TEMPEST - FIX
            if (playerCount == 0 )//|| HttpContext.Current.User.IsInRole(PvPStatics.Permissions_MultiAccountWhitelist))
            {
                playerCount = 1;
            }
            xpGain = xpGain / playerCount;

            InanimateXP xp = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == me.Id);

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

                if (me.Mobility == "inanimate")
                {
                    new Thread(() =>
                        StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__InanimateXPEarned, (float)xpGain)
                    ).Start();
                }
                else if (me.Mobility == "animal")
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

                if (me.Mobility == "inanimate")
                {
                    new Thread(() =>
                        StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__InanimateXPEarned, (float)xpGain)
                    ).Start();
                }
                else if (me.Mobility == "animal")
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

            string resultMessage = "  ";



            if (xp.Amount >= Convert.ToDecimal(ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level)))
            {
                xp.Amount -= Convert.ToDecimal(ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level));
                inanimateMe.Level++;
                itemRep.SaveItem(inanimateMe);

                resultMessage += "  You have gained " + xpGain + " xp.  <b>Congratulations, you have gained a level!  Your owner will be so proud...</b>";

                string wearerMessage = "<span style='color: darkgreen'>" + me.FirstName + " " + me.LastName + ", currently your " + ItemStatics.GetStaticItem(inanimateMe.dbName).FriendlyName + ", has gained a level!  Treat them kindly and they might keep helping you out...</span>";


                // now we need to change the owner's max health or mana based on this leveling
                if (inanimateMe.OwnerId != -1)
                {
                    ItemViewModel inanimateMePlus = ItemProcedures.GetItemViewModel(inanimateMe.Id);

                    if (inanimateMePlus.Item.HealthBonusPercent != 0.0M || inanimateMePlus.Item.ManaBonusPercent != 0.0M)
                    {

                        Player myowner = playerRepo.Players.FirstOrDefault(p => p.Id == inanimateMe.OwnerId);

                        decimal healthChange = PvPStatics.Item_LevelBonusModifier * inanimateMePlus.Item.HealthBonusPercent;
                        decimal manaChange = PvPStatics.Item_LevelBonusModifier * inanimateMePlus.Item.ManaBonusPercent;

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


                PlayerLogProcedures.AddPlayerLog(inanimateMe.OwnerId, wearerMessage, true);
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

            InanimateXP inanimXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

            int currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

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
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate++;

            double strugglesMade = Convert.ToDouble(inanimXP.TimesStruggled);

            Random rand = new Random();
            double roll = rand.NextDouble() * 100;

            // if player is in dungeon, make struggling chance much lower
            if (dungeonHalfPoints)
            {
                roll = roll * 3;
            }

            Item dbPlayerItem = ItemProcedures.GetItemByVictimName(player.FirstName, player.LastName);

            if (dbPlayerItem.OwnerId > 0)
            {
                Player owner = PlayerProcedures.GetPlayer(dbPlayerItem.OwnerId);
                dbPlayer.dbLocationName = owner.dbLocationName;
            }

            DbStaticItem itemPlus = ItemStatics.GetStaticItem(dbPlayerItem.dbName);

            if (roll < strugglesMade)
            {

                // assert that the covenant the victim is in is not too full to accept them back in
                if (dbPlayer.Covenant > 0)
                {
                    Covenant victimCov = CovenantProcedures.GetCovenantViewModel(dbPlayer.Covenant).dbCovenant;
                    if (victimCov != null && CovenantProcedures.GetPlayerCountInCovenant(victimCov, true) >= PvPStatics.Covenant_MaximumAnimatePlayerCount)
                    {
                        return "Although you had enough energy to break free from your body as a " + itemPlus.FriendlyName + " and restore your regular body, you were unfortunately not able to break free because there is no more room in your covenant for any more animate mages.";
                    }
                }


                // if the item has an owner, notify them via a message.
                if (dbPlayerItem.OwnerId != -1)
                {
                    string message = player.FirstName + " " + player.LastName + ", your " + itemPlus.FriendlyName + ", successfully struggles against your magic and reverses their transformation.  You can no longer claim them as your property, not unless you manage to turn them back again...";
                    PlayerLogProcedures.AddPlayerLog(dbPlayerItem.OwnerId, message, true);
                }

                // change the player's form and mobility
                dbPlayer.Form = dbPlayer.OriginalForm;
                if (FormStatics.GetForm(dbPlayer.OriginalForm).Gender == "male")
                {
                    dbPlayer.Gender = "male";
                }
                else
                {
                    dbPlayer.Gender = "female";
                }

                dbPlayer.Mobility = "full";
                dbPlayer.ActionPoints = PvPStatics.MaximumStoreableActionPoints;
                dbPlayer.ActionPoints_Refill = PvPStatics.MaximumStoreableActionPoints_Refill;
                dbPlayer.CleansesMeditatesThisRound = PvPStatics.MaxCleansesMeditatesPerUpdate;
                dbPlayer.TimesAttackingThisUpdate = PvPStatics.MaxAttacksPerUpdate;

                // don't let the player spawn in the dungeon if they are not in PvP mode
                if (dbPlayer.GameMode < 2 && dbPlayer.IsInDungeon())
                {
                    dbPlayer.dbLocationName = LocationsStatics.GetRandomLocation();
                }

                dbPlayer = PlayerProcedures.ReadjustMaxes(dbPlayer, ItemProcedures.GetPlayerBuffs(dbPlayer));
                dbPlayer.Health = dbPlayer.MaxHealth / 3;
                dbPlayer.Mana = dbPlayer.MaxHealth / 3;
                playerRepo.SavePlayer(dbPlayer);

                // delete the item or animal that this player had turned into
                itemRepo.DeleteItem(dbPlayerItem.Id);

                // delete the inanimate XP item
                inanimXpRepo.DeleteInanimateXP(inanimXP.Id);

                // give the player the recovery buff
                EffectProcedures.GivePerkToPlayer(PvPStatics.Effect_Back_On_Your_Feet, dbPlayer);

                string msg = "You have managed to break free from your form as " + itemPlus.FriendlyName + " and occupy an animate body once again!";

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

                string message = player.FirstName + " " + player.LastName + ", your " + itemPlus.FriendlyName + ", struggles but fails to return to an animate form.  [Recovery chance Recovery chance::  " + inanimXP.TimesStruggled + "%]";
                PlayerLogProcedures.AddPlayerLog(dbPlayerItem.OwnerId, message, true);

                PlayerLogProcedures.AddPlayerLog(dbPlayer.Id, "You struggled to return to a human form.", false);

                return "Unfortunately you are not able to struggle free from your form as " + itemPlus.FriendlyName + ".  Keep trying and you might succeed later... [Recovery chance next struggle:  " + inanimXP.TimesStruggled + "%]";
            }
        }

        public static string CurseTransformOwner(Player player, Player owner, Item playerItem, DbStaticItem playerItemPlus)
        {
            Random rand = new Random();
            double roll = rand.NextDouble() * 100;


            IInanimateXPRepository inanimateXpRepo = new EFInanimateXPRepository();
            InanimateXP xp = inanimateXpRepo.InanimateXPs.FirstOrDefault(x => x.OwnerId == player.Id);
            int gameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();

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
            TFMessage tf = tfMessageRepo.TFMessages.FirstOrDefault(t => t.FormDbName == playerItemPlus.CurseTFFormdbName);

            string ownerMessage = "";
            string playerMessage = "";



            // success; owner is transformed!
            if (roll < chanceOfSuccess)
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                Player dbOwner = playerRepo.Players.FirstOrDefault(p => p.Id == owner.Id);
                DbStaticForm newForm = FormStatics.GetForm(playerItemPlus.CurseTFFormdbName);

                if (newForm.MobilityType == "full")
                {
                    dbOwner.Form = playerItemPlus.CurseTFFormdbName;
                    dbOwner.Gender = newForm.Gender;
                    dbOwner.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(dbOwner));
                    dbOwner.Mana -= dbOwner.MaxMana * .5M;
                    dbOwner.NormalizeHealthMana();
                    playerRepo.SavePlayer(dbOwner);

                    if (owner.Gender == "male" && tf.CursedTF_Succeed_M != null && tf.CursedTF_Succeed_M != "")
                    {
                        ownerMessage = tf.CursedTF_Succeed_M;
                    }
                    else if (owner.Gender == "female" && tf.CursedTF_Succeed_F != null && tf.CursedTF_Succeed_F != "")
                    {
                        ownerMessage = tf.CursedTF_Succeed_F;
                    }
                    else if (tf.CursedTF_Succeed != null && tf.CursedTF_Succeed != "")
                    {
                        ownerMessage = tf.CursedTF_Succeed;
                    }

                    playerMessage = "Your subtle transformation curse overwhelms your owner, transforming them into a " + newForm.FriendlyName + "!";
                    PlayerLogProcedures.AddPlayerLog(owner.Id, ownerMessage, true);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, "<b> " + owner.GetFullName() + " is suddenly transformed by " + playerItem.GetFullName() + " the " + playerItemPlus.FriendlyName + ", one of their belongings!</b>");
                }
            }


            // fail; owner is not transformed
            else
            {
                if (owner.Gender == "male" && tf.CursedTF_Fail_M != null && tf.CursedTF_Fail_M != "")
                {
                    ownerMessage = tf.CursedTF_Fail_M;
                }
                else if (owner.Gender == "female" && tf.CursedTF_Fail_F != null && tf.CursedTF_Fail_F != "")
                {
                    ownerMessage = tf.CursedTF_Fail_F;
                }
                else if (tf.CursedTF_Fail != null && tf.CursedTF_Fail != "")
                {
                    ownerMessage = tf.CursedTF_Fail;
                }

                playerMessage = "Unfortunately your subtle transformation curse fails to transform your owner.";
                PlayerLogProcedures.AddPlayerLog(owner.Id, ownerMessage, true);
            }

            
           // else
           // {

                double timeBonus = gameTurn - xp.LastActionTurnstamp;
                if (timeBonus > InanimateXPStatics.ItemMaxTurnsBuildup)
                {
                    timeBonus = InanimateXPStatics.ItemMaxTurnsBuildup;
                }
                else if (timeBonus < 0)
                {
                    timeBonus = 0;
                }


                decimal xpGain = Convert.ToDecimal(timeBonus) * InanimateXPStatics.XPGainPerInanimateAction;

                xp.Amount += xpGain;

                // lock the player into their fate if their inanimate XP gets too high
                if (xp.TimesStruggled <= -100 && xp.TimesStruggled > -160)
                {
                    playerMessage += "  Careful, if you keep doing this you may find yourself stuck in your current form forever...";
                }

                if (xp.TimesStruggled <= -160 && !playerItem.IsPermanent)
                {

                    IItemRepository itemRepo = new EFItemRepository();
                    Item dbItemPlayer = itemRepo.Items.FirstOrDefault(i => i.Id == playerItem.Id);
                    dbItemPlayer.IsPermanent = true;
                    itemRepo.SaveItem(dbItemPlayer);
                    playerMessage += "  <b>You find the last of your old human self slip away as you permanently embrace your new form.</b>";
                }


                PlayerProcedures.AddAttackCount(player);

            return playerMessage;
        }


    }
}
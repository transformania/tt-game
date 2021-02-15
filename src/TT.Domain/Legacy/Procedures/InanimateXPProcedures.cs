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

        public static decimal GetStruggleChance(Player player, bool dungeonPenalty)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            decimal output = -6 * player.Level;
            var myItemXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

            var quotient = dungeonPenalty ? 3.0m : 1.0m;

            if (myItemXP != null)
            {
                return myItemXP.TimesStruggled / quotient;
            }
            else
            {
                return output / quotient;
            }
        }

        public static string GetProspectsMessage(Player player)
        {
            IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
            IItemRepository itemRep = new EFItemRepository();

            var inanimateMe = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = player.Id});
            var meItem = itemRep.Items.FirstOrDefault(i => i.Id == inanimateMe.Id);
            var myItemXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

            if (meItem == null || myItemXP == null)
            {
                return null;
            }

            var turnsSinceLastAction = Math.Max(0, PvPWorldStatProcedures.GetWorldTurnNumber() - myItemXP.LastActionTurnstamp);

            // Time until lock - at 2% per turn  (negative threshold)
            var turnsUntilLocked = (myItemXP.TimesStruggled - TurnTimesStatics.GetStruggleXPBeforeItemPermanentLock()) / 2 - turnsSinceLastAction;

            if (!meItem.IsPermanent && turnsUntilLocked <= TurnTimesStatics.GetItemMaxTurnsBuildup())
            {
                if (turnsUntilLocked <= 1)
                {
                    return "<b style=\"color: red;\">Be careful!</b>  Just one more move and you might never be human again!";
                }
                else
                {
                    var time = turnsUntilLocked * TurnTimesStatics.GetTurnLengthInSeconds();

                    return $"If you keep enjoying your current form you might find yourself locked into it forever!  That could happen in as little as <b>{SecondsToDurationString(time)}</b> or so!";
                }
            }

            // Time until chance of escaping - at 1% per turn outside Chaos
            var turnsUntilAbleToStruggle = 1 - myItemXP.TimesStruggled - turnsSinceLastAction;
            if (ItemProcedures.ItemIncursDungeonPenalty(inanimateMe))
            {
                turnsUntilAbleToStruggle *= 3;
            }

            if (!PvPStatics.ChaosMode && turnsUntilAbleToStruggle > 1 && turnsUntilAbleToStruggle <= TurnTimesStatics.GetItemMaxTurnsBuildup())
            {
                var time = turnsUntilAbleToStruggle * TurnTimesStatics.GetTurnLengthInSeconds();

                return $"You could be free in approximately <b>{SecondsToDurationString(time)}</b> if you keep fighting!";
            }

            return null;
        }

        private static string SecondsToDurationString(int numSeconds)
        {
            var unit = "minute";
            var amount = Math.Round(numSeconds / 60.0);

            if (amount > 60)
            {
                unit = "hour";
                amount = Math.Round(amount / 60.0);
            }

            var s = (amount == 1) ? "" : "s";
            return $"{(int)amount} {unit}{s}";
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
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__InanimateXPEarned, (float) xpGain);
                }
                else if (me.Mobility == PvPStatics.MobilityPet)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__PetXPEarned, (float)xpGain);
                }


            }
            else
            {

                double turnsSinceLastAction = currentGameTurn - xp.LastActionTurnstamp;

                if (turnsSinceLastAction > TurnTimesStatics.GetItemMaxTurnsBuildup())
                {
                    turnsSinceLastAction = TurnTimesStatics.GetItemMaxTurnsBuildup();
                }

                if (turnsSinceLastAction < 0)
                {
                    turnsSinceLastAction = 0;
                }

                xpGain += Convert.ToDecimal(turnsSinceLastAction) * InanimateXPStatics.XPGainPerInanimateAction;
                xpGain = xpGain / playerCount;

                if (me.Mobility == PvPStatics.MobilityInanimate)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__InanimateXPEarned, (float) xpGain);
                }
                else if (me.Mobility == PvPStatics.MobilityPet)
                {
                    StatsProcedures.AddStat(me.MembershipId, StatsProcedures.Stat__PetXPEarned, (float) xpGain);
                }

                xp.Amount += xpGain;
                xp.TimesStruggled -= 2 * Convert.ToInt32(turnsSinceLastAction);
                xp.LastActionTimestamp = DateTime.UtcNow;
                xp.LastActionTurnstamp = currentGameTurn;
            }

            var resultMessage = "  ";

            if (xp.Amount >= Convert.ToDecimal(ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level)))
            {
                xp.Amount -= Convert.ToDecimal(ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level));
                inanimateMe.Level++;
                itemRep.SaveItem(inanimateMe);

                resultMessage += $"  You have gained {xpGain:0.#} xp.  <b>Congratulations, you have gained a level!  Your owner will be so proud...</b>";

                var wearerMessage = "<span style='color: darkgreen'>" + me.FirstName + " " + me.LastName + ", currently your " + ItemStatics.GetStaticItem(inanimateMe.ItemSourceId).FriendlyName + ", has gained a level!  Treat them kindly and they might keep helping you out...</span>";

                // now we need to change the owner's max health or mana based on this leveling
                if (inanimateMe.OwnerId > 0)
                {
                    PlayerLogProcedures.AddPlayerLog((int)inanimateMe.OwnerId, wearerMessage, true);
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

                        if (myowner.Mana > myowner.MaxMana)
                        {
                            myowner.Mana = myowner.MaxMana;
                        }

                        playerRepo.SavePlayer(myowner);

                    }

                }


            }
            else
            {
                resultMessage = $"  You have gained {xpGain:0.#} xp.  ({xp.Amount:0.#}/{ItemProcedures.GetXPRequiredForItemPetLevelup(inanimateMe.Level):0.#} to next level).";
            }

            inanimXpRepo.SaveInanimateXP(xp);

            // lock the player into their fate if their inanimate XP gets too high
            if (xp.TimesStruggled <= TurnTimesStatics.GetStruggleXPBeforeItemPermanentLock() * .5 && xp.TimesStruggled > TurnTimesStatics.GetStruggleXPBeforeItemPermanentLock() && !inanimateMe.IsPermanent)
            {
                resultMessage += "  Careful, if you keep doing this you may find yourself stuck in your current form forever...";
            }

            if (xp.TimesStruggled <= TurnTimesStatics.GetStruggleXPBeforeItemPermanentLock() && !inanimateMe.IsPermanent)
            {
                inanimateMe.IsPermanent = true;
                itemRep.SaveItem(inanimateMe);
                DomainRegistry.Repository.Execute(new RemoveSoulbindingOnPlayerItems {PlayerId = me.Id});
                resultMessage += "  <b>You find the last of your old human self slip away as you permanently embrace your new form.</b>";
            }

            return resultMessage;


        }

        public static string ReturnToAnimate(Player player, bool dungeonPenalty)
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

            if (strugglebonus > TurnTimesStatics.GetItemMaxTurnsBuildup())
            {
                strugglebonus = TurnTimesStatics.GetItemMaxTurnsBuildup();
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

            var strugglesMade = Convert.ToDouble(GetStruggleChance(player, dungeonPenalty));

            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            var dbPlayerItem = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = player.Id});

            if (dbPlayerItem.Owner != null)
            {
                var owner = PlayerProcedures.GetPlayer(dbPlayerItem.Owner.Id);
                dbPlayer.dbLocationName = owner.dbLocationName;
            }

            var itemPlus = ItemStatics.GetStaticItem(dbPlayerItem.ItemSource.Id);

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
                    FormSourceId = dbPlayer.OriginalFormSourceId
                });

                dbPlayer.ActionPoints = TurnTimesStatics.GetActionPointLimit();
                dbPlayer.ActionPoints_Refill = TurnTimesStatics.GetActionPointReserveLimit();
                dbPlayer.CleansesMeditatesThisRound = PvPStatics.MaxCleansesMeditatesPerUpdate;
                dbPlayer.TimesAttackingThisUpdate = PvPStatics.MaxAttacksPerUpdate;

                // don't let the player spawn in the dungeon as they will have Back On Your Feet
                // and may not be meet the level and game mode requirements
                if (dbPlayer.IsInDungeon())
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
                EffectProcedures.GivePerkToPlayer(PvPStatics.Effect_BackOnYourFeetSourceId, dbPlayer);

                var msg = "You have managed to break free from your form as " + itemPlus.FriendlyName + " and occupy an animate body once again!";

                if (PvPStatics.ChaosMode)
                {
                    msg += $" [CHAOS MODE:  Struggle value overriden to {strugglebonus:0}% per struggle.]";
                }

                PlayerLogProcedures.AddPlayerLog(dbPlayer.Id, msg, false);

                StatsProcedures.AddStat(dbPlayer.MembershipId, StatsProcedures.Stat__SuccessfulStruggles, 1);

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
                    var message = $"{player.FirstName} {player.LastName}, your {itemPlus.FriendlyName}, struggles but fails to return to an animate form.  [Recovery chance next struggle:  {(int)GetStruggleChance(player, dungeonPenalty)}%]";
                    PlayerLogProcedures.AddPlayerLog(dbPlayerItem.Owner.Id, message, true);
                }

                PlayerLogProcedures.AddPlayerLog(dbPlayer.Id, "You struggled to return to a human form.", false);

                return $"Unfortunately you are not able to struggle free from your form as {itemPlus.FriendlyName}.  Keep trying and you might succeed later... [Recovery chance next struggle:  {(int)GetStruggleChance(player, dungeonPenalty)}%]";
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
            var tf = tfMessageRepo.TFMessages.FirstOrDefault(t => t.FormSourceId == playerItem.ItemSource.CurseTFFormSourceId);

            var ownerSuccessMessage = "";
            var ownerFailureMessage = "";
            var playerMessage = "";

            var newFormSourceId = -1;

            if (playerItem.ItemSource.CurseTFFormSourceId == null)
            {
                // No item-provided TF curse - reduce chance of transforming to a preset form
                chanceOfSuccess /= 4.0;
                newFormSourceId = PvPStatics.DefaultTFCurseForms[rand.Next(PvPStatics.DefaultTFCurseForms.Length)];
            }
            else
            {
                // Regular TF curse - load its details
                newFormSourceId = playerItem.ItemSource.CurseTFFormSourceId.Value;

                if (playerItem.Owner.Gender == PvPStatics.GenderMale && !tf.CursedTF_Succeed_M.IsNullOrEmpty())
                {
                    ownerSuccessMessage = tf.CursedTF_Succeed_M;
                }
                else if (playerItem.Owner.Gender == PvPStatics.GenderFemale && !tf.CursedTF_Succeed_F.IsNullOrEmpty())
                {
                    ownerSuccessMessage = tf.CursedTF_Succeed_F;
                }
                else if (!tf.CursedTF_Succeed.IsNullOrEmpty())
                {
                    ownerSuccessMessage = tf.CursedTF_Succeed;
                }

                if (playerItem.Owner.Gender == PvPStatics.GenderMale && !tf.CursedTF_Fail_M.IsNullOrEmpty())
                {
                    ownerFailureMessage = tf.CursedTF_Fail_M;
                }
                else if (playerItem.Owner.Gender == PvPStatics.GenderFemale && !tf.CursedTF_Fail_F.IsNullOrEmpty())
                {
                    ownerFailureMessage = tf.CursedTF_Fail_F;
                }
                else if (!tf.CursedTF_Fail.IsNullOrEmpty())
                {
                    ownerFailureMessage = tf.CursedTF_Fail;
                }
            }


            // success; owner is transformed!
            if (roll < chanceOfSuccess)
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newForm = FormStatics.GetForm(newFormSourceId);

                if (newForm.MobilityType == PvPStatics.MobilityFull)
                {
                    DomainRegistry.Repository.Execute(new ChangeForm
                    {
                        PlayerId = playerItem.Owner.Id,
                        FormSourceId = newFormSourceId
                    });

                    var dbOwner = playerRepo.Players.FirstOrDefault(p => p.Id == playerItem.Owner.Id);
                    dbOwner.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(dbOwner));
                    dbOwner.Mana -= dbOwner.MaxMana * .5M;
                    dbOwner.NormalizeHealthMana();
                    playerRepo.SavePlayer(dbOwner);

                    if (ownerSuccessMessage.IsNullOrEmpty())
                    {
                        ownerSuccessMessage = $"One of your items, {playerItem.FormerPlayer.FullName}, attempts to trigger a curse placed upon it.  Suddenly you are overwhelmed as you find yourself transformed into a {newForm.FriendlyName}!";
                    }
                    playerMessage = "Your subtle transformation curse overwhelms your owner, transforming them into a " + newForm.FriendlyName + "!";

                    PlayerLogProcedures.AddPlayerLog(playerItem.Owner.Id, ownerSuccessMessage, true);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, "<b> " + owner.GetFullName() + " is suddenly transformed by " + playerItem.FormerPlayer.FullName + " the " + playerItem.ItemSource.FriendlyName + ", one of their belongings!</b>");
                }
            }


            // fail; owner is not transformed
            else
            {
                if (ownerFailureMessage.IsNullOrEmpty())
                {
                    ownerFailureMessage = "One of your items attempts to trigger a curse placed upon it, but it fails to transform you.";
                }
                playerMessage = "Unfortunately your subtle transformation curse fails to transform your owner.";

                PlayerLogProcedures.AddPlayerLog(owner.Id, ownerFailureMessage, true);
            }
            
            PlayerProcedures.AddAttackCount(player);
            return playerMessage + GiveInanimateXP(player.MembershipId, isWhitelist);
        }

        public static bool CanShowTFCurseButton(ItemDetail item)
        {
            //  Does the item have an associated TF curse
            if (item.ItemSource.CurseTFFormSourceId != null)
            {
                return true;
            }

            // Is there a theme TF curse available?
            if (PvPStatics.ChaosMode && PvPStatics.DefaultTFCurseForms != null && !PvPStatics.DefaultTFCurseForms.IsEmpty())
            {
                return true;
            }

            return false;
        }
    }
}

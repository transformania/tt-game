using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class CharacterPrankProcedures
    {
        // Stat boosting effect sourec IDs
        public static readonly int[] BOOST_EFFECTS = { 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239 };
        public static readonly int[] PENALTY_EFFECTS = { 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250 };

        // Ids of specific and behavior-altering effect sources
        public const int SNEAK_REVEAL_1 = 226;
        public const int SNEAK_REVEAL_2 = 227;
        public const int SNEAK_REVEAL_3 = 228;

        public const int BLINDED_EFFECT = 204;
        public const int DIZZY_EFFECT = 205;
        public const int HUSHED_EFFECT = 206;

        #region Effects pranks

        public static string MildEffectsPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, JokeShopProcedures.ROOT_EFFECT, 1);
            }
            else if (roll < 60)  // 50%
            {
                return GiveRandomEffect(player, BOOST_EFFECTS, rand);
            }
            else if (roll < 90)  // 30%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS, rand);
            }
            else  // 10%
            {
                return GiveEffect(player, SNEAK_REVEAL_1);
            }
        }

        public static string MischievousEffectsPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, JokeShopProcedures.ROOT_EFFECT, 2);
            }
            else if (roll < 30)  // 20%
            {
                return GiveRandomEffect(player, BOOST_EFFECTS, rand);
            }
            else if (roll < 50)  // 20%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS, rand);
            }
            else if (roll < 55)  // 5%
            {
                return GiveEffect(player, BLINDED_EFFECT);
            }
            else if (roll < 60)  // 5%
            {
                return GiveEffect(player, DIZZY_EFFECT);
            }
            else if (roll < 65)  // 5%
            {
                return GiveEffect(player, HUSHED_EFFECT);
            }
            else if (roll < 70)  // 5%
            {
                return MakeInvisible(player);
            }
            else if (roll < 85)  // 15%
            {
                return GiveEffect(player, JokeShopProcedures.INSTINCT_EFFECT);
            }
            else if (roll < 90)  // 5%
            {
                return GiveEffect(player, SNEAK_REVEAL_2);
            }
            else  // 10%
            {
                return LiftRandomCurse(player, rand);
            }
        }

        public static string MeanEffectsPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, JokeShopProcedures.ROOT_EFFECT, 4);
            }
            else if (roll < 80)  // 70%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS, rand);
            }
            else if (roll < 90)  // 10%
            {
                return MakePsychotic(player);
            }
            else  // 10%
            {
                return GiveEffect(player, SNEAK_REVEAL_3);
            }
        }

        public static string GiveEffect(Player player, int effectSourceId, int duration = 3)
        {
            if (EffectProcedures.PlayerHasEffect(player, effectSourceId))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effectSourceId, player, Duration: duration, Cooldown: duration);
        }

        public static string GiveRandomEffect(Player player, IEnumerable<int> effectSourceIds, Random rand = null)
        {
            if (effectSourceIds.IsEmpty())
            {
                return null;
            }

            rand = rand ?? new Random();
            var effectSourceId = effectSourceIds.ElementAt(rand.Next(effectSourceIds.Count()));

            if (EffectProcedures.PlayerHasEffect(player, effectSourceId))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effectSourceId, player);
        }

        public static string ApplyLocalCurse(Player player, string dbLocationName, Random rand = null)
        {
            var effectSources = EffectStatics.GetEffectGainedAtLocation(dbLocationName).ToArray();

            if (effectSources.Any())
            {
                rand = rand ?? new Random();
                var effectSource = effectSources[rand.Next(effectSources.Count())];

                if (!EffectProcedures.PlayerHasEffect(player, effectSource.Id))
                {
                    return GiveEffect(player, effectSource.Id);
                }
            }

            return null;
        }

        private static string LiftRandomCurse(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var effects = EffectProcedures.GetPlayerEffects2(player.Id).Where(e => e.Effect.IsRemovable && e.dbEffect.Duration > 0).ToArray();

            if (effects.IsEmpty())
            {
                return null;
            }

            var effect = effects[rand.Next(effects.Count())];

            if (effect.dbEffect.EffectSourceId == PvPStatics.Effect_BackOnYourFeetSourceId)
            {
                EffectProcedures.RemovePerkFromPlayer(effect.dbEffect.EffectSourceId, player);
            }
            else
            {
                EffectProcedures.SetPerkDurationToZero(effect.dbEffect.EffectSourceId, player);
            }

            return $"The powers of the Joke Shop liberate you from the <strong>{effect.Effect.FriendlyName}</strong> effect!";
        }

        private static string MakePsychotic(Player player)
        {
            var message = GiveEffect(player, JokeShopProcedures.PSYCHOTIC_EFFECT, 3);

            // Give player the psychopath AI until the effect expires
            var playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.BotId = AIStatics.PsychopathBotId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, "You have temporarily become a psychopath!", false);

            return message;
        }

        public static string UndoPsychotic(int playerId)
        {
            var playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == playerId);

            if (user.BotId == AIStatics.ActivePlayerBotId)
            {
                return null;
            }

            user.BotId = AIStatics.ActivePlayerBotId;
            playerRepo.SavePlayer(user);

            AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(playerId);

            var message = "The psychotic voices leave your head.. for now..";
            PlayerLogProcedures.AddPlayerLog(playerId, message, true);

            return message;
        }

        private static string MakeInvisible(Player player)
        {
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }
            
            var message = GiveEffect(player, JokeShopProcedures.INVISIBILITY_EFFECT, 2);

            // Give player 'invisibility' until the effect expires, then revert to PvP
            // We intentionally don't ripple the change in game mode down to the player's items.
            var playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.GameMode = (int)GameModeStatics.GameModes.Invisible;
            playerRepo.SavePlayer(user);

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} vanishes into thin air!");
            PlayerLogProcedures.AddPlayerLog(player.Id, "You become invisible!", false);

            return message;
        }

        #endregion
        
        #region Form, name and MC pranks

        public static string MildTransformationPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 55)  // 55%
            {
                return AnimateTransform(player, null);
            }
            else if (roll < 65)  // 10%
            {
                return TGTransform(player);
            }
            else if (roll < 75)  // 10%
            {
                return BodySwap(player, true, rand);  // clone
            }
            else if (roll < 85)  // 10%
            {
                return RestoreBaseForm(player);
            }
            else  // 15%
            {
                return RestoreName(player);
            }
        }

        public static string MischievousTransformationPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return ImmobileTransform(player, rand.Next(2) == 0, rand);
            }
            else if (roll < 20)  // 10%
            {
                return MobileInanimateTransform(player, rand);
            }
            else if (roll < 30)  // 10%
            {
                return BodySwap(player, false, rand);
            }
            else if (roll < 40) //  10%
            {
                return ChangeBaseForm(player, rand);
            }
            else if (roll < 45)  // 5%
            {
                return SetBaseFormToCurrent(player);
            }
            else if (roll < 65)  // 20%
            {
                return IdentityChange(player, rand);
            }
            else if (roll < 90)  // 25%
            {
                return TransformToMindControlledForm(player, rand);
            }
            else  // 10%
            {
                return InanimateTransform(player, true, false, rand);
            }
        }

        public static string MeanTransformationPrank(Player player, Random rand = null)
        {
            return InanimateTransform(player, false, false, rand);
        }

        private static string AnimateTransform(Player player, Random rand = null)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                var warning = JokeShopProcedures.EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }

            var forms = JokeShopProcedures.Forms(f => f.Category == PvPStatics.MobilityFull);

            if (forms.IsEmpty())
            {
                return null;
            }

            rand = rand ?? new Random();
            var index = rand.Next(forms.Count());
            var form = forms.ElementAt(index);

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            return $"You feel the familar tingle of your body reshaping.  When you look down you realize you are now a <b>{form.FriendlyName}</b>!";
        }

        private static string ImmobileTransform(Player player, bool temporary, Random rand = null)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                var warning = JokeShopProcedures.EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }

            var forms = JokeShopProcedures.Forms(f => f.Category == JokeShopProcedures.LIMITED_MOBILITY);

            if (forms.IsEmpty())
            {
                return null;
            }

            rand = rand ?? new Random();
            var index = rand.Next(forms.Count());
            var form = forms.ElementAt(index);
            String message;

            if (temporary)
            {
                var duration = JokeShopProcedures.PlayerHasBeenWarnedTwice(player) ? 3 : 2;
                GiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT, duration);
                message = $"A mysterious fog crosses your mind and you feel yourself falling into the ether!  As you familiarize yourself with your surroundings you begin to feel giddy and confused.  You've always been a {form.FriendlyName}, haven't you?  Urgh!  That cloud is messing with your head!  It might take another {duration} turns for it to clear!";
            }
            else
            {
                message = $"Your limbs start to stiffen, your bones begin to fuse.  By the time you realize what's happening it's too late and you've already become a <b>{form.FriendlyName}</b>!";
            }

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            return message;
        }

        private static string InanimateTransform(Player player, bool temporary, bool dropInventory = false, Random rand = null)
        {
            var warning = JokeShopProcedures.EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var forms = JokeShopProcedures.Forms(f => f.Category != PvPStatics.MobilityFull && f.Category != JokeShopProcedures.LIMITED_MOBILITY);

            if (forms.IsEmpty())
            {
                return null;
            }

            var duration = 0;
            if (temporary)
            {
                duration = JokeShopProcedures.PlayerHasBeenWarnedTwice(player) ? 10 : 5;
                GiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT, duration);

                // If no autorestore we can't do temporary
                if (!EffectProcedures.PlayerHasActiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT))
                {
                    return null;
                }
            }

            rand = rand ?? new Random();
            var index = rand.Next(forms.Count());
            FormDetail form = forms.ElementAt(index);

            if (!TryInanimateTransform(player, form.FormSourceId, dropInventory: dropInventory, createItem: !temporary, severe: !temporary))
            {
                return null;
            }

            if (temporary)
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, $"A mysterious fog crosses your mind and you feel yourself falling into the ether!  As you familiarize yourself with your surroundings you begin to feel giddy and confused.  You've always been a {form.FriendlyName}, haven't you?  Urgh!  That cloud is messing with your head!  It might take another {duration} turns for it to clear!", true);
            }

            return $"All these joke items on the shelves have absorbed so much magic they're almost alive, almost calling to you, willing you to join them, to provide them with company while they remain frozen in time for all eternity.  You try to back out but the enchantment has already permeated your being, turning you into a <b>{form.FriendlyName}</b>!";
        }

        private static string MobileInanimateTransform(Player player, Random rand = null)
        {
            // Turning a player into a rune or consumable is a bit too involved as there are no static forms for those items in the DB,
            // however we can make a player inanimate without a player item and pretend they are fully mobile...

            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                var warning = JokeShopProcedures.EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }

            var forms = JokeShopProcedures.Forms(f => f.Category != PvPStatics.MobilityFull && f.Category != JokeShopProcedures.LIMITED_MOBILITY);

            if (forms.IsEmpty())
            {
                return null;
            }

            rand = rand ?? new Random();
            var index = rand.Next(forms.Count());
            FormDetail form = forms.ElementAt(index);

            // Give player an inanimate form without creating a player item
            if (!TryInanimateTransform(player, form.FormSourceId, dropInventory: false, createItem: false, severe: false))
            {
                return null;
            }

            // Coerce player into mobility
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.Mobility = PvPStatics.MobilityFull;
            playerRepo.SavePlayer(target);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into an animate <b>{form.FriendlyName}</b>.");

            return $"You sense a build-up of transformative energy.  Somebody trying to make you inanimate.  Somebody?  Or this cursed place itself?  No, it can try to turn you into a {form.FriendlyName}, but you will not allow it to take your mobility!";
        }

        private static string TGTransform(Player player)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                var warning = JokeShopProcedures.EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }

            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var altForm = formsRepo.DbStaticForms.Where(form => form.Id == player.FormSourceId)
                                                 .Select(form => new { form.AltSexFormSourceId, form.Gender })
                                                 .FirstOrDefault();

            if (altForm == null || !altForm.AltSexFormSourceId.HasValue)
            {
                return null;
            }

            if (!TryAnimateTransform(player, altForm.AltSexFormSourceId.Value, false))
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var newPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You suddenly became {newPlayer.Gender}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} suddenly became {newPlayer.Gender}.");

            return $"Your body spasms in the way you have become accustomed to after being hit by an orb.  As you shake the dysphoria of your reproportioned form you cast your eyes down over your body.  Yup, {newPlayer.Gender} again, you think with a sigh.";
        }

        private static bool PlayerCanBeCloned(Player player)
        {
            // Don't clone players who are in a mobile inanimate form (giving someone their form would make
            // the other player inanimate due to the form source mobility) or whose current form is not provided
            // by a discoverable spell.  (Cloning customs may be a future enhancement)
            return JokeShopProcedures.Forms(f => (f.Category == PvPStatics.MobilityFull ||
                                                  f.Category == JokeShopProcedures.LIMITED_MOBILITY) &&
                                                 f.FormSourceId == player.FormSourceId)
                                     .Any();
        }

        private static string BodySwap(Player player, bool clone, Random rand = null)
        {
            rand = rand ?? new Random();
            var candidates = JokeShopProcedures.ActivePlayersInJokeShopApartFrom(player)
                .Where(p => p.FormSourceId != player.FormSourceId).ToList();

            if (candidates.Count() == 0)
            {
                return null;
            }

            if (clone)
            {
                var victim = candidates[rand.Next(candidates.Count())];

                if (!PlayerCanBeCloned(victim) || !TryAnimateTransform(player, victim.FormSourceId, false))
                {
                    return null;
                }

                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have become a clone of {victim.GetFullName()}", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> became a clone of <b>{victim.GetFullName()}</b>.");

                return $"You spot {victim.GetFullName()}.  Don't they look stunning?  Wouldn't you like to be like them?  \"As you wish,\" echoes an ephereal voice, seemingly hearing your innermost thoughts...";
            }
            else
            {
                // Find nearboy player with sufficient consent
                Player victim = null;

                do
                {
                    var index = rand.Next(candidates.Count());
                    var candidate = candidates[index];

                    if ((candidate.GameMode != (int)GameModeStatics.GameModes.Superprotection || JokeShopProcedures.PlayerHasBeenWarned(candidate)) &&
                        PlayerCanBeCloned(player))
                    {
                        victim = candidate;
                    }
                    else
                    {
                        candidates.RemoveAt(index);
                    }
                } while (victim == null && candidates.Count() > 0);

                if (victim == null)
                {
                    return null;
                }

                // Swap forms
                TryAnimateTransform(player, victim.FormSourceId, false);
                TryAnimateTransform(victim, player.FormSourceId, false);

                PlayerLogProcedures.AddPlayerLog(victim.Id, $"You have swapped bodies with {player.GetFullName()}!", true);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have swapped bodies with {victim.GetFullName()}!", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> swapped bodies with <b>{victim.GetFullName()}</b>.");

                return $"A contraption whirs up and the next thing you know your head is being sucked into some sort of funnel while your body finds itself bound in iron shackles and leather straps.  In no time you are being held firm against a wall, and the same seems to be happening to {victim.GetFullName()}!  You both start to shake violently as your bodies bulge, flatten and contort, the tubes connecting you pumping full of magical energies.  The whirring dies down and you are able to wriggle free.  You check to see whether {victim.GetFullName()} is OK, but both of you are shocked as you each come face-to-face with yourself!";
            }
        }

        public static void UndoTemporaryForm(int playerId)
        {
            var player = PlayerProcedures.GetPlayer(playerId);

            // Avoid reverting form if player is in a mobile animate form
            var canRemove = false;
            if (player.Mobility != PvPStatics.MobilityFull)
            {
                canRemove = true;
            }
            else
            {
                IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
                var moveActionPointDiscount = formsRepo.DbStaticForms.Where(form => form.Id == player.FormSourceId).Select(form => form.MoveActionPointDiscount).FirstOrDefault();
                canRemove = moveActionPointDiscount < -5;

                if (!canRemove)
                {
                    // Can also revert if player is an inanimate posing as animate
                    // (don't think this can happen any more as mobile forms shouldn't have associated items)
                    var inanimatePlayer = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = playerId });
                    canRemove = inanimatePlayer != null;
                }
            }

            if (canRemove)
            {
                RestoreBaseForm(player, true);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"As your mind begins to clear, the illusion gradually subsides.  It isn't long before you realize you are back in a familiar form.", true);
            }
        }

        public static string RestoreBaseForm(Player player, bool force = false)
        {
            // Require extra warning for SP players, who might want to keep their form
            if (!force && player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                return null;
            }

            // Use chaos restore.  Should delete item & reset skills.  Self-restore, struggle restore and classic me do similar
            PlayerProcedures.InstantRestoreToBase(player);

            // No Back On Your Feet, but clear any XP ready for next inanimation - unless Chaos, where people like to lock fast
            if (!PvPStatics.ChaosMode)
            {
                IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
                var inanimXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);
                inanimXpRepo.DeleteInanimateXP(inanimXP.Id);
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You returned to base form.", false);

            return "You accidentally spill some Classic Me restorative lotion and revert to your base form, just as you remember it!  Right?";
        }

        private static string ChangeBaseForm(Player player, Random rand = null)
        {
            var availableForms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MISCHIEVOUS_FORMS).ToArray();

            if (availableForms.IsEmpty())
            {
                return null;
            }

            return ChangeBaseForm(player, availableForms, rand);
        }

        private static string ChangeBaseForm(Player player, int[] availableForms, Random rand = null)
        {
            rand = rand ?? new Random();
            var formSourceId = availableForms[rand.Next(availableForms.Count())];

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form was changed.", false);

            return "You feel something changing deep within you, but you're not sure what.  You suspect somebody's playing a trick on you, and you're not going to find out what it is just yet!";
        }

        public static string SetBaseFormToRegular(Player player)
        {
            var formRepo = new EFDbStaticFormRepository();
            var baseForms = formRepo.DbStaticForms.Where(f => (f.FriendlyName == "Regular Guy" || f.FriendlyName == "Regular Girl") &&
                                                               f.Id != player.OriginalFormSourceId)
                                                  .Select(f => f.Id).ToArray();

            if (baseForms.Any())
            {
                ChangeBaseForm(player, baseForms);
                return "You spot a fortune cookie and open it to see the message inside:  \"True purity can only come from the deepest of cleanses.\"  Let's hope the shopkeeper didn't see you!";
            }

            return null;
        }

        private static string SetBaseFormToCurrent(Player player)
        {
            if (player.OriginalFormSourceId == player.FormSourceId)
            {
                return null;
            }

            var formSourceId = player.FormSourceId;

            // Check we're not setting an inanimate/pet as base form, in case that causes problems...
            if (!PlayerCanBeCloned(player))
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form has changed to your current form.", false);

            return "\"Are you feeling comfortable?\" asks the shopkeeper.  \"Er, yes,\" you reply.  \"Good.  That form really suits you!\"  You eye the shopkeeper with suspicion.";
        }

        public static string RestoreName(Player player)
        {
            if (player.FirstName == player.OriginalFirstName && player.LastName == player.OriginalLastName)
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.FirstName = user.OriginalFirstName;
            user.LastName = user.OriginalLastName;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your name was changed back to {user.GetFullName()}.", false);

            return "In a sudden moment of clarity you remember who you truly are!";
        }

        private static string IdentityChange(Player player, Random rand = null)
        {
            rand = rand ?? new Random();

            int[] forms = Array.Empty<int>();
            var firstName = player.OriginalFirstName;
            var lastName = player.OriginalLastName;
            var mindControl = false;
            var message = "";

            var roll = rand.Next(8);

            // Pick changes to name and form
            if (roll == 0)  // Dogs
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DOGS).ToArray();
                mindControl = true;
                message = "You hear some barking outside and are suddenly compelled to join in!";

                switch (rand.Next(2))
                {
                    case 0:
                        string[] prefixes = { "Dog", "Doggy", "Canine", "Barker" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "Dogg", "Woof", "Barker" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                }
            }
            else if (roll == 1)  // Cats
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.CATS_AND_NEKOS).ToArray();
                mindControl = true;
                message = "A mouse scurries across the ground in front of you.  Suddenly you feel the urge to pounce!";

                switch (rand.Next(3))
                {
                    case 0:
                        string[] prefixes = { "Kitty", "Cat", "Neko", "Feline" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "Cat", "Neko", "Feline" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                    case 2:
                        firstName = firstName.ToLower().Replace("a", "nya");
                        lastName = lastName.ToLower().Replace("a", "nya");
                        firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);
                        lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                        break;
                }
            }
            else if (roll == 2)  // Drones
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DRONES).ToArray();
                mindControl = false;
                message = "This store actually has quite a good selection of comic books.  You pick one up one entitled \"Colony 2079\" and read the bubbles on the cover.  \"Be like us!\"  \"Individuality is overrated!\"  \"Join the collective!\"  This could be interesting.  You turn the page, wondering why you've never seen this publication in Extended Universe..";

                string[] designators = { "Unit", "Drone", "Clone", "Entity", "Bot" };
                var designator = designators[rand.Next(designators.Count())];

                switch (rand.Next(3))
                {
                    case 0:
                        var name = rand.Next(2) == 0 ? firstName : lastName;
                        var order = rand.Next(2);
                        firstName = order == 0 ? designator : name;
                        lastName = order == 0 ? name : designator;
                        break;
                    case 1:
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = designator;
                        lastName = lastName.ToUpper().Replace('I', '1')
                                                     .Replace('L', '1')
                                                     .Replace('Z', '2')
                                                     .Replace('E', '3')
                                                     .Replace('A', '4')
                                                     .Replace('S', '5')
                                                     .Replace('G', '6')
                                                     .Replace('R', '7')
                                                     .Replace('B', '8')
                                                     .Replace('Q', '9')
                                                     .Replace('O', '0');
                        break;
                    case 2:
                        firstName = designator;
                        lastName = $"#{rand.Next(100000)}";
                        break;
                }
            }
            else if (roll == 3)  // Ghosts
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.GHOSTS).ToArray();
                mindControl = true;
                message = "You come face-to-face with a ghost and jump right out of your skin!  Unfortunately that also leaves you looking like a ghost!";

                switch (rand.Next(3))
                {
                    case 0:
                        string[] prefixes = { "Ghost", "Ghostly", "Spooky", "Poltergeist", "Haunting" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] prefixes2 = { "Ghost of", "Spirit of" };
                        lastName = $"{firstName} {lastName}";
                        firstName = prefixes2[rand.Next(prefixes2.Count())];
                        break;
                    case 2:
                        firstName = firstName.ToLower().Replace("o", "oo");
                        lastName = lastName.ToLower().Replace("o", "oo");
                        firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);
                        lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                        break;
                }
            }
            else if (roll == 4)  // Sheep
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.SHEEP).ToArray();
                mindControl = true;
                message = "You suddenly sprout a fleece and feel compelled to follow your flock!";

                switch (rand.Next(3))
                {
                    case 0:
                        string[] prefixes = { "Shep", "Sheep", "Lamb", "Little Lamb" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "Shep", "Lamb" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                    case 2:
                        firstName = firstName.ToLower().Replace("a", "aa");
                        lastName = lastName.ToLower().Replace("a", "aa");
                        firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);
                        lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                        break;
                }
            }
            else if (roll == 5)  // Maids
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MAIDS).ToArray();
                mindControl = true;
                message = "The shop is full of dust and cobwebs.  You feel it could do with a good clean.  If you enjoy sweeping the dirt away maybe you could serve others is town?";

                string[] prefixes = { "Maid", "Servant", "Miss", "Cleaner", "Waitress" };
                lastName = rand.Next(2) == 0 ? firstName : lastName;
                firstName = prefixes[rand.Next(prefixes.Count())];
            }
            else if (roll == 6)  // Strippers
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.STRIPPERS).ToArray();
                mindControl = true;
                message = "You feel a sudden and irrepressible compulsion to start removing your clothes.  Is the shop too hot, or are you just doing it for attention and to show off your body to anyone who will watch?  Only you know...";

                lastName = rand.Next(2) == 0 ? firstName : lastName;
                firstName = "Stripper";
            }
            else if (roll == 7)  // Renames
            {
                mindControl = false;
                message = "A heady fragrance fills the air, numbing your mind.  You forget who you are for a moment, but then it comes back to you.  How could you forget your own name like that?!";

                switch (rand.Next(1))
                {
                    case 0:
                        lastName = $"Mc{firstName}face";
                        if (!firstName.EndsWith("y"))
                        {
                            firstName = $"{firstName}y";
                        }
                        break;
                }
            }

            // Change form
            if (forms != null && !forms.IsEmpty())
            {
                var formSourceId = forms[new Random().Next(forms.Count())];
                if (!TryAnimateTransform(player, formSourceId))
                {
                    return null;
                }
            }

            // Change name
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.FirstName = firstName;
            user.LastName = lastName;
            playerRepo.SavePlayer(user);

            // Impose behavior
            if (mindControl)
            {
                GiveEffect(player, JokeShopProcedures.INSTINCT_EFFECT);
            }

            if (message.IsNullOrEmpty())
            {
                message = "You are shocked to discover you have taken on a whole new identity!";
            }

            return message;
        }

        private static string TransformToMindControlledForm(Player player, Random rand = null)
        {
            rand = rand ?? new Random();

            int[][] mcForms = {
                JokeShopProcedures.CATS_AND_NEKOS,
                JokeShopProcedures.DOGS, 
                JokeShopProcedures.MAIDS,
                JokeShopProcedures.SHEEP,
                JokeShopProcedures.STRIPPERS,
                JokeShopProcedures.GHOSTS
                };
            var genre = mcForms[rand.Next(mcForms.Count())];
            var forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(genre).ToArray();

            if (forms == null || forms.IsEmpty())
            {
                return null;
            }

            if (!TryAnimateTransform(player, forms[rand.Next(forms.Count())]))
            {
                return null;
            }

            GiveEffect(player, JokeShopProcedures.INSTINCT_EFFECT);
            return "A shrieking laugh turns the air to ice, freezing you where you stand.  \"Now you're mine!\" comes a the bloodcurdling taunt of a witch directly above you!  You then feel your arms moving involuntarily, being hoisted up by your wrists as if on strings, forcing you to perform a little jig against your will.  \"I will control your body, and I will control your brain!\" cackles the voice from overhead.  You try to run free, but you can still hear that voice in your mind:  \"You're still mine, wherever you go, and you will behave as I have made you!\"  Then as a freak gust of wind hits, you start to feel.. different..";
        }

        public static bool TryAnimateTransform(Player player, int formSourceId, bool logChanges = true)
        {
            // Require extra warning for SP players, who might want to keep their form
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);
            DomainRegistry.Repository.Execute(new ReadjustMaxes
            {
                playerId = player.Id,
                buffs = ItemProcedures.GetPlayerBuffs(player)
            });

            if (logChanges)
            {
                var form = FormStatics.GetForm(formSourceId);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");
            }

            return true;
        }

        private static bool TryInanimateTransform(Player player, int formSourceId, bool dropInventory, bool createItem = true, bool severe = true)
        {
            if (severe && !JokeShopProcedures.PlayerHasBeenWarnedTwice(player) || !severe && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);
            var form = FormStatics.GetForm(formSourceId);

            // If item is not created player will have no actions and not be visible to other players,
            // so some external mechanism must be in place to restore the player to animate form.
            if (createItem)
            {
                ItemProcedures.PlayerBecomesItem(player, form, null, dropInventory);
                // If inventory isn't dropped at point of TF then it will be dropped if/when player locks.
            }
            else if (dropInventory)
            {
                DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = player.Id, IgnoreRunes = false });
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");

            return true;
        }

        #endregion

    }
}

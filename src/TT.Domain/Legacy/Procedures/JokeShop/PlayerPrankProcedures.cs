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
    public static class PlayerPrankProcedures
    {
        // These fields back properies populated by the database
        private static List<int> BoostEffects = null;
        private static List<int> PenaltyEffects = null;

        private static int? SneakReveal1 = null;
        private static int? SneakReveal2 = null;
        private static int? SneakReveal3 = null;

        private static int? BlindedEffect = null;
        private static int? DizzyEffect = null;
        private static int? HushedEffect = null;

        // Stat boosting effects
        public static List<int> BOOST_EFFECTS
        {
            get
            {
                if (BoostEffects == null)
                {
                    BoostEffects = JokeShopProcedures.EffectsWithNamesStarting("effect_Joke_Shop_Boost_") ?? new List<int>();
                }

                return BoostEffects;
            }
            set
            {
                BoostEffects = value;
            }
        }
        public static List<int> PENALTY_EFFECTS
        {
            get
            {
                if (PenaltyEffects == null)
                {
                    PenaltyEffects = JokeShopProcedures.EffectsWithNamesStarting("effect_Joke_Shop_Penalty_") ?? new List<int>();
                }

                return PenaltyEffects;
            }
            set
            {
                PenaltyEffects = value;
            }
        }

        // Specific and behavior-altering effects
        public static int? SNEAK_REVEAL_1
        {
            get
            {
                if (!SneakReveal1.HasValue)
                {
                    SneakReveal1 = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Track_1") ?? -1;
                }

                return SneakReveal1.Value == -1 ? null : SneakReveal1;
            }
            set
            {
                SneakReveal1 = value;
            }
        }

        public static int? SNEAK_REVEAL_2
        {
            get
            {
                if (!SneakReveal2.HasValue)
                {
                    SneakReveal2 = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Track_2") ?? -1;
                }

                return SneakReveal2.Value == -1 ? null : SneakReveal2;
            }
            set
            {
                SneakReveal2 = value;
            }
        }

        public static int? SNEAK_REVEAL_3
        {
            get
            {
                if (!SneakReveal3.HasValue)
                {
                    SneakReveal3 = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Track_3") ?? -1;
                }

                return SneakReveal3.Value == -1 ? null : SneakReveal3;
            }
            set
            {
                SneakReveal3 = value;
            }
        }

        public static int? BLINDED_EFFECT
        {
            get
            {
                if (!BlindedEffect.HasValue)
                {
                    BlindedEffect = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Blinded") ?? -1;
                }

                return BlindedEffect == -1 ? null : BlindedEffect; ;
            }
            set
            {
                BlindedEffect = value;
            }
        }

        public static int? DIZZY_EFFECT
        {
            get
            {
                if (!DizzyEffect.HasValue)
                {
                    DizzyEffect = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Dizzy") ?? -1;
                }

                return DizzyEffect == -1 ? null : DizzyEffect;
            }
            set
            {
                DizzyEffect = value;
            }
        }

        public static int? HUSHED_EFFECT
        {
            get
            {
                if (!HushedEffect.HasValue)
                {
                    HushedEffect = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Hushed") ?? -1;
                }

                return HushedEffect == -1 ? null : HushedEffect;
            }
            set
            {
                HushedEffect = value;
            }
        }

        #region Effects pranks

        public static string MildEffectsPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, JokeShopProcedures.ROOT_EFFECT, 1);
            }
            else if (roll < 60)  // 50%
            {
                return GiveRandomEffect(player, BOOST_EFFECTS);
            }
            else if (roll < 90)  // 30%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS);
            }
            else  // 10%
            {
                return GiveEffect(player, SNEAK_REVEAL_1);
            }
        }

        public static string MischievousEffectsPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, JokeShopProcedures.ROOT_EFFECT, 2);
            }
            else if (roll < 30)  // 20%
            {
                return GiveRandomEffect(player, BOOST_EFFECTS);
            }
            else if (roll < 50)  // 20%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS);
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
                return LiftRandomCurse(player);
            }
        }

        public static string MeanEffectsPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, JokeShopProcedures.ROOT_EFFECT, 4);
            }
            else if (roll < 80)  // 70%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS);
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

        public static string GiveEffect(Player player, int? effect, int duration = 3)
        {
            if (!effect.HasValue || EffectProcedures.PlayerHasEffect(player, effect.Value))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effect.Value, player, Duration: duration, Cooldown: duration);
        }

        private static string GiveRandomEffect(Player player, IEnumerable<int> effects)
        {
            if (effects.IsEmpty())
            {
                return null;
            }

            var effect = effects.ElementAt(new Random().Next(effects.Count()));

            if (EffectProcedures.PlayerHasEffect(player, effect))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effect, player);
        }

        public static string ApplyLocalCurse(Player player, string dbLocationName)
        {
            var effects = EffectStatics.GetEffectGainedAtLocation(dbLocationName).ToArray();

            if (effects.Any())
            {
                var effect = effects[new Random { }.Next(effects.Count())];

                if (!EffectProcedures.PlayerHasEffect(player, effect.Id))
                {
                    return GiveEffect(player, effect.Id);
                }
            }

            return null;
        }

        private static string LiftRandomCurse(Player player)
        {
            var rand = new Random();
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

            return $"Your <strong>{effect.Effect.FriendlyName}</strong> effect has been lifted!";  // TODO joke_shop flavor text
        }

        private static string MakePsychotic(Player player)
        {
            if (!JokeShopProcedures.PSYCHOTIC_EFFECT.HasValue)
            {
                return null;
            }

            var message = GiveEffect(player, JokeShopProcedures.PSYCHOTIC_EFFECT, 3);

            // Give player the psychopath AI until the effect expires
            var playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.BotId = AIStatics.PsychopathBotId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, "You have temporarily become a psychopath!", false);

            return message;
        }

        private static string MakeInvisible(Player player)
        {
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP && !JokeShopProcedures.INVISIBILITY_EFFECT.HasValue)
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

        public static string MildTransformationPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 55)  // 55%
            {
                return AnimateTransform(player);
            }
            else if (roll < 65)  // 10%
            {
                return TGTransform(player);
            }
            else if (roll < 75)  // 10%
            {
                return BodySwap(player, true);  // clone
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

        public static string MischievousTransformationPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 15)  // 10%
            {
                return ImmobileTransform(player, rand.Next(2) == 0);
            }
            else if (roll < 20)  // 10%
            {
                return MobileInanimateTransform(player);
            }
            else if (roll < 30)  // 10%
            {
                return BodySwap(player, false);
            }
            else if (roll < 40) //  10%
            {
                return ChangeBaseForm(player);
            }
            else if (roll < 45)  // 5%
            {
                return SetBaseFormToCurrent(player);
            }
            else if (roll < 65)  // 20%
            {
                return IdentityChange(player);
            }
            else if (roll < 90)  // 25%
            {
                return TransformToMindControlledForm(player);
            }
            else  // 10%
            {
                return InanimateTransform(player, true);
            }
        }

        public static string MeanTransformationPrank(Player player)
        {
            return InanimateTransform(player, false);
        }

        private static string AnimateTransform(Player player)
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

            var index = new Random().Next(forms.Count());
            var form = forms.ElementAt(index);

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            return $"You are an animate {form.FriendlyName}.";  // TODO joke_shop flavor text
        }

        private static string ImmobileTransform(Player player, bool temporary)
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

            FormDetail form;

            if (temporary)
            {
                GiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT, JokeShopProcedures.PlayerHasBeenWarnedTwice(player) ? 3 : 2);
            }

            var index = new Random().Next(forms.Count());
            form = forms.ElementAt(index);

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            return $"You are an immobile {form.FriendlyName}.";  // TODO joke_shop flavor text
        }

        private static string InanimateTransform(Player player, bool temporary, bool dropInventory = false)
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
                if (!JokeShopProcedures.AUTO_RESTORE_EFFECT.HasValue || !EffectProcedures.PlayerHasActiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT.Value))
                {
                    return null;
                }
            }

            var index = new Random().Next(forms.Count());
            FormDetail form = forms.ElementAt(index);

            if (!TryInanimateTransform(player, form.FormSourceId, dropInventory: dropInventory, createItem: !temporary, severe: !temporary))
            {
                return null;
            }

            if (temporary)
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You fall into the ether and are stuck as a {form.FriendlyName} for the next {duration} turns!", true);  // TODO joke_shop flavor text - ensure this message and effect message are consistent
            }

            return $"You are an inanimate {form.FriendlyName}.";  // TODO joke_shop flavor text - must inform player when they will auto revert, if they will
        }

        private static string MobileInanimateTransform(Player player)
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

            var index = new Random().Next(forms.Count());
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

            return $"You are a mobile inanimate {form.FriendlyName}.";  // TODO joke_shop flavor text
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
            var altForm = formsRepo.DbStaticForms.Where(form => form.Id == player.FormSourceId).Select(form => new { form.AltSexFormSourceId, form.Gender }).FirstOrDefault();

            if (altForm == null || !altForm.AltSexFormSourceId.HasValue)
            {
                return null;
            }

            TryAnimateTransform(player, altForm.AltSexFormSourceId.Value);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You suddenly became {altForm.Gender}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} suddenly became {altForm.Gender}.");

            return "TG";  // TODO joke_shop flavor text
        }

        private static string BodySwap(Player player, bool clone)
        {
            var rand = new Random();
            List<Player> candidates = JokeShopProcedures.ActivePlayersInJokeShopApartFrom(player);

            if (candidates.Count() == 0)
            {
                return null;
            }

            if (clone)
            {
                var victim = candidates[rand.Next(candidates.Count())];

                TryAnimateTransform(player, victim.FormSourceId);

                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have become a clone of {victim.GetFullName()}", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> became a clone of <b>{victim.GetFullName()}</b>.");

                return "Clone";  // TODO joke_shop flavor text
            }
            else
            {
                // Find nearboy player with sufficient consent
                Player victim = null;

                do
                {
                    var index = rand.Next(candidates.Count());
                    var candidate = candidates[index];

                    if (candidate.GameMode != (int)GameModeStatics.GameModes.Superprotection || JokeShopProcedures.PlayerHasBeenWarned(candidate))
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
                TryAnimateTransform(player, victim.FormSourceId);
                TryAnimateTransform(victim, player.FormSourceId);

                PlayerLogProcedures.AddPlayerLog(victim.Id, $"You have swapped bodies with {player.GetFullName()}", true);  // TODO joke_shop flavor text
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have swapped bodies with {victim.GetFullName()}", false);  // TODO joke_shop flavor text
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> swapped bodies with <b>{victim.GetFullName()}</b>.");

                return "Body swap";  // TODO joke_shop flavor text
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

            return "Restore to base";  // TODO joke_shop flavor text
        }

        private static string ChangeBaseForm(Player player)
        {
            var availableForms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MISCHIEVOUS_FORMS).ToArray();

            if (availableForms.IsEmpty())
            {
                return null;
            }

            return ChangeBaseForm(player, availableForms);
        }

        private static string ChangeBaseForm(Player player, int[] availableForms)
        {
            var formSourceId = availableForms[new Random().Next(availableForms.Count())];

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form was changed.", false);

            return "Base form changed";  // TODO joke_shop flavor text
        }

        public static string SetBaseFormToRegular(Player player)
        {
            string message;
            var formRepo = new EFDbStaticFormRepository();
            var baseForms = formRepo.DbStaticForms.Where(f => (f.FriendlyName == "Regular Guy" || f.FriendlyName == "Regular Girl") &&
                                                               f.Id != player.OriginalFormSourceId)
                                                  .Select(f => f.Id).ToArray();

            if (baseForms.Count() > 0)
            {
                ChangeBaseForm(player, baseForms);
            }
            message = "You spot a fortune cookie and open it to see the message inside:  \"True purity can only come from the deepest of cleanses.\"  Let's hope the shopkeeper didn't see you!";
            return message;
        }

        private static string SetBaseFormToCurrent(Player player)
        {
            if (player.OriginalFormSourceId == player.FormSourceId)
            {
                return null;
            }

            var formSourceId = player.FormSourceId;

            // Check we're not setting an inanimate/pet as base form, in case that causes problems...
            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var mobility = formsRepo.DbStaticForms.Where(f => f.Id == formSourceId).Select(f => f.MobilityType).FirstOrDefault();

            if (mobility != PvPStatics.MobilityFull)
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form has changed to your current form.", false);

            return "Base form set to current";  // TODO joke_shop flavor text
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

            return "Name restored";  // TODO joke_shop flavor text
        }

        private static string IdentityChange(Player player)
        {
            var rand = new Random();

            int[] forms = Array.Empty<int>();
            var firstName = player.OriginalFirstName;
            var lastName = player.OriginalLastName;
            var mindControl = false;
            var message = "";

            var roll = rand.Next(4);

            // Pick changes to name and form
            if (roll == 0)  // Dogs
            {
                forms = JokeShopProcedures.STABLE_FORMS.Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DOGS).ToArray();
                mindControl = true;
                message = "";  // TODO joke_shop flavor text

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
                message = "";  // TODO joke_shop flavor text

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
                message = "";  // TODO joke_shop flavor text

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
            else if (roll == 3)  // Renames
            {
                mindControl = false;
                message = "";  // TODO joke_shop flavor text

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
                message = "You have taken on a whole new identity!";  // TODO joke_shop flavor text
            }

            return message;
        }

        private static string TransformToMindControlledForm(Player player)
        {
            var rand = new Random();

            int[][] mcForms = {
                JokeShopProcedures.CATS_AND_NEKOS,
                JokeShopProcedures.DOGS, 
                JokeShopProcedures.MAIDS,
                JokeShopProcedures. SHEEP,
                JokeShopProcedures.STRIPPERS };
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
            return "You are now in a mind controlled form";  // TODO joke_shop flavor text
        }

        public static bool TryAnimateTransform(Player player, int formSourceId)
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

            var form = FormStatics.GetForm(formSourceId);
            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");

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

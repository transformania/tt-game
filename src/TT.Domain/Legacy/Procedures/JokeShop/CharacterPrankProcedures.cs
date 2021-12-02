using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.World.Queries;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class CharacterPrankProcedures
    {
        // Ids of stats effect sources
        public const int DISCIPLINE_BOOST = 229;
        public const int PERCEPTION_BOOST = 230;
        public const int CHARISMA_BOOST = 231;
        public const int FORTITUDE_BOOST = 232;
        public const int AGILITY_BOOST = 233;
        public const int RESTORATION_BOOST = 234;
        public const int MAGICKA_BOOST = 235;
        public const int REGENERATION_BOOST = 236;
        public const int LUCK_BOOST = 237;
        public const int INVENTORY_BOOST = 238;
        public const int MOBILITY_BOOST = 239;

        public const int DISCIPLINE_PENALTY = 240;
        public const int PERCEPTION_PENALTY = 241;
        public const int CHARISMA_PENALTY = 242;
        public const int FORTITUDE_PENALTY = 243;
        public const int AGILITY_PENALTY = 244;
        public const int RESTORATION_PENALTY = 245;
        public const int MAGICKA_PENALTY = 246;
        public const int REGENERATION_PENALTY = 247;
        public const int LUCK_PENALTY = 248;
        public const int INVENTORY_PENALTY = 249;
        public const int MOBILITY_PENALTY = 250;

        // Ids of specific and behavior-altering effect sources
        public const int SNEAK_REVEAL_1 = 226;
        public const int SNEAK_REVEAL_2 = 227;
        public const int SNEAK_REVEAL_3 = 228;

        public const int BLINDED_EFFECT = 204;
        public const int DIZZY_EFFECT = 205;
        public const int HUSHED_EFFECT = 206;

        // Stat boosting effect source IDs
        public static readonly int[] BOOST_EFFECTS = { DISCIPLINE_BOOST, PERCEPTION_BOOST, CHARISMA_BOOST, FORTITUDE_BOOST, AGILITY_BOOST, RESTORATION_BOOST, MAGICKA_BOOST, REGENERATION_BOOST, LUCK_BOOST, INVENTORY_BOOST, MOBILITY_BOOST };
        public static readonly int[] PENALTY_EFFECTS = { DISCIPLINE_PENALTY, PERCEPTION_PENALTY, CHARISMA_PENALTY, FORTITUDE_PENALTY, AGILITY_PENALTY, RESTORATION_PENALTY, MAGICKA_PENALTY, REGENERATION_PENALTY, LUCK_PENALTY, INVENTORY_PENALTY, MOBILITY_PENALTY };

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
                return GiveEffect(player, JokeShopProcedures.INSTINCT_EFFECT, merge: true);
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

        public static string GiveEffect(Player player, int effectSourceId, int? duration = null, int? cooldown = null, bool merge = false)
        {
            duration = duration ?? 3;
            cooldown = cooldown ?? duration;

            if (merge)
            {
                return EffectProcedures.MergePlayerPerk(effectSourceId, player, duration: duration, cooldown: cooldown);
            }

            if (EffectProcedures.PlayerHasEffect(player, effectSourceId))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effectSourceId, player, Duration: duration, Cooldown: cooldown);
        }

        public static string GiveRandomEffect(Player player, IEnumerable<int> effectSourceIds, Random rand = null, bool merge = false, int? duration = null, int? cooldown = null)
        {
            if (effectSourceIds.IsEmpty())
            {
                return null;
            }

            rand = rand ?? new Random();
            var effectSourceId = effectSourceIds.ElementAt(rand.Next(effectSourceIds.Count()));

            return GiveEffect(player, effectSourceId, merge: merge, duration: duration, cooldown: cooldown);
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

        public static string LiftRandomCurse(Player player, Random rand = null)
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

        public static string MakePsychotic(Player player, int? duration = null, int? cooldown = null)
        {
            duration = duration ?? 3;
            var message = GiveEffect(player, JokeShopProcedures.PSYCHOTIC_EFFECT, duration: duration, cooldown: cooldown);

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

        public static string MakeInvisible(Player player, int? duration = null, int? cooldown = null)
        {
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }
            
            duration = duration ?? 2;
            var message = GiveEffect(player, JokeShopProcedures.INVISIBILITY_EFFECT, duration: duration, cooldown: cooldown);

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

        public static void UndoInvisible(Player player)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Invisible && !EffectProcedures.PlayerHasActiveEffect(player.Id, JokeShopProcedures.PSYCHOTIC_EFFECT))
            {
                MakePlayerVisible(player.Id);
                EnsureItemsAreVisible();
            }
        }

        public static void MakePlayerVisible(int player)
        {
            var playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player);

            if (user.GameMode != (int)GameModeStatics.GameModes.Invisible)
            {
                return;
            }

            DomainRegistry.Repository.Execute(new ChangeGameMode
            {
                MembershipId = user.MembershipId,
                GameMode = (int)GameModeStatics.GameModes.PvP,
                Force = true
            });

            PlayerLogProcedures.AddPlayerLog(player, "Your cloak of invisibility starts to fade, leaving you visible to the world once more.", true);
            LocationLogProcedures.AddLocationLog(user.dbLocationName, $"{user.GetFullName()} seems to appear from nowhere!");
        }

        public static void EnsureItemsAreVisible()
        {
            var itemRepo = new EFItemRepository();
            var itemsInLimbo = itemRepo.Items.Where(i => i.PvPEnabled == (int)GameModeStatics.GameModes.Invisible);

            foreach (var item in itemsInLimbo)
            {
                item.PvPEnabled = (int)GameModeStatics.GameModes.PvP;
                itemRepo.SaveItem(item);
            }
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
                return SetBaseFormToCurrent(player, rand);
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

        public static string AnimateTransform(Player player, Random rand = null)
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

            string message;

            switch (rand.Next(9))
            {
                case 0:
                    message = $"Snap!  You're a <b>{form.FriendlyName}</b>!";
                    break;
                case 1:
                    message = $"You feel lightheaded and your feet lift from the ground.  You become weightless as your mass shifts upward, leaving you floating in midair.  Suddenly you're pulled to the ground with some force.  When you regain your senses you realize your body is different - you have become a <b>{form.FriendlyName}</b>!";
                    break;
                case 2:
                    message = $"A jar smashes at your feet and you are engulfed in a great cloud of smoke.  When the air clears you find you have become a <b>{form.FriendlyName}</b>!";
                    break;
                case 3:
                    message = $"Your're thinking of a {form.FriendlyName}.  You can't help it.  You can't get that {form.FriendlyName} out of your mind.  You want to be a {form.FriendlyName}!  You want to be <i>that</i> {form.FriendlyName}!  Admit it!  <b>Admit it!</b>  Become the {form.FriendlyName}!";
                    break;
                case 4:
                    message = $"You stumble upon another trap in this cursed shop and immediately feel your body melting, dissolving into a pool on the ground!  You desperately try to utter a counterspell, but it comes out all wrong.  Your body starts to regain form, but as it firms up you know your body is not how you remember it.  Your spell has left you as a <b>{form.FriendlyName}</b>!";
                    break;
                case 5:
                    message = $"A beam of bright light hits you from nowhere!  You feel it changing you, and when you realize it is harmless you blurt out a laugh!  But as the last tingles of energy leave your body you notice it has left you as a <b>{form.FriendlyName}</b>!";
                    break;
                case 6:
                    message = $"Argh!  Why are you in this form?  It's so wrong!  Quick, cast a spell!  Phew, back to being a <b>{form.FriendlyName}</b>!  What a relief!";
                    break;
                case 7:
                    message = $"\"Hello {player.FirstName}, my little {form.FriendlyName}\" booms an almighty voice!  \"B-but I'm not a {form.FriendlyName}!\" you protest.  \"Oh, aren't you?\" replies the voice.  You can hear the taunting smugness in its tone as the sound is carried away on the wind.";
                    break;
                default:
                    message = $"You feel the familiar tingle of your body reshaping.  When you look down you realize you are now a <b>{form.FriendlyName}</b>!";
                    break;
            }

            return message;
        }

        public static string ImmobileTransform(Player player, bool temporary, Random rand = null)
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
                GiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT, duration, merge: true);
                message = $"A mysterious fog crosses your mind and you feel yourself falling into the ether!  As you familiarize yourself with your surroundings you begin to feel giddy and confused.  You've always been a {form.FriendlyName}, haven't you?  Urgh!  That cloud is messing with your head!  It might take another {duration} turns for it to clear!";
            }
            else
            {
                switch (rand.Next(3))
                {
                    case 0:
                        message = $"\"Freeze!\" shouts someone from close behind you.  You try to face them but you can't seem to move - you're already a <b>{form.FriendlyName}</b>!";
                        break;
                    case 1:
                        message = $"You find the sights of the shop distracting... so distracting you forget to keep moving and don't notice the ground swallowing your feet up!  In fact you don't have a clue anything is wrong - until you catch sight of a <b>{form.FriendlyName}</b> looking back at you through a mirror!";
                        break;
                    default:
                        message = $"Your limbs start to stiffen, your bones begin to fuse.  By the time you realize what's happening it's too late:  You've already become a <b>{form.FriendlyName}</b>!";
                        break;
                }
            }

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            return message;
        }

        public static string InanimateTransform(Player player, bool temporary, bool dropInventory = false, Random rand = null)
        {
            var warning = JokeShopProcedures.EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var forms = JokeShopProcedures.InanimateForms();

            if (forms.IsEmpty())
            {
                return null;
            }

            var duration = 0;
            if (temporary)
            {
                duration = JokeShopProcedures.PlayerHasBeenWarnedTwice(player) ? 10 : 5;
                GiveEffect(player, JokeShopProcedures.AUTO_RESTORE_EFFECT, duration, merge: true);

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

            string message;

            switch (rand.Next(5))
            {
                case 0:
                    message = $"Suddenly you feel yourself shrinking.  Your body contorts, your skin changes texture.  It all takes place faster than gravity can respond.  Then, with a loud clatter, you land on the floor, ready for your new life as a <b>{form.FriendlyName}</b>!";
                    break;
                case 1:
                    message = $"\"{player.GetFullName()}?\" Asks the shopkeeper quizically.  \"What kind of a name is that?\"  \"It's <i>my</i> name,\" you respond.  \"Don't be silly,\" mocks the shopkeeper, \"Why would a {form.FriendlyName} have a name like that?\"  The shopkeeper then reaches down to pick you up and places you back on your shelf.";
                    break;
                case 2:
                    message = $"You feel a stranger tap you on the shoulder and the next thing you know you're falling through their fingers, landing on the ground as a <b>{form.FriendlyName}</b>!";
                    break;
                case 3:
                    message = $"A thought crosses your mind:  What would happen if those souls you are wearing could still wield magic?  What retribution might they seek?  You tense up at the thought, but a mage's misplaced idea can have a life of its own, and soon your items are beckoning you to join them as a <b>{form.FriendlyName}</b>!";
                    break;
                default:
                    message = $"All these joke items on the shelves have absorbed so much magic they're almost alive, almost calling to you, willing you to join them, to provide them with company while they remain frozen in time for all eternity.  You try to back out but the enchantment has already permeated your being, turning you into a <b>{form.FriendlyName}</b>!";
                    break;
            }

            return message;
        }

        public static string MobileInanimateTransform(Player player, Random rand = null)
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

            var forms = JokeShopProcedures.InanimateForms();

            if (forms.IsEmpty())
            {
                return null;
            }

            rand = rand ?? new Random();
            var index = rand.Next(forms.Count());
            FormDetail form = forms.ElementAt(index);

            // Give player an inanimate form without creating a player item
            if (!TryInanimateTransform(player, form.FormSourceId, dropInventory: false, createItem: false, severe: false, logChanges: false))
            {
                return null;
            }

            // Coerce player into mobility
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.Mobility = PvPStatics.MobilityFull;
            playerRepo.SavePlayer(target);

            var mobileTarget = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            mobileTarget.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(mobileTarget));
            playerRepo.SavePlayer(mobileTarget);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into an animate <b>{form.FriendlyName}</b>.");

            return $"You sense a build-up of transformative energy.  Somebody trying to make you inanimate.  Somebody?  Or this cursed place itself?  No, it can try to turn you into a {form.FriendlyName}, but you will not allow it to take your mobility!";
        }

        public static string TGTransform(Player player)
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
            return JokeShopProcedures.AnimateForms().Where(f => f.FormSourceId == player.FormSourceId).Any();
        }

        public static string BodySwap(Player player, bool clone, Random rand = null)
        {
            rand = rand ?? new Random();
            var candidates = JokeShopProcedures.ActivePlayersInJokeShopApartFrom(player)
                .Where(p => p.FormSourceId != player.FormSourceId).ToList();

            if (candidates.Count() == 0)
            {
                return null;
            }

            if (clone)  // Clone
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
            else if (PlayerCanBeCloned(player))  // Body swap
            {
                // Find nearboy player with sufficient consent
                Player victim = null;

                do
                {
                    var index = rand.Next(candidates.Count());
                    var candidate = candidates[index];

                    if ((candidate.GameMode != (int)GameModeStatics.GameModes.Superprotection || JokeShopProcedures.PlayerHasBeenWarned(candidate)) &&
                        PlayerCanBeCloned(candidate) && candidate.InQuest <= 0 && candidate.InDuel <= 0)
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

            return null;
        }

        public static void UndoTemporaryForm(int playerId)
        {
            var player = PlayerProcedures.GetPlayer(playerId);

            // Avoid reverting form if player is in a mobile animate form or has somehow rerolled
            var canRemove = false;

            // Allow for the possibility they may be a temporary psychopath rather than an active player
            if (player.BotId != AIStatics.RerolledPlayerBotId)
            {
                if (player.Mobility != PvPStatics.MobilityFull)
                {
                    // Always undo inanimate/pet forms
                    canRemove = true;
                }
                else
                {
                    // Only undo animate forms if they have limited mobility
                    IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
                    var moveActionPointDiscount = formsRepo.DbStaticForms.Where(form => form.Id == player.FormSourceId).Select(form => form.MoveActionPointDiscount).FirstOrDefault();
                    canRemove = moveActionPointDiscount < -5;
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

            if (player.OriginalFormSourceId == player.FormSourceId)
            {
                return null;
            }

            // Use chaos restore.  Should delete item & reset skills.  Self-restore, struggle restore and Classic Me! Restorative Lotion do similar
            PlayerProcedures.InstantRestoreToBase(player);

            // No Back On Your Feet, but clear any XP ready for next inanimation - unless Chaos, where people like to lock fast
            if (!PvPStatics.ChaosMode)
            {
                IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
                var inanimXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);

                if (inanimXP != null)
                {
                    inanimXpRepo.DeleteInanimateXP(inanimXP.Id);
                }
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You returned to base form.", false);

            return "You accidentally spill some Classic Me! Restorative Lotion and revert to your base form, just as you remember it!  Right?";
        }

        public static string ChangeBaseForm(Player player, Random rand = null)
        {
            var availableForms = Array.Empty<int>();
            var flavorText = "";

            var month = DateTime.UtcNow.Month;
            var day = DateTime.UtcNow.Day;

            if (month == 2)  // February: Valentines
            {
                availableForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.ROMANTIC_FORMS).ToArray();
                flavorText = "A deep and intense loving warmth flushes through you.  Your true inner self is trying to get out!";
            }
            else if (month == 4 && day < 15)  // April: Fools
            {
                availableForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MISCHIEVOUS_FORMS).ToArray();
                flavorText = "You feel something changing deep within you, but you're not sure what.  You suspect somebody's playing a trick on you, and you're not going to find out what it is just yet!";
            }
            else if ((month == 3 && day > 15) || month == 4)  // March-April: Easter
            {
                availableForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.EASTER_FORMS).ToArray();
                flavorText = "You find yourself with a smile on your face and a sudden and unexpected spring in your step.  Why is that?  You're not really sure, but it's a glorious day outside, so why not go out and enjoy it?  You'll find out why you have all that youthful energy soon enough!";
            }
            else if ((month == 10 && day > 15) || (month == 11 && day < 15))  // October-November: Halloween
            {
                availableForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.HALLOWEEN_FORMS).ToArray();
                flavorText = "A sudden icy chill causes you to freeze where you stand.  It's not so much the sense of somebody walking over your grave as an otherworldly spirit floating through you!";
            }
            else if (month == 12)  // December: Christmas
            {
                availableForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.CHRISTMAS_FORMS).ToArray();
                flavorText = "You look at an item on the shelf, pondering whether or not to buy it.  \"Don't be such a Scrooge!\" shouts the shopkeeper.  And you know what?  They're right!  Good will to all forms!  You rummage around to try and find the money you need.  Show this place some good spirit and it might return the favor.  After all, you don't want to end up out on the street again, do you?";
            }

            if (availableForms.IsEmpty())
            {
                return null;
            }

            var message = ChangeBaseForm(player, availableForms, rand);
            return flavorText.IsEmpty() ? message : flavorText;
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

            return "Your base form has been changed.";
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

        public static string SetBaseFormToCurrent(Player player, Random rand = null)
        {
            rand = rand ?? new Random();

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

            string message;

            switch(rand.Next(5))
            {
                case 0:
                    message = "You feel a strong pulse from deep within you, rocking you to your very core.  You have been many things, but nothing has resonated with you as much as your current form.";
                    break;
                case 1:
                    message = "It's a tranquil moment, so you pause to think.  You didn't realize how much you were enjoying your current form.  Perhaps you'll revisit it sometime?";
                    break;
                case 2:
                    message = "Hmmm, is that body not to your liking?  How humiliating would it be to find you couldnn't shake it?";
                    break;
                case 3:
                    message = "You get a sudden urge to take a selfie.  You whip your smartphone out from your pocket and snap some pics.  Maybe you'll look at them later.";
                    break;
                default:
                    message = "\"Are you feeling comfortable?\" asks the shopkeeper.  \"Er, yes,\" you reply.  \"Good.  That form really suits you!\"  You eye the shopkeeper with suspicion.";
                    break;
            }

            return message;
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

        public static string IdentityChange(Player player, Random rand = null)
        {
            rand = rand ?? new Random();

            int[] forms = Array.Empty<int>();
            var firstName = player.OriginalFirstName;
            var lastName = player.OriginalLastName;
            var mindControl = false;
            var message = "";

            var roll = rand.Next(12);

            // Pick changes to name and form
            if (roll == 0)  // Dogs
            {
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DOGS).ToArray();
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
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.CATS_AND_NEKOS).ToArray();
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
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.DRONES).ToArray();
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
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.GHOSTS).ToArray();
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
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.SHEEP).ToArray();
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
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MAIDS).ToArray();
                mindControl = true;
                message = "The shop is full of dust and cobwebs.  You feel it could do with a good clean.  If you enjoy sweeping the dirt away maybe you could serve others in town?";

                string[] prefixes = { "Maid", "Servant", "Miss", "Cleaner", "Waitress" };
                lastName = rand.Next(2) == 0 ? firstName : lastName;
                firstName = prefixes[rand.Next(prefixes.Count())];
            }
            else if (roll == 6)  // Strippers
            {
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.STRIPPERS).ToArray();
                mindControl = true;
                message = "You feel a sudden and irrepressible compulsion to start removing your clothes.  Is the shop too hot, or are you just doing it for attention and to show off your body to anyone who will watch?  Only you know...";

                lastName = rand.Next(2) == 0 ? firstName : lastName;
                firstName = "Stripper";
            }
            else if (roll == 7)  // Rodents
            {
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.RODENTS).ToArray();
                message = "You've never minded cats before, but now you feel terrified!";

                switch (rand.Next(2))
                {
                    case 0:
                        string[] prefixes = { "Critter", "Mousey", "Ratty", "Rodent", "Vermin" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "Creature", "Critter", "Mouse", "Rat", "Rodent", "Vermin" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                }
            }
            else if (roll == 8)  // Bimbos
            {
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.BIMBOS).ToArray();
                message = "Oh my gawd, sumthin bout u feels liek totally different n stuff but u're like not shur wat. I mean u still hav ur amazin body, rite???";

                switch (rand.Next(3))
                {
                    case 0:
                        string[] prefixes = { "Airhead", "Bimbo", "Ditzy", "Like", "Totally", "Hawt" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "Bimbo", "Ditz", "Like" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                    case 2:
                        firstName = firstName.ToLower().Replace("u", "oo").Replace("you", "u").Replace("too", "2").Replace("to", "2").Replace("s", "z").Replace("ng", "n").Replace("th", "d").Replace("an", "n").Replace("er", "a");
                        lastName = lastName.ToLower().Replace("u", "oo").Replace("you", "u").Replace("too", "2").Replace("to", "2").Replace("s", "z").Replace("ng", "n").Replace("th", "d").Replace("an", "n").Replace("er", "a");
                        firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);
                        lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                        break;
                }
            }
            else if (roll == 9)  // Thieves
            {
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.THIEVES).ToArray();
                message = "So many valuable items on these shelves.. surely it wouldn't hurt if you took just one?";

                switch (rand.Next(2))
                {
                    case 0:
                        string[] prefixes = { "Thieving", "Pilfering", "Shoplifter", "Looting", "Bandit", "Highwayman", "Sly", "Light-Fingered", "Stealthy", "Pickpocket", "Sneaky" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "The Thief", "The Light-Fingered", "The Sly", "Moneygrabber" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                }
            }
            else if (roll == 10)  // Fairies
            {
                forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.FAIRIES).ToArray();
                message = "Some dust glimmers as it passes through a shaft of light.  It reminds you of the fairies from the grove.  Wait.. what kind of dust was that?";

                switch (rand.Next(2))
                {
                    case 0:
                        string[] prefixes = { "Fairy", "Fae", "Floral" };
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = { "Fairy", "Fae" };
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                }
            }
            else if (roll == 11)  // Renames
            {
                mindControl = false;
                message = "A heady fragrance fills the air, numbing your mind.  You forget who you are for a moment, but then it comes back to you.  How could you forget your own name like that?!";

                var nouns = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.DungeonNouns.xml");
                var adjectives = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.DungeonAdjectives.xml");

                switch (rand.Next(7))
                {
                    case 0:
                        lastName = $"Mc{firstName}face";
                        if (!firstName.EndsWith("y"))
                        {
                            firstName = $"{firstName}y";
                        }
                        break;

                    case 1:
                        char[] characters = firstName.ToLower().ToCharArray();
                        Array.Reverse(characters);
                        firstName = new String(characters);
                        firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);

                        characters = lastName.ToLower().ToCharArray();
                        Array.Reverse(characters);
                        lastName = new String(characters);
                        lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                        break;

                    case 2:
                        firstName = firstName.Replace("d", "_").Replace("b", "d").Replace("p", "b").Replace("q", "p").Replace("_", "q")
                                             .Replace("m", "_").Replace("w", "m").Replace("_", "w")
                                             .Replace("u", "_").Replace("n", "u").Replace("_", "n")
                                             .Replace("s", "_").Replace("z", "s").Replace("_", "z");
                        lastName = lastName.Replace("d", "_").Replace("b", "d").Replace("p", "b").Replace("q", "p").Replace("_", "q")
                                           .Replace("m", "_").Replace("w", "m").Replace("_", "w")
                                           .Replace("u", "_").Replace("n", "u").Replace("_", "n")
                                           .Replace("s", "_").Replace("z", "s").Replace("_", "z");
                        break;

                    case 3:
                        lastName = $"The {adjectives[rand.Next(adjectives.Count())]}";
                        break;

                    case 4:
                        lastName = $"The {nouns[rand.Next(nouns.Count())]}";
                        if (rand.Next(2) == 0)
                        {
                            lastName = "Of " + lastName;
                        }
                        break;

                    case 5:
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = adjectives[rand.Next(adjectives.Count())];
                        break;

                    case 6:
                        var firstNameStart = firstName.Substring(0, 1);
                        firstName = lastName.Substring(0, 1) + firstName.Substring(1);
                        lastName = firstNameStart + lastName.Substring(1);
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
                GiveEffect(player, JokeShopProcedures.INSTINCT_EFFECT, merge: true);
            }

            if (message.IsNullOrEmpty())
            {
                message = "You are shocked to discover you have taken on a whole new identity!";
            }

            return message;
        }

        public static string TransformToMindControlledForm(Player player, Random rand = null)
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
            var forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(genre).ToArray();

            if (forms == null || forms.IsEmpty())
            {
                return null;
            }

            if (!TryAnimateTransform(player, forms[rand.Next(forms.Count())]))
            {
                return null;
            }

            GiveEffect(player, JokeShopProcedures.INSTINCT_EFFECT, merge: true);
            return "A shrieking laugh turns the air to ice, freezing you where you stand.  \"Now you're mine!\" comes a the bloodcurdling taunt of a witch directly above you!  You then feel your arms moving involuntarily, being hoisted up by your wrists as if on strings, forcing you to perform a little jig against your will.  \"I will control your body, and I will control your brain!\" cackles the voice from overhead.  You try to run free, but you can still hear that voice in your mind:  \"You're still mine, wherever you go, and you will behave as I have made you!\"  Then as a freak gust of wind hits, you start to feel.. different..";
        }

        public static bool TryAnimateTransform(Player player, int formSourceId, bool logChanges = true)
        {
            // Require extra warning for SP players, who might want to keep their form
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !JokeShopProcedures.PlayerHasBeenWarned(player))
            {
                return false;
            }

            if (player.FormSourceId == formSourceId)
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);

            if (logChanges)
            {
                var form = FormStatics.GetForm(formSourceId);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");
            }

            return true;
        }

        private static bool TryInanimateTransform(Player player, int formSourceId, bool dropInventory, bool createItem = true, bool severe = true, bool logChanges = true)
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

                SetInitialItemXP(player);
            }
            else if (dropInventory)
            {
                DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = player.Id, IgnoreRunes = false });
            }

            if (logChanges)
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");
            }

            return true;
        }

        private static void SetInitialItemXP(Player player)
        {
            var inanimXpRepo = new EFInanimateXPRepository();

            if (inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id) != null)
            {
                // Previous XP - player probably chaos restored.  Let them keep it.
                return;
            }

            var currentGameTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            var xp = new InanimateXP
            {
                OwnerId = player.Id,
                Amount = 0,
                // Give player a head start on struggling - low enough to be fun, high enough that using it to farm AP is not worthwhile
                TimesStruggled = -Math.Min(50, 2 + 4 * player.Level),
                LastActionTimestamp = DateTime.UtcNow,
                LastActionTurnstamp = currentGameTurn - 1,
            };

            inanimXpRepo.SaveInanimateXP(xp);
        }

        public static string BossPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();

            var world = DomainRegistry.Repository.FindSingle(new GetWorld());

            if (world.Boss_Bimbo == AIStatics.ACTIVE)
            {
                if (rand.Next(5) == 0)
                {
                    return ItemProcedures.GiveNewItemToPlayer(player, BossProcedures_BimboBoss.CureItemSourceId);
                }
                else if (TryAnimateTransform(player, BossProcedures_BimboBoss.RegularBimboFormSourceId))
                {
                    return "U hear there's a liek supes smart syentist in town who u hav totes got 2 c!!!";
                }
            }

            else if (world.Boss_Donna == AIStatics.ACTIVE)
            {
                if (rand.Next(4) == 0 && TryAnimateTransform(player, 684))  // Farmer's daughter
                {
                    return "You hear a voice calling you from the streets, demanding you go to the ranch.  There's so much work to be done and you must do as you're told.  Somebody needs to take care of the farm and prepare the enclosures for all the new animals!";
                }
                else
                {
                    int[] farmAnimalishSubs = { 34, 40, 123, 204, 294, 431, 455, 637, 924, 932, 950, 961, 1007, 1035, 1043, 1074, 1123 };

                    if (CharacterPrankProcedures.TryAnimateTransform(player, farmAnimalishSubs[rand.Next(farmAnimalishSubs.Count())]))
                    {
                        return "There is a great power out on the streets, calling to your animal instincts.  There is a place for you on the ranch, and a simpler life.  All you need to do is listen to that voice, join your kind on the farm, and obey your Mistress...";
                    }
                }
            }

            else if (world.Boss_Faeboss == AIStatics.ACTIVE)
            {
                var forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.FAIRIES).ToArray();

                if (CharacterPrankProcedures.TryAnimateTransform(player, forms[rand.Next(forms.Count())]))
                {
                    return "You run into the shop and slam the door behind you, your back pressed to it as you pant heavily, narrowly escaping the abject terror of the purge out on the streets.  The distant sound of piercing crackles grows ever closer, each fizzling scream a soul lost to the mad conquest of a fallen fairy.  You can't keep running.  It's time to pick a side:  Are you on the side of light, or the side or dark?";
                }
            }

            else if (world.Boss_MotorcycleGang == AIStatics.ACTIVE && TryAnimateTransform(player, BossProcedures_MotorcycleGang.BikerFollowerFormSourceId))
            {
                return "You hear the engines revving up outside.. they're waiting for you!  You have to go now - or they might leave without you!";
            }

            else if (world.Boss_Sisters == AIStatics.ACTIVE)
            {
                if (rand.Next(2) == 0 && TryAnimateTransform(player, BossProcedures_Sisters.BimboSpellFormSourceId))
                {
                    return "Like OMG!!  Those stoopid nerds fink there silly brains r eva sooo important!! Why do they spend all dat time studyin when it's sooo much more fun lookin hawt?!?  U just hav 2 flutter ur cute eyelashes n dat body will get u nythin u want!!";
                }
                else if (TryAnimateTransform(player, BossProcedures_Sisters.NerdSpellFormSourceId))
                {
                    return "Those dumb bimbos don't know a thing!  It's time to show them brains win out over beauty every time!";
                }
            }

            else if (world.Boss_Thief == AIStatics.ACTIVE)
            {

                if (rand.Next(2) == 0)
                {
                    var forms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.THIEVES).ToArray();

                    if (CharacterPrankProcedures.TryAnimateTransform(player, forms[rand.Next(forms.Count())]))
                    {
                        return "Ah, the experts are in town!  And every good thief needs an accomplice!  But you want to become the master!";
                    }
                }
                else
                {
                    int[] richForms = { 141, 234, 497, 1151 };

                    if (CharacterPrankProcedures.TryAnimateTransform(player, richForms[rand.Next(richForms.Count())]))
                    {
                        PlayerProcedures.GiveMoneyToPlayer(player, 300 + rand.Next(401));
                        return "Hold onto your gold!  There are sneaky thieves about who might want it for themselves!!";
                    }
                }
            }

            // Skip Vaentine/Krampus for now

            return null;
        }

        #endregion

    }
}

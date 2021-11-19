using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class JokeShopAdminProcedures
    {
        public static string RunAction(Player victim, JokeShopActionViewModel input)
        {
            switch (input.Action)
            {
                case JokeShopActions.None:
                    break;
                case JokeShopActions.WarnPlayer:
                    return JokeShopProcedures.EnsurePlayerIsWarned(victim, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.RemindPlayer:
                    return JokeShopProcedures.EnsurePlayerIsWarnedTwice(victim, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.BanPlayer:
                    return JokeShopProcedures.BanCharacter(victim, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.UnbanPlayer:
                    return RemoveEffect(victim, JokeShopProcedures.BANNED_FROM_JOKE_SHOP_EFFECT, "Lifted Joke Shop ban on player");
                case JokeShopActions.EjectPlayer:
                    return JokeShopProcedures.EjectCharacter(victim);
                case JokeShopActions.EjectOfflinePlayers:
                    JokeShopProcedures.EjectOfflineCharacters();
                    return "Ejected offline players";

                case JokeShopActions.MildPrank:
                    return JokeShopProcedures.MildPrank(victim);
                case JokeShopActions.MischievousPrank:
                    return JokeShopProcedures.MischievousPrank(victim);
                case JokeShopActions.MeanPrank:
                    return JokeShopProcedures.MeanPrank(victim);

                case JokeShopActions.Search:
                    return JokeShopProcedures.Search(victim);
                case JokeShopActions.Cleanse:
                    return JokeShopProcedures.Cleanse(victim);
                case JokeShopActions.Meditate:
                    return JokeShopProcedures.Meditate(victim);
                case JokeShopActions.SelfRestore:
                    return JokeShopProcedures.SelfRestore(victim);

                case JokeShopActions.Activate:
                    JokeShopProcedures.SetJokeShopActive(true);
                    return "Joke shop activated";
                case JokeShopActions.Deactivate:
                    JokeShopProcedures.SetJokeShopActive(false);
                    return "Joke shop deactivated";
                case JokeShopActions.Relocate:
                    LocationsStatics.MoveJokeShop();
                    return "Joke Shop moved";

                case JokeShopActions.AnimateSafetyNet:
                    return JokeShopProcedures.Restore(victim);
                case JokeShopActions.BlowWhistle:
                    AIDirectiveProcedures.DeaggroPsychopathsOnPlayer(victim);
                    return "Whistle blown";

                case JokeShopActions.DiceGame:
                    return NovelPrankProcedures.DiceGame(victim);
                case JokeShopActions.RandomShout:
                    return NovelPrankProcedures.RandomShout(victim);
                case JokeShopActions.CombatRadar:
                    return NovelPrankProcedures.LocatePlayerInCombat(victim);
                case JokeShopActions.RareFind:
                    return EnvironmentPrankProcedures.RareFind(victim);

                case JokeShopActions.SummonPsychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, aggro: input.PsychoAggro);
                case JokeShopActions.SummonEvilTwin:
                    return NovelPrankProcedures.SummonDoppelganger(victim, aggro: input.PsychoAggro);
                case JokeShopActions.OpenPsychoNip:
                    return NovelPrankProcedures.OpenPsychoNip(victim);

                case JokeShopActions.SummonLvl1Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 0, aggro: input.PsychoAggro);
                case JokeShopActions.SummonLvl3Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 1, aggro: input.PsychoAggro);
                case JokeShopActions.SummonLvl5Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 2, aggro: input.PsychoAggro);
                case JokeShopActions.SummonLvl7Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 3, aggro: input.PsychoAggro);
                case JokeShopActions.SummonLvl9Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 4, aggro: input.PsychoAggro);
                case JokeShopActions.SummonLvl11Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 5, aggro: input.PsychoAggro);
                case JokeShopActions.SummonLvl13Psychopath:
                    return NovelPrankProcedures.SummonPsychopath(victim, strengthOverride: 6, aggro: input.PsychoAggro);

                case JokeShopActions.PlaceBounty:
                    return NovelPrankProcedures.PlaceBountyOnPlayersHead(victim);
                case JokeShopActions.AwardChallenge:
                    {
                        var minDuration = input.MinChallengeDuration ?? 1;
                        var maxDuration = input.MaxChallengeDuration ?? 480;
                        var penalties = (bool?)null;
                        return NovelPrankProcedures.AwardChallenge(victim, minDuration, maxDuration, penalties);
                    }
                case JokeShopActions.ClearChallenge:
                    foreach(var challengeType in ChallengeProcedures.CHALLENGE_TYPES)
                    {
                        RemoveEffect(victim, challengeType.EffectSourceId);
                    }
                    return "Challenge cleared";
                case JokeShopActions.CheckChallenge:
                    ChallengeProcedures.CheckChallenge(victim, false);
                    return "Challenge checked";

                case JokeShopActions.ForceAttack:
                    return NovelPrankProcedures.ForceAttack(victim);
                case JokeShopActions.Incite:
                    return NovelPrankProcedures.Incite(victim);

                case JokeShopActions.FillInventory:
                    return EnvironmentPrankProcedures.FillInventory(victim, overflow: false);

                case JokeShopActions.LearnSpell:
                    return EnvironmentPrankProcedures.LearnSpell(victim);
                case JokeShopActions.UnlearnSpell:
                    return EnvironmentPrankProcedures.UnlearnSpell(victim);
                case JokeShopActions.BlockAttacks:
                    return EnvironmentPrankProcedures.BlockAttacks(victim);
                case JokeShopActions.BlockCleanses:
                    return EnvironmentPrankProcedures.BlockCleanseMeditates(victim);
                case JokeShopActions.BlockItemUses:
                    return EnvironmentPrankProcedures.BlockItemUses(victim);
                case JokeShopActions.ResetCombatTimer:
                    return EnvironmentPrankProcedures.ResetCombatTimer(victim);
                case JokeShopActions.ResetActivityTimer:
                    EnvironmentPrankProcedures.ResetActivityTimer(victim);
                    return "Activity timer reset";
                case JokeShopActions.LiftRandomCurse:
                    return CharacterPrankProcedures.LiftRandomCurse(victim);

                case JokeShopActions.Boost:
                    return CharacterPrankProcedures.GiveRandomEffect(victim, CharacterPrankProcedures.BOOST_EFFECTS, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.DisciplineBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.DISCIPLINE_BOOST);
                case JokeShopActions.PerceptionBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.PERCEPTION_BOOST);
                case JokeShopActions.CharismaBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.CHARISMA_BOOST);
                case JokeShopActions.FortitudeBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.FORTITUDE_BOOST);
                case JokeShopActions.AgilityBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.AGILITY_BOOST);
                case JokeShopActions.RestorationBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.RESTORATION_BOOST);
                case JokeShopActions.MagickaBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.MAGICKA_BOOST);
                case JokeShopActions.RegenerationBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.REGENERATION_BOOST);
                case JokeShopActions.LuckBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.LUCK_BOOST);
                case JokeShopActions.InventoryBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.INVENTORY_BOOST);
                case JokeShopActions.MobilityBoost:
                    return GiveEffect(victim, input, CharacterPrankProcedures.MOBILITY_BOOST);

                case JokeShopActions.Penalty:
                    return CharacterPrankProcedures.GiveRandomEffect(victim, CharacterPrankProcedures.PENALTY_EFFECTS, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.DisciplinePenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.DISCIPLINE_PENALTY);
                case JokeShopActions.PerceptionPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.PERCEPTION_PENALTY);
                case JokeShopActions.CharismaPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.CHARISMA_PENALTY);
                case JokeShopActions.FortitudePenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.FORTITUDE_PENALTY);
                case JokeShopActions.AgilityPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.AGILITY_PENALTY);
                case JokeShopActions.RestorationPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.RESTORATION_PENALTY);
                case JokeShopActions.MagickaPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.MAGICKA_PENALTY);
                case JokeShopActions.RegenerationPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.REGENERATION_PENALTY);
                case JokeShopActions.LuckPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.LUCK_PENALTY);
                case JokeShopActions.InventoryPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.INVENTORY_PENALTY);
                case JokeShopActions.MobilityPenalty:
                    return GiveEffect(victim, input, CharacterPrankProcedures.MOBILITY_PENALTY);

                case JokeShopActions.Blind:
                    return GiveEffect(victim, input, CharacterPrankProcedures.BLINDED_EFFECT);
                case JokeShopActions.Dizzy:
                    return GiveEffect(victim, input, CharacterPrankProcedures.DIZZY_EFFECT);
                case JokeShopActions.Hush:
                    return GiveEffect(victim, input, CharacterPrankProcedures.HUSHED_EFFECT);
                case JokeShopActions.SneakLow:
                    return GiveEffect(victim, input, CharacterPrankProcedures.SNEAK_REVEAL_1);
                case JokeShopActions.SneakMedium:
                    return GiveEffect(victim, input, CharacterPrankProcedures.SNEAK_REVEAL_2);
                case JokeShopActions.SneakHigh:
                    return GiveEffect(victim, input, CharacterPrankProcedures.SNEAK_REVEAL_3);
                case JokeShopActions.MakeInvisible:
                    return CharacterPrankProcedures.MakeInvisible(victim, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.UndoInvisible:
                    RemoveEffect(victim, JokeShopProcedures.INVISIBILITY_EFFECT);
                    CharacterPrankProcedures.UndoInvisible(victim);
                    return "Triggered undo invisible";
                case JokeShopActions.UndoInvisibleItems:
                    CharacterPrankProcedures.EnsureItemsAreVisible();
                    return "Invisible items fixed";
                case JokeShopActions.MakePsychotic:
                    return CharacterPrankProcedures.MakePsychotic(victim, duration: input.EffectDuration, cooldown: input.EffectCooldown);
                case JokeShopActions.UndoPsychotic:
                    RemoveEffect(victim, JokeShopProcedures.PSYCHOTIC_EFFECT);
                    return CharacterPrankProcedures.UndoPsychotic(victim.Id);
                case JokeShopActions.AutoRestore:
                    return GiveEffect(victim, input, JokeShopProcedures.AUTO_RESTORE_EFFECT);
                case JokeShopActions.ClearAutoRestore:
                    return RemoveEffect(victim, JokeShopProcedures.AUTO_RESTORE_EFFECT, "Player will no longer autorestore.<br><b>Important:</b>  If they are a lost item they will be trapped in limbo and require you to give them a form change in order to escape!");

                case JokeShopActions.TeleportToOverworld:
                    return EnvironmentPrankProcedures.TeleportToOverworld(victim, root: false, curse: false);
                case JokeShopActions.TeleportToDungeon:
                    return EnvironmentPrankProcedures.TeleportToDungeon(victim, meanness: 0);
                case JokeShopActions.TeleportToFriendlyNPC:
                    return EnvironmentPrankProcedures.TeleportToFriendlyNPC(victim);
                case JokeShopActions.TeleportToHostileNPC:
                    return EnvironmentPrankProcedures.TeleportToHostileNPC(victim, attack: false);
                case JokeShopActions.TeleportToBar:
                    return EnvironmentPrankProcedures.TeleportToBar(victim, root: false);
                case JokeShopActions.TeleportToQuest:
                    return EnvironmentPrankProcedures.TeleportToQuest(victim);
                case JokeShopActions.RunAway:
                    return EnvironmentPrankProcedures.RunAway(victim);
                case JokeShopActions.WanderAimlessly:
                    return EnvironmentPrankProcedures.WanderAimlessly(victim);

                case JokeShopActions.AnimateTransform:
                    return CharacterPrankProcedures.AnimateTransform(victim);
                case JokeShopActions.ImmobileTransform:
                    return CharacterPrankProcedures.ImmobileTransform(victim, temporary: false);
                case JokeShopActions.InanimateTransform:
                    return CharacterPrankProcedures.InanimateTransform(victim, temporary: false);
                case JokeShopActions.LostItemTransform:
                    return CharacterPrankProcedures.InanimateTransform(victim, temporary: true);
                case JokeShopActions.MobileInanimateTransform:
                    return CharacterPrankProcedures.MobileInanimateTransform(victim);
                case JokeShopActions.TGTransform:
                    return CharacterPrankProcedures.TGTransform(victim);
                case JokeShopActions.BodySwap:
                    return CharacterPrankProcedures.BodySwap(victim, clone: false);
                case JokeShopActions.Clone:
                    return CharacterPrankProcedures.BodySwap(victim, clone: true);
                case JokeShopActions.UndoTemporaryForm:
                    CharacterPrankProcedures.UndoTemporaryForm(victim.Id);
                    return RemoveEffect(victim, JokeShopProcedures.AUTO_RESTORE_EFFECT, "Triggered undo of temporary form");
                case JokeShopActions.RestoreBaseForm:
                    return CharacterPrankProcedures.RestoreBaseForm(victim);
                case JokeShopActions.RestoreName:
                    return CharacterPrankProcedures.RestoreName(victim);
                case JokeShopActions.IdentityChange:
                    return CharacterPrankProcedures.IdentityChange(victim);
                case JokeShopActions.TransformToMindControlledForm:
                    return CharacterPrankProcedures.TransformToMindControlledForm(victim);

                case JokeShopActions.ChangeBaseForm:
                    return CharacterPrankProcedures.ChangeBaseForm(victim);
                case JokeShopActions.SetBaseFormToRegular:
                    return CharacterPrankProcedures.SetBaseFormToRegular(victim);
                case JokeShopActions.SetBaseFormToCurrent:
                    return CharacterPrankProcedures.SetBaseFormToCurrent(victim);

                case JokeShopActions.BossPrank:
                    return CharacterPrankProcedures.BossPrank(victim);

                case JokeShopActions.Update:
                    // Unreachable = case should have been handled by earlier code
                    break;
            }

            return null;
        }

        private static string RemoveEffect(Player victim, int effectSourceId, string message = null)
        {
            if (!EffectProcedures.PlayerHasEffect(victim, effectSourceId))
            {
                return null;
            }

            EffectProcedures.RemovePerkFromPlayer(effectSourceId, victim);
            return message;
        }

        private static string GiveEffect(Player victim, JokeShopActionViewModel input, int effectSourceId)
        {
            return CharacterPrankProcedures.GiveEffect(victim, effectSourceId, duration: input.EffectDuration, cooldown: input.EffectCooldown);
        }
    }
}

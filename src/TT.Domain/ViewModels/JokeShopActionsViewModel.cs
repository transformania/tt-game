namespace TT.Domain.ViewModels
{
    public enum JokeShopActions
    {
        None = 0,

        WarnPlayer,
        RemindPlayer,
        BanPlayer,
        UnbanPlayer,
        EjectPlayer,
        EjectOfflinePlayers,

        MildPrank,
        MischievousPrank,
        MeanPrank,

        Search,
        Cleanse,
        Meditate,
        SelfRestore,

        Activate,
        Deactivate,
        Relocate,

        AnimateSafetyNet,
        BlowWhistle,

        DiceGame,
        RandomShout,
        CombatRadar,
        RareFind,

        SummonPsychopath,
        SummonEvilTwin,
        OpenPsychoNip,

        SummonLvl1Psychopath,
        SummonLvl3Psychopath,
        SummonLvl5Psychopath,
        SummonLvl7Psychopath,
        SummonLvl9Psychopath,
        SummonLvl11Psychopath,
        SummonLvl13Psychopath,

        PlaceBounty,
        AwardChallenge,
        ClearChallenge,
        CheckChallenge,

        ForceAttack,
        Incite,

        FillInventory,

        LearnSpell,
        UnlearnSpell,
        BlockAttacks,
        BlockCleanses,
        BlockItemUses,
        ResetCombatTimer,
        ResetActivityTimer,
        LiftRandomCurse,

        Boost,
        DisciplineBoost,
        PerceptionBoost,
        CharismaBoost,
        FortitudeBoost,
        AgilityBoost,
        RestorationBoost,
        MagickaBoost,
        RegenerationBoost,
        LuckBoost,
        InventoryBoost,
        MobilityBoost,

        Penalty,
        DisciplinePenalty,
        PerceptionPenalty,
        CharismaPenalty,
        FortitudePenalty,
        AgilityPenalty,
        RestorationPenalty,
        MagickaPenalty,
        RegenerationPenalty,
        LuckPenalty,
        InventoryPenalty,
        MobilityPenalty,

        Blind,
        Dizzy,
        Hush,
        SneakLow,
        SneakMedium,
        SneakHigh,
        MakeInvisible,
        UndoInvisible,
        UndoInvisibleItems,
        MakePsychotic,
        UndoPsychotic,
        AutoRestore,
        ClearAutoRestore,

        TeleportToOverworld,
        TeleportToDungeon,
        TeleportToFriendlyNPC,
        TeleportToHostileNPC,
        TeleportToBar,
        TeleportToQuest,
        RunAway,
        WanderAimlessly,

        AnimateTransform,
        ImmobileTransform,
        InanimateTransform,
        LostItemTransform,
        MobileInanimateTransform,
        TGTransform,
        Clone,
        BodySwap,
        UndoTemporaryForm,
        RestoreBaseForm,
        RestoreName,
        IdentityChange,
        TransformToMindControlledForm,

        ChangeBaseForm,
        SetBaseFormToRegular,
        SetBaseFormToCurrent,

        BossPrank,

        Update
    }

    public class JokeShopActionViewModel
    {
        public int Id { get; set; }
        public JokeShopActions Action { get; set; }
        public int? EffectDuration { get; set; }
        public int? EffectCooldown { get; set; }
        public int? MinChallengeDuration { get; set; }
        public int? MaxChallengeDuration { get; set; }
        public bool? ReturnToPage { get; set; }
        public bool? PsychoAggro { get; set; }
    }
}

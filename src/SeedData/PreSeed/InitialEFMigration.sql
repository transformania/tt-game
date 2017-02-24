SET NOCOUNT ON
USE [Stats]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 2/25/2017 9:50:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AchievementBadges]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AchievementBadges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[AchievementType] [nvarchar](max) NULL,
	[Round] [nvarchar](max) NULL,
	[Amount] [real] NOT NULL,
 CONSTRAINT [PK_dbo.AchievementBadges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Achievements]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Achievements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[Amount] [real] NOT NULL,
	[AchievementType] [nvarchar](max) NULL,
	[TimesEarned] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Achievements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AIDirectives]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AIDirectives](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[State] [nvarchar](max) NULL,
	[TargetPlayerId] [int] NOT NULL,
	[TargetLocation] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[Var1] [decimal](18, 2) NOT NULL,
	[Var2] [decimal](18, 2) NOT NULL,
	[Var3] [decimal](18, 2) NOT NULL,
	[Var4] [decimal](18, 2) NOT NULL,
	[Var5] [decimal](18, 2) NOT NULL,
	[sVar1] [nvarchar](max) NULL,
	[sVar2] [nvarchar](max) NULL,
	[sVar3] [nvarchar](max) NULL,
	[DoNotRecycleMe] [bit] NOT NULL,
	[SpawnTurn] [int] NOT NULL,
 CONSTRAINT [PK_dbo.AIDirectives] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AuthorArtistBios]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuthorArtistBios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[PlayerNamePrivacyLevel] [int] NOT NULL,
	[OtherNames] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Url1] [nvarchar](max) NULL,
	[Url2] [nvarchar](max) NULL,
	[Url3] [nvarchar](max) NULL,
	[Text] [nvarchar](max) NULL,
	[AcceptingComissions] [int] NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
	[AnimateImages] [nvarchar](max) NULL,
	[InanimateImages] [nvarchar](max) NULL,
	[AnimalImages] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AuthorArtistBios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BlacklistEntries]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BlacklistEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreatorMembershipId] [nvarchar](128) NULL,
	[TargetMembershipId] [nvarchar](128) NULL,
	[Timestamp] [datetime] NOT NULL,
	[BlacklistLevel] [int] NOT NULL,
 CONSTRAINT [PK_dbo.BlacklistEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BookReadings]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookReadings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[BookDbName] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.BookReadings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BossDamages]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BossDamages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[BossBotId] [int] NOT NULL,
	[PlayerAttacksOnBoss] [int] NOT NULL,
	[BossAttacksOnPlayer] [int] NOT NULL,
	[PlayerDamageOnBoss] [real] NOT NULL,
	[BossDamageOnPlayer] [real] NOT NULL,
	[TotalPoints] [real] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.BossDamages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Buffs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Buffs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HealthBonusPercent] [decimal](18, 2) NOT NULL,
	[ManaBonusPercent] [decimal](18, 2) NOT NULL,
	[ExtraSkillCriticalPercent] [decimal](18, 2) NOT NULL,
	[HealthRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[ManaRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[SneakPercent] [decimal](18, 2) NOT NULL,
	[EvasionPercent] [decimal](18, 2) NOT NULL,
	[EvasionNegationPercent] [decimal](18, 2) NOT NULL,
	[MeditationExtraMana] [decimal](18, 2) NOT NULL,
	[CleanseExtraHealth] [decimal](18, 2) NOT NULL,
	[MoveActionPointDiscount] [decimal](18, 2) NOT NULL,
	[SpellExtraTFEnergyPercent] [decimal](18, 2) NOT NULL,
	[SpellExtraHealthDamagePercent] [decimal](18, 2) NOT NULL,
	[CleanseExtraTFEnergyRemovalPercent] [decimal](18, 2) NOT NULL,
	[SpellMisfireChanceReduction] [decimal](18, 2) NOT NULL,
	[SpellHealthDamageResistance] [decimal](18, 2) NOT NULL,
	[SpellTFEnergyDamageResistance] [decimal](18, 2) NOT NULL,
	[ExtraInventorySpace] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.Buffs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Characters]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Characters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Form] [nvarchar](max) NULL,
	[Health] [decimal](18, 2) NOT NULL,
	[Mana] [decimal](18, 2) NOT NULL,
	[HealthMax] [decimal](18, 2) NOT NULL,
	[ManaMax] [decimal](18, 2) NOT NULL,
	[SimpleMembershipId] [nvarchar](128) NULL,
	[AtScene] [nvarchar](max) NULL,
	[LastDbSave] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Characters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChatLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChatLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[Room] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ChatLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Contributions]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contributions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[SubmitterName] [nvarchar](max) NULL,
	[AdditionalSubmitterNames] [nvarchar](max) NULL,
	[SubmitterUrl] [nvarchar](max) NULL,
	[Skill_FriendlyName] [nvarchar](max) NULL,
	[Skill_FormFriendlyName] [nvarchar](max) NULL,
	[Skill_Description] [nvarchar](max) NULL,
	[Skill_ManaCost] [decimal](18, 2) NOT NULL,
	[Skill_TFPointsAmount] [decimal](18, 2) NOT NULL,
	[Skill_HealthDamageAmount] [decimal](18, 2) NOT NULL,
	[Skill_LearnedAtLocationOrRegion] [nvarchar](max) NULL,
	[Skill_LearnedAtRegion] [nvarchar](max) NULL,
	[Skill_DiscoveryMessage] [nvarchar](max) NULL,
	[Skill_IsPlayerLearnable] [bit] NOT NULL,
	[Form_FriendlyName] [nvarchar](max) NULL,
	[Form_Description] [nvarchar](max) NULL,
	[Form_TFEnergyRequired] [decimal](18, 2) NOT NULL,
	[Form_Gender] [nvarchar](max) NULL,
	[Form_MobilityType] [nvarchar](max) NULL,
	[Form_BecomesItemDbName] [nvarchar](max) NULL,
	[Form_Bonuses] [nvarchar](max) NULL,
	[Form_TFMessage_20_Percent_1st] [nvarchar](max) NULL,
	[Form_TFMessage_40_Percent_1st] [nvarchar](max) NULL,
	[Form_TFMessage_60_Percent_1st] [nvarchar](max) NULL,
	[Form_TFMessage_80_Percent_1st] [nvarchar](max) NULL,
	[Form_TFMessage_100_Percent_1st] [nvarchar](max) NULL,
	[Form_TFMessage_Completed_1st] [nvarchar](max) NULL,
	[Form_TFMessage_20_Percent_1st_M] [nvarchar](max) NULL,
	[Form_TFMessage_40_Percent_1st_M] [nvarchar](max) NULL,
	[Form_TFMessage_60_Percent_1st_M] [nvarchar](max) NULL,
	[Form_TFMessage_80_Percent_1st_M] [nvarchar](max) NULL,
	[Form_TFMessage_100_Percent_1st_M] [nvarchar](max) NULL,
	[Form_TFMessage_Completed_1st_M] [nvarchar](max) NULL,
	[Form_TFMessage_20_Percent_1st_F] [nvarchar](max) NULL,
	[Form_TFMessage_40_Percent_1st_F] [nvarchar](max) NULL,
	[Form_TFMessage_60_Percent_1st_F] [nvarchar](max) NULL,
	[Form_TFMessage_80_Percent_1st_F] [nvarchar](max) NULL,
	[Form_TFMessage_100_Percent_1st_F] [nvarchar](max) NULL,
	[Form_TFMessage_Completed_1st_F] [nvarchar](max) NULL,
	[Form_TFMessage_20_Percent_3rd] [nvarchar](max) NULL,
	[Form_TFMessage_40_Percent_3rd] [nvarchar](max) NULL,
	[Form_TFMessage_60_Percent_3rd] [nvarchar](max) NULL,
	[Form_TFMessage_80_Percent_3rd] [nvarchar](max) NULL,
	[Form_TFMessage_100_Percent_3rd] [nvarchar](max) NULL,
	[Form_TFMessage_Completed_3rd] [nvarchar](max) NULL,
	[Form_TFMessage_20_Percent_3rd_M] [nvarchar](max) NULL,
	[Form_TFMessage_40_Percent_3rd_M] [nvarchar](max) NULL,
	[Form_TFMessage_60_Percent_3rd_M] [nvarchar](max) NULL,
	[Form_TFMessage_80_Percent_3rd_M] [nvarchar](max) NULL,
	[Form_TFMessage_100_Percent_3rd_M] [nvarchar](max) NULL,
	[Form_TFMessage_Completed_3rd_M] [nvarchar](max) NULL,
	[Form_TFMessage_20_Percent_3rd_F] [nvarchar](max) NULL,
	[Form_TFMessage_40_Percent_3rd_F] [nvarchar](max) NULL,
	[Form_TFMessage_60_Percent_3rd_F] [nvarchar](max) NULL,
	[Form_TFMessage_80_Percent_3rd_F] [nvarchar](max) NULL,
	[Form_TFMessage_100_Percent_3rd_F] [nvarchar](max) NULL,
	[Form_TFMessage_Completed_3rd_F] [nvarchar](max) NULL,
	[Item_FriendlyName] [nvarchar](max) NULL,
	[Item_Description] [nvarchar](max) NULL,
	[Item_ItemType] [nvarchar](max) NULL,
	[Item_UseCooldown] [int] NOT NULL,
	[Item_Bonuses] [nvarchar](max) NULL,
	[CursedTF_FormdbName] [nvarchar](max) NULL,
	[CursedTF_Fail] [nvarchar](max) NULL,
	[CursedTF_Fail_M] [nvarchar](max) NULL,
	[CursedTF_Fail_F] [nvarchar](max) NULL,
	[CursedTF_Succeed] [nvarchar](max) NULL,
	[CursedTF_Succeed_M] [nvarchar](max) NULL,
	[CursedTF_Succeed_F] [nvarchar](max) NULL,
	[Item_UsageMessage_Player] [nvarchar](max) NULL,
	[Item_UsageMessage_Item] [nvarchar](max) NULL,
	[IsReadyForReview] [bit] NOT NULL,
	[IsLive] [bit] NOT NULL,
	[AdminApproved] [bit] NOT NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[AssignedToArtist] [nvarchar](max) NULL,
	[ProofreadingCopy] [bit] NOT NULL,
	[ProofreadingLockIsOn] [bit] NOT NULL,
	[CheckedOutBy] [nvarchar](max) NULL,
	[NeedsToBeUpdated] [bit] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[ProofreadingCopyForOriginalId] [int] NOT NULL,
	[History] [nvarchar](max) NULL,
	[ImageURL] [nvarchar](max) NULL,
	[IsNonstandard] [bit] NOT NULL,
	[HealthBonusPercent] [decimal](18, 2) NOT NULL,
	[ManaBonusPercent] [decimal](18, 2) NOT NULL,
	[ExtraSkillCriticalPercent] [decimal](18, 2) NOT NULL,
	[HealthRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[ManaRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[SneakPercent] [decimal](18, 2) NOT NULL,
	[EvasionPercent] [decimal](18, 2) NOT NULL,
	[EvasionNegationPercent] [decimal](18, 2) NOT NULL,
	[MeditationExtraMana] [decimal](18, 2) NOT NULL,
	[CleanseExtraHealth] [decimal](18, 2) NOT NULL,
	[MoveActionPointDiscount] [decimal](18, 2) NOT NULL,
	[SpellExtraTFEnergyPercent] [decimal](18, 2) NOT NULL,
	[SpellExtraHealthDamagePercent] [decimal](18, 2) NOT NULL,
	[CleanseExtraTFEnergyRemovalPercent] [decimal](18, 2) NOT NULL,
	[SpellMisfireChanceReduction] [decimal](18, 2) NOT NULL,
	[SpellHealthDamageResistance] [decimal](18, 2) NOT NULL,
	[SpellTFEnergyDamageResistance] [decimal](18, 2) NOT NULL,
	[ExtraInventorySpace] [decimal](18, 2) NOT NULL,
	[Discipline] [real] NOT NULL,
	[Perception] [real] NOT NULL,
	[Charisma] [real] NOT NULL,
	[Submission_Dominance] [real] NOT NULL,
	[Fortitude] [real] NOT NULL,
	[Agility] [real] NOT NULL,
	[Allure] [real] NOT NULL,
	[Corruption_Purity] [real] NOT NULL,
	[Magicka] [real] NOT NULL,
	[Succour] [real] NOT NULL,
	[Luck] [real] NOT NULL,
	[Chaos_Order] [real] NOT NULL,
 CONSTRAINT [PK_dbo.Contributions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ContributorCustomForms]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContributorCustomForms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[CustomForm_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ContributorCustomForms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CovenantApplications]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CovenantApplications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[CovenantId] [int] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.CovenantApplications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CovenantLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CovenantLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CovenantId] [int] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[IsImportant] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.CovenantLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Covenants]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Covenants](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[FlagUrl] [nvarchar](max) NULL,
	[SelfDescription] [nvarchar](200) NOT NULL,
	[LastMemberAcceptance] [datetime] NOT NULL,
	[LeaderId] [int] NOT NULL,
	[FounderMembershipId] [nvarchar](128) NULL,
	[IsPvP] [bit] NOT NULL,
	[formerMembers] [nvarchar](max) NULL,
	[Captains] [nvarchar](max) NULL,
	[Money] [decimal](18, 2) NOT NULL,
	[HomeLocation] [nvarchar](max) NULL,
	[Level] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Covenants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DbStaticEffects]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbStaticEffects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[AvailableAtLevel] [int] NOT NULL,
	[PreRequesite] [nvarchar](max) NULL,
	[isLevelUpPerk] [bit] NOT NULL,
	[Duration] [int] NOT NULL,
	[Cooldown] [int] NOT NULL,
	[ObtainedAtLocation] [nvarchar](max) NULL,
	[IsRemovable] [bit] NOT NULL,
	[BlessingCurseStatus] [int] NOT NULL,
	[MessageWhenHit] [nvarchar](max) NULL,
	[MessageWhenHit_M] [nvarchar](max) NULL,
	[MessageWhenHit_F] [nvarchar](max) NULL,
	[AttackerWhenHit] [nvarchar](max) NULL,
	[AttackerWhenHit_M] [nvarchar](max) NULL,
	[AttackerWhenHit_F] [nvarchar](max) NULL,
	[HealthBonusPercent] [decimal](18, 2) NOT NULL,
	[ManaBonusPercent] [decimal](18, 2) NOT NULL,
	[ExtraSkillCriticalPercent] [decimal](18, 2) NOT NULL,
	[HealthRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[ManaRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[SneakPercent] [decimal](18, 2) NOT NULL,
	[EvasionPercent] [decimal](18, 2) NOT NULL,
	[EvasionNegationPercent] [decimal](18, 2) NOT NULL,
	[MeditationExtraMana] [decimal](18, 2) NOT NULL,
	[CleanseExtraHealth] [decimal](18, 2) NOT NULL,
	[MoveActionPointDiscount] [decimal](18, 2) NOT NULL,
	[SpellExtraTFEnergyPercent] [decimal](18, 2) NOT NULL,
	[SpellExtraHealthDamagePercent] [decimal](18, 2) NOT NULL,
	[CleanseExtraTFEnergyRemovalPercent] [decimal](18, 2) NOT NULL,
	[SpellMisfireChanceReduction] [decimal](18, 2) NOT NULL,
	[SpellHealthDamageResistance] [decimal](18, 2) NOT NULL,
	[SpellTFEnergyDamageResistance] [decimal](18, 2) NOT NULL,
	[ExtraInventorySpace] [decimal](18, 2) NOT NULL,
	[Discipline] [real] NOT NULL,
	[Perception] [real] NOT NULL,
	[Charisma] [real] NOT NULL,
	[Submission_Dominance] [real] NOT NULL,
	[Fortitude] [real] NOT NULL,
	[Agility] [real] NOT NULL,
	[Allure] [real] NOT NULL,
	[Corruption_Purity] [real] NOT NULL,
	[Magicka] [real] NOT NULL,
	[Succour] [real] NOT NULL,
	[Luck] [real] NOT NULL,
	[Chaos_Order] [real] NOT NULL,
 CONSTRAINT [PK_dbo.DbStaticEffects] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DbStaticForms]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbStaticForms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[TFEnergyType] [nvarchar](max) NULL,
	[TFEnergyRequired] [decimal](18, 2) NOT NULL,
	[Gender] [nvarchar](max) NULL,
	[MobilityType] [nvarchar](max) NULL,
	[BecomesItemDbName] [nvarchar](max) NULL,
	[PortraitUrl] [nvarchar](max) NULL,
	[IsUnique] [bit] NOT NULL,
	[HealthBonusPercent] [decimal](18, 2) NOT NULL,
	[ManaBonusPercent] [decimal](18, 2) NOT NULL,
	[ExtraSkillCriticalPercent] [decimal](18, 2) NOT NULL,
	[HealthRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[ManaRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[SneakPercent] [decimal](18, 2) NOT NULL,
	[EvasionPercent] [decimal](18, 2) NOT NULL,
	[EvasionNegationPercent] [decimal](18, 2) NOT NULL,
	[MeditationExtraMana] [decimal](18, 2) NOT NULL,
	[CleanseExtraHealth] [decimal](18, 2) NOT NULL,
	[MoveActionPointDiscount] [decimal](18, 2) NOT NULL,
	[SpellExtraTFEnergyPercent] [decimal](18, 2) NOT NULL,
	[SpellExtraHealthDamagePercent] [decimal](18, 2) NOT NULL,
	[CleanseExtraTFEnergyRemovalPercent] [decimal](18, 2) NOT NULL,
	[SpellMisfireChanceReduction] [decimal](18, 2) NOT NULL,
	[SpellHealthDamageResistance] [decimal](18, 2) NOT NULL,
	[SpellTFEnergyDamageResistance] [decimal](18, 2) NOT NULL,
	[ExtraInventorySpace] [decimal](18, 2) NOT NULL,
	[Discipline] [real] NOT NULL,
	[Perception] [real] NOT NULL,
	[Charisma] [real] NOT NULL,
	[Submission_Dominance] [real] NOT NULL,
	[Fortitude] [real] NOT NULL,
	[Agility] [real] NOT NULL,
	[Allure] [real] NOT NULL,
	[Corruption_Purity] [real] NOT NULL,
	[Magicka] [real] NOT NULL,
	[Succour] [real] NOT NULL,
	[Luck] [real] NOT NULL,
	[Chaos_Order] [real] NOT NULL,
 CONSTRAINT [PK_dbo.DbStaticForms] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DbStaticFurnitures]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbStaticFurnitures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbType] [nvarchar](max) NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[GivesEffect] [nvarchar](max) NULL,
	[APReserveRefillAmount] [decimal](18, 2) NOT NULL,
	[BaseCost] [decimal](18, 2) NOT NULL,
	[BaseContractTurnLength] [int] NOT NULL,
	[GivesItem] [nvarchar](max) NULL,
	[MinutesUntilReuse] [decimal](18, 2) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[PortraitUrl] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.DbStaticFurnitures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DbStaticItems]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbStaticItems](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[PortraitUrl] [nvarchar](max) NULL,
	[MoneyValue] [decimal](18, 2) NOT NULL,
	[MoneyValueSell] [decimal](18, 2) NOT NULL,
	[ItemType] [nvarchar](max) NULL,
	[UseCooldown] [int] NOT NULL,
	[UsageMessage_Item] [nvarchar](max) NULL,
	[UsageMessage_Player] [nvarchar](max) NULL,
	[Findable] [bit] NOT NULL,
	[FindWeight] [float] NOT NULL,
	[GivesEffect] [nvarchar](max) NULL,
	[IsUnique] [bit] NOT NULL,
	[CurseTFFormdbName] [nvarchar](max) NULL,
	[HealthBonusPercent] [decimal](18, 2) NOT NULL,
	[ManaBonusPercent] [decimal](18, 2) NOT NULL,
	[ExtraSkillCriticalPercent] [decimal](18, 2) NOT NULL,
	[HealthRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[ManaRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[SneakPercent] [decimal](18, 2) NOT NULL,
	[EvasionPercent] [decimal](18, 2) NOT NULL,
	[EvasionNegationPercent] [decimal](18, 2) NOT NULL,
	[MeditationExtraMana] [decimal](18, 2) NOT NULL,
	[CleanseExtraHealth] [decimal](18, 2) NOT NULL,
	[MoveActionPointDiscount] [decimal](18, 2) NOT NULL,
	[SpellExtraTFEnergyPercent] [decimal](18, 2) NOT NULL,
	[SpellExtraHealthDamagePercent] [decimal](18, 2) NOT NULL,
	[CleanseExtraTFEnergyRemovalPercent] [decimal](18, 2) NOT NULL,
	[SpellMisfireChanceReduction] [decimal](18, 2) NOT NULL,
	[SpellHealthDamageResistance] [decimal](18, 2) NOT NULL,
	[SpellTFEnergyDamageResistance] [decimal](18, 2) NOT NULL,
	[ExtraInventorySpace] [decimal](18, 2) NOT NULL,
	[Discipline] [real] NOT NULL,
	[Perception] [real] NOT NULL,
	[Charisma] [real] NOT NULL,
	[Submission_Dominance] [real] NOT NULL,
	[Fortitude] [real] NOT NULL,
	[Agility] [real] NOT NULL,
	[Allure] [real] NOT NULL,
	[Corruption_Purity] [real] NOT NULL,
	[Magicka] [real] NOT NULL,
	[Succour] [real] NOT NULL,
	[Luck] [real] NOT NULL,
	[Chaos_Order] [real] NOT NULL,
	[InstantHealthRestore] [decimal](18, 2) NOT NULL,
	[InstantManaRestore] [decimal](18, 2) NOT NULL,
	[ReuseableHealthRestore] [decimal](18, 2) NOT NULL,
	[ReuseableManaRestore] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_dbo.DbStaticItems] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DbStaticSkills]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DbStaticSkills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[FriendlyName] [nvarchar](max) NULL,
	[FormdbName] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
	[ManaCost] [decimal](18, 2) NOT NULL,
	[TFPointsAmount] [decimal](18, 2) NOT NULL,
	[HealthDamageAmount] [decimal](18, 2) NOT NULL,
	[LearnedAtRegion] [nvarchar](max) NULL,
	[LearnedAtLocation] [nvarchar](max) NULL,
	[DiscoveryMessage] [nvarchar](max) NULL,
	[IsLive] [nvarchar](max) NULL,
	[IsPlayerLearnable] [bit] NOT NULL,
	[GivesEffect] [nvarchar](max) NULL,
	[ExclusiveToForm] [nvarchar](max) NULL,
	[ExclusiveToItem] [nvarchar](max) NULL,
	[MobilityType] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.DbStaticSkills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DMRolls]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMRolls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MembershipOwnerId] [nvarchar](128) NULL,
	[Message] [nvarchar](max) NULL,
	[Tags] [nvarchar](max) NULL,
	[ActionType] [nvarchar](max) NULL,
	[IsLive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DMRolls] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Donators]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Donators](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[PatreonName] [nvarchar](max) NULL,
	[Tier] [int] NOT NULL,
	[ActualDonationAmount] [decimal](18, 2) NOT NULL,
	[SpecialNotes] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Donators] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DuelCombatants]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuelCombatants](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DuelId] [int] NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Team] [int] NOT NULL,
	[StartForm] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.DuelCombatants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DuelRules]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DuelRules](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DuelId] [int] NOT NULL,
	[NoItems] [bit] NOT NULL,
	[AnimateSpellsOnly] [bit] NOT NULL,
	[NoCleansing] [bit] NOT NULL,
	[NoMeditating] [bit] NOT NULL,
	[CastsPerRound] [int] NOT NULL,
 CONSTRAINT [PK_dbo.DuelRules] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Duels]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Duels](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProposalTurn] [int] NOT NULL,
	[StartTurn] [int] NOT NULL,
	[CompletionTurn] [int] NOT NULL,
	[Status] [nvarchar](max) NULL,
	[LastResetTimestamp] [datetime] NOT NULL,
	[Rules_Id] [int] NULL,
 CONSTRAINT [PK_dbo.Duels] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EffectContributions]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EffectContributions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMemberhipId] [nvarchar](128) NULL,
	[SubmitterName] [nvarchar](max) NULL,
	[AdditionalSubmitterNames] [nvarchar](max) NULL,
	[SubmitterURL] [nvarchar](max) NULL,
	[Skill_FriendlyName] [nvarchar](max) NULL,
	[Skill_UniqueToForm] [nvarchar](max) NULL,
	[Skill_UniqueToItem] [nvarchar](max) NULL,
	[Skill_UniqueToLocation] [nvarchar](max) NULL,
	[Skill_Description] [nvarchar](max) NULL,
	[Skill_ManaCost] [decimal](18, 2) NOT NULL,
	[Effect_FriendlyName] [nvarchar](max) NULL,
	[Effect_Description] [nvarchar](max) NULL,
	[Effect_IsRemovable] [bit] NOT NULL,
	[Effect_Duration] [int] NOT NULL,
	[Effect_Cooldown] [int] NOT NULL,
	[Effect_Bonuses] [nvarchar](max) NULL,
	[Effect_VictimHitText] [nvarchar](max) NULL,
	[Effect_VictimHitText_M] [nvarchar](max) NULL,
	[Effect_VictimHitText_F] [nvarchar](max) NULL,
	[Effect_AttackHitText] [nvarchar](max) NULL,
	[Effect_AttackHitText_M] [nvarchar](max) NULL,
	[Effect_AttackHitText_F] [nvarchar](max) NULL,
	[ReadyForReview] [bit] NOT NULL,
	[ApprovedByAdmin] [bit] NOT NULL,
	[IsLive] [bit] NOT NULL,
	[ProofreadingCopy] [bit] NOT NULL,
	[ProofreadingLockIsOn] [bit] NOT NULL,
	[CheckedOutBy] [nvarchar](max) NULL,
	[Notes] [nvarchar](max) NULL,
	[History] [nvarchar](max) NULL,
	[ProofreadingCopyForOriginalId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[NeedsToBeUpdated] [bit] NOT NULL,
	[HealthBonusPercent] [decimal](18, 2) NOT NULL,
	[ManaBonusPercent] [decimal](18, 2) NOT NULL,
	[ExtraSkillCriticalPercent] [decimal](18, 2) NOT NULL,
	[HealthRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[ManaRecoveryPerUpdate] [decimal](18, 2) NOT NULL,
	[SneakPercent] [decimal](18, 2) NOT NULL,
	[EvasionPercent] [decimal](18, 2) NOT NULL,
	[EvasionNegationPercent] [decimal](18, 2) NOT NULL,
	[MeditationExtraMana] [decimal](18, 2) NOT NULL,
	[CleanseExtraHealth] [decimal](18, 2) NOT NULL,
	[MoveActionPointDiscount] [decimal](18, 2) NOT NULL,
	[SpellExtraTFEnergyPercent] [decimal](18, 2) NOT NULL,
	[SpellExtraHealthDamagePercent] [decimal](18, 2) NOT NULL,
	[CleanseExtraTFEnergyRemovalPercent] [decimal](18, 2) NOT NULL,
	[SpellMisfireChanceReduction] [decimal](18, 2) NOT NULL,
	[SpellHealthDamageResistance] [decimal](18, 2) NOT NULL,
	[SpellTFEnergyDamageResistance] [decimal](18, 2) NOT NULL,
	[ExtraInventorySpace] [decimal](18, 2) NOT NULL,
	[Discipline] [real] NOT NULL,
	[Perception] [real] NOT NULL,
	[Charisma] [real] NOT NULL,
	[Submission_Dominance] [real] NOT NULL,
	[Fortitude] [real] NOT NULL,
	[Agility] [real] NOT NULL,
	[Allure] [real] NOT NULL,
	[Corruption_Purity] [real] NOT NULL,
	[Magicka] [real] NOT NULL,
	[Succour] [real] NOT NULL,
	[Luck] [real] NOT NULL,
	[Chaos_Order] [real] NOT NULL,
 CONSTRAINT [PK_dbo.EffectContributions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Effects]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Effects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[Duration] [int] NOT NULL,
	[IsPermanent] [bit] NOT NULL,
	[Level] [int] NOT NULL,
	[Cooldown] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Effects] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Friends]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Friends](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[FriendMembershipId] [nvarchar](128) NULL,
	[FriendsSince] [datetime] NOT NULL,
	[IsAccepted] [bit] NOT NULL,
	[OwnerNicknameForFriend] [nvarchar](max) NULL,
	[FriendNicknameForOwner] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Friends] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Furnitures]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Furnitures](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbType] [nvarchar](max) NULL,
	[CovenantId] [int] NOT NULL,
	[LastUseTimestamp] [datetime] NOT NULL,
	[ContractTurnDuration] [int] NOT NULL,
	[ContractStartTurn] [int] NOT NULL,
	[ContractEndTurn] [int] NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[LastUsersIds] [nvarchar](max) NULL,
	[HumanName] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Furnitures] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GameshowStats]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameshowStats](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FinishedTransformations] [int] NOT NULL,
	[FinishedTransformationsAI] [int] NOT NULL,
	[FinishedTransformationsHuman] [int] NOT NULL,
	[Discus_Wins] [int] NOT NULL,
	[Discus_Losses] [int] NOT NULL,
	[Dpong_Wins] [int] NOT NULL,
	[Dpong_Losses] [int] NOT NULL,
	[Sequence_Wins] [int] NOT NULL,
	[Sequence_Losses] [int] NOT NULL,
	[Popup_Wins] [int] NOT NULL,
	[Popup_Losses] [int] NOT NULL,
	[Target_Wins] [int] NOT NULL,
	[Target_Losses] [int] NOT NULL,
	[Roulette_Wins] [int] NOT NULL,
	[Roulette_Losses] [int] NOT NULL,
	[Match2_Wins] [int] NOT NULL,
	[Match2_Losses] [int] NOT NULL,
 CONSTRAINT [PK_dbo.GameshowStats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[InanimateXPs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InanimateXPs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[TimesStruggled] [int] NOT NULL,
	[LastActionTimestamp] [datetime] NOT NULL,
	[LastActionTurnstamp] [int] NOT NULL,
 CONSTRAINT [PK_dbo.InanimateXPs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Items]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Items](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[OwnerId] [int] NULL,
	[dbLocationName] [nvarchar](max) NULL,
	[VictimName] [nvarchar](max) NULL,
	[IsEquipped] [bit] NOT NULL,
	[TurnsUntilUse] [int] NOT NULL,
	[Level] [int] NOT NULL,
	[TimeDropped] [datetime] NOT NULL,
	[EquippedThisTurn] [bit] NOT NULL,
	[PvPEnabled] [int] NOT NULL,
	[IsPermanent] [bit] NOT NULL,
	[Nickname] [nvarchar](max) NULL,
	[LastSouledTimestamp] [datetime] NOT NULL,
	[LastSold] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Items] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemTransferLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemTransferLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ItemTransferLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[JewdewfaeEncounters]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JewdewfaeEncounters](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbLocationName] [nvarchar](max) NULL,
	[IntroText] [nvarchar](max) NULL,
	[CorrectFormText] [nvarchar](max) NULL,
	[FailureText] [nvarchar](max) NULL,
	[RequiredForm] [nvarchar](max) NULL,
	[IsLive] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.JewdewfaeEncounters] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LocationInfoes]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationInfoes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[CovenantId] [int] NOT NULL,
	[TakeoverAmount] [real] NOT NULL,
	[LastTakeoverTurn] [int] NOT NULL,
 CONSTRAINT [PK_dbo.LocationInfoes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[LocationLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LocationLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbLocationName] [nvarchar](max) NULL,
	[Message] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[ConcealmentLevel] [int] NOT NULL,
 CONSTRAINT [PK_dbo.LocationLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Messages]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SenderId] [int] NOT NULL,
	[ReceiverId] [int] NOT NULL,
	[IsRead] [bit] NOT NULL,
	[ReadStatus] [int] NOT NULL,
	[MessageText] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[DoNotRecycleMe] [bit] NOT NULL,
	[ReceiverMarkedAsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Messages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MindControls]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MindControls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MasterId] [int] NOT NULL,
	[VictimId] [int] NOT NULL,
	[TurnsRemaining] [int] NOT NULL,
	[Type] [nvarchar](max) NULL,
	[TimesUsedThisTurn] [int] NOT NULL,
 CONSTRAINT [PK_dbo.MindControls] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NewsPosts]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NewsPosts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[ViewState] [int] NOT NULL,
 CONSTRAINT [PK_dbo.NewsPosts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlayerBios]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerBios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[Text] [nvarchar](max) NULL,
	[WebsiteURL] [nvarchar](max) NULL,
	[PublicVisibility] [int] NOT NULL,
	[OtherNames] [nvarchar](max) NULL,
	[Tags] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.PlayerBios] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlayerExtras]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerExtras](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[ProtectionToggleTurnsRemaining] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PlayerExtras] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlayerLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[IsImportant] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.PlayerLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PlayerQuests]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayerQuests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[QuestId] [int] NOT NULL,
	[Outcome] [int] NOT NULL,
 CONSTRAINT [PK_dbo.PlayerQuests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Players]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Players](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MembershipId] [nvarchar](128) NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[dbLocationName] [nvarchar](max) NULL,
	[Form] [nvarchar](max) NULL,
	[Health] [decimal](18, 2) NOT NULL,
	[MaxHealth] [decimal](18, 2) NOT NULL,
	[Mana] [decimal](18, 2) NOT NULL,
	[MaxMana] [decimal](18, 2) NOT NULL,
	[ActionPoints] [decimal](18, 2) NOT NULL,
	[ActionPoints_Refill] [decimal](18, 2) NOT NULL,
	[Gender] [nvarchar](max) NULL,
	[Mobility] [nvarchar](max) NULL,
	[BotId] [int] NOT NULL,
	[IsPetToId] [int] NULL,
	[MindControlIsActive] [bit] NOT NULL,
	[XP] [decimal](18, 2) NOT NULL,
	[Level] [int] NOT NULL,
	[TimesAttackingThisUpdate] [int] NOT NULL,
	[IpAddress] [nvarchar](max) NULL,
	[LastActionTimestamp] [datetime] NOT NULL,
	[LastCombatTimestamp] [datetime] NOT NULL,
	[LastCombatAttackedTimestamp] [datetime] NOT NULL,
	[FlaggedForAbuse] [bit] NOT NULL,
	[UnusedLevelUpPerks] [int] NOT NULL,
	[GameMode] [int] NOT NULL,
	[InRP] [bit] NOT NULL,
	[CleansesMeditatesThisRound] [int] NOT NULL,
	[Money] [decimal](18, 2) NOT NULL,
	[Covenant] [int] NOT NULL,
	[OriginalForm] [nvarchar](max) NULL,
	[PvPScore] [decimal](18, 2) NOT NULL,
	[DonatorLevel] [int] NOT NULL,
	[Nickname] [nvarchar](max) NULL,
	[OnlineActivityTimestamp] [datetime] NOT NULL,
	[IsBannedFromGlobalChat] [bit] NOT NULL,
	[ChatColor] [nvarchar](max) NULL,
	[ShoutsRemaining] [int] NOT NULL,
	[InDuel] [int] NOT NULL,
	[InQuest] [int] NOT NULL,
	[InQuestState] [int] NOT NULL,
	[ItemsUsedThisTurn] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Players] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PollEntries]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PollEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[PollId] [int] NOT NULL,
	[Num1] [int] NOT NULL,
	[Num2] [int] NOT NULL,
	[Num3] [int] NOT NULL,
	[Num4] [int] NOT NULL,
	[Num5] [int] NOT NULL,
	[String1] [nvarchar](500) NULL,
	[String2] [nvarchar](500) NULL,
	[String3] [nvarchar](500) NULL,
	[String4] [nvarchar](500) NULL,
	[String5] [nvarchar](500) NULL,
	[Timestamp] [datetime] NOT NULL,
	[Round] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.PollEntries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PvPWorldStats]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PvPWorldStats](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurnNumber] [int] NOT NULL,
	[LastUpdateTimestamp] [datetime] NOT NULL,
	[WorldIsUpdating] [bit] NOT NULL,
	[LastUpdateTimestamp_Finished] [datetime] NOT NULL,
	[Boss_DonnaActive] [bit] NOT NULL,
	[Boss_Donna] [nvarchar](max) NULL,
	[Boss_Valentine] [nvarchar](max) NULL,
	[Boss_Bimbo] [nvarchar](max) NULL,
	[Boss_Thief] [nvarchar](max) NULL,
	[Boss_Sisters] [nvarchar](max) NULL,
	[Boss_Faeboss] [nvarchar](max) NULL,
	[GameNewsDate] [nvarchar](max) NULL,
	[TestServer] [bit] NOT NULL,
	[ChaosMode] [bit] NOT NULL,
	[RoundDuration] [int] NOT NULL,
	[InbetweenRoundsNonChaos] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.PvPWorldStats] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestConnectionRequirements]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestConnectionRequirements](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequirementType] [int] NOT NULL,
	[VariabledbName] [nvarchar](max) NULL,
	[Operator] [int] NOT NULL,
	[RequirementValue] [nvarchar](max) NULL,
	[QuestId] [int] NOT NULL,
	[QuestConnectionRequirementName] [nvarchar](max) NULL,
	[IsRandomRoll] [bit] NOT NULL,
	[RollModifier] [real] NOT NULL,
	[RollOffset] [real] NOT NULL,
	[QuestConnectionId_Id] [int] NULL,
	[QuestState_Id] [int] NULL,
 CONSTRAINT [PK_dbo.QuestConnectionRequirements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestConnections]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestConnections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QuestStateFromId] [int] NOT NULL,
	[QuestStateToId] [int] NOT NULL,
	[QuestStateFailToId] [int] NOT NULL,
	[ActionName] [nvarchar](max) NULL,
	[ConnectionName] [nvarchar](max) NULL,
	[QuestId] [int] NOT NULL,
	[HideIfRequirementsNotMet] [bit] NOT NULL,
	[RankInList] [int] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[Text] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.QuestConnections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestEnds]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestEnds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EndType] [int] NOT NULL,
	[RewardType] [int] NOT NULL,
	[RewardAmount] [nvarchar](max) NULL,
	[QuestEndName] [nvarchar](max) NULL,
	[QuestId] [int] NOT NULL,
	[QuestStateId_Id] [int] NULL,
 CONSTRAINT [PK_dbo.QuestEnds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestPlayerStatus]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestPlayerStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[QuestId] [int] NOT NULL,
	[Outcome] [int] NOT NULL,
	[StartedTurn] [int] NOT NULL,
	[LastEndedTurn] [int] NOT NULL,
 CONSTRAINT [PK_dbo.QuestPlayerStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestPlayerVariables]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestPlayerVariables](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[QuestId] [int] NOT NULL,
	[VariableName] [nvarchar](max) NULL,
	[VariableValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.QuestPlayerVariables] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestStarts]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestStarts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dbName] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[MinStartTurn] [int] NOT NULL,
	[MaxStartTurn] [int] NOT NULL,
	[MinStartLevel] [int] NOT NULL,
	[MaxStartLevel] [int] NOT NULL,
	[PrerequisiteQuest] [int] NOT NULL,
	[RequiredGender] [int] NOT NULL,
	[StartState] [int] NOT NULL,
	[IsLive] [bit] NOT NULL,
	[Tags] [nvarchar](max) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.QuestStarts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestStatePreactions]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestStatePreactions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QuestId] [int] NOT NULL,
	[QuestStatePreactionName] [nvarchar](max) NULL,
	[ActionType] [int] NOT NULL,
	[ActionValue] [nvarchar](max) NULL,
	[AddOrSet] [int] NOT NULL,
	[VariableName] [nvarchar](max) NULL,
	[QuestStateId_Id] [int] NULL,
 CONSTRAINT [PK_dbo.QuestStatePreactions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestStates]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestStates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QuestStateName] [nvarchar](max) NULL,
	[QuestId] [int] NOT NULL,
	[Text] [nvarchar](max) NULL,
	[QuestEndId] [int] NOT NULL,
	[HideIfRequirementsNotMet] [bit] NOT NULL,
	[Notes] [nvarchar](max) NULL,
	[PinToDiagram] [bit] NOT NULL,
	[X] [real] NOT NULL,
	[Y] [real] NOT NULL,
 CONSTRAINT [PK_dbo.QuestStates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestWriterLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestWriterLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[User] [nvarchar](max) NULL,
	[Text] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
	[QuestId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.QuestWriterLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QuestWriterPermissions]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QuestWriterPermissions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QuestId] [int] NOT NULL,
	[PlayerMembershipId] [nvarchar](max) NULL,
	[PermissionType] [int] NOT NULL,
 CONSTRAINT [PK_dbo.QuestWriterPermissions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Rerolls]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rerolls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MembershipId] [nvarchar](128) NULL,
	[CharacterGeneration] [int] NOT NULL,
	[LastCharacterCreation] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Rerolls] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReservedNames]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservedNames](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MembershipId] [nvarchar](128) NULL,
	[FullName] [nvarchar](max) NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.ReservedNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RPClassifiedAds]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RPClassifiedAds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[Text] [nvarchar](max) NULL,
	[YesThemes] [nvarchar](max) NULL,
	[NoThemes] [nvarchar](max) NULL,
	[CreationTimestamp] [datetime] NOT NULL,
	[RefreshTimestamp] [datetime] NOT NULL,
	[PreferredTimezones] [nvarchar](max) NULL,
	[Title] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.RPClassifiedAds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RPPoints]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RPPoints](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerMembershipId] [nvarchar](128) NULL,
	[Amount] [int] NOT NULL,
	[RemainingPointsToGive] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RPPoints] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServerLogs]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerLogs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TurnNumber] [int] NOT NULL,
	[Errors] [int] NOT NULL,
	[FullLog] [nvarchar](max) NULL,
	[StartTimestamp] [datetime] NOT NULL,
	[FinishTimestamp] [datetime] NOT NULL,
	[Population] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ServerLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Skills]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skills](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Duration] [decimal](18, 2) NOT NULL,
	[Charge] [decimal](18, 2) NOT NULL,
	[TurnStamp] [int] NOT NULL,
	[IsArchived] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Skills] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TFEnergies]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TFEnergies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlayerId] [int] NOT NULL,
	[FormName] [nvarchar](max) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[CasterId] [int] NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.TFEnergies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TFMessages]    Script Date: 2/23/2017 4:32:51 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TFMessages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FormDbName] [nvarchar](max) NULL,
	[TFMessage_20_Percent_1st] [nvarchar](max) NULL,
	[TFMessage_40_Percent_1st] [nvarchar](max) NULL,
	[TFMessage_60_Percent_1st] [nvarchar](max) NULL,
	[TFMessage_80_Percent_1st] [nvarchar](max) NULL,
	[TFMessage_100_Percent_1st] [nvarchar](max) NULL,
	[TFMessage_Completed_1st] [nvarchar](max) NULL,
	[TFMessage_20_Percent_1st_M] [nvarchar](max) NULL,
	[TFMessage_40_Percent_1st_M] [nvarchar](max) NULL,
	[TFMessage_60_Percent_1st_M] [nvarchar](max) NULL,
	[TFMessage_80_Percent_1st_M] [nvarchar](max) NULL,
	[TFMessage_100_Percent_1st_M] [nvarchar](max) NULL,
	[TFMessage_Completed_1st_M] [nvarchar](max) NULL,
	[TFMessage_20_Percent_1st_F] [nvarchar](max) NULL,
	[TFMessage_40_Percent_1st_F] [nvarchar](max) NULL,
	[TFMessage_60_Percent_1st_F] [nvarchar](max) NULL,
	[TFMessage_80_Percent_1st_F] [nvarchar](max) NULL,
	[TFMessage_100_Percent_1st_F] [nvarchar](max) NULL,
	[TFMessage_Completed_1st_F] [nvarchar](max) NULL,
	[TFMessage_20_Percent_3rd] [nvarchar](max) NULL,
	[TFMessage_40_Percent_3rd] [nvarchar](max) NULL,
	[TFMessage_60_Percent_3rd] [nvarchar](max) NULL,
	[TFMessage_80_Percent_3rd] [nvarchar](max) NULL,
	[TFMessage_100_Percent_3rd] [nvarchar](max) NULL,
	[TFMessage_Completed_3rd] [nvarchar](max) NULL,
	[TFMessage_20_Percent_3rd_M] [nvarchar](max) NULL,
	[TFMessage_40_Percent_3rd_M] [nvarchar](max) NULL,
	[TFMessage_60_Percent_3rd_M] [nvarchar](max) NULL,
	[TFMessage_80_Percent_3rd_M] [nvarchar](max) NULL,
	[TFMessage_100_Percent_3rd_M] [nvarchar](max) NULL,
	[TFMessage_Completed_3rd_M] [nvarchar](max) NULL,
	[TFMessage_20_Percent_3rd_F] [nvarchar](max) NULL,
	[TFMessage_40_Percent_3rd_F] [nvarchar](max) NULL,
	[TFMessage_60_Percent_3rd_F] [nvarchar](max) NULL,
	[TFMessage_80_Percent_3rd_F] [nvarchar](max) NULL,
	[TFMessage_100_Percent_3rd_F] [nvarchar](max) NULL,
	[TFMessage_Completed_3rd_F] [nvarchar](max) NULL,
	[CursedTF_Fail] [nvarchar](max) NULL,
	[CursedTF_Fail_M] [nvarchar](max) NULL,
	[CursedTF_Fail_F] [nvarchar](max) NULL,
	[CursedTF_Succeed] [nvarchar](max) NULL,
	[CursedTF_Succeed_M] [nvarchar](max) NULL,
	[CursedTF_Succeed_F] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.TFMessages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Index [IX_CustomForm_Id]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_CustomForm_Id] ON [dbo].[ContributorCustomForms]
(
	[CustomForm_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_DuelId]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_DuelId] ON [dbo].[DuelCombatants]
(
	[DuelId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Rules_Id]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_Rules_Id] ON [dbo].[Duels]
(
	[Rules_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_MembershipId]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_MembershipId] ON [dbo].[Players]
(
	[MembershipId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_MembershipIdAndCovenant]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_MembershipIdAndCovenant] ON [dbo].[Players]
(
	[MembershipId] ASC,
	[Covenant] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_MembershipIdAndInPvP]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_MembershipIdAndInPvP] ON [dbo].[Players]
(
	[MembershipId] ASC,
	[GameMode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_QuestConnectionId_Id]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_QuestConnectionId_Id] ON [dbo].[QuestConnectionRequirements]
(
	[QuestConnectionId_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_QuestState_Id]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_QuestState_Id] ON [dbo].[QuestConnectionRequirements]
(
	[QuestState_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_QuestStateId_Id]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_QuestStateId_Id] ON [dbo].[QuestEnds]
(
	[QuestStateId_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_QuestStateId_Id]    Script Date: 2/23/2017 4:32:51 PM ******/
CREATE NONCLUSTERED INDEX [IX_QuestStateId_Id] ON [dbo].[QuestStatePreactions]
(
	[QuestStateId_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ContributorCustomForms]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ContributorCustomForms_dbo.DbStaticForms_CustomForm_Id] FOREIGN KEY([CustomForm_Id])
REFERENCES [dbo].[DbStaticForms] ([Id])
GO
ALTER TABLE [dbo].[ContributorCustomForms] CHECK CONSTRAINT [FK_dbo.ContributorCustomForms_dbo.DbStaticForms_CustomForm_Id]
GO
ALTER TABLE [dbo].[DuelCombatants]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DuelCombatants_dbo.Duels_DuelId] FOREIGN KEY([DuelId])
REFERENCES [dbo].[Duels] ([Id])
GO
ALTER TABLE [dbo].[DuelCombatants] CHECK CONSTRAINT [FK_dbo.DuelCombatants_dbo.Duels_DuelId]
GO
ALTER TABLE [dbo].[Duels]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Duels_dbo.DuelRules_Rules_Id] FOREIGN KEY([Rules_Id])
REFERENCES [dbo].[DuelRules] ([Id])
GO
ALTER TABLE [dbo].[Duels] CHECK CONSTRAINT [FK_dbo.Duels_dbo.DuelRules_Rules_Id]
GO
ALTER TABLE [dbo].[QuestConnectionRequirements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuestConnectionRequirements_dbo.QuestConnections_QuestConnectionId_Id] FOREIGN KEY([QuestConnectionId_Id])
REFERENCES [dbo].[QuestConnections] ([Id])
GO
ALTER TABLE [dbo].[QuestConnectionRequirements] CHECK CONSTRAINT [FK_dbo.QuestConnectionRequirements_dbo.QuestConnections_QuestConnectionId_Id]
GO
ALTER TABLE [dbo].[QuestConnectionRequirements]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuestConnectionRequirements_dbo.QuestStates_QuestState_Id] FOREIGN KEY([QuestState_Id])
REFERENCES [dbo].[QuestStates] ([Id])
GO
ALTER TABLE [dbo].[QuestConnectionRequirements] CHECK CONSTRAINT [FK_dbo.QuestConnectionRequirements_dbo.QuestStates_QuestState_Id]
GO
ALTER TABLE [dbo].[QuestEnds]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuestEnds_dbo.QuestStates_QuestStateId_Id] FOREIGN KEY([QuestStateId_Id])
REFERENCES [dbo].[QuestStates] ([Id])
GO
ALTER TABLE [dbo].[QuestEnds] CHECK CONSTRAINT [FK_dbo.QuestEnds_dbo.QuestStates_QuestStateId_Id]
GO
ALTER TABLE [dbo].[QuestStatePreactions]  WITH CHECK ADD  CONSTRAINT [FK_dbo.QuestStatePreactions_dbo.QuestStates_QuestStateId_Id] FOREIGN KEY([QuestStateId_Id])
REFERENCES [dbo].[QuestStates] ([Id])
GO
ALTER TABLE [dbo].[QuestStatePreactions] CHECK CONSTRAINT [FK_dbo.QuestStatePreactions_dbo.QuestStates_QuestStateId_Id]
GO
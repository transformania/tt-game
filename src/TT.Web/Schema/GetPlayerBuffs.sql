USE [Stats]
GO
/****** Object:  StoredProcedure [dbo].[GetPlayerBuffs]    Script Date: 5/11/2015 9:48:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Arrhae Khellian
-- Create date: 2015-04-06
-- Description:	Fetch player buffs
-- =============================================
IF OBJECT_ID('[dbo].[GetPlayerBuffs]') IS NULL
	EXEC('CREATE PROCEDURE [dbo].[GetPlayerBuffs] AS SET NOCOUNT ON;');
GO
ALTER PROCEDURE [dbo].[GetPlayerBuffs]
	-- Add the parameters for the stored procedure here
	@PlayerId int = -1,
	@Item_LevelBonusModifier decimal(18,2) = 0.1
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	IF (SELECT COUNT(*) FROM [dbo].Players WHERE [Players].Id = @PlayerId) = 0 OR (SELECT Mobility FROM [dbo].Players WHERE [Players].Id = @PlayerId) <> 'full'
			RETURN -1;
	ELSE
		BEGIN
	(
		SELECT
		'Items' AS Type,
		ISNULL(SUM(si.HealthBonusPercent + si.HealthBonusPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS HealthBonusPercent
		,ISNULL(SUM(si.ManaBonusPercent + si.ManaBonusPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS ManaBonusPercent
		,ISNULL(SUM(si.MeditationExtraMana + si.MeditationExtraMana * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS MeditationExtraMana
		,ISNULL(SUM(si.CleanseExtraHealth + si.CleanseExtraHealth * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS CleanseExtraHealth
		,ISNULL(SUM(si.ExtraSkillCriticalPercent + si.ExtraSkillCriticalPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS ExtraSkillCriticalPercent
		,ISNULL(SUM(si.HealthRecoveryPerUpdate + si.HealthRecoveryPerUpdate * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS HealthRecoveryPerUpdate
		,ISNULL(SUM(si.ManaRecoveryPerUpdate + si.ManaRecoveryPerUpdate * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS ManaRecoveryPerUpdate
		,ISNULL(SUM(si.SneakPercent + si.SneakPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS SneakPercent
		,ISNULL(SUM(si.EvasionPercent + si.EvasionPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS EvasionPercent
		,ISNULL(SUM(si.EvasionNegationPercent + si.EvasionNegationPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS EvasionNegationPercent
		,ISNULL(SUM(si.MoveActionPointDiscount + si.MoveActionPointDiscount * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS MoveActionPointDiscount
		,ISNULL(SUM(si.SpellExtraTFEnergyPercent + si.SpellExtraTFEnergyPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS SpellExtraTFEnergyPercent
		,ISNULL(SUM(si.SpellExtraHealthDamagePercent + si.SpellExtraHealthDamagePercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS SpellExtraHealthDamagePercent
		,ISNULL(SUM(si.CleanseExtraTFEnergyRemovalPercent + si.CleanseExtraTFEnergyRemovalPercent * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS CleanseExtraTFEnergyRemovalPercent
		,ISNULL(SUM(si.SpellMisfireChanceReduction + si.SpellMisfireChanceReduction * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS SpellMisfireChanceReduction
		,ISNULL(SUM(si.SpellHealthDamageResistance + si.SpellHealthDamageResistance * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS SpellHealthDamageResistance
		,ISNULL(SUM(si.SpellTFEnergyDamageResistance + si.SpellTFEnergyDamageResistance * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS SpellTFEnergyDamageResistance
		,ISNULL(SUM(si.ExtraInventorySpace + si.ExtraInventorySpace * ([Items].Level - 1)*@Item_LevelBonusModifier), 0) AS ExtraInventorySpace

		-- New stats
		,CAST(ISNULL(SUM(si.Discipline + si.Discipline * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Discipline
		,CAST(ISNULL(SUM(si.Perception + si.Perception * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Perception
		,CAST(ISNULL(SUM(si.Charisma + si.Charisma * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Charisma
		,CAST(ISNULL(SUM(si.Submission_Dominance + si.Submission_Dominance * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Submission_Dominance

		,CAST(ISNULL(SUM(si.Fortitude + si.Fortitude * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Fortitude
		,CAST(ISNULL(SUM(si.Agility + si.Agility * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Agility
		,CAST(ISNULL(SUM(si.Allure + si.Allure * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Allure
		,CAST(ISNULL(SUM(si.Corruption_Purity + si.Corruption_Purity * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Corruption_Purity

		,CAST(ISNULL(SUM(si.Magicka + si.Magicka * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Magicka
		,CAST(ISNULL(SUM(si.Succour + si.Succour * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Succour
		,CAST(ISNULL(SUM(si.Luck + si.Luck * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Luck
		,CAST(ISNULL(SUM(si.Chaos_Order + si.Chaos_Order * ([Items].Level - 1) * @Item_LevelBonusModifier), 0) AS real) AS Chaos_Order

		FROM [dbo].[Players]
		JOIN [dbo].[Items] ON [Items].OwnerId=@PlayerId
		JOIN [dbo].[DbStaticItems] AS si ON si.Id=[Items].ItemSourceId
		WHERE [Items].[IsEquipped]=1 AND [Players].Id = @PlayerId
	)
	UNION ALL
	(
		SELECT 'Form' AS Type,
		ISNULL(sf.HealthBonusPercent, 0)
		,ISNULL(sf.ManaBonusPercent, 0)
		,ISNULL(sf.MeditationExtraMana, 0)
		,ISNULL(sf.CleanseExtraHealth, 0)
		,ISNULL(sf.ExtraSkillCriticalPercent, 0)
		,ISNULL(sf.HealthRecoveryPerUpdate, 0)
		,ISNULL(sf.ManaRecoveryPerUpdate, 0)
		,ISNULL(sf.SneakPercent, 0)
		,ISNULL(sf.EvasionPercent, 0)
		,ISNULL(sf.EvasionNegationPercent, 0)
		,ISNULL(sf.MoveActionPointDiscount, 0)
		,ISNULL(sf.SpellExtraTFEnergyPercent, 0)
		,ISNULL(sf.SpellExtraHealthDamagePercent, 0)
		,ISNULL(sf.CleanseExtraTFEnergyRemovalPercent, 0)
		,ISNULL(sf.SpellMisfireChanceReduction, 0)
		,ISNULL(sf.SpellHealthDamageResistance, 0)
		,ISNULL(sf.SpellTFEnergyDamageResistance, 0)
		,ISNULL(sf.ExtraInventorySpace, 0)

		-- New stats
		,ISNULL(sf.Discipline, 0)
		,ISNULL(sf.Perception, 0)
		,ISNULL(sf.Charisma, 0)
		,ISNULL(sf.Submission_Dominance, 0)

		,ISNULL(sf.Fortitude, 0)
		,ISNULL(sf.Agility, 0)
		,ISNULL(sf.Allure, 0)
		,ISNULL(sf.Corruption_Purity, 0)

		,ISNULL(sf.Magicka, 0)
		,ISNULL(sf.Succour, 0)
		,ISNULL(sf.Luck, 0)
		,ISNULL(sf.Chaos_Order, 0)


		FROM [dbo].Players
		JOIN [dbo].[DbStaticForms] AS sf ON sf.Id=[Players].FormSourceId WHERE [Players].Id = @PlayerId
	)
	UNION ALL
	(
		SELECT 'Effects' AS Type,
		ISNULL(SUM(se.HealthBonusPercent), 0)
		,ISNULL(SUM(se.ManaBonusPercent), 0)
		,ISNULL(SUM(se.MeditationExtraMana), 0)
		,ISNULL(SUM(se.CleanseExtraHealth), 0)
		,ISNULL(SUM(se.ExtraSkillCriticalPercent), 0)
		,ISNULL(SUM(se.HealthRecoveryPerUpdate), 0)
		,ISNULL(SUM(se.ManaRecoveryPerUpdate), 0)
		,ISNULL(SUM(se.SneakPercent), 0)
		,ISNULL(SUM(se.EvasionPercent), 0)
		,ISNULL(SUM(se.EvasionNegationPercent), 0)
		,ISNULL(SUM(se.MoveActionPointDiscount), 0)
		,ISNULL(SUM(se.SpellExtraTFEnergyPercent), 0)
		,ISNULL(SUM(se.SpellExtraHealthDamagePercent), 0)
		,ISNULL(SUM(se.CleanseExtraTFEnergyRemovalPercent), 0)
		,ISNULL(SUM(se.SpellMisfireChanceReduction), 0)
		,ISNULL(SUM(se.SpellHealthDamageResistance), 0)
		,ISNULL(SUM(se.SpellTFEnergyDamageResistance), 0)
		,ISNULL(SUM(se.ExtraInventorySpace), 0)

		-- New stats
		,CAST(ISNULL(SUM(se.Discipline), 0) AS real)
		,CAST(ISNULL(SUM(se.Perception), 0) AS real)
		,CAST(ISNULL(SUM(se.Charisma), 0) AS real)
		,CAST(ISNULL(SUM(se.Submission_Dominance), 0) AS real)

		,CAST(ISNULL(SUM(se.Fortitude), 0) AS real)
		,CAST(ISNULL(SUM(se.Agility), 0) AS real)
		,CAST(ISNULL(SUM(se.Allure), 0) AS real)
		,CAST(ISNULL(SUM(se.Corruption_Purity), 0) AS real)

		,CAST(ISNULL(SUM(se.Magicka), 0) AS real)
		,CAST(ISNULL(SUM(se.Succour), 0) AS real)
		,CAST(ISNULL(SUM(se.Luck), 0) AS real)
		,CAST(ISNULL(SUM(se.Chaos_Order), 0) AS real)

		FROM [dbo].Effects
		JOIN [dbo].DbStaticEffects AS se ON se.Id=Effects.EffectSourceId WHERE Effects.OwnerId = @PlayerId AND Effects.Duration > 0
	)
	RETURN 1;
	END
END
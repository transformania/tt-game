USE [Stats]
GO

IF EXISTS (
	SELECT 'X'
	FROM sysindexes
	WHERE id = (SELECT OBJECT_ID('Players'))
	AND name = 'idx_DCh_60_59_Players'
)
DROP INDEX Players.idx_DCh_60_59_Players;

IF EXISTS (
	SELECT 'X'
	FROM sysindexes
	WHERE id = (SELECT OBJECT_ID('Players'))
	AND name = 'idx_DCh_53421_53420_Players'
)
DROP INDEX Players.idx_DCh_53421_53420_Players;

IF EXISTS (
	SELECT 'X'
	FROM sysindexes
	WHERE id = (SELECT OBJECT_ID('Players'))
	AND name = 'idx_DCh_193454_193453_Players'
)
DROP INDEX Players.idx_DCh_193454_193453_Players;

IF EXISTS (
	SELECT 'X'
	FROM sysindexes
	WHERE id = (SELECT OBJECT_ID('Players'))
	AND name = 'idx_DCh_3766_3765_Players'
)
DROP INDEX Players.idx_DCh_3766_3765_Players;

GO

IF (OBJECT_ID('DF__PollEntri__Owner__37C5420D') IS NOT NULL)
BEGIN
    ALTER TABLE [dbo].[PollEntries]
    DROP CONSTRAINT [DF__PollEntri__Owner__37C5420D]
END
GO


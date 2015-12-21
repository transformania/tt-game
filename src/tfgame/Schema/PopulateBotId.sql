USE [Stats]
GO

UPDATE [dbo].Players
SET [BotId] = MembershipId
WHERE MembershipId <= 0
GO

UPDATE [dbo].[Players]
   SET [BotId] = 0
 WHERE MembershipId > 0
GO


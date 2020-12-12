SET NOCOUNT ON;
USE [Stats]
GO

INSERT [dbo].[PvPWorldStats] ([TurnNumber], [LastUpdateTimestamp], [WorldIsUpdating], [LastUpdateTimestamp_Finished], [Boss_DonnaActive], [Boss_Donna], [GameNewsDate], [Boss_Valentine], [TestServer], [Boss_Bimbo], [InbetweenRoundsNonChaos], [Boss_Thief], [Boss_Sisters], [ChaosMode], [RoundDuration], [TurnTimeConfiguration]) VALUES (1, GETUTCDATE(), 0, GETUTCDATE(), 0, N'completed', N'December 21', N'unstarted', 0, N'unstarted', 0, N'unstarted', N'unstarted', 1, 4000, N'5min')
GO
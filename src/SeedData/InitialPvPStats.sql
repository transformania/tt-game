USE [Stats]
GO

INSERT [dbo].[PvPWorldStats] ([TurnNumber], [LastUpdateTimestamp], [WorldIsUpdating], [LastUpdateTimestamp_Finished], [Boss_DonnaActive], [Boss_Donna], [GameNewsDate], [Boss_Valentine], [TestServer], [Boss_Bimbo], [InbetweenRoundsNonChaos], [Boss_Thief], [Boss_Sisters], [ChaosMode], [RoundDuration]) VALUES (1, GETDATE(), 0, GETDATE(), 0, N'completed', N'December 21', N'unstarted', 0, N'unstarted', 0, N'unstarted', N'unstarted', 1, 4000)
GO

SET NOCOUNT ON;
USE [Stats]

/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 26/12/2015 15:46:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'1', N'moderator')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'10', N'questwriter')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'2', N'proofreader')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'3', N'whitelisted')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'4', N'artist')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'5', N'admin')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'6', N'json')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'7', N'previewer')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'8', N'killswitch')
GO
INSERT [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'9', N'publisher')
GO

PRINT 'Roles seeded';
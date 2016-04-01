SET NOCOUNT ON;
USE [Stats]

/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 26/12/2015 15:46:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AspNetUsers] (
    [AccessFailedCount]    INT            NOT NULL,
    [Email]                NVARCHAR (MAX) NULL,
    [EmailConfirmed]       BIT            DEFAULT ((0)) NULL,
    [Id]                   NVARCHAR (128) NOT NULL,
    [LockoutEnabled]       BIT            DEFAULT ((0)) NULL,
    [LockoutEndDateUtc]           DATETIME2 (7)  NULL,
    [PasswordHash]         NVARCHAR (MAX) NULL,
    [PhoneNumber]          NVARCHAR (MAX) NULL,
    [PhoneNumberConfirmed] BIT            DEFAULT ((0)) NULL,
    [SecurityStamp]        NVARCHAR (MAX) NULL,
    [TwoFactorEnabled]     BIT            DEFAULT ((0)) NULL,
    [UserName]             NVARCHAR (MAX) NULL,
	[CreateDate]                              DATETIME       NULL,
    [ConfirmationToken]                       NVARCHAR (128) NULL,
    [IsConfirmed]                             BIT            DEFAULT ((0)) NULL,
    [LastPasswordFailureDate]                 DATETIME       NULL,
    [PasswordFailuresSinceLastSuccess]        INT            DEFAULT ((0)) NULL,
	[PasswordChangedDate]                     DATETIME       NULL,
    [PasswordVerificationToken]               NVARCHAR (128) NULL,
    [PasswordVerificationTokenExpirationDate] DATETIME       NULL,
    CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[AspNetRoles] (
    [Id]   NVARCHAR (128) NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
CREATE TABLE [dbo].[AspNetUserRoles] (
    [UserId] NVARCHAR (128) NOT NULL,
    [RoleId] NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_RoleId]
    ON [dbo].[AspNetUserRoles]([RoleId] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[AspNetUserRoles]([UserId] ASC);
GO
CREATE TABLE [dbo].[AspNetUserLogins] (
    [UserId]        NVARCHAR (128) NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [ProviderKey]   NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_UserId]
    ON [dbo].[AspNetUserLogins]([UserId] ASC);
GO
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    [UserId]    NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_User_Id] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IX_User_Id]
    ON [dbo].[AspNetUserClaims]([UserId] ASC);
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
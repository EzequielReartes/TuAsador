-- ============================================================
-- TuAsador - Database Schema
-- ============================================================
-- Ejecutar en SQL Server Management Studio (SSMS) o sqlcmd.
-- Crea la base de datos y todas las tablas necesarias.
-- ============================================================

IF DB_ID('TuAsadorDev') IS NULL
    CREATE DATABASE [TuAsadorDev];
GO

USE [TuAsadorDev];
GO

-- ============================================================
-- TABLAS DE IDENTITY
-- ============================================================

CREATE TABLE [AspNetRoles] (
    [Id]               nvarchar(450)  NOT NULL,
    [Name]             nvarchar(256)  NULL,
    [NormalizedName]   nvarchar(256)  NULL,
    [ConcurrencyStamp] nvarchar(max)  NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [AspNetUsers] (
    [Id]                   nvarchar(450)   NOT NULL,
    [UserName]             nvarchar(256)   NULL,
    [NormalizedUserName]   nvarchar(256)   NULL,
    [Email]                nvarchar(256)   NULL,
    [NormalizedEmail]      nvarchar(256)   NULL,
    [EmailConfirmed]       bit             NOT NULL DEFAULT 0,
    [PasswordHash]         nvarchar(max)   NULL,
    [SecurityStamp]        nvarchar(max)   NULL,
    [ConcurrencyStamp]     nvarchar(max)   NULL,
    [PhoneNumber]          nvarchar(max)   NULL,
    [PhoneNumberConfirmed] bit             NOT NULL DEFAULT 0,
    [TwoFactorEnabled]     bit             NOT NULL DEFAULT 0,
    [LockoutEnd]           datetimeoffset  NULL,
    [LockoutEnabled]       bit             NOT NULL DEFAULT 0,
    [AccessFailedCount]    int             NOT NULL DEFAULT 0,
    -- Custom columns
    [Name]                 nvarchar(max)   NOT NULL,
    [WhatsApp]             nvarchar(max)   NULL,
    [Role]                 nvarchar(max)   NOT NULL DEFAULT N'Cliente',
    [IsActive]             bit             NOT NULL DEFAULT 1,
    [ProfilePictureUrl]    nvarchar(max)   NULL,
    [CreatedAt]            datetime2       NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [AspNetRoleClaims] (
    [Id]         int            IDENTITY(1,1) NOT NULL,
    [RoleId]     nvarchar(450)  NOT NULL,
    [ClaimType]  nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id]         int            IDENTITY(1,1) NOT NULL,
    [UserId]     nvarchar(450)  NOT NULL,
    [ClaimType]  nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider]       nvarchar(450)  NOT NULL,
    [ProviderKey]         nvarchar(450)  NOT NULL,
    [ProviderDisplayName] nvarchar(max)  NULL,
    [UserId]              nvarchar(450)  NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId])
        REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId]        nvarchar(450)  NOT NULL,
    [LoginProvider] nvarchar(450)  NOT NULL,
    [Name]          nvarchar(450)  NOT NULL,
    [Value]         nvarchar(max)  NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

-- ============================================================
-- TABLAS DE DOMINIO
-- ============================================================

CREATE TABLE [AsadorProfiles] (
    [Id]                uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId]            nvarchar(450)    NOT NULL,
    [Description]       nvarchar(max)    NULL,
    [Instagram]         nvarchar(max)    NULL,
    [PhotoUrl]          nvarchar(max)    NULL,
    [MainCity]          nvarchar(max)    NOT NULL,
    [Status]            nvarchar(max)    NOT NULL DEFAULT N'Pendiente',
    [CancellationRate]  float            NOT NULL DEFAULT 0,
    [AverageRating]     float            NOT NULL DEFAULT 0,
    [PunctualityRating] float            NOT NULL DEFAULT 0,
    [PresenceRating]    float            NOT NULL DEFAULT 0,
    [PerformanceRating] float            NOT NULL DEFAULT 0,
    [CreatedAt]         datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_AsadorProfiles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AsadorProfiles_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Specialties] (
    [Id]   uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Name] nvarchar(max)    NOT NULL,
    CONSTRAINT [PK_Specialties] PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [AsadorSpecialties] (
    [AsadoresId]    uniqueidentifier NOT NULL,
    [SpecialtiesId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AsadorSpecialties] PRIMARY KEY CLUSTERED ([AsadoresId] ASC, [SpecialtiesId] ASC),
    CONSTRAINT [FK_AsadorSpecialties_AsadorProfiles_AsadoresId] FOREIGN KEY ([AsadoresId])
        REFERENCES [AsadorProfiles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AsadorSpecialties_Specialties_SpecialtiesId] FOREIGN KEY ([SpecialtiesId])
        REFERENCES [Specialties] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [PortfolioImages] (
    [Id]              uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AsadorProfileId] uniqueidentifier NOT NULL,
    [ImageUrl]        nvarchar(max)    NOT NULL,
    [IsApproved]      bit              NULL,
    [CreatedAt]       datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_PortfolioImages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PortfolioImages_AsadorProfiles_AsadorProfileId] FOREIGN KEY ([AsadorProfileId])
        REFERENCES [AsadorProfiles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Availabilities] (
    [Id]              uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [AsadorProfileId] uniqueidentifier NOT NULL,
    [Date]            date             NOT NULL,
    [IsAvailable]     bit              NOT NULL DEFAULT 1,
    CONSTRAINT [PK_Availabilities] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Availabilities_AsadorProfiles_AsadorProfileId] FOREIGN KEY ([AsadorProfileId])
        REFERENCES [AsadorProfiles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Events] (
    [Id]              uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ClientId]        nvarchar(450)    NOT NULL,
    [Date]            datetime2        NOT NULL,
    [Time]            time             NOT NULL,
    [City]            nvarchar(max)    NOT NULL,
    [Address]         nvarchar(max)    NOT NULL,
    [PeopleCount]     int              NOT NULL,
    [EventType]       nvarchar(max)    NOT NULL,
    [ServiceDesired]  nvarchar(max)    NULL,
    [Notes]           nvarchar(max)    NULL,
    [Status]          nvarchar(max)    NOT NULL DEFAULT N'Disponible',
    [CreatedAt]       datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Events_AspNetUsers_ClientId] FOREIGN KEY ([ClientId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [EventImages] (
    [Id]        uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EventId]   uniqueidentifier NOT NULL,
    [ImageUrl]  nvarchar(max)    NOT NULL,
    [CreatedAt] datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_EventImages] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventImages_Events_EventId] FOREIGN KEY ([EventId])
        REFERENCES [Events] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [EventApplications] (
    [Id]              uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EventId]         uniqueidentifier NOT NULL,
    [AsadorProfileId] uniqueidentifier NOT NULL,
    [Status]          nvarchar(max)    NOT NULL DEFAULT N'Pendiente',
    [CreatedAt]       datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_EventApplications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EventApplications_Events_EventId] FOREIGN KEY ([EventId])
        REFERENCES [Events] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_EventApplications_AsadorProfiles_AsadorProfileId] FOREIGN KEY ([AsadorProfileId])
        REFERENCES [AsadorProfiles] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Contracts] (
    [Id]              uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ClientId]        nvarchar(450)    NOT NULL,
    [AsadorProfileId] uniqueidentifier NOT NULL,
    [EventId]         uniqueidentifier NULL,
    [Type]            nvarchar(max)    NOT NULL DEFAULT N'Directa',
    [Date]            datetime2        NOT NULL,
    [Time]            time             NOT NULL,
    [Address]         nvarchar(max)    NOT NULL,
    [City]            nvarchar(max)    NOT NULL,
    [PeopleCount]     int              NOT NULL,
    [EventType]       nvarchar(max)    NOT NULL,
    [ServiceDesired]  nvarchar(max)    NULL,
    [Notes]           nvarchar(max)    NULL,
    [Status]          nvarchar(max)    NOT NULL DEFAULT N'Pendiente',
    [CreatedAt]       datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Contracts_AspNetUsers_ClientId] FOREIGN KEY ([ClientId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Contracts_AsadorProfiles_AsadorProfileId] FOREIGN KEY ([AsadorProfileId])
        REFERENCES [AsadorProfiles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Contracts_Events_EventId] FOREIGN KEY ([EventId])
        REFERENCES [Events] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Ratings] (
    [Id]               uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ContractId]       uniqueidentifier NOT NULL,
    [ReviewerId]       nvarchar(450)    NOT NULL,
    [RevieweeId]       nvarchar(450)    NOT NULL,
    [PunctualityScore] int              NOT NULL,
    [PresenceScore]    int              NOT NULL,
    [PerformanceScore] int              NOT NULL,
    [Comment]          nvarchar(max)    NULL,
    [CreatedAt]        datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Ratings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Ratings_Contracts_ContractId] FOREIGN KEY ([ContractId])
        REFERENCES [Contracts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ratings_AspNetUsers_ReviewerId] FOREIGN KEY ([ReviewerId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ratings_AspNetUsers_RevieweeId] FOREIGN KEY ([RevieweeId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Notifications] (
    [Id]        uniqueidentifier NOT NULL DEFAULT NEWSEQUENTIALID(),
    [UserId]    nvarchar(450)    NOT NULL,
    [Type]      nvarchar(max)    NOT NULL,
    [Message]   nvarchar(max)    NOT NULL,
    [IsRead]    bit              NOT NULL DEFAULT 0,
    [CreatedAt] datetime2        NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId])
        REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

-- ============================================================
-- ÍNDICES
-- ============================================================

CREATE NONCLUSTERED INDEX [IX_AsadorProfiles_UserId] ON [AsadorProfiles] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AsadorSpecialties_SpecialtiesId] ON [AsadorSpecialties] ([SpecialtiesId]);
CREATE NONCLUSTERED INDEX [IX_PortfolioImages_AsadorProfileId] ON [PortfolioImages] ([AsadorProfileId]);
CREATE NONCLUSTERED INDEX [IX_Availabilities_AsadorProfileId] ON [Availabilities] ([AsadorProfileId]);
CREATE NONCLUSTERED INDEX [IX_Events_ClientId] ON [Events] ([ClientId]);
CREATE NONCLUSTERED INDEX [IX_EventImages_EventId] ON [EventImages] ([EventId]);
CREATE NONCLUSTERED INDEX [IX_EventApplications_EventId] ON [EventApplications] ([EventId]);
CREATE NONCLUSTERED INDEX [IX_EventApplications_AsadorProfileId] ON [EventApplications] ([AsadorProfileId]);
CREATE NONCLUSTERED INDEX [IX_Contracts_ClientId] ON [Contracts] ([ClientId]);
CREATE NONCLUSTERED INDEX [IX_Contracts_AsadorProfileId] ON [Contracts] ([AsadorProfileId]);
CREATE NONCLUSTERED INDEX [IX_Contracts_EventId] ON [Contracts] ([EventId]);
CREATE NONCLUSTERED INDEX [IX_Ratings_ContractId] ON [Ratings] ([ContractId]);
CREATE NONCLUSTERED INDEX [IX_Ratings_ReviewerId] ON [Ratings] ([ReviewerId]);
CREATE NONCLUSTERED INDEX [IX_Ratings_RevieweeId] ON [Ratings] ([RevieweeId]);
CREATE NONCLUSTERED INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

-- Identity indexes (generados por EF Core)
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
CREATE UNIQUE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]) WHERE [NormalizedEmail] IS NOT NULL;

GO

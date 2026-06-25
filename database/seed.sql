-- ============================================================
-- TuAsador - Seed Data
-- ============================================================
-- Inserta datos de prueba para desarrollo local.
-- Contraseña de todos los usuarios: TuAsador123!
-- ============================================================

USE [TuAsadorDev];
GO

-- ============================================================
-- ROLES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [Name] = N'Admin')
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (N'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA', N'Admin',   N'ADMIN',   NEWID());

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [Name] = N'Asador')
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (N'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB', N'Asador',  N'ASADOR',  NEWID());

IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [Name] = N'Cliente')
    INSERT INTO [AspNetRoles] ([Id], [Name], [NormalizedName], [ConcurrencyStamp])
    VALUES (N'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC', N'Cliente', N'CLIENTE', NEWID());
GO

-- ============================================================
-- USUARIOS
-- ============================================================
-- PasswordHash generado con PasswordHasher<IdentityUser> para "TuAsador123!"
-- SecurityStamp y ConcurrencyStamp son GUIDs fijos para reproducibilidad.

DECLARE @PasswordHash nvarchar(max) = N'AQAAAAIAAYagAAAAEDHyWcKT3W5K5ApqZ7vdvicDKidzE/qNPSJ4h5qFhPHgx2eYYONVJdnCPxuXOMVITQ==';
DECLARE @SecurityStamp nvarchar(max) = N'3e5cf4e6-d1e1-4c47-a58b-570bcfe5d59f';
DECLARE @ConcurrencyStamp nvarchar(max) = N'4781f3e0-7aa0-407a-9403-b8fca2673555';
DECLARE @Now datetime2 = SYSUTCDATETIME();

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [UserName] = N'admin')
    INSERT INTO [AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [Name], [Role], [IsActive], [CreatedAt])
    VALUES (N'A1B2C3D4-E5F6-7890-ABCD-EF1234567890', N'admin',       N'ADMIN',       N'admin@tuasador.com', N'ADMIN@TUASADOR.COM', 1, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, 0, 0, 0, 0, N'Admin TuAsador', N'Admin',   1, @Now);

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [UserName] = N'carlosgrill')
    INSERT INTO [AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [Name], [WhatsApp], [Role], [IsActive], [CreatedAt])
    VALUES (N'B2C3D4E5-F6A7-8901-BCDE-F12345678901', N'carlosgrill', N'CARLOSGRILL', N'carlos@email.com',  N'CARLOS@EMAIL.COM',  1, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, 0, 0, 0, 0, N'Carlos El Parrillero', N'+5491112345678', N'Asador',  1, @Now);

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [UserName] = N'martinfuego')
    INSERT INTO [AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [Name], [WhatsApp], [Role], [IsActive], [CreatedAt])
    VALUES (N'C3D4E5F6-A7B8-9012-CDEF-123456789012', N'martinfuego', N'MARTINFUEGO', N'martin@email.com',  N'MARTIN@EMAIL.COM',  1, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, 0, 0, 0, 0, N'Martin Fuego',       N'+5491123456789', N'Asador',  1, @Now);

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [UserName] = N'juanperez')
    INSERT INTO [AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [Name], [WhatsApp], [Role], [IsActive], [CreatedAt])
    VALUES (N'D4E5F6A7-B8C9-0123-DEFA-234567890123', N'juanperez',  N'JUANPEREZ',   N'juan@email.com',   N'JUAN@EMAIL.COM',    1, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, 0, 0, 0, 0, N'Juan Pérez',         N'+5491134567890', N'Cliente', 1, @Now);

IF NOT EXISTS (SELECT 1 FROM [AspNetUsers] WHERE [UserName] = N'marialog')
    INSERT INTO [AspNetUsers] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnabled], [AccessFailedCount], [Name], [WhatsApp], [Role], [IsActive], [CreatedAt])
    VALUES (N'E5F6A7B8-C9D0-1234-EFAB-345678901234', N'marialog',   N'MARIALOG',    N'maria@email.com',  N'MARIA@EMAIL.COM',   1, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, 0, 0, 0, 0, N'María López',        N'+5491145678901', N'Cliente', 1, @Now);
GO

-- ============================================================
-- ASIGNACIÓN DE ROLES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] WHERE [UserId] = N'A1B2C3D4-E5F6-7890-ABCD-EF1234567890')
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (N'A1B2C3D4-E5F6-7890-ABCD-EF1234567890', N'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA');

IF NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] WHERE [UserId] = N'B2C3D4E5-F6A7-8901-BCDE-F12345678901')
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (N'B2C3D4E5-F6A7-8901-BCDE-F12345678901', N'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB');

IF NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] WHERE [UserId] = N'C3D4E5F6-A7B8-9012-CDEF-123456789012')
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (N'C3D4E5F6-A7B8-9012-CDEF-123456789012', N'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB');

IF NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] WHERE [UserId] = N'D4E5F6A7-B8C9-0123-DEFA-234567890123')
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (N'D4E5F6A7-B8C9-0123-DEFA-234567890123', N'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC');

IF NOT EXISTS (SELECT 1 FROM [AspNetUserRoles] WHERE [UserId] = N'E5F6A7B8-C9D0-1234-EFAB-345678901234')
    INSERT INTO [AspNetUserRoles] ([UserId], [RoleId])
    VALUES (N'E5F6A7B8-C9D0-1234-EFAB-345678901234', N'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC');
GO

-- ============================================================
-- ESPECIALIDADES
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [Specialties])
BEGIN
    INSERT INTO [Specialties] ([Id], [Name]) VALUES (N'33333333-3333-3333-3333-333333333333', N'Asado');
    INSERT INTO [Specialties] ([Id], [Name]) VALUES (N'44444444-4444-4444-4444-444444444444', N'Estaca');
    INSERT INTO [Specialties] ([Id], [Name]) VALUES (N'55555555-5555-5555-5555-555555555555', N'Disco');
    INSERT INTO [Specialties] ([Id], [Name]) VALUES (N'66666666-6666-6666-6666-666666666666', N'Parrilla');
    INSERT INTO [Specialties] ([Id], [Name]) VALUES (N'77777777-7777-7777-7777-777777777777', N'Cocina al horno');
END
GO

-- ============================================================
-- PERFILES DE ASADOR
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [AsadorProfiles])
BEGIN
    INSERT INTO [AsadorProfiles] ([Id], [UserId], [MainCity], [Description], [Status], [Instagram])
    VALUES (N'11111111-1111-1111-1111-111111111111',
            N'B2C3D4E5-F6A7-8901-BCDE-F12345678901',
            N'Buenos Aires',
            N'Asador profesional con 10 años de experiencia en todo tipo de eventos.',
            N'Verificado',
            N'@carlosgrill');

    INSERT INTO [AsadorProfiles] ([Id], [UserId], [MainCity], [Description], [Status], [Instagram])
    VALUES (N'22222222-2222-2222-2222-222222222222',
            N'C3D4E5F6-A7B8-9012-CDEF-123456789012',
            N'Córdoba',
            N'Especialista en asados tradicionales y cocina al disco.',
            N'Pendiente',
            N'@martinfuego');
END
GO

-- ============================================================
-- IMÁGENES DE PORTFOLIO
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [PortfolioImages])
BEGIN
    INSERT INTO [PortfolioImages] ([Id], [AsadorProfileId], [ImageUrl], [IsApproved])
    VALUES (N'88888888-8888-8888-8888-888888888888',
            N'11111111-1111-1111-1111-111111111111',
            N'https://placehold.co/600x400/1a3a1a/46cf5d?text=Parrillada+Completa', 1);

    INSERT INTO [PortfolioImages] ([Id], [AsadorProfileId], [ImageUrl], [IsApproved])
    VALUES (N'99999999-9999-9999-9999-999999999999',
            N'11111111-1111-1111-1111-111111111111',
            N'https://placehold.co/600x400/3a2a1a/f59e0b?text=Asado+Pendiente', NULL);

    INSERT INTO [PortfolioImages] ([Id], [AsadorProfileId], [ImageUrl], [IsApproved])
    VALUES (N'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAA1',
            N'11111111-1111-1111-1111-111111111111',
            N'https://placehold.co/600x400/991b1b/ffffff?text=Bondiola+Rechazada', 0);

    INSERT INTO [PortfolioImages] ([Id], [AsadorProfileId], [ImageUrl], [IsApproved])
    VALUES (N'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBB1',
            N'22222222-2222-2222-2222-222222222222',
            N'https://placehold.co/600x400/3a2a1a/f59e0b?text=Estaca+Criolla', NULL);

    INSERT INTO [PortfolioImages] ([Id], [AsadorProfileId], [ImageUrl], [IsApproved])
    VALUES (N'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCC1',
            N'22222222-2222-2222-2222-222222222222',
            N'https://placehold.co/600x400/3a2a1a/f59e0b?text=Parrilla+de+Campo', NULL);
END
GO

-- ============================================================
-- EVENTOS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM [Events])
BEGIN
    INSERT INTO [Events] ([Id], [ClientId], [Date], [Time], [City], [Address], [PeopleCount], [EventType], [ServiceDesired], [Notes], [Status])
    VALUES (N'DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDD1',
            N'D4E5F6A7-B8C9-0123-DEFA-234567890123',
            DATEADD(DAY, 15, SYSUTCDATETIME()), '12:00:00',
            N'Buenos Aires', N'Av. Corrientes 1234', 15,
            N'Cumpleaños', N'Parrilla completa',
            N'Cerca del mediodía, ideal si traen todo el equipo',
            N'Disponible');

    INSERT INTO [Events] ([Id], [ClientId], [Date], [Time], [City], [Address], [PeopleCount], [EventType], [ServiceDesired], [Notes], [Status])
    VALUES (N'EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEE1',
            N'D4E5F6A7-B8C9-0123-DEFA-234567890123',
            DATEADD(DAY, 30, SYSUTCDATETIME()), '20:00:00',
            N'Buenos Aires', N'Calle Florida 567', 8,
            N'Cena familiar', N'Asado criollo',
            NULL,
            N'Disponible');

    INSERT INTO [Events] ([Id], [ClientId], [Date], [Time], [City], [Address], [PeopleCount], [EventType], [ServiceDesired], [Notes], [Status])
    VALUES (N'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF',
            N'E5F6A7B8-C9D0-1234-EFAB-345678901234',
            DATEADD(DAY, 20, SYSUTCDATETIME()), '11:00:00',
            N'La Plata', N'Calle 7 y 50', 25,
            N'Evento empresarial', N'Parrilla con ensaladas y acompañamientos',
            N'Preferentemente con experiencia en eventos corporativos',
            N'Disponible');
END
GO

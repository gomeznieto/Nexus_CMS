USE [master]
GO

-- Crear base de datos
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'blog')
BEGIN
    ALTER DATABASE [blog] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [blog]
END

CREATE DATABASE [blog]
GO

USE [blog]
GO

-- Habilitar configuraciones iniciales
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Crear tabla de usuarios primero (referenciada por varias tablas)
CREATE TABLE [dbo].[users](
    [id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[username] [varchar](100) NOT NULL,
	[usernameNormalizado] [varchar](100) NOT NULL,
	[role] [int] NOT NULL,
	[img] [varchar](255) NULL,
	[headline] [varchar](255) NULL,
	[cv] [varchar](255) NULL,
	[about] [varchar](255) NULL,
	[hobbies] [varchar](255) NULL,
	[email] [varchar](100) NOT NULL,
	[passwordHash] [varchar](100) NOT NULL,
	[emailNormalizado] [varchar](255) NULL,
	[securityStamp] [uniqueidentifier] NULL,
	[apiKey] [varchar](255) NULL
	CONSTRAINT [PK_users] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- Crear tabla de roles
CREATE TABLE [dbo].[role](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	CONSTRAINT [PK_role] PRIMARY KEY CLUSTERED ([id] ASC)
) ON [PRIMARY]
GO

-- Crear tabla de biografías
CREATE TABLE [dbo].[bio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[work] [varchar](1000) NOT NULL,
	[year] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	CONSTRAINT [PK_bio] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_bio_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
) ON [PRIMARY]
GO

-- Crear tabla de categorías
CREATE TABLE [dbo].[category](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[user_id] [int] NULL,
	CONSTRAINT [PK_category] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_category_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
) ON [PRIMARY]
GO

-- Crear tabla de formatos
CREATE TABLE [dbo].[format](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[user_id] [int] NULL,
	CONSTRAINT [PK_format] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_format_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
) ON [PRIMARY]
GO

-- Crear tabla de publicaciones
CREATE TABLE [dbo].[post](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[title] [varchar](100) NOT NULL,
	[description] [text] NOT NULL,
	[cover] [varchar](100) NULL,
	[user_id] [int] NOT NULL,
	[format_id] [int] NOT NULL,
	[created_at] [datetime] NOT NULL DEFAULT (getdate()),
	[modify_at] [datetime] NULL DEFAULT (NULL),
	[draft] [bit] NOT NULL DEFAULT ((0)),
	CONSTRAINT [PK_post] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_post_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]),
	CONSTRAINT [FK_post_format] FOREIGN KEY ([format_id]) REFERENCES [dbo].[format]([id])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Crear tabla de fuentes
CREATE TABLE [dbo].[source](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[icon] [varchar](100) NOT NULL,
	[user_id] [int] NULL,
	CONSTRAINT [PK_source] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_source_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
) ON [PRIMARY]
GO

-- Crear tabla de enlaces
CREATE TABLE [dbo].[link](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[url] [varchar](100) NOT NULL,
	[post_id] [int] NOT NULL,
	[source_id] [int] NOT NULL,
	CONSTRAINT [PK_link] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_link_post] FOREIGN KEY ([post_id]) REFERENCES [dbo].[post]([id]),
	CONSTRAINT [FK_link_source] FOREIGN KEY ([source_id]) REFERENCES [dbo].[source]([id])
) ON [PRIMARY]
GO

-- Crear tabla de tipos de medios
CREATE TABLE [dbo].[mediatype](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[user_id] [int] NULL,
	CONSTRAINT [PK_mediatype] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_mediatype_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
) ON [PRIMARY]
GO

-- Crear tabla de medios
CREATE TABLE [dbo].[media](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[url] [varchar](100) NOT NULL,
	[post_id] [int] NOT NULL,
	[mediatype_id] [int] NOT NULL,
	CONSTRAINT [PK_media] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_media_post] FOREIGN KEY ([post_id]) REFERENCES [dbo].[post]([id]),
	CONSTRAINT [FK_media_mediatype] FOREIGN KEY ([mediatype_id]) REFERENCES [dbo].[mediatype]([id])
) ON [PRIMARY]
GO

-- Crear tabla de redes sociales
CREATE TABLE [dbo].[social_network](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [varchar](100) NOT NULL,
	[url] [varchar](100) NOT NULL,
	[username] [varchar](100) NOT NULL,
	[icon] [varchar](100) NOT NULL,
	[user_id] [int] NOT NULL,
	CONSTRAINT [PK_social_network] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_social_network_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id])
) ON [PRIMARY]
GO

-- Crear tabla de relación entre categorías y publicaciones
CREATE TABLE [dbo].[category_post](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[category_id] [int] NOT NULL,
	[post_id] [int] NOT NULL,
	CONSTRAINT [PK_category_post] PRIMARY KEY CLUSTERED ([id] ASC),
	CONSTRAINT [FK_category_post_category] FOREIGN KEY ([category_id]) REFERENCES [dbo].[category]([id]),
	CONSTRAINT [FK_category_post_post] FOREIGN KEY ([post_id]) REFERENCES [dbo].[post]([id])
) ON [PRIMARY]
GO


-- Crear tabla de relación entre usuarios y roles
CREATE TABLE [dbo].[user_role](
	[user_id] INT NOT NULL,          
	[role_id] INT NOT NULL,   
	CONSTRAINT [PK_user_role] PRIMARY KEY CLUSTERED ([user_id], [role_id]), -- Clave primaria combinada
	CONSTRAINT [FK_user_role_user] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]), -- Clave foránea hacia users
	CONSTRAINT [FK_user_role_role] FOREIGN KEY ([role_id]) REFERENCES [dbo].[role]([id])   -- Clave foránea hacia role
) ON [PRIMARY];
GO

-- Tabla de secciones del home
CREATE TABLE HomeSection (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    [Order] INT NOT NULL,
    MaxItems INT NULL,
    UserId INT NOT NULL,

    CONSTRAINT FK_HomeSection_Users FOREIGN KEY (UserId)
        REFERENCES users(Id) ON DELETE CASCADE

);

-- Tabla de relación entre sección y post
CREATE TABLE HomeSectionPost (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HomeSectionId INT NOT NULL,
    PostId INT NOT NULL,
    [Order] INT NOT NULL,

    CONSTRAINT FK_HomeSectionPost_HomeSection FOREIGN KEY (HomeSectionId)
        REFERENCES HomeSection(Id) ON DELETE CASCADE,

    CONSTRAINT FK_HomeSectionPost_Post FOREIGN KEY (PostId)
        REFERENCES Post(Id) ON DELETE CASCADE,

    CONSTRAINT UQ_HomeSectionPost UNIQUE (HomeSectionId, [Order]),
    CONSTRAINT UQ_HomeSectionPostPost UNIQUE (HomeSectionId, PostId)
);

-- Tabla de Layout Home Section
CREATE TABLE UserLayoutSections (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    SectionType NVARCHAR(50) NOT NULL,
    SectionId INT NOT NULL,
    DisplayOrder INT NOT NULL,
	Title NVARCHAR(50) NOT NULL,
	SectionConfig NVARCHAR(MAX) NULL,
    CONSTRAINT UQ_UserSectionOrder UNIQUE (UserId, DisplayOrder)
);
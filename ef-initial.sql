IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Category] (
    [CategoryID] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([CategoryID])
);

CREATE TABLE [Product] (
    [ProductID] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Image] nvarchar(500) NULL,
    CONSTRAINT [PK_Product] PRIMARY KEY ([ProductID])
);

CREATE TABLE [ProductCategory] (
    [ProductID] int NOT NULL,
    [CategoryID] int NOT NULL,
    CONSTRAINT [PK_ProductCategory] PRIMARY KEY ([ProductID], [CategoryID]),
    CONSTRAINT [FK_ProductCategory_Category_CategoryID] FOREIGN KEY ([CategoryID]) REFERENCES [Category] ([CategoryID]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductCategory_Product_ProductID] FOREIGN KEY ([ProductID]) REFERENCES [Product] ([ProductID]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_Category_Name] ON [Category] ([Name]);

CREATE INDEX [IX_Product_Name] ON [Product] ([Name]);

CREATE INDEX [IX_ProductCategory_CategoryID] ON [ProductCategory] ([CategoryID]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250819134712_InitialCreate', N'9.0.8');

COMMIT;
GO


USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagementDb')
BEGIN
    CREATE DATABASE TaskManagementDb;
END
GO

USE TaskManagementDb;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id        INT           IDENTITY(1,1) PRIMARY KEY,
        FirstName NVARCHAR(50)  NOT NULL,
        LastName  NVARCHAR(50)  NOT NULL,
        Email     NVARCHAR(150) NOT NULL
    );
    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Projects' AND xtype='U')
BEGIN
    CREATE TABLE Projects (
        Id          INT           IDENTITY(1,1) PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Tasks' AND xtype='U')
BEGIN
    CREATE TABLE Tasks (
        Id             INT           IDENTITY(1,1) PRIMARY KEY,
        Title          NVARCHAR(200) NOT NULL,
        Description    NVARCHAR(MAX) NULL,
        Status         INT           NOT NULL DEFAULT 0,
        ProjectId      INT           NOT NULL,
        AssignedUserId INT           NULL,
        CONSTRAINT FK_Tasks_Projects FOREIGN KEY (ProjectId)      REFERENCES Projects(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Tasks_Users    FOREIGN KEY (AssignedUserId) REFERENCES Users(Id)    ON DELETE SET NULL
    );
    CREATE INDEX IX_Tasks_ProjectId      ON Tasks(ProjectId);
    CREATE INDEX IX_Tasks_AssignedUserId ON Tasks(AssignedUserId);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comments' AND xtype='U')
BEGIN
    CREATE TABLE Comments (
        Id      INT            IDENTITY(1,1) PRIMARY KEY,
        Content NVARCHAR(1000) NOT NULL,
        TaskId  INT            NOT NULL,
        CONSTRAINT FK_Comments_Tasks FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_Comments_TaskId ON Comments(TaskId);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='__EFMigrationsHistory' AND xtype='U')
BEGIN
    CREATE TABLE __EFMigrationsHistory (
        MigrationId    NVARCHAR(150) NOT NULL PRIMARY KEY,
        ProductVersion NVARCHAR(32)  NOT NULL
    );
    INSERT INTO __EFMigrationsHistory VALUES ('20240601000000_InitialCreate', '8.0.4');
END
GO

IF NOT EXISTS (SELECT 1 FROM Users)
BEGIN
    SET IDENTITY_INSERT Users ON;
    INSERT INTO Users (Id, FirstName, LastName, Email) VALUES
        (1, 'Giorgi', 'Beridze',   'giorgi.beridze@gmail.com'),
        (2, 'Nikoloz',   'Shelia', 'n_shelia@cu.edu.ge');
    SET IDENTITY_INSERT Users OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM Projects)
BEGIN
    SET IDENTITY_INSERT Projects ON;
    INSERT INTO Projects (Id, Name, Description) VALUES
        (1, 'Website Redesign', 'Full redesign of the company website'),
        (2, 'Mobile App',       'Cross-platform mobile application');
    SET IDENTITY_INSERT Projects OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM Tasks)
BEGIN
    SET IDENTITY_INSERT Tasks ON;
    INSERT INTO Tasks (Id, Title, Description, Status, ProjectId, AssignedUserId) VALUES
        (1, 'Design mockups', 'Create initial design mockups', 2, 1, 1),
        (2, 'Implement API',  'Build REST API endpoints',      1, 1, 2),
        (3, 'Setup CI/CD',    'Configure pipeline',            0, 2, NULL);
    SET IDENTITY_INSERT Tasks OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM Comments)
BEGIN
    SET IDENTITY_INSERT Comments ON;
    INSERT INTO Comments (Id, Content, TaskId) VALUES
        (1, 'Mockups approved by client.',             1),
        (2, 'Starting with authentication endpoints.', 2);
    SET IDENTITY_INSERT Comments OFF;
END
GO

PRINT 'Database TaskManagementDb created and seeded successfully.';
GO

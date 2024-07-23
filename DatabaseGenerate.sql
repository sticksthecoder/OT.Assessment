-- Create the database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OT_Assessment_DB')
BEGIN
    CREATE DATABASE OT_Assessment_DB;
END
GO

USE OT_Assessment_DB;
GO

-- Drop existing tables and procedures if they exist
IF OBJECT_ID('dbo.PlayerAccount', 'U') IS NOT NULL
    DROP TABLE dbo.PlayerAccount;
IF OBJECT_ID('dbo.PlayerCasinoWager', 'U') IS NOT NULL
    DROP TABLE dbo.PlayerCasinoWager;
IF OBJECT_ID('dbo.AddPlayerAccount', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddPlayerAccount;
IF OBJECT_ID('dbo.GetPlayerAccountById', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetPlayerAccountById;
IF OBJECT_ID('dbo.AddCasinoWager', 'P') IS NOT NULL
    DROP PROCEDURE dbo.AddCasinoWager;
IF OBJECT_ID('dbo.GetCasinoWagersByAccountId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetCasinoWagersByAccountId;
IF OBJECT_ID('dbo.GetTotalCasinoWagersByAccountId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetTotalCasinoWagersByAccountId;
IF OBJECT_ID('dbo.GetTopSpenders', 'P') IS NOT NULL
    DROP PROCEDURE dbo.GetTopSpenders;
GO

-- Create the PlayerAccount table
CREATE TABLE PlayerAccount (
    AccountId UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL
);
GO

-- Create an index on Username
CREATE INDEX IDX_PlayerAccount_Username ON PlayerAccount(Username);
GO

-- Stored procedure to insert a new player account
CREATE PROCEDURE AddPlayerAccount
    @AccountId UNIQUEIDENTIFIER,
    @Username NVARCHAR(50)
AS
BEGIN
    INSERT INTO PlayerAccount (AccountId, Username)
    VALUES (@AccountId, @Username);
END;
GO

-- Stored procedure to retrieve a player account by AccountId
CREATE PROCEDURE GetPlayerAccountById
    @AccountId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT AccountId, Username
    FROM PlayerAccount
    WHERE AccountId = @AccountId;
END;
GO

-- Create the PlayerCasinoWager table
CREATE TABLE PlayerCasinoWager (
    WagerId UNIQUEIDENTIFIER PRIMARY KEY,
    Game NVARCHAR(100) NOT NULL,
    Provider NVARCHAR(100) NOT NULL,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18, 2) NOT NULL,
    CreatedDateTime DATETIME2 NOT NULL,
    CONSTRAINT FK_PlayerCasinoWager_PlayerAccount FOREIGN KEY (AccountId) REFERENCES PlayerAccount(AccountId)
);
GO

-- Create an index on AccountId and CreatedDateTime
CREATE INDEX IDX_PlayerCasinoWager_AccountId_CreatedDateTime ON PlayerCasinoWager(AccountId, CreatedDateTime);
GO

-- Stored procedure to insert a new casino wager
CREATE PROCEDURE AddCasinoWager
    @WagerId UNIQUEIDENTIFIER,
    @GameName NVARCHAR(100),
    @Provider NVARCHAR(100),
    @AccountId UNIQUEIDENTIFIER,
    @Amount DECIMAL(18, 2),
    @CreatedDateTime DATETIME2
AS
BEGIN
    INSERT INTO PlayerCasinoWager (WagerId, Game, Provider, AccountId, Amount, CreatedDateTime)
    VALUES (@WagerId, @GameName, @Provider, @AccountId, @Amount, @CreatedDateTime);
END;
GO

-- Stored procedure to retrieve casino wagers by AccountId
CREATE PROCEDURE GetCasinoWagersByAccountId
    @AccountId UNIQUEIDENTIFIER,
    @Page INT,
    @PageSize INT
AS
BEGIN
    SELECT WagerId, Game, Provider, Amount, CreatedDateTime
    FROM PlayerCasinoWager
    WHERE AccountId = @AccountId
    ORDER BY CreatedDateTime DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- Stored procedure to get total casino wagers count by AccountId
CREATE PROCEDURE GetTotalCasinoWagersByAccountId
    @AccountId UNIQUEIDENTIFIER
AS
BEGIN
    SELECT COUNT(*)
    FROM PlayerCasinoWager
    WHERE AccountId = @AccountId;
END;
GO

-- Stored procedure to retrieve top spenders
CREATE PROCEDURE GetTopSpenders
    @Count INT
AS
BEGIN
    SELECT TOP (@Count) AccountId, SUM(Amount) AS TotalAmountSpend
    FROM PlayerCasinoWager
    GROUP BY AccountId
    ORDER BY SUM(Amount) DESC;
END;
GO

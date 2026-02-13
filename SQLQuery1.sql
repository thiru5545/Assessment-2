CREATE DATABASE OttPlatformDB;
GO
USE OttPlatformDB;
GO

-- Enums as INT
-- Subscription: 0 = Free, 1 = Premium
-- Role: 0 = User, 1 = Admin
-- RequestType: 0 = Pending, 1 = Approved, 2 = Rejected

CREATE TABLE Users (
    Id INT IDENTITY(1000,1) PRIMARY KEY,
    Username NVARCHAR(50),
    Password NVARCHAR(50),
    Email NVARCHAR(100),
    Subscription INT,
    Role INT
);


CREATE TABLE Videos (
    VideoId INT PRIMARY KEY,
    VideoName NVARCHAR(100),
    VideoUrl NVARCHAR(200),
    Subscription INT
);

CREATE TABLE Requests (
    RequestId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    Message NVARCHAR(200),
    RequestType INT,
    Status INT DEFAULT 0,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);


DROP PROCEDURE IF EXISTS [dbo].[sp_CreateUser];
DROP PROCEDURE IF EXISTS [dbo].[sp_ViewAllUsers];
DROP PROCEDURE IF EXISTS [dbo].[sp_RemoveUser];
DROP PROCEDURE IF EXISTS [dbo].[sp_UserLogin];
GO



--  Create User Procedure
CREATE PROCEDURE [dbo].[sp_CreateUser]
    @Username NVARCHAR(100),
    @Password NVARCHAR(100),
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if username already exists
    IF EXISTS (SELECT 1 FROM Users WHERE Username = @Username)
    BEGIN
        SELECT -1 AS NewUserId; -- Username already exists
        RETURN;
    END

    -- Check if email already exists
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
    BEGIN
        SELECT -2 AS NewUserId; -- Email already exists
        RETURN;
    END

    -- Insert new user
    INSERT INTO Users (Username, Password, Email, Subscription, Role)
    VALUES (@Username, @Password, @Email, 0, 0);

    -- Return the new user ID
    SELECT SCOPE_IDENTITY() AS NewUserId;
END
GO

--  View All Users Procedure
CREATE PROCEDURE [dbo].[sp_ViewAllUsers]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Username, Email, Subscription, Role
    FROM Users
    ORDER BY Id;
END
GO


CREATE PROCEDURE [dbo].[sp_RemoveUser]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Delete user
    DELETE FROM Users 
    WHERE Id = @UserId;

 
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO

-- User Login Procedure
CREATE PROCEDURE [dbo].[sp_UserLogin]
    @UserId INT,
    @Password NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

   
    SELECT Role 
    FROM Users 
    WHERE Id = @UserId AND Password = @Password;
END
GO
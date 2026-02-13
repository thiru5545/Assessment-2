

--  Add Request Procedure
CREATE PROCEDURE [dbo].[sp_AddRequest]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Requests (UserId, Message, RequestType) 
    VALUES (@UserId, 'Upgrade to Premium', 0);

    SELECT SCOPE_IDENTITY() AS NewRequestId;
END
GO

--  View All Requests Procedure
CREATE PROCEDURE [dbo].[sp_ViewAllRequests]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RequestId, UserId, RequestType, Message
    FROM Requests;
END
GO

--  View User Requests Procedure
CREATE PROCEDURE [dbo].[sp_ViewUserRequests]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RequestId, RequestType
    FROM Requests 
    WHERE UserId = @UserId;
END
GO

--  View Pending Requests Procedure
CREATE PROCEDURE [dbo].[sp_ViewPendingRequests]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT RequestId, UserId, Message, RequestType
    FROM Requests 
    WHERE RequestType = 0;
END
GO
 
-- Approve Request Procedure (with transaction)
CREATE PROCEDURE [dbo].[sp_ApproveRequest]
    @RequestId INT,
    @Success BIT OUTPUT,
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT;

    BEGIN TRANSACTION;

    BEGIN TRY
        -- Get UserId from Request
        SELECT @UserId = UserId 
        FROM Requests 
        WHERE RequestId = @RequestId AND RequestType = 0;

        IF @UserId IS NULL
        BEGIN
            SET @Success = 0;
            SET @Message = 'Invalid Request ID or request already processed.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        UPDATE Requests 
        SET RequestType = 1 
        WHERE RequestId = @RequestId;

        UPDATE Users 
        SET Subscription = 1 
        WHERE Id = @UserId;

        COMMIT TRANSACTION;

        SET @Success = 1;
        SET @Message = 'Request approved successfully.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Success = 0;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE PROCEDURE [dbo].[sp_RejectRequest]
    @RequestId INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Requests 
    SET RequestType = 2 
    WHERE RequestId = @RequestId AND RequestType = 0;

    SELECT @@ROWCOUNT AS RowsAffected;
END
GO



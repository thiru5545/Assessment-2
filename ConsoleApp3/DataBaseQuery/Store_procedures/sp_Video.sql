-- Drop existing procedures if they exist
DROP PROCEDURE IF EXISTS [dbo].[sp_VideoList];
DROP PROCEDURE IF EXISTS [dbo].[sp_VideoListByUser];
DROP PROCEDURE IF EXISTS [dbo].[sp_AddVideo];
DROP PROCEDURE IF EXISTS [dbo].[sp_RemoveVideo];
GO



--  Video List (All Videos) Procedure
CREATE PROCEDURE [dbo].[sp_VideoList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT VideoId, VideoName, VideoUrl, Subscription
    FROM Videos
    ORDER BY VideoId;
END
GO

--  Video List By User Subscription Procedure
CREATE PROCEDURE [dbo].[sp_VideoListByUser]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserSubscription INT;

    -- Get user subscription status
    SELECT @UserSubscription = Subscription
    FROM Users
    WHERE Id = @UserId;

    -- If user not found, return nothing
    IF @UserSubscription IS NULL
    BEGIN
        SELECT -1 AS ErrorCode, 'User not found.' AS ErrorMessage;
        RETURN;
    END

    -- If free user (Subscription = 0), show only free videos
    IF @UserSubscription = 0
    BEGIN
        SELECT VideoId, VideoName, VideoUrl, Subscription
        FROM Videos
        WHERE Subscription = 0
        ORDER BY VideoId;
    END
    ELSE
    BEGIN
        -- Premium user (Subscription = 1), show all videos
        SELECT VideoId, VideoName, VideoUrl, Subscription
        FROM Videos
        ORDER BY VideoId;
    END
END
GO

--  Add Video Procedure
CREATE PROCEDURE [dbo].[sp_AddVideo]
    @VideoName NVARCHAR(100),
    @VideoUrl NVARCHAR(200),
    @Subscription INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if video with same name already exists
    IF EXISTS (SELECT 1 FROM Videos WHERE VideoName = @VideoName)
    BEGIN
        SELECT -1 AS NewVideoId; -- Video already exists
        RETURN;
    END

    -- Validate subscription value (0 or 1)
    IF @Subscription NOT IN (0, 1)
    BEGIN
        SELECT -2 AS NewVideoId; -- Invalid subscription value
        RETURN;
    END

    -- Insert new video
    INSERT INTO Videos (VideoName, VideoUrl, Subscription)
    VALUES (@VideoName, @VideoUrl, @Subscription);

    -- Return the new video ID
    SELECT SCOPE_IDENTITY() AS NewVideoId;
END
GO

-- Remove Video Procedure
CREATE PROCEDURE [dbo].[sp_RemoveVideo]
    @VideoId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Delete video
    DELETE FROM Videos 
    WHERE VideoId = @VideoId;

    -- Return number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END
GO
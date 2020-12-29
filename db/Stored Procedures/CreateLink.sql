
CREATE PROCEDURE [dbo].[CreateLink]
(
    @UserID UNIQUEIDENTIFIER,
    @ID UNIQUEIDENTIFIER,
    @Title NVARCHAR(256),
    @Url NVARCHAR(256),
    @Abstract NVARCHAR(512),
    @Tags [dbo].[TagList] READONLY
)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @Now DATETIME = GETDATE()

    INSERT INTO
        Links (
            ID,
            Title,
            [Url],
            Abstract,
            UserID,
            Created,
            Updated
        )
    VALUES (
        @ID,
        @Title,
        @Url,
        @Abstract,
        @UserID,
        @Now,
        @Now
    )

    EXEC UpdateTags @UserID, @ID, @Tags
END

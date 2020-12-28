
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

    INSERT INTO
        Links (
            ID,
            Title,
            [Url],
            Abstract,
            UserID
        )
    VALUES (
        @ID,
        @Title,
        @Url,
        @Abstract,
        @UserID
    )

    EXEC UpdateTags @ID, @Tags
END


CREATE PROCEDURE [dbo].[UpdateLink]
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

    UPDATE
        Links
    SET
        Title = @Title,
        [Url] = @Url,
        Abstract = @Abstract,
        UserID = @UserID
    WHERE
        ID = @ID

    EXEC UpdateTags @ID, @Tags
END

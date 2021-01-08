
CREATE PROCEDURE [dbo].[ReadLinks]
(
    @UserID UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        ID,
        Title,
        Url,
        Abstract,
        Tags
    FROM
        Links
    WHERE
        UserID = @UserID
    ORDER BY
        Title
END

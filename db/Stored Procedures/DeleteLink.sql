
CREATE PROCEDURE [dbo].[DeleteLink]
(
    @ID UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON

    /*
    INSERT INTO
        DeletedLinks (
            ID,
            Title,
            [Url],
            Abstract,
            UserID
        )
    SELECT
        ID,
        Title,
        [Url],
        Abstract,
        UserID
    FROM
        Links
    WHERE
        ID = @ID
    */

    DELETE
        t
    FROM
        Tags_Links t
    WHERE
        t.LinkID = @ID

    DELETE
        l
    FROM
        Links l
    WHERE
        l.ID = @ID
END

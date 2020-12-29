
CREATE PROCEDURE [dbo].[DeleteLink]
(
    @UserID UNIQUEIDENTIFIER,
    @ID UNIQUEIDENTIFIER
)
AS
BEGIN
    SET NOCOUNT ON

    IF EXISTS (SELECT * FROM Links WHERE ID = @ID AND UserID = @UserID)
    BEGIN
        INSERT INTO
            DeletedLinks (
                ID,
                Title,
                [Url],
                Abstract,
                UserID,
                Deleted
            )
        SELECT
            ID,
            Title,
            [Url],
            Abstract,
            UserID,
            GETDATE()
        FROM
            Links
        WHERE
            ID = @ID

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
END

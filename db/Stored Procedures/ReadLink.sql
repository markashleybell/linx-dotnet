
CREATE PROCEDURE [dbo].[ReadLink]
(
    @ID UNIQUEIDENTIFIER = NULL
)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @Link TABLE (
        [ID]       UNIQUEIDENTIFIER  NOT NULL,
        [Title]    NVARCHAR (256) NOT NULL,
        [Url]     NVARCHAR (256) NULL,
        [Abstract] NVARCHAR (512) NULL,
        [Tags]     NVARCHAR(1024) NULL
    )

    INSERT INTO
        @Link
    SELECT
        ID,
        Title,
        [Url],
        Abstract,
        NULL
    FROM
        Links
    WHERE
        ID = @ID

    UPDATE
        link
    SET
        Tags = (
            SELECT STUFF((
                SELECT
                    '|' + [Label]
                FROM
                    Tags
                INNER JOIN
                    Tags_Links ON Tags_Links.TagID = Tags.ID
                WHERE
                    Tags_Links.LinkID = link.ID
                FOR XML PATH ('')
            ), 1, 1, '')
        )
    FROM
        @Link link

    SELECT * FROM @Link
END

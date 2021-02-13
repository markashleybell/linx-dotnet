
CREATE PROCEDURE [dbo].[MergeTags]
(
    @UserID UNIQUEIDENTIFIER,
    @TagID UNIQUEIDENTIFIER,
    @TagIdsToMerge [dbo].[GuidList] READONLY
)
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRY
        BEGIN TRAN
            -- Update any references to tags we're merging to point to the merge target tag
            UPDATE
                td
            SET
                td.TagID = @TagID
            FROM
                Tags_Links td
            INNER JOIN
                Tags t ON t.ID = td.TagID AND t.UserID = @UserID
            INNER JOIN
                @TagIdsToMerge m ON m.ID = td.TagID
            WHERE NOT EXISTS (
                -- If there is already a relationship between this document and the merge target,
                -- we need to ignore it, otherwise we'll attempt to add a duplicate
                SELECT * FROM Tags_Links c WHERE c.TagID = @TagID and c.LinkID = td.LinkID
            )

            -- Clean up any join records referencing merged tags which didn't get updated above
            DELETE
                td
            FROM
                Tags_Links td
            INNER JOIN
                @TagIdsToMerge m ON m.ID = td.TagID

            -- Delete the original tag records
            DELETE
                t
            FROM
                Tags t
            INNER JOIN
                @TagIdsToMerge m ON m.ID = t.ID

            -- Update the Tags column in Links for efficient display
            DECLARE @DisplayTags TABLE (id UNIQUEIDENTIFIER, dt NVARCHAR(64))

            INSERT INTO
                @DisplayTags
            SELECT
                tl.LinkID,
                t.[Label]
            FROM
                Tags t
            INNER JOIN
                Tags_Links tl ON tl.TagID = t.ID
            WHERE
                t.UserID = @UserID

            UPDATE
                l
            SET
                Tags = (SELECT STRING_AGG(t.dt, '|') FROM @DisplayTags t WHERE t.id = l.ID)
            FROM
                Links l
        COMMIT TRAN
    END TRY
    BEGIN CATCH
        IF(@@TRANCOUNT > 0)
            ROLLBACK TRAN;

        THROW;
    END CATCH
END

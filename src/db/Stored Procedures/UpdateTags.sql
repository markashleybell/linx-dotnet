
CREATE PROCEDURE [dbo].[UpdateTags]
(
    @UserID UNIQUEIDENTIFIER,
    @LinkID UNIQUEIDENTIFIER,
    @Tags [dbo].[TagList] READONLY
)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @TagsToLink TABLE (
        TagID UNIQUEIDENTIFIER,
        [Label] NVARCHAR(64)
    )

    DECLARE @NewTagData TABLE (
        TagID UNIQUEIDENTIFIER,
        [Label] NVARCHAR(64)
    )

    -- For any tags which were passed in that already exist, use the current record
    INSERT INTO
        @TagsToLink (
            TagID,
            [Label]
        )
    SELECT
        t.ID,
        t.[Label]
    FROM
        Tags t
    INNER JOIN
        @Tags dt ON dt.[Label] = t.Label AND t.UserID = @UserID

    -- Create tag records for any new tags (those which aren't already in @TagsToLink)
    INSERT INTO
        @NewTagData (
            TagID,
            [Label]
        )
    SELECT
        NEWID() AS TagID,
        [Label]
    FROM
        @Tags t
    WHERE
        NOT EXISTS (SELECT * FROM @TagsToLink WHERE [Label] = t.[Label])

    INSERT INTO
        Tags (
            ID,
            [Label],
            UserID
        )
    SELECT
        TagID,
        [Label],
        @UserID
    FROM
        @NewTagData

    -- Add the newly-created tag data to the update table
    INSERT INTO
        @TagsToLink (
            TagID,
            [Label]
        )
    SELECT
        TagID,
        [Label]
    FROM
        @NewTagData

    -- Delete all existing tag joins (some tags may have been deleted)
    DELETE FROM
        Tags_Links
    WHERE
        LinkID = @LinkID

    -- Re-add joins for all existing and new tags
    INSERT INTO
        Tags_Links (
            TagID,
            LinkID
        )
    SELECT
        TagID,
        @LinkID
    FROM
        @TagsToLink

    -- Update the Tags column in Links for efficient display
    DECLARE @DisplayTags TABLE (dt NVARCHAR(64))

    INSERT INTO
        @DisplayTags
    SELECT
        t.[Label]
    FROM
        Tags t
    INNER JOIN
        Tags_Links tl ON tl.TagID = t.ID
    WHERE
        tl.LinkID = @LinkID

	UPDATE
		Links
	SET
		Tags = (SELECT STRING_AGG(dt, '|') FROM @DisplayTags)
	WHERE
		ID = @LinkID
END

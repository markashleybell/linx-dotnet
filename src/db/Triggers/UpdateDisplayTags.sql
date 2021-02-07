
CREATE TRIGGER
    UpdateDisplayTags
ON
    Tags_Links
FOR
    INSERT,
    DELETE
AS
BEGIN
    DECLARE @Delete BIT = CASE WHEN (SELECT COUNT(*) FROM DELETED) > 0 THEN 1 ELSE 0 END
    DECLARE @Insert BIT = CASE WHEN (SELECT COUNT(*) FROM INSERTED) > 0 THEN 1 ELSE 0 END

    DECLARE @LinkID UNIQUEIDENTIFIER = CASE
        WHEN @Delete = 1 THEN (SELECT TOP 1 LinkID FROM DELETED)
        ELSE (SELECT TOP 1 LinkID FROM INSERTED)
    END

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
GO

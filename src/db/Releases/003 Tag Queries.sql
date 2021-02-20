
ALTER PROCEDURE [dbo].[ReadLinks]
(
    @UserID UNIQUEIDENTIFIER,
    @Page INT,
    @RowsPerPage INT,
    @OrderByColumn NVARCHAR(32),
    @OrderDirection NVARCHAR(4),
    @Tags [dbo].[TagList] READONLY
)
AS
BEGIN
    SET NOCOUNT ON

    CREATE TABLE #Links (
        [ID] UNIQUEIDENTIFIER  NOT NULL,
        [Title] NVARCHAR (256) NOT NULL,
        [Url] NVARCHAR (256) NOT NULL,
        [Abstract] NVARCHAR (512) NULL,
        [Tags] NVARCHAR (1024) NULL,
        [Created] DATETIME NOT NULL,
        [Updated] DATETIME NOT NULL
    );

    IF NOT EXISTS (SELECT * FROM @Tags)
    BEGIN
        INSERT INTO
            #Links
        SELECT
            l.ID,
            l.Title,
            l.Url,
            l.Abstract,
            l.Tags,
            l.Created,
            l.Updated
        FROM
            Links l
        WHERE
            l.UserID = @UserID
    END
    ELSE
    BEGIN
        DECLARE @TagCount INT = (SELECT COUNT(*) FROM @Tags)

        INSERT INTO
            #Links
        SELECT
            l.ID,
            l.Title,
            l.Url,
            l.Abstract,
            l.Tags,
            l.Created,
            l.Updated
        FROM
            Links l
        INNER JOIN
            Tags_Links tl ON tl.LinkID = l.ID
        INNER JOIN
            Tags t ON t.ID = tl.TagID AND t.[Label] IN (SELECT [Label] FROM @Tags)
        WHERE
            l.UserID = @UserID
        GROUP BY
            l.ID,
            l.Title,
            l.Url,
            l.Abstract,
            l.Tags,
            l.Created,
            l.Updated
        HAVING
            COUNT(l.ID) = @TagCount
    END

    -- Data
    ;WITH d AS (
        SELECT
            ID,
            Title,
            Url,
            Abstract,
            Tags,
            Created,
            Updated
        FROM
            #Links
    ),
    c AS (
        SELECT COUNT(*) AS TotalRows FROM d
    )
    SELECT
        *
    INTO
        #CurrentPage
    FROM
        d CROSS JOIN c -- Every row will now have a TotalRows column containing the row count
    ORDER BY
        CASE WHEN @OrderByColumn = 'Created' AND @OrderDirection = 'ASC' THEN Created END,
        CASE WHEN @OrderByColumn = 'Created' AND @OrderDirection = 'DESC' THEN Created END DESC,
        CASE WHEN @OrderByColumn = 'Title' AND @OrderDirection = 'ASC' THEN Title END,
        CASE WHEN @OrderByColumn = 'Title' AND @OrderDirection = 'DESC' THEN Title END DESC
    OFFSET (@Page - 1) * @RowsPerPage ROWS
    FETCH NEXT @RowsPerPage ROWS ONLY

    DECLARE @TotalRows INT = (SELECT TOP 1 TotalRows FROM #CurrentPage)

    -- Return total count for this user
    SELECT @TotalRows

    -- Return number of pages
    SELECT ((@TotalRows + @RowsPerPage - 1) / @RowsPerPage)

    -- Return current page contents
    SELECT
        ID,
        Title,
        Url,
        Abstract,
        Tags,
        Created,
        Updated
    FROM
        #CurrentPage
END

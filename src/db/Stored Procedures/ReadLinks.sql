
CREATE PROCEDURE [dbo].[ReadLinks]
(
    @UserID UNIQUEIDENTIFIER,
    @Page INT,
    @RowsPerPage INT,
    @OrderByColumn NVARCHAR(32),
    @OrderDirection NVARCHAR(4)
)
AS
BEGIN
    SET NOCOUNT ON

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
            Links
        WHERE
            UserID = @UserID
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

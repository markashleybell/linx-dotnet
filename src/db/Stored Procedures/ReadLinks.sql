
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
    ORDER BY  
        CASE WHEN @OrderByColumn = 'Created' AND @OrderDirection = 'ASC' THEN Created END,
        CASE WHEN @OrderByColumn = 'Created' AND @OrderDirection = 'DESC' THEN Created END DESC,
        CASE WHEN @OrderByColumn = 'Title' AND @OrderDirection = 'ASC' THEN Title END,
        CASE WHEN @OrderByColumn = 'Title' AND @OrderDirection = 'DESC' THEN Title END DESC
    OFFSET (@Page - 1) * @RowsPerPage ROWS
    FETCH NEXT @RowsPerPage ROWS ONLY
END

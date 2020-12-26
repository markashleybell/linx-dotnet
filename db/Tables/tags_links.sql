CREATE TABLE [dbo].[tags_links] (
    [tag_id]  INT NOT NULL,
    [link_id] INT NOT NULL,
    CONSTRAINT [pk_tags_links] PRIMARY KEY CLUSTERED ([tag_id] ASC, [link_id] ASC),
    CONSTRAINT [fk_tags_links_link_id] FOREIGN KEY ([link_id]) REFERENCES [dbo].[links] ([id]),
    CONSTRAINT [fk_tags_links_tag_id] FOREIGN KEY ([tag_id]) REFERENCES [dbo].[tags] ([id])
);

GO

-- Update display tags fields in link records after tag association added/removed
CREATE TRIGGER update_display_tags_delete
ON tags_links
FOR INSERT, DELETE
AS
BEGIN
    DECLARE @Delete BIT = CASE WHEN (SELECT COUNT(*) FROM DELETED) > 0 THEN 1 ELSE 0 END
    DECLARE @Insert BIT = CASE WHEN (SELECT COUNT(*) FROM INSERTED) > 0 THEN 1 ELSE 0 END

    DECLARE @LinkID INT = CASE WHEN @Delete = 1 THEN (SELECT TOP 1 link_id FROM DELETED) ELSE (SELECT TOP 1 link_id FROM INSERTED) END

	UPDATE
		links
	SET
		tags = (
			SELECT STUFF((SELECT '|' + tag
					FROM tags
							INNER JOIN tags_links ON tags_links.tag_id = tags.id
							WHERE tags_links.link_id = @LinkID
							FOR XML PATH ('')), 1, 1, '')
		)
	WHERE
		id = @LinkID
END

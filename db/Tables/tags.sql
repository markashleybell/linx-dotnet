CREATE TABLE [dbo].[tags] (
    [id]      INT           IDENTITY (1, 1) NOT NULL,
    [tag]     NVARCHAR (50) NOT NULL,
    [user_id] INT           NOT NULL,
    CONSTRAINT [pk_tags] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [fk_tags_user_id] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([id])
);

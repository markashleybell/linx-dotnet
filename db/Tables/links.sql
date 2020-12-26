CREATE TABLE [dbo].[links] (
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [title]    NVARCHAR (256) NOT NULL,
    [url]      NVARCHAR (256) NOT NULL,
    [abstract] NVARCHAR (512) NULL,
    [tags]     NVARCHAR (256) NULL,
    [user_id]  INT            NOT NULL,
    CONSTRAINT [pk_links] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [fk_links_user_id] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([id])
);

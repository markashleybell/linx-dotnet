CREATE TABLE [dbo].[users] (
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [email] NVARCHAR (256) NOT NULL,
    [password] NVARCHAR (256) NOT NULL,
    CONSTRAINT [pk_users] PRIMARY KEY CLUSTERED ([id] ASC)
);

﻿CREATE TABLE [dbo].[Users] (
    [ID] UNIQUEIDENTIFIER NOT NULL,
    [Email] NVARCHAR (256)  NOT NULL,
    [Password] NVARCHAR (2048) NOT NULL,
    [ApiKey] NVARCHAR(32) NOT NULL, 
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([ID] ASC)
)

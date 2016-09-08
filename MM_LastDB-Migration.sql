CREATE TABLE [dbo].[tblSmsSenderDataProvider] (
    [Id]                                 INT            IDENTITY (1, 1) NOT NULL,
    [Name]                               NVARCHAR (150) NOT NULL,
    [IsActive]                           BIT            NOT NULL,
	[FromDisplayName]					NVARCHAR (300) NOT NULL,
    [FromPhoneNumber]                   NVARCHAR (300) NOT NULL,
    [Username]							NVARCHAR (MAX) NOT NULL,
    [Password]							NVARCHAR (MAX) NOT NULL,
	[MessageInterval]					INT NOT NULL,
    [SendProviderRestrictionDataJSON]    NVARCHAR (MAX) NULL,
    [SendProviderRestrictionDataLogJSON] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
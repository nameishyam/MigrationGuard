--Up migration for 202302011803371289_DX34245_AddOrganizationContactNumbersTable

IF OBJECT_ID('[Foundation].[OrganizationContactNumbers]', 'U') IS NULL
BEGIN
    CREATE TABLE [Foundation].[OrganizationContactNumbers] (
        [Id] INT IDENTITY(1, 1)
        , [WhatsAppMobileNumberId] [nvarchar](100)
        , [MobileNumber] [nvarchar](20) NOT NULL
        , [OrganizationId] [int] NOT NULL
        , [CreatedAt] [datetime]
		, [UpdatedAt] [datetime]
        )
END

IF OBJECT_ID('[Foundation].[DF_OrganizationContactNumbers_CreatedAt]', 'D') IS NULL
BEGIN
	ALTER TABLE [Foundation].[OrganizationContactNumbers]
    ADD CONSTRAINT [DF_OrganizationContactNumbers_CreatedAt] 
    DEFAULT GETUTCDATE() FOR [CreatedAt];
END



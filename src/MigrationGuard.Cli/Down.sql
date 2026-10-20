--Down migration for 202302011803371289_DX34245_AddOrganizationContactNumbersTable

IF OBJECT_ID('[Foundation].[OrganizationContactNumbers]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [Foundation].[OrganizationContactNumbers]
END

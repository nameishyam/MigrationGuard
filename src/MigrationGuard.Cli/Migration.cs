using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace MigrationGuard.Cli
{
    [Migration(202302011803371289)]
    public class DX34245_AddOrganizationContactNumbersTable : Migration
    {
        public override void Up()
        {
            if (ConnectionString.Contains("dxdev"))
            {
                Down();
            }

            Execute.Script(@"Up.sql");
        }

        public override void Down()
        {
            Execute.Script(@"Down.sql");
        }
    }
}

using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace Zinc.DeveloperCenter.Data.Migrations.Migrations
{
    /// <summary>
    /// Adds initial schema for the ADR database.
    /// </summary>
    [Migration(2022071203)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are named according to our standards.")]
    public class Migration_2022071203_AdjustColumns : ForwardOnlyMigration
    {
        private static string schemaName = "developercenter";
        private static string tableName = "architecture_decision_record";

        /// <inheritdoc/>
        public override void Up()
        {
            Rename
                .Column("content").OnTable(tableName).InSchema(schemaName)
                .To("content_url");

            Alter.Table(tableName).InSchema(schemaName)
                .AddColumn("element").AsAnsiString().NotNullable();
        }
    }
}

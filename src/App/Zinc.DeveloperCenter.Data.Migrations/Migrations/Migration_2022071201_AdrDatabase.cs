using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace Zinc.DeveloperCenter.Data.Migrations.Migrations
{
    /// <summary>
    /// Adds initial schema for the ADR database.
    /// </summary>
    [Migration(2022071201)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are named according to our standards.")]
    public class Migration_2022071201_AdrDatabase : Migration
    {
        private static string schemaName = "developercenter";
        private static string tableName = "architecture_decision_record";

        /// <inheritdoc/>
        public override void Up()
        {
            Create.Schema(schemaName);
            Create
                .Table(tableName)
                .InSchema(schemaName)
                .WithColumn("application_name").AsAnsiString().NotNullable()
                .WithColumn("application_display_name").AsAnsiString().NotNullable()
                .WithColumn("title").AsAnsiString().NotNullable()
                .WithColumn("number").AsAnsiString().NotNullable()
                .WithColumn("last_updated").AsAnsiString().Nullable()
                .WithColumn("content").AsAnsiString().NotNullable()
                ;

            Create
                .PrimaryKey("architecture_decision_record_pk")
                .OnTable(tableName)
                .WithSchema(schemaName)
                .Columns("application_name", "number");

            Create
                .Index("architecture_decision_record_appname_idx")
                .OnTable(tableName)
                .InSchema(schemaName)
                .OnColumn("application_name")
                ;
        }

        /// <inheritdoc/>
        public override void Down()
        {
            Delete.Table(tableName);
            Delete.Schema(schemaName);
        }
    }
}

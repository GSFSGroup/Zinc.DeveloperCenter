using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace Zinc.DeveloperCenter.Data.Migrations.Migrations
{
    /// <summary>
    /// Rebuild the database as more has been learned.
    /// </summary>
    [Migration(2022071502)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are named according to our standards.")]
    public class Migration_2022071502_RebuildDatabase : ForwardOnlyMigration
    {
        private static string schemaName = "developercenter";
        private static string applicationTableName = "application";
        private static string adrTableName = "architecture_decision_record";
        private static string searchTableName = "architecture_decision_record_search";

        /// <inheritdoc/>
        public override void Up()
        {
            Delete.Table(adrTableName).InSchema(schemaName);

            // application table
            Create
                .Table(applicationTableName).InSchema(schemaName)
                .WithColumn("application_name").AsAnsiString().NotNullable().PrimaryKey($"{applicationTableName}_key")
                .WithColumn("application_display_name").AsAnsiString().NotNullable()
                .WithColumn("application_element").AsAnsiString().Nullable().WithDefaultValue(string.Empty)
                ;

            // adr table
            Create
                .Table(adrTableName).InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{adrTableName}_pkey").Identity()
                .WithColumn("application_name").AsAnsiString().NotNullable()
                .WithColumn("number").AsInt32().NotNullable()
                .WithColumn("title").AsAnsiString().NotNullable()
                .WithColumn("download_url").AsAnsiString().NotNullable()
                .WithColumn("html_url").AsAnsiString().NotNullable()
                .WithColumn("last_updated").AsAnsiString().Nullable()
                ;

            // adr key
            Create
                .UniqueConstraint($"{adrTableName}_key")
                .OnTable(adrTableName).WithSchema(schemaName)
                .Columns("application_name", "number")
                ;

            // adr table to application table foreign key
            Create
                .ForeignKey($"{adrTableName}_application_name_fkey")
                .FromTable(adrTableName).InSchema(schemaName).ForeignColumn("application_name")
                .ToTable(applicationTableName).InSchema(schemaName).PrimaryColumn("application_name")
                ;

            // search table
            Create
                .Table(searchTableName).InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{searchTableName}_pkey")
                .WithColumn("content_search").AsCustom("tsvector").NotNullable()
                ;

            // search table to adr table foreign key
            Create
                .ForeignKey($"{searchTableName}_sid_fk")
                .FromTable(searchTableName).InSchema(schemaName).ForeignColumn("sid")
                .ToTable(adrTableName).InSchema(schemaName).PrimaryColumn("sid")
                ;

            // search table full text index
            Execute.Sql($"CREATE INDEX {searchTableName}_search_idx ON {schemaName}.{searchTableName} USING GIN (content_search);");
        }
    }
}

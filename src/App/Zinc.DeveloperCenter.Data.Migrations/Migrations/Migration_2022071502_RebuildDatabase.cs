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
        private static string tableName = "architecture_decision_record";
        private static string searchTableName = "architecture_decision_record_search";

        /// <inheritdoc/>
        public override void Up()
        {
            Delete.Table(tableName).InSchema(schemaName);

            Create
                .Table(tableName)
                .InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{tableName}_pk").Identity()
                .WithColumn("application_element").AsAnsiString().NotNullable()
                .WithColumn("application_name").AsAnsiString().NotNullable()
                .WithColumn("application_display_name").AsAnsiString().NotNullable()
                .WithColumn("title").AsAnsiString().NotNullable()
                .WithColumn("number").AsAnsiString().NotNullable()
                .WithColumn("content_url").AsAnsiString().NotNullable()
                .WithColumn("last_updated").AsAnsiString().Nullable()
                ;

            Create
                .UniqueConstraint($"{tableName}_key")
                .OnTable(tableName).WithSchema(schemaName)
                .Columns("application_name", "number")
                ;

            Create
                .Table(searchTableName)
                .InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{searchTableName}_pk")
                .WithColumn("content_search").AsCustom("tsvector").NotNullable()
                ;

            Execute.Sql($"CREATE INDEX {searchTableName}_search_idx ON {schemaName}.{searchTableName} USING GIN (content_search);");
        }
    }
}

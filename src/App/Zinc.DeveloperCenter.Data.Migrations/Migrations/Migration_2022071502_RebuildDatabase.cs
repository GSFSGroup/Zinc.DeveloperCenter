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
        private static string favoriteTableName = "architecture_decision_record_favorite";
        private static string viewCountTableName = "architecture_decision_record_viewcount";

        /// <inheritdoc/>
        public override void Up()
        {
            Delete.Table(adrTableName).InSchema(schemaName);

            CreateTables();
            CreateBusinessKeys();
            CreateForeignKeys();
            CreateIndexes();
        }

        private void CreateTables()
        {
            // application table
            Create
                .Table(applicationTableName).InSchema(schemaName)
                .WithColumn("tenant_id").AsAnsiString().NotNullable()
                .WithColumn("application_name").AsAnsiString().NotNullable()
                .WithColumn("application_display_name").AsAnsiString().NotNullable()
                .WithColumn("application_element").AsAnsiString().Nullable().WithDefaultValue(string.Empty)
                ;

            // architecture_decision_record table
            Create
                .Table(adrTableName).InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{adrTableName}_pkey").Identity()
                .WithColumn("tenant_id").AsAnsiString().NotNullable()
                .WithColumn("application_name").AsAnsiString().NotNullable()
                .WithColumn("number").AsInt32().NotNullable()
                .WithColumn("title").AsAnsiString().NotNullable()
                .WithColumn("download_url").AsAnsiString().NotNullable()
                .WithColumn("html_url").AsAnsiString().NotNullable()
                .WithColumn("last_updated_by").AsAnsiString().Nullable()
                .WithColumn("last_updated_on").AsDate().Nullable()
                ;

            // architecture_decision_record_search table
            Create
                .Table(searchTableName).InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{searchTableName}_pkey")
                .WithColumn("search_vector").AsCustom("tsvector").NotNullable()
                ;

            // architecture_decision_record_favorite table
            Create
                .Table(favoriteTableName).InSchema(schemaName)
                .WithColumn($"{adrTableName}_sid").AsInt32().NotNullable()
                .WithColumn("user_id").AsAnsiString().NotNullable()
                ;

            // architecture_decision_record_viewcount table
            Create
                .Table(viewCountTableName).InSchema(schemaName)
                .WithColumn("sid").AsInt32().NotNullable().PrimaryKey($"{viewCountTableName}_pkey")
                .WithColumn("view_count").AsInt32().NotNullable().WithDefaultValue(0)
                ;
        }

        private void CreateBusinessKeys()
        {
            // application key
            Create
                .PrimaryKey($"{applicationTableName}_key")
                .OnTable(applicationTableName).WithSchema(schemaName)
                .Columns("tenant_id", "application_name")
                ;

            // architecture_decision_record key
            Create
                .UniqueConstraint($"{adrTableName}_key")
                .OnTable(adrTableName).WithSchema(schemaName)
                .Columns("tenant_id", "application_name", "number")
                ;

            // architecture_decision_record_favorite key
            Create
                .UniqueConstraint($"{favoriteTableName}_key")
                .OnTable(favoriteTableName).WithSchema(schemaName)
                .Columns($"{adrTableName}_sid", "user_id");
        }

        private void CreateForeignKeys()
        {
            // architecture_decision_record to application foreign key
            Create
                .ForeignKey($"{adrTableName}_tenant_id_application_name_fkey")
                .FromTable(adrTableName).InSchema(schemaName).ForeignColumns("tenant_id", "application_name")
                .ToTable(applicationTableName).InSchema(schemaName).PrimaryColumns("tenant_id", "application_name")
                ;

            // architecture_decision_record_search to architecture_decision_record foreign key
            Create
                .ForeignKey($"{searchTableName}_sid_fkey")
                .FromTable(searchTableName).InSchema(schemaName).ForeignColumn("sid")
                .ToTable(adrTableName).InSchema(schemaName).PrimaryColumn("sid")
                ;

            // architecture_decision_record_favorite to architecture_decision_record foreign key
            Create
                .ForeignKey($"{adrTableName}_sid_fkey")
                .FromTable(favoriteTableName).InSchema(schemaName).ForeignColumn($"{adrTableName}_sid")
                .ToTable(adrTableName).InSchema(schemaName).PrimaryColumn("sid");

            // architecture_decision_record_viewcount to architecture_decision_record foreign key
            Create
                .ForeignKey($"{viewCountTableName}_sid_fkey")
                .FromTable(viewCountTableName).InSchema(schemaName).ForeignColumn("sid")
                .ToTable(adrTableName).InSchema(schemaName).PrimaryColumn("sid");
        }

        private void CreateIndexes()
        {
            // architecture_decision_record foreign key index
            Create.Index($"{adrTableName}_tenant_id_application_name_fkey_idx")
                .OnTable(adrTableName).InSchema(schemaName)
                .OnColumn("tenant_id").Ascending()
                .OnColumn("application_name").Ascending()
                ;

            // architecture_decision_record_search full text index
            Execute.Sql($"CREATE INDEX {searchTableName}_search_vector_idx ON {schemaName}.{searchTableName} USING GIN (search_vector);");
        }
    }
}

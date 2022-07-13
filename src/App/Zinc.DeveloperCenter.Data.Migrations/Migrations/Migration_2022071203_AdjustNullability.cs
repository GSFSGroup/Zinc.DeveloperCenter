using System.Diagnostics.CodeAnalysis;
using FluentMigrator;

namespace Zinc.DeveloperCenter.Data.Migrations.Migrations
{
    /// <summary>
    /// Adds initial schema for the ADR database.
    /// </summary>
    [Migration(2022071201)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are named according to our standards.")]
    public class Migration_2022071203_AdjustNullability : ForwardOnlyMigration
    {
        private static string schemaName = "developercenter";
        private static string tableName = "architecture_decision_record";

        /// <inheritdoc/>
        public override void Up()
        {
            Alter.Table(tableName).InSchema(schemaName)
                .AlterColumn("content").AsAnsiString().Nullable();
        }
    }
}

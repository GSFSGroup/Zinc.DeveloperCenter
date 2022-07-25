using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using FluentMigrator;

namespace Zinc.DeveloperCenter.Data.Migrations.Migrations
{
    /// <summary>
    /// Adds test data for local development.
    /// </summary>
    [Migration(2022072101)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Migrations are named according to our standards.")]
    [Tags(TagBehavior.RequireAny, "Development", "docker-local", "docker-circleci")]
    public class Migration_2022072101_TestData : ForwardOnlyMigration
    {
        private static string schemaName = "developercenter";
        private static string applicationTableName = "application";
        private static string adrTableName = "architecture_decision_record";
        private static string searchTableName = "architecture_decision_record_search";

        /// <inheritdoc/>
        public override void Up()
        {
            Delete.FromTable(searchTableName).InSchema(schemaName).AllRows();
            Delete.FromTable(adrTableName).InSchema(schemaName).AllRows();
            Delete.FromTable(applicationTableName).InSchema(schemaName).AllRows();

            Execute.EmbeddedScript("Migration_2022072101_TestData.sql");
            Execute.WithConnection(InsertTestData);
        }

        private void InsertTestData(IDbConnection connection, IDbTransaction transaction)
        {
            /* We are inserting these embedded resources into the search table:
             * Migration_2022072101_TestData_adr_01.md
             * Migration_2022072101_TestData_adr_02.md
             * Migration_2022072101_TestData_adr_03.md
             * Migration_2022072101_TestData_adr_04.md
             * Migration_2022072101_TestData_adr_05.md
             * */

            var ids = connection.Query<int>(
                $"SELECT id FROM {schemaName}.{adrTableName} ORDER BY id",
                null,
                transaction)
                .AsList();

            for (int i = 0; i < ids.Count; i++)
            {
                var content = EmbeddedResources.EmbeddedResource.Read($"Migration_2022072101_TestData_adr_0{i + 1}.md");

                connection.Execute(
                    $"INSERT INTO {schemaName}.{searchTableName} (id, search_vector) VALUES (@id, to_tsvector('english', @content))",
                    new { id = ids[i], content = content },
                    transaction);
            }
        }
    }
}

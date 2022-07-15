using System.Collections.Generic;
using System.Data;
using Dapper;
using RedLine.Data.Repositories;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Gets the list of applications to display on the AppList screen.
    /// </summary>
    public class UXAppListGetApplicationsDataQuery : DataQueryBase<IDbConnection, IEnumerable<UXAppListGetApplicationsDataQuery.Result>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UXAppListGetApplicationsDataQuery()
        {
            Resolve = async connection => (await connection.QueryAsync<Result>(
                "SELECT DISTINCT application_element, application_name, application_display_name FROM developercenter.architecture_decision_record").ConfigureAwait(false))
                .AsList();
        }

        /// <summary>
        /// The result from the query.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// The application element name.
            /// </summary>
            public string? ApplicationElement { get; set; }

            /// <summary>
            /// The application name.
            /// </summary>
            public string? ApplicationName { get; set; }

            /// <summary>
            /// The application display name.
            /// </summary>
            public string? ApplicationDisplayName { get; set; }
        }
    }
}

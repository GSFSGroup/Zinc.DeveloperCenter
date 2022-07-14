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
                    "SELECT application_name, application_display_name, element FROM developercenter.architecture_decision_record"))
                    .AsList();
        }

        /// <summary>
        /// The result from the query.
        /// </summary>
        public class Result
        {
            /// <summary>
            /// The application name.
            /// </summary>
            public string ApplicationName { get; set; }

            /// <summary>
            /// The application display name.
            /// </summary>
            public string ApplicationDisplayName { get; set; }

            /// <summary>
            /// The element name.
            /// </summary>
            public string Element { get; set; }
        }
    }
}

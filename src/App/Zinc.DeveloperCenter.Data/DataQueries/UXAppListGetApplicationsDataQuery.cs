using RedLine.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Zinc.DeveloperCenter.Data.DataQueries
{
    /// <summary>
    /// Gets the list of applications to display on the AppList screen.
    /// </summary>
    public class UXAppListGetApplicationsDataQuery : DataQueryBase<IDbConnection, IEnumerable<UXAppListGetApplicationsDataQuery.UXAppListGetApplicationsDataQueryResult>>
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public UXAppListGetApplicationsDataQuery()
        {
            Resolve = async connection => (await connection.QueryAsync<UXAppListGetApplicationsDataQueryResult>(
                    "SELECT application_name, application_display_name, element FROM developercenter.architecture_decision_record"))
                    .AsList();
        }

        /// <summary>
        /// The result from the query.
        /// </summary>
        public class UXAppListGetApplicationsDataQueryResult
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

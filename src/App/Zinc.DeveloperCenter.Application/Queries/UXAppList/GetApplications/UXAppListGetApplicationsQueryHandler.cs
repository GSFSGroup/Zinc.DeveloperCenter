using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Repositories;
using Zinc.DeveloperCenter.Data.DataQueries;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications
{
    internal class UXAppListGetApplicationsQueryHandler : IRequestHandler<UXAppListGetApplicationsQuery, IEnumerable<UXAppListGetApplicationsQueryModel>>
    {
        private readonly IRepository repository;

        public UXAppListGetApplicationsQueryHandler(IRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<UXAppListGetApplicationsQueryModel>> Handle(UXAppListGetApplicationsQuery request, CancellationToken cancellationToken)
        {
            var query = new UXAppListGetApplicationsDataQuery();
            var results = await repository.Query(query).ConfigureAwait(false);

            return results.Select(x => new UXAppListGetApplicationsQueryModel
            {
                ApplicationElement = x.ApplicationElement,
                ApplicationName = x.ApplicationName,
                ApplicationDisplayName = x.ApplicationDisplayName,
            }).ToArray();
        }
    }
}
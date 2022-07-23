using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications
{
    internal class UXAppListGetApplicationsQueryHandler : IRequestHandler<UXAppListGetApplicationsQuery, PageableResult<UXAppListGetApplicationsQueryModel>>
    {
        private readonly IApplicationRepository repository;

        public UXAppListGetApplicationsQueryHandler(IApplicationRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXAppListGetApplicationsQueryModel>> Handle(UXAppListGetApplicationsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetApplicationsDataQuery(request.TenantId);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => new UXAppListGetApplicationsQueryModel
                {
                    ApplicationDescription = x.Description,
                    ApplicationDisplayName = x.DisplayName,
                    ApplicationElement = x.Element,
                    ApplicationName = x.Name,
                    ApplicationUrl = x.Url,
                });

            return new PageableResult<UXAppListGetApplicationsQueryModel>(items);
        }
    }
}

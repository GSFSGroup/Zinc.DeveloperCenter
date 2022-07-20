using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetApplications
{
    internal class UXAppListGetApplicationsQueryHandler : IRequestHandler<UXAppListGetApplicationsQuery, PageableResult<UXAppListGetApplicationsQueryModel>>
    {
        private readonly IApplicationRepository repository;
        private readonly IMapper mapper;

        public UXAppListGetApplicationsQueryHandler(
            IApplicationRepository repository,
            IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<PageableResult<UXAppListGetApplicationsQueryModel>> Handle(UXAppListGetApplicationsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetApplicationsDataQuery();

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => mapper.Map<UXAppListGetApplicationsQueryModel>(x));

            return new PageableResult<UXAppListGetApplicationsQueryModel>(items);
        }
    }
}

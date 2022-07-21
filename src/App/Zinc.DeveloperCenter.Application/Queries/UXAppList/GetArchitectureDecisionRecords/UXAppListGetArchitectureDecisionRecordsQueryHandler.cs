using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetArchitectureDecisionRecords
{
    /// <summary>
    /// A handler for <see cref="UXAppListGetArchitectureDecisionRecordsQuery"/>.
    /// </summary>
    internal class UXAppListGetArchitectureDecisionRecordsQueryHandler : IRequestHandler<UXAppListGetArchitectureDecisionRecordsQuery, PageableResult<UXAppListGetArchitectureDecisionRecordsQueryModel>>
    {
        private readonly IArchitectureDecisionRecordRepository repository;
        private readonly IMapper mapper;

        public UXAppListGetArchitectureDecisionRecordsQueryHandler(
            IArchitectureDecisionRecordRepository repository,
            IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<PageableResult<UXAppListGetArchitectureDecisionRecordsQueryModel>> Handle(UXAppListGetArchitectureDecisionRecordsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetArchitectureDecisionRecordsDataQuery(request.TenantId, request.ApplicationName);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => mapper.Map<UXAppListGetArchitectureDecisionRecordsQueryModel>(x));

            return new PageableResult<UXAppListGetArchitectureDecisionRecordsQueryModel>(items);
        }
    }
}

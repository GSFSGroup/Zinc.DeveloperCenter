using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using RedLine.Domain.Repositories;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXAppList.GetArchitectureDecisionRecords
{
    /// <summary>
    /// A handler for <see cref="UXAppListGetArchitectureDecisionRecordsQuery"/>.
    /// </summary>
    internal class UXAppListGetArchitectureDecisionRecordsQueryHandler : IRequestHandler<UXAppListGetArchitectureDecisionRecordsQuery, IEnumerable<UXAppListGetArchitectureDecisionRecordsQueryModel>>
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

        public async Task<IEnumerable<UXAppListGetArchitectureDecisionRecordsQueryModel>> Handle(UXAppListGetArchitectureDecisionRecordsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new UXAppListGetArchitectureDecisionRecordsDataQuery(request.ApplicationName);
            var result = await repository.Query(dataQuery).ConfigureAwait(false);

            return result.Items.Select(x => mapper.Map<UXAppListGetArchitectureDecisionRecordsQueryModel>(x)).ToArray()!;
        }
    }
}

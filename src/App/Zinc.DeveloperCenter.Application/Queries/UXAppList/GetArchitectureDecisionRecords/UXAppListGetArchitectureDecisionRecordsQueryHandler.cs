using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public UXAppListGetArchitectureDecisionRecordsQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<UXAppListGetArchitectureDecisionRecordsQueryModel>> Handle(UXAppListGetArchitectureDecisionRecordsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new UXAppListGetArchitectureDecisionRecordsDataQuery(request.ApplicationName);
            var result = await repository.Query(dataQuery).ConfigureAwait(false);

            return result.Items.Select(x => new UXAppListGetArchitectureDecisionRecordsQueryModel
            {
                ApplicationName = x.ApplicationName,
                ContentUrl = x.ContentUrl,
                LastUpdated = x.LastUpdated,
                Number = x.Number,
                Title = x.Title,
            }).ToArray();
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public UXAppListGetArchitectureDecisionRecordsQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXAppListGetArchitectureDecisionRecordsQueryModel>> Handle(UXAppListGetArchitectureDecisionRecordsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetArchitectureDecisionRecordsDataQuery(request.TenantId, request.ApplicationName);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => new UXAppListGetArchitectureDecisionRecordsQueryModel
                {
                    ApplicationName = x.ApplicationName,
                    LastUpdatedBy = x.LastUpdatedBy,
                    LastUpdatedOn = x.LastUpdatedOn,
                    Id = x.Id,
                    Number = x.Number,
                    NumberDisplay = x.NumberDisplay,
                    Title = x.Title,
                    TitleDisplay = x.TitleDisplay,
                });

            return new PageableResult<UXAppListGetArchitectureDecisionRecordsQueryModel>(items);
        }
    }
}

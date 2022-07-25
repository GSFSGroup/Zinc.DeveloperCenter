using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrList.GetArchitectureDecisionRecords
{
    /// <summary>
    /// A handler for <see cref="UXAdrListGetArchitectureDecisionRecordsQuery"/>.
    /// </summary>
    internal class UXAdrListGetArchitectureDecisionRecordsQueryHandler : IRequestHandler<UXAdrListGetArchitectureDecisionRecordsQuery, PageableResult<UXAdrListGetArchitectureDecisionRecordsQueryModel>>
    {
        private readonly IArchitectureDecisionRecordRepository repository;

        public UXAdrListGetArchitectureDecisionRecordsQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXAdrListGetArchitectureDecisionRecordsQueryModel>> Handle(UXAdrListGetArchitectureDecisionRecordsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetArchitectureDecisionRecordsDataQuery(request.TenantId, request.ApplicationName);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => new UXAdrListGetArchitectureDecisionRecordsQueryModel
                {
                    ApplicationName = x.ApplicationName,
                    FilePath = x.FilePath,
                    LastUpdatedBy = x.LastUpdatedBy,
                    LastUpdatedOn = x.LastUpdatedOn,
                    Number = x.Number,
                    NumberDisplay = x.NumberDisplay,
                    Title = x.Title,
                    TitleDisplay = x.TitleDisplay,
                })
                .OrderBy(x => x.Number);

            return new PageableResult<UXAdrListGetArchitectureDecisionRecordsQueryModel>(items);
        }
    }
}

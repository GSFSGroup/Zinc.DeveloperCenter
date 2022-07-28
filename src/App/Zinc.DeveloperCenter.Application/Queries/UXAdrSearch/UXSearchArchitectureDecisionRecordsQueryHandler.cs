using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXAdrSearch
{
    internal class UXSearchArchitectureDecisionRecordsQueryHandler : IRequestHandler<UXSearchArchitectureDecisionRecordsQuery, PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>>
    {
        private readonly IArchitectureDecisionRecordRepository repository;

        public UXSearchArchitectureDecisionRecordsQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>> Handle(UXSearchArchitectureDecisionRecordsQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new SearchArchitectureDecisionRecordsDataQuery(request.TenantId, request.SearchPattern);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => new UXSearchArchitectureDecisionRecordsQueryModel
                {
                    ApplicationName = x.ApplicationName,
                    FilePath = x.FilePath,
                    LastUpdatedBy = x.LastUpdatedBy,
                    LastUpdatedOn = x.LastUpdatedOn,
                    Number = x.Number,
                    NumberDisplay = x.NumberDisplay,
                    Title = x.Title,
                    TitleDisplay = x.TitleDisplay,
                    TotalViews = x.TotalViews,
                })
                .OrderBy(x => x.ApplicationName).ThenBy(x => x.Number);

            return new PageableResult<UXSearchArchitectureDecisionRecordsQueryModel>(items);
        }
    }
}

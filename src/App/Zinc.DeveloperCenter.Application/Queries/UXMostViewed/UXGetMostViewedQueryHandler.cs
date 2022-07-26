using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXMostUsed.GetMostViewed
{
    internal class UXGetMostViewedQueryHandler : IRequestHandler<UXGetMostViewedQuery, PageableResult<UXGetMostViewedQueryModel>>
    {
        private readonly IArchitectureDecisionRecordRepository repository;

        public UXGetMostViewedQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXGetMostViewedQueryModel>> Handle(UXGetMostViewedQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetMostViewedArchitectureDecisionRecordsDataQuery(
                request.TenantId,
                request.TopN);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => new UXGetMostViewedQueryModel
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
                });

            return new PageableResult<UXGetMostViewedQueryModel>(items);
        }
    }
}

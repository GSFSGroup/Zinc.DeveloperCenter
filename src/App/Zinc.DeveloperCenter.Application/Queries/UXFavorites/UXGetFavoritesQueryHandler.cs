using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXFavorites
{
    internal class UXGetFavoritesQueryHandler : IRequestHandler<UXGetFavoritesQuery, PageableResult<UXGetFavoritesQueryModel>>
    {
        private readonly IArchitectureDecisionRecordRepository repository;

        public UXGetFavoritesQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXGetFavoritesQueryModel>> Handle(UXGetFavoritesQuery request, CancellationToken cancellationToken)
        {
            var dataQuery = new GetFavoriteArchitectureDecisionRecordsDataQuery(
                request.TenantId,
                request.UserId);

            var items = (await repository.Query(dataQuery).ConfigureAwait(false))
                .Items
                .Select(x => new UXGetFavoritesQueryModel
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

            return new PageableResult<UXGetFavoritesQueryModel>(items);
        }
    }
}

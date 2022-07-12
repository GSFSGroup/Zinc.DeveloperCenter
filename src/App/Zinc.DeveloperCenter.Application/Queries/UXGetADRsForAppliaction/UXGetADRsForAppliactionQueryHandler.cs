using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RedLine.Domain.Model;
using Zinc.DeveloperCenter.Data.DataQueries;
using Zinc.DeveloperCenter.Domain.Model;
using Zinc.DeveloperCenter.Domain.Repositories;

namespace Zinc.DeveloperCenter.Application.Queries.UXGetADRsForAppliaction
{
    internal class UXGetADRsForAppliactionQueryHandler : IRequestHandler<UXGetADRsForAppliactionQuery, PageableResult<UXGetADRsForAppliactionQueryModel>>
    {
        private readonly IArchitectureDecisionRecordRepository repository;

        public UXGetADRsForAppliactionQueryHandler(IArchitectureDecisionRecordRepository repository)
        {
            this.repository = repository;
        }

        public async Task<PageableResult<UXGetADRsForAppliactionQueryModel>> Handle(UXGetADRsForAppliactionQuery request, CancellationToken cancellationToken)
        {
            var query = new GetArchitectureDecisionRecordsForApplication(request.ApplicationName);

            var items = (await repository.Query(query).ConfigureAwait(false))
                .Items
                .Select(x => new UXGetADRsForAppliactionQueryModel
                {
                    ApplicationDisplayName = x.ApplicationDisplayName,
                    ApplicationName = x.ApplicationName,
                    Content = x.Content,
                    LastUpdated = x.LastUpdated,
                    Number = x.Number,
                    Title = x.Title,
                }).ToList();

            return new PageableResult<UXGetADRsForAppliactionQueryModel>(items);
        }
    }
}

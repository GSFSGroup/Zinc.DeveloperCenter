using System.Threading;
using System.Threading.Tasks;
using RedLine.Application.Jobs;

namespace Zinc.DeveloperCenter.Application.Jobs.RefreshAdrs
{
    internal class RefreshAdrsJobHandler : JobHandlerBase<RefreshAdrsJob>
    {
        public override async Task<JobResult> Handle(RefreshAdrsJob request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return JobResult.NoWorkPerformed;
        }
    }
}

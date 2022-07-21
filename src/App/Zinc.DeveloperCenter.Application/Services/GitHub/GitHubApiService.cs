using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application.Services.GitHub
{
    /// <summary>
    /// Provides an implementation of the <see cref="IGitHubApiService"/>.
    /// </summary>
    public class GitHubApiService : IGitHubApiService
    {
        /// <inheritdoc/>
        public Task<string> DownloadArchitectureDecisionRecord(string tenantId, string downloadUrl)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(string tenantId, string repositoryName)
        {
            // TODO make the two calls to get the ADRs + the lastUpdated date
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GitHubRepositoryModel>> GetRepositories(string tenantId)
        {
            throw new NotImplementedException();
        }
    }
}

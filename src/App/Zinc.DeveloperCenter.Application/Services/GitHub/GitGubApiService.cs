using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinc.DeveloperCenter.Domain.Services.GitHub;

namespace Zinc.DeveloperCenter.Application.Services.GitHub
{
    /// <summary>
    /// Provides an implementation of the <see cref="IGitGubApiService"/>.
    /// </summary>
    public class GitGubApiService : IGitGubApiService
    {
        /// <inheritdoc/>
        public Task<IEnumerable<GitHubArchitectureDecisionRecordModel>> GetArchitectureDecisionRecords(string repositoryName, bool includeContent = false)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IEnumerable<GitHubRepositoryModel>> GetRepositories()
        {
            throw new NotImplementedException();
        }
    }
}

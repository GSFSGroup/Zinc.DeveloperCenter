using Xunit;

namespace Zinc.DeveloperCenter.IntegrationTests.Jobs
{
    [CollectionDefinition(nameof(JobsTestCollection))]
    public class JobsTestCollection : ICollectionFixture<JobsTestFixture>
    {
    }
}

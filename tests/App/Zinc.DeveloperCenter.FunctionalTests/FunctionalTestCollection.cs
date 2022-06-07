using Xunit;

namespace Zinc.DeveloperCenter.FunctionalTests
{
    [CollectionDefinition(nameof(FunctionalTestCollection))]
    public class FunctionalTestCollection : ICollectionFixture<FunctionalTestFixture>
    {
    }
}

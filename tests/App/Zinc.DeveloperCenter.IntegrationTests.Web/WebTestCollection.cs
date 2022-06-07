using Xunit;

namespace Zinc.DeveloperCenter.IntegrationTests.Web
{
    [CollectionDefinition(nameof(WebTestCollection))]
    public class WebTestCollection : ICollectionFixture<WebTestFixture>
    {
    }
}

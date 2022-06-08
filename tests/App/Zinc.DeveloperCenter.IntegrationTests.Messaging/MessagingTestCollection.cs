using Xunit;

namespace Zinc.DeveloperCenter.IntegrationTests.Messaging
{
    [CollectionDefinition(nameof(MessagingTestCollection))]
    public class MessagingTestCollection : ICollectionFixture<MessagingTestFixture>
    {
    }
}

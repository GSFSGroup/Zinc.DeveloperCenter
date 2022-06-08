using Opa.Wasm;
using RedLine.Domain;
using Xunit;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    [Collection(nameof(AuthorizationCollection))]
    public abstract class AuthorizationTestBase
    {
        protected AuthorizationTestBase(AuthorizationFixture fixture)
        {
            Fixture = fixture;
        }

        protected AuthorizationFixture Fixture { get; }

        public static ITenantId TenantId { get; } = new TenantId("GSFSGroup");

        protected OpaPolicy CreateOpaPolicy() => Fixture.Factory.CreatePolicy();
    }
}

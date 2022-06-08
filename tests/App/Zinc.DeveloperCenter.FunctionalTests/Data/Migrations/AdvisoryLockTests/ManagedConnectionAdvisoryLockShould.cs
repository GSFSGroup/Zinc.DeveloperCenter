using FluentAssertions;
using Npgsql;
using Xunit;
using Xunit.Abstractions;
using Zinc.DeveloperCenter.Data;

namespace Zinc.DeveloperCenter.FunctionalTests.Data.AdvisoryLockTests
{
    public class ManagedConnectionAdvisoryLockShould : AdvisoryLockTestBase
    {
        public ManagedConnectionAdvisoryLockShould(FunctionalTestFixture fixture, ITestOutputHelper outputHelper)
            : base(fixture, outputHelper)
        {
        }

        [Fact]
        public void CreateAdvisoryLockWhenInstantiatedWithConnectionString()
        {
            var lockId = GetRandomLockId();
            using (var advisoryLock = new AdvisoryLock(ConnectionString, lockId))
            {
                var lockIds = GetLocks();
                lockIds.Should().Contain(lockId);
            }
        }

        [Fact]
        public void ReleaseAdvisoryLockWhenDisposed()
        {
            var lockId = GetRandomLockId();
            using (var advisoryLock = new AdvisoryLock(ConnectionString, lockId))
            {
                // no-op.
            }

            var lockIds = GetLocks();
            lockIds.Should().NotContain(lockId);
        }

        [Fact(Skip = "CommandTimeout is 30 seconds, so this test is slow.")]
        public void BlockWhenLockAlreadyExists()
        {
            var lockId = GetRandomLockId();
            using (var advisoryLock = new AdvisoryLock(ConnectionString, lockId))
            {
                Assert.Throws<NpgsqlException>(() => new AdvisoryLock(ConnectionString, lockId));
            }
        }
    }
}

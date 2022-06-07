using System;
using RedLine.Application.A3.Authorization;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    public class AuthorizationFixture : IDisposable
    {
        public AuthorizationFixture()
        {
            Factory = new AuthorizationPolicyFactory();
        }

        internal AuthorizationPolicyFactory Factory { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the thing.
        /// </summary>
        /// <param name="disposing">Are we disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Factory?.Dispose();
            }
        }
    }
}

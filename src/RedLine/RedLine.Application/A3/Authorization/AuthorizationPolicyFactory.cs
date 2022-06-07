namespace RedLine.Application.A3.Authorization
{
    internal class AuthorizationPolicyFactory : OpaPolicyFactory
    {
        public AuthorizationPolicyFactory()
            : base(typeof(AssemblyMarker).Assembly, "A3.Authorization.authorization.wasm")
        {
        }
    }
}

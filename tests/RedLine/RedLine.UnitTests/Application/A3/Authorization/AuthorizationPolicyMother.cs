using System;
using Opa.Wasm;
using RedLine.Application.A3.Authorization;

namespace RedLine.UnitTests.Application.A3.Authorization
{
    public static class AuthorizationPolicyMother
    {
        internal static AuthorizationPolicy Policy(this OpaPolicy opaPolicy, Func<OpaPolicyDataBuilder, OpaPolicyDataBuilder> dataCustomizations)
        {
            var data = dataCustomizations(new OpaPolicyDataBuilder()).Build();
            opaPolicy.SetData(data);
            return new(opaPolicy, AuthorizationTestBase.TenantId, "local@redline.services");
        }
    }
}

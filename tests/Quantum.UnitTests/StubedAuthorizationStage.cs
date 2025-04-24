using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Quantum.PolicyProvider;

namespace Quantum.UnitTests
{
    public class StubedAuthorizationStage : AuthorizationStage
    {
        public List<ApplicationPermissions> _applicationPermissions;


        public static StubedAuthorizationStage WhichReturnThisWhenCallUserAccessRights(
            List<ApplicationPermissions> applicationPermissions, 
            IAccessRightPolicyRegistrar registrar)
        {
            return new StubedAuthorizationStage(registrar, 
                
                new PolicyProviderAclStub(new List<ApplicationPermissions>()),
                NullLogger<AuthorizationStage>.Instance
            )
            {
                _applicationPermissions = applicationPermissions
            };
        }


        public StubedAuthorizationStage(IAccessRightPolicyRegistrar resolver,
            IPolicyProviderAcl policyProviderAcl,
            ILogger<AuthorizationStage> logger
            ) : base(resolver,  policyProviderAcl , logger)
        {
        }


        protected override async Task<List<ApplicationPermissions>> GetUserAccessRights(string[] applicationNames)
        {
            return _applicationPermissions;
        }
    }
}
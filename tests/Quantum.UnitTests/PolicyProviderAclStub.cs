using Quantum.PolicyProvider;

namespace Quantum.UnitTests;

public class PolicyProviderAclStub : IPolicyProviderAcl
{
    private readonly List<ApplicationPermissions> _expectedPermissions;

    public PolicyProviderAclStub(List<ApplicationPermissions> expectedPermissions)
    {
        _expectedPermissions = expectedPermissions;
    }


    public async Task<List<ApplicationPermissions>> GetGetUserAccessRights(string[] applicationNames)
    {
        return _expectedPermissions;

    }
}
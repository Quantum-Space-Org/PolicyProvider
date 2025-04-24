using Quantum.PolicyProvider;

public interface IPolicyProviderAcl
{
    Task<List<ApplicationPermissions>> GetGetUserAccessRights(string[] applicationNames);
}
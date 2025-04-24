namespace Quantum.PolicyProvider;

public class UserPoliciesViewModel
{
    public string Application { get; set; }
    public string Namespace { get; set; }
    public List<PermissionViewModel> Permissions { get; set; }

    public bool HasAccessRight(string accessRight)
        => Permissions != null && Permissions.Any(p => p.Name == accessRight);

    public class PermissionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
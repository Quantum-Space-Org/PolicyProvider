namespace Quantum.PolicyProvider;

public class ApplicationPermissions
{
    public string ApplicationId { get; set; }
    public string ApplicationName { get; set; }
    public NamespacePermissions Permissions { get; set; }
}

public class NamespacePermissions
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<PermissionStatus> Permissions { get; set; }
}

public class PermissionStatus
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public State State { get; set; }
}

public enum State
{
    NotSet,
    Allow,
    Deny,
    InheritedAllow,
    InheritedDeny,
}
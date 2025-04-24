using Quantum.Specification;
using System.Linq.Expressions;

namespace Quantum.PolicyProvider;

public class ShouldHasAccessToAPolicySpecification(
    string accessRight,
    string nameSpace,
    string application)
    : Specification<List<ApplicationPermissions>>
{
    public override Expression<Func<List<ApplicationPermissions>, bool>> Condition()
    {
        return userAccessRightStatus =>

            userAccessRightStatus != null
            && userAccessRightStatus
                .Any(p => p.ApplicationName == application
                          && p.Permissions.Name == nameSpace
                          && p.Permissions.Permissions.Any(p => p.Name == accessRight)
                );
    }
}
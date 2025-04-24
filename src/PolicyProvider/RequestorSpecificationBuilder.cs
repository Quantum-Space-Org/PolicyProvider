using Quantum.Specification;

namespace Quantum.PolicyProvider;

public class RequestorSpecificationBuilder
{
    private string _expectedAccessRight;
    private string _expectedNameSpace;
    private string _expectedApplication;

    public RequestorSpecificationBuilder HavePrivilegeToBeAbleTo(string accessRight)
    {
        _expectedAccessRight = accessRight;
        return this;
    }


    public RequestorSpecificationBuilder InTheNameSpace(string nameSpace)
    {
        _expectedNameSpace = nameSpace ?? throw new ArgumentNullException(nameof(nameSpace));
        return this;
    }

    public Specification<List<ApplicationPermissions>> InTheApplication(string application)
    {
        _expectedApplication = application ?? throw new ArgumentNullException(nameof(application));
        return new ShouldHasAccessToAPolicySpecification(_expectedAccessRight, _expectedNameSpace, _expectedApplication);
    }
}
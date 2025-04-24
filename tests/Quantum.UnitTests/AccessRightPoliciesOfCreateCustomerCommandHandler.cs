using Quantum.PolicyProvider;
using Quantum.Specification;

namespace Quantum.UnitTests.Dispatcher;

public class AccessRightPoliciesOfCreateCustomerCommandHandler
    : IWantToSpecifyTheAccessRightPolicyOf<CreateCustomerCommand>
{
    private bool _isCalled;

    public override Specification<List<ApplicationPermissions>> Specification(CreateCustomerCommand command)
    {
        _isCalled = true;

        return Requestor
            .Should
            .HavePrivilegeToBeAbleTo("edit-personel-action")
            .InTheNameSpace("namespace")
            .InTheApplication("application")
            .And(Requestor
                .Should
                .HavePrivilegeToBeAbleTo("another-access-right")
                .InTheNameSpace("namespace")
                .InTheApplication("application"));
    }

    public override string[] ApplicationNames()
    {
        return new[] { "Application" };
    }

    internal void Verify()
    {
        _isCalled.Should().BeTrue();
    }
}
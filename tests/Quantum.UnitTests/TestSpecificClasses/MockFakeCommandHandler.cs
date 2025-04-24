using Quantum.Command.Handlers;
using Quantum.Domain.Messages.Event;

namespace Quantum.UnitTests.TestSpecificClasses;

using Quantum.PolicyProvider;
using Quantum.Specification;

public class IWantToSpeccifyAccessRightPolicyOfMockFakeCommandHandler : IWantToSpecifyTheAccessRightPolicyOf<FakeCommand>
{
    public override string[] ApplicationNames()
    {
        return new[] { "Application" };
    }

    public override Specification<List<ApplicationPermissions>> Specification(FakeCommand command)
    {
        return Requestor.Should
            .HavePrivilegeToBeAbleTo("edit-personel-action")
            .InTheNameSpace("namespace")
            .InTheApplication("application")
            .And(Requestor
                .Should
                .HavePrivilegeToBeAbleTo("another-access-right")
                .InTheNameSpace("namespace")
                .InTheApplication("application"));
    }
}

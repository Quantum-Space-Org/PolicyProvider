using Quantum.PolicyProvider;

namespace Quantum.UnitTests
{
    public class RequestorShould
    {
       

        [Fact]
        public void has_access_right_something()
        {
            var requestorSpec =
                Requestor
                    .Should
                        .HavePrivilegeToBeAbleTo("create-person")
                        .InTheNameSpace("namespace")
                        .InTheApplication("application")
                    .And(Requestor
                        .Should
                        .HavePrivilegeToBeAbleTo("update-person")
                        .InTheNameSpace("namespace")
                        .InTheApplication("application"));

            var userAccessRightStatus
                = new List<ApplicationPermissions>
                {
                    new ApplicationPermissions
                    {
                        ApplicationId = "application",
                        ApplicationName = "application",
                        Permissions = new NamespacePermissions
                        {
                            Id = "namespace",
                            Name = "namespace",
                            Permissions = new List<PermissionStatus>
                            {
                               new PermissionStatus
                               {
                                   Name = "create-person",
                               },
                               new PermissionStatus
                               {
                                   Name = "update-person",
                               },
                            },
                        },
                    },
                };

            requestorSpec
                .IsItVerify(userAccessRightStatus)
                .Should()
                .BeTrue();
        }
    }
}
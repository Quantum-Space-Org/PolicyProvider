using Microsoft.Extensions.Logging.Abstractions;
using Quantum.Command.Pipeline;
using Quantum.Domain.Messages.Command;
using Quantum.Domain.Messages.Header;
using Quantum.PolicyProvider;
using Quantum.Specification;
using Quantum.UnitTests.Dispatcher;

namespace Quantum.UnitTests
{
    public class CreateActionItemCommand : IAmACommand
    {
        public object Clone()
        {
            throw new NotImplementedException();
        }

        public MessageMetadata Metadata { get; set; }
        public string GetCorrelationId()
        {
            throw new NotImplementedException();
        }

        public string GetId()
        {
            throw new NotImplementedException();
        }

        public void SetCorrelationId(string correlationId)
        {
            throw new NotImplementedException();
        }
    }
    public class ActionItemPolicyAccessRights : IWantToSpecifyTheAccessRightPolicyOf<CreateActionItemCommand>
    {
        public override Specification<List<ApplicationPermissions>> Specification(CreateActionItemCommand command)
        {
            return Requestor
                .Should
                .HavePrivilegeToBeAbleTo("Add")
                .InTheNameSpace("ActionItems")
                .InTheApplication("PayrollManagement");
        }

        public override string[] ApplicationNames()
        {
            return new[] { "PayrollManagement" };
        }
    }

    public class AccessRightPolicyTests
    {
        private readonly IAccessRightPolicyRegistrar _registrar;

        public AccessRightPolicyTests()
        {
            _registrar = GetRegistrar();
        }

        [Fact]
        public async Task authorization_userHasAccessRight_willNotThrowException()
        {
            var accessRights = @"[
    {
        ""applicationId"": ""11836265-c7cc-44cb-9bfe-5a0a2ecf2d76"",
        ""applicationName"": ""PayrollManagement"",
        ""permissions"": {
            ""id"": ""60559175-9493-4c68-b44e-f827165ad857"",
            ""name"": ""ActionItems"",
            ""permissions"": [
                {
                    ""id"": ""0a596eda-9fcc-42a2-8fa9-8183fdbe9972"",
                    ""name"": ""Edit"",
                    ""displayName"": ""ویرایش"",
                    ""state"": 1
                },
 {
                    ""id"": ""0a596eda-9fcc-42a2-8fa9-8183fdbe9972"",
                    ""name"": ""Add"",
                    ""displayName"": ""ایجاد"",
                    ""state"": 1
                },
                {
                    ""id"": ""530aefc7-f7ea-460b-a651-c24604826851"",
                    ""name"": ""View"",
                    ""displayName"": ""نمایش"",
                    ""state"": 1
                },
                {
                    ""id"": ""bafa9979-9c9b-4422-b7a6-e75c902d6b1f"",
                    ""name"": ""Delete"",
                    ""displayName"": ""حذف"",
                    ""state"": 1
                }
            ],
            ""permissionDictionary"": {
                ""Edit"": 1,
                ""View"": 1,
                ""Delete"": 1
            }
        }
    },
]";

            var applicationPermissionsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApplicationPermissions>>(accessRights);

            _registrar.Register(new ActionItemPolicyAccessRights());

            var stubedAuthorizationStage = StubedAuthorizationStage.WhichReturnThisWhenCallUserAccessRights(applicationPermissionsList, _registrar);

            var action = async () => await stubedAuthorizationStage.Process(new CreateActionItemCommand(), new StageContext());

            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task AuthorizationStage_should_successfully_call_access_right_policy_of_a_command()
        {
            var accessRightPolicyHandler = new AccessRightPoliciesOfCreateCustomerCommandHandler();

            _registrar.Register(accessRightPolicyHandler);

            IAmAPipelineStage stage = StubedAuthorizationStage.WhichReturnThisWhenCallUserAccessRights(new List<ApplicationPermissions>(), _registrar);

            await stage.Process(new CreateCustomerCommand("", "", ""), new StageContext());

            accessRightPolicyHandler.Verify();
        }

        [Fact]
        public async Task resolve_AlwaysTrue_access_right_policy_If_there_is_not_any_registered_access_right_policy_for_a_command()
        {
            var result = _registrar.Resolve<CreateCustomerCommand>();

            result.GetType().Should().Be(typeof(AlwaysTrueAuthorizationSpecification<CreateCustomerCommand>));
        }

        [Fact]
        public async Task register_access_right_policies_in_an_assembly()
        {
            ((CommandHandlerAccessRightRegistrar)_registrar).RegisterUsing(typeof(CreateCustomerCommand).Assembly);

            var result = _registrar.Resolve<CreateCustomerCommand>();

            result.GetType().Should().Be(typeof(AccessRightPoliciesOfCreateCustomerCommandHandler));
        }

        [Fact]
        public void createPolicyProviderConfig_baseAddressIsEmpty_exceptionWillBeThrown()
        {
            Action action = () => new PolicyProviderConfig("");
            action.Should().Throw<PolicyProviderConfig.BaseAddressIsNullOrWhitespaceException>();
        }

        [Theory]
        [InlineData("aasasda")]
        [InlineData("http")]
        [InlineData("http://")]
        [InlineData("127.0.0.1")]
        [InlineData("localhost")]
        public void createPolicyProviderConfig_baseAddressIsInvalid_exceptionWillBeThrown(string baseAddress)
        {
            Action action = () => new PolicyProviderConfig(baseAddress);
            action.Should().Throw<PolicyProviderConfig.BaseAddressIsNotAbsoluteUrlException>();
        }

        [Fact]
        public async Task shouldThrowException_If_ApplicationNamesAre_Empty_Or_Null()
        {
            var command = new CreateCustomerCommand("national-code", "Uncle", "Bob");

            var accessRightPolicyRegistrar = new CommandHandlerAccessRightRegistrar();
            string[] emptyApplicationNames = null;
            accessRightPolicyRegistrar.Register(StubAccessRightPolicyOf.WithApplications(emptyApplicationNames));

            var stage = CreateAuthorizationStage(accessRightPolicyRegistrar, new PolicyProviderConfig("http://localhost:8025"));

            var action = async () =>
            {
                await stage.Process(command, new StageContext());
            };

            await action.Should().ThrowAsync<AccessRightPolicyApplicationNamesAreEmptyException>();
        }

        [Fact]
        public async Task shouldThrowException_If_ApplicationNamesAre_UserIsNotLoggedIn()
        {
            var command = new CreateCustomerCommand("national-code", "Uncle", "Bob");

            var accessRightPolicyRegistrar = new CommandHandlerAccessRightRegistrar();
            accessRightPolicyRegistrar.Register(StubAccessRightPolicyOf.WithApplications(new[] { "http://localhost:8025" }));

            var stage = CreateAuthorizationStage(accessRightPolicyRegistrar, new PolicyProviderConfig("http://localhost:8025"));

            var action = async () =>
            {
                await stage.Process(command, new StageContext());
            };

            await action.Should().ThrowAsync<UnAuthenticatedException>();
        }

        private static AuthorizationStage CreateAuthorizationStage(CommandHandlerAccessRightRegistrar commandHandlerAccessRightRegistrar, PolicyProviderConfig policyProviderConfig)
        {
            return new AuthorizationStage(commandHandlerAccessRightRegistrar,
                new PolicyProviderAclStub(new List<ApplicationPermissions>()),
                NullLogger<AuthorizationStage>.Instance);
        }

        private static CommandHandlerAccessRightRegistrar GetRegistrar()
        {
            return new CommandHandlerAccessRightRegistrar();
        }

        private static PolicyProviderConfig GetPolicyProviderConfig()
        {
            return new PolicyProviderConfig("http://localhost", "application");
        }

    }

    public class CreateCustomerCommand : IsACommand
    {
        public string NationalCode { get; }
        public string Name { get; }
        public string Family { get; }

        public CreateCustomerCommand(string NationalCode, string Name, string Family)
        {
            this.NationalCode = NationalCode;
            this.Name = Name;
            this.Family = Family;
        }
    }

    public class StubAccessRightPolicyOf : IWantToSpecifyTheAccessRightPolicyOf<CreateCustomerCommand>
    {
        private string[] _applicationNames;

        public override Specification<List<ApplicationPermissions>> Specification(CreateCustomerCommand command)
        {
            return new AlwaysTrueSpec();
        }

        public override string[] ApplicationNames()
        {
            return _applicationNames;
        }

        public static IWantToSpecifyTheAccessRightPolicyOf<CreateCustomerCommand> WithApplications(string[] applicationNames)
        {
            return new StubAccessRightPolicyOf
            {
                _applicationNames = applicationNames
            };
        }
    }
}
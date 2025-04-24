using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Quantum.Command.Pipeline;
using Quantum.Core;

namespace Quantum.PolicyProvider;

public class AuthorizationStage(
    IAccessRightPolicyRegistrar resolver,
    IPolicyProviderAcl policyProviderAcl,
    ILogger<AuthorizationStage> logger)
    : IAmAPipelineStage
{
    private readonly IPolicyProviderAcl _policyProviderAcl = policyProviderAcl;

    public override async Task Process<TCommand>(TCommand command, StageContext context)
        => await CheckAccessRights(command);

    private async Task CheckAccessRights<TCommand>(TCommand command)
        where TCommand : IAmACommand
    {
        var accessRightPolicyOf = resolver.Resolve<TCommand>();

        if (accessRightPolicyOf is AlwaysTrueAuthorizationSpecification<TCommand>)
            return;

        var applicationNames = ApplicationNames(accessRightPolicyOf);

        logger.LogInformation(GetTheFirstApplicationName(applicationNames));

        var spec = accessRightPolicyOf.Specification(command);

        if (This.Is.True(spec.IsItVerify(await GetUserAccessRights(applicationNames))))
            return;

        throw new UnauthorizedException("you don't have access rights!");
    }

    private static string[] ApplicationNames<TCommand>(IWantToSpecifyTheAccessRightPolicyOf<TCommand> accessRightPolicyOf) where TCommand : IAmACommand
    {
        var applicationNames = accessRightPolicyOf.ApplicationNames();

        if (applicationNames == null || !applicationNames.Any())
            throw new AccessRightPolicyApplicationNamesAreEmptyException($"Application names are empty. {nameof(accessRightPolicyOf)}");

        return applicationNames;
    }

    protected virtual async Task<List<ApplicationPermissions>> GetUserAccessRights(string[] applicationNames)
        => await _policyProviderAcl.GetGetUserAccessRights(applicationNames);

    private static string GetTheFirstApplicationName(IReadOnlyList<string> applicationNames)
        => applicationNames[0];
}

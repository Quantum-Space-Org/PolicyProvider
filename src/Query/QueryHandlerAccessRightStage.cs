using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quantum.Core;
using Quantum.Query;
using Quantum.Query.Pipeline;

namespace Quantum.PolicyProvider.Query;

public class QueryHandlerAccessRightStage(
    IQueryHandlerAccessRightRegistrar registrar,
    ILogger<QueryHandlerAccessRightStage> logger,
    IPolicyProviderAcl policyProviderAcl)
    : IAmQueryHandlerStage
{
    public override async Task<TResult> Handle<TQuery, TResult>(TQuery query)
    {

        var accessRightPolicyOf = registrar.Resolve<TQuery, TResult>();

        if (accessRightPolicyOf is AlwaysTrueAuthorizationForQueryHandlerSpecification<TQuery, TResult>)
        {
            if (Next is not null)
                return await Next.Handle<TQuery, TResult>(query);
            return default;
        }

        var applicationNames = ApplicationNames(accessRightPolicyOf);

        logger.LogInformation(GetTheFirstApplicationName(applicationNames));

        var spec = accessRightPolicyOf.Specification(query);

        if (!This.Is.True(spec.IsItVerify(await GetUserAccessRights(applicationNames))))
            throw new UnAuthenticatedException("you don't have access rights!");

        if (Next is not null)
            return await Next.Handle<TQuery, TResult>(query);
        return default;

    }

    private static string[] ApplicationNames<TQuery, TResult>(IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult> accessRightPolicyOf)
        where TQuery : IAmAQuery
    {
        var applicationNames = accessRightPolicyOf.ApplicationNames();

        if (applicationNames == null || !applicationNames.Any())
            throw new AccessRightPolicyApplicationNamesAreEmptyException($"Application names are empty. {nameof(accessRightPolicyOf)}");

        return applicationNames;
    }

    protected virtual async Task<List<ApplicationPermissions>> GetUserAccessRights(string[] applicationNames)
        => await policyProviderAcl.GetGetUserAccessRights(applicationNames);

    private static string GetTheFirstApplicationName(string[] applicationNames)
        => applicationNames[0];
}
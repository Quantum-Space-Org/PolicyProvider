using System.Collections.Generic;
using Quantum.Query;
using Quantum.Specification;

namespace Quantum.PolicyProvider;

public abstract class IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult>
{
    public abstract Specification<List<ApplicationPermissions>> Specification(TQuery query);
    public abstract string[] ApplicationNames();
}

public class AlwaysTrueAuthorizationSpecification<TQuery, TResult> : IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult>
    where TQuery : IAmAQuery
{
    public static AlwaysTrueAuthorizationSpecification<TQuery, TResult> New<TQuery>() where TQuery : IAmAQuery => new();

    public override Specification<List<ApplicationPermissions>> Specification(TQuery command)
        => new AlwaysTrueSpec();

    public override string[] ApplicationNames()
        => new[] { "AlwaysTrue" };
}

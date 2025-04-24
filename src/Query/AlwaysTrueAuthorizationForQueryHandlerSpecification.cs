using System.Collections.Generic;
using Quantum.Query;
using Quantum.Specification;

namespace Quantum.PolicyProvider;

public class AlwaysTrueAuthorizationForQueryHandlerSpecification<TQuery, TResult>
    : IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult> where TQuery : IAmAQuery

{
    public override Specification<List<ApplicationPermissions>> Specification(TQuery query)
        => new AlwaysTrueSpec();

    public override string[] ApplicationNames()
        => new[] { "AlwaysTrue" };
}
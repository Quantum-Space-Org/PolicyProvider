using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Quantum.Specification;

namespace Quantum.PolicyProvider;

public class AlwaysTrueSpec : Specification<List<ApplicationPermissions>>
{
    public override Expression<Func<List<ApplicationPermissions>, bool>> Condition()
    {
        return a => true;
    }
}
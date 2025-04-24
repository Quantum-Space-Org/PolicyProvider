using Quantum.Specification;
using System.Collections.Generic;

namespace Quantum.PolicyProvider;

public abstract class IWantToSpecifyTheAccessRightPolicyOf<TCommand> where TCommand : IAmACommand
{
    public abstract Specification<List<ApplicationPermissions>> Specification(TCommand command);
    public abstract string[] ApplicationNames();
}


public class AlwaysTrueAuthorizationSpecification<T> : IWantToSpecifyTheAccessRightPolicyOf<T>
    where T : IAmACommand
{
    public static AlwaysTrueAuthorizationSpecification<T> New<T>() where T : IAmACommand => new();

    public override Specification<List<ApplicationPermissions>> Specification(T command) 
        => new AlwaysTrueSpec();

    public override string[] ApplicationNames() 
        => new[] { "AlwaysTrue" };
}
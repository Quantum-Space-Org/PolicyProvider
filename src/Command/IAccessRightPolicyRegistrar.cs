using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Quantum.PolicyProvider;

public interface IAccessRightPolicyRegistrar
{
    IWantToSpecifyTheAccessRightPolicyOf<TCommand> Resolve<TCommand>() where TCommand : IAmACommand;
    void Register<T>(IWantToSpecifyTheAccessRightPolicyOf<T> handler)
        where T : IAmACommand;
}


public class CommandHandlerAccessRightRegistrar : IAccessRightPolicyRegistrar
{
    private readonly Dictionary<Type, object> Registry = new();

    public void Register<T>(IWantToSpecifyTheAccessRightPolicyOf<T> handler) where T : IAmACommand
        => Registry[typeof(T)] = handler;


    public void RegisterUsing(params Assembly[] assemblies)
    {
        foreach (var ass in assemblies)
        {
            RegisterUsing(ass);
        }
    }

    public void RegisterUsing(Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t =>
                t.BaseType != null &&
                t.BaseType.Name == typeof(IWantToSpecifyTheAccessRightPolicyOf<>).Name);

        foreach (var type in types)
        {
            var baseType = type.BaseType;

            if (baseType == null) continue;

            var genericArgs = baseType.GetGenericArguments();

            Registry[genericArgs[0]] = Activator.CreateInstance(type);
        }
    }

    public IWantToSpecifyTheAccessRightPolicyOf<TCommand> Resolve<TCommand>() where TCommand : IAmACommand
    {
        return Registry.TryGetValue(typeof(TCommand), out var res)
            ? (IWantToSpecifyTheAccessRightPolicyOf<TCommand>)res
            : AlwaysTrueAuthorizationSpecification<TCommand>
                .New<TCommand>();
    }
}
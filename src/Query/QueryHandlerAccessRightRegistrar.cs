using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quantum.Query;

namespace Quantum.PolicyProvider.Query;

public interface IQueryHandlerAccessRightRegistrar
{
    IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult> Resolve<TQuery, TResult>()
        where TQuery : IAmAQuery;

    void Register<T, T1>();

    void Register<T>(object accessRight);

    void Register(params Assembly[] assemblies);
}
public class QueryHandlerAccessRightRegistrar: IQueryHandlerAccessRightRegistrar
{
    private readonly IDictionary<Type, object> _dictionary = new ConcurrentDictionary<Type, object>();

    public IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult> Resolve<TQuery, TResult>()
        where TQuery : IAmAQuery
    {
        if (_dictionary.TryGetValue(typeof(IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult>),
                out object instance))
        {

            return instance as IWantToHandlerAccessRightOfQueryHandler<TQuery, TResult>;
        }

        return new AlwaysTrueAuthorizationSpecification<TQuery, TResult>();
    }

    public void Register<T, T1>()
    {
        AddTo(typeof(T), typeof(T1));
    }

    public void Register<T>(object accessRight)
    {
        AddInstance(typeof(T), accessRight);
    }

    public void Register(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t =>
                    t.BaseType != null &&
                    t.BaseType.Name == typeof(IWantToHandlerAccessRightOfQueryHandler<,>).Name);

            foreach (var type in types)
            {
                var baseType = type.BaseType;

                if (baseType == null) continue;

                AddTo(baseType, type);
            }
        }
    }

    private void AddTo(Type baseType, Type type)
    {
        AddInstance(baseType, Activator.CreateInstance(type));
    }

    private void AddInstance(Type type, object instance)
    {
        if (_dictionary.TryGetValue(type, out object inst))
            throw new MultipleQueryHandlerAccessRightPolicyException(inst, instance);

        _dictionary[type] = instance;
    }
}

internal class MultipleQueryHandlerAccessRightPolicyException : Exception
{
    public MultipleQueryHandlerAccessRightPolicyException(object instance1, object instance2)
    : base($"{instance1.ToString()} - {instance2.ToString()}")
    {
    }
}
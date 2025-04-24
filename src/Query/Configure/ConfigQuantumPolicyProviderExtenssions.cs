using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Configurator;
using Quantum.PolicyProvider.Query;

namespace Quantum.PolicyProvider.Configure;

public static class ConfigQuantumPolicyProviderExtensions
{
    public static ConfigQuantumPolicyProviderBuilder ConfigQuantumPolicyProvider(this QuantumServiceCollection collection)
    {
        return new ConfigQuantumPolicyProviderBuilder(collection);
    }
}

public class ConfigQuantumPolicyProviderBuilder
{
    private readonly QuantumServiceCollection _quantumServiceCollection;

    public ConfigQuantumPolicyProviderBuilder(QuantumServiceCollection collection)
    {
        _quantumServiceCollection = collection;
    }



    public ConfigQuantumPolicyProviderBuilder RegisterPolicyProviderConfigurationAsSingleton(PolicyProviderConfig policyProviderConfig)
    {
        _quantumServiceCollection.Collection.Add(new ServiceDescriptor(typeof(PolicyProviderConfig), sp => policyProviderConfig, ServiceLifetime.Singleton));
        return this;
    }



    public ConfigQuantumPolicyProviderBuilder RegisterAccessTokenService(IAccessTokenService instance, ServiceLifetime lifetime)
    {
        _quantumServiceCollection.Collection.Add(new ServiceDescriptor(typeof(IAccessTokenService), sp => instance, lifetime));
        return this;
    }

    public ConfigQuantumPolicyProviderBuilder RegisterQueryHandlerAccessRightRegistrar(params Assembly[] assemblies)
    {
        var queryHandlerAccessRightRegistrar = new QueryHandlerAccessRightRegistrar();
        queryHandlerAccessRightRegistrar.Register(assemblies);

        _quantumServiceCollection.Collection.AddSingleton<IQueryHandlerAccessRightRegistrar>(sp => queryHandlerAccessRightRegistrar);
        return this;
    }
        
    public ConfigQuantumPolicyProviderBuilder RegisterQueryHandlerAccessRightRegistrar(IQueryHandlerAccessRightRegistrar registrar)
    {
        _quantumServiceCollection.Collection.AddSingleton(sp => registrar);
        return this;
    }

    public QuantumServiceCollection and()
    {
        return _quantumServiceCollection;
    }

}
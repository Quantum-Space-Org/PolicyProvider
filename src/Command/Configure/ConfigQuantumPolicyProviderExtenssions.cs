using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Quantum.Configurator;

namespace Quantum.PolicyProvider.Configure;

public static class ConfigQuantumPolicyProviderExtensions
{
    public static ConfigQuantumPolicyProviderBuilder ConfigQuantumPolicyProvider(this QuantumServiceCollection collection)
    {
        return new ConfigQuantumPolicyProviderBuilder(collection);
    }
}

public class ConfigQuantumPolicyProviderBuilder(QuantumServiceCollection collection)
{
    public ConfigQuantumPolicyProviderBuilder RegisterPolicyProviderConfigurationAsSingleton(PolicyProviderConfig policyProviderConfig)
    {
        collection.Collection.Add(new ServiceDescriptor(typeof(PolicyProviderConfig), sp => policyProviderConfig, ServiceLifetime.Singleton));
        return this;
    }

    public ConfigQuantumPolicyProviderBuilder RegisterAccessRightPolicyRegistrarAsSingleton(IAccessRightPolicyRegistrar registrar)
    {
        collection.Collection.Add(new ServiceDescriptor(typeof(IAccessRightPolicyRegistrar), sp => registrar, ServiceLifetime.Singleton));

        return this;
    }

    public ConfigQuantumPolicyProviderBuilder RegisterAccessRightPolicyRegistrarInAssemblies(params Assembly[] assemblies)
    {
        var registrar = new CommandHandlerAccessRightRegistrar();
        registrar.RegisterUsing(assemblies);

        return this.RegisterAccessRightPolicyRegistrarAsSingleton(registrar);
    }

    //public ConfigQuantumPolicyProviderBuilder RegisterDefaultPolicyProviderAcl()
    //{
    //    collection.Collection.AddTransient<IPolicyProviderAcl, PolicyProviderAcl>();
    //    return this;
    //}

    public ConfigQuantumPolicyProviderBuilder RegisterDefaultAccessTokenService()
    {
        collection.Collection.AddTransient<IAccessTokenService, HttpContextAccessTokenService>();
        return this;
    }

    public ConfigQuantumPolicyProviderBuilder RegisterAccessTokenService(IAccessTokenService instance, ServiceLifetime lifetime)
    {
        collection.Collection.Add(new ServiceDescriptor(typeof(IAccessTokenService), sp => instance, lifetime));
        return this;
    }
    public QuantumServiceCollection and()
    {
        return collection;
    }

}
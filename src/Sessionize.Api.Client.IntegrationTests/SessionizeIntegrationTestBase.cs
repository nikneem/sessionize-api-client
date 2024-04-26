using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sessionize.Api.Client.DependencyInjection;

namespace Sessionize.Api.Client.IntegrationTests;

public class SessionizeIntegrationTestBase
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    private IServiceProvider? _serviceProvider;
    private readonly IConfigurationBuilder _configurationBuilder = new ConfigurationBuilder();



    protected void WithSessionizeClientRegistered()
    {
        _serviceCollection.AddSessionizeApiClient(_configurationBuilder);
    }

    protected void WithAppSettingsConfiguration(string jsonFile)
    {
        _configurationBuilder.AddJsonFile(jsonFile);
    }

    protected TService GetService<TService>() where TService : class
    {
        _serviceProvider ??= _serviceCollection.BuildServiceProvider();

        return _serviceProvider.GetRequiredService<TService>();
    }
}
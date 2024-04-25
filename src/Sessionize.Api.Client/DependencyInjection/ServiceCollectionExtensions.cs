using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Configuration;

namespace Sessionize.Api.Client.DependencyInjection;

public static class ServiceCollectionExtensions
{


    public static IServiceCollection AddSessionizeApiClient(this IServiceCollection services, IConfigurationBuilder configurationBuilder)
    {

        var configuration = configurationBuilder.Build();
        services.AddOptions<SessionizeConfiguration>()
            .Bind(configuration.GetSection(SessionizeConfiguration.SectionName))
            .ValidateOnStart();

        var sessionizeConfiguration = configuration.GetSection(SessionizeConfiguration.SectionName)
            .Get<SessionizeConfiguration>();

        services.AddHttpClient(SessionizeConstants.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(sessionizeConfiguration.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddStandardResilienceHandler();

        services.AddScoped<ISessionizeApiClient, SessionizeApiClient>();

        return services;
    }

}
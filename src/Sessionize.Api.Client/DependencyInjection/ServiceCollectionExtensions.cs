using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sessionize.Api.Client.Abstractions;
using Sessionize.Api.Client.Configuration;
using Sessionize.Api.Client.Exceptions;

namespace Sessionize.Api.Client.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSessionizeApiClient(
        this IServiceCollection services, 
        IConfigurationBuilder configurationBuilder,
        string? configurationSectionName = null)
    {
        var sectionName = configurationSectionName?? SessionizeConfiguration.SectionName;
        var configuration = configurationBuilder.Build();
        services.AddOptions<SessionizeConfiguration>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateOnStart();

        var sessionizeConfiguration = configuration.GetSection(sectionName)
            .Get<SessionizeConfiguration>();

        if (sessionizeConfiguration == null)
        {
            throw new SessionizeApiClientException(ErrorCode.InvalidConfiguration);
        }

        services.AddHttpClient<SessionizeApiClient>()
            .AddStandardResilienceHandler();

        services.AddScoped<ISessionizeApiClient, SessionizeApiClient>();

        return services;
    }

}
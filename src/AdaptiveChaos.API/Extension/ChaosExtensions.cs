using AdaptiveChaos.Infrastructure.Implementation;
using AdaptiveChaos.Infrastructure.Interfaces;
using AdaptiveChaos.Shared.Domain;

namespace AdaptiveChaos.API.Extension
{
    public static class ChaosExtensions
    {
        public static void AddChaosPolicy(this IServiceCollection services, string system, ILogger logger)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var chaosFactory = serviceProvider.GetRequiredService<IChaosPolicyFactory>();
            var systemConfig = configuration.GetSection(system).Get<SystemConfiguration>();
            if (systemConfig == null)
            {
                throw new InvalidOperationException($"{system}  name not found in config");
            }

            var uri = new Uri(systemConfig.BaseUrl);
            logger.LogInformation($"Registered chaos setting for name {system}");
            chaosFactory.SaveChaosPolicy(uri.Host, systemConfig.OperationChaosSettings);

            services.AddHttpClient(system, client =>
            {
                client.BaseAddress = new Uri(systemConfig.BaseUrl);
            }).AddHttpMessageHandler<ChaosDelegatingHandler>();
        }
    }
}

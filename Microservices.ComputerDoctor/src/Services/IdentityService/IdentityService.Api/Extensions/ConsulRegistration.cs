using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace IdentityService.Api.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(x => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));

            return services;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder builder, IHostApplicationLifetime lifetime)
        {
            var consulClient = builder.ApplicationServices.GetRequiredService<ConsulClient>();

            var loggingFactory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var features = builder.Properties["server.Features"] as FeatureCollection;

            var adresses = features.Get<IServerAddressesFeature>();

            var address = adresses.Addresses.First();


            var uri = new Uri(address);
            var registration = new AgentServiceRegistration()
            {
                ID = $"IdentityService",
                Name = "IdentityService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Identity Service", "Identity" }
            };

            logger.LogInformation("Registring with Consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistring from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return builder;
        }
    }
}

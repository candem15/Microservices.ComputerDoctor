using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace OrderService.Api.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection AddConsulServiceDiscoveryRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(x => new ConsulClient(consulConfig =>
            {
                var address = configuration["ConsulConfig:Address"];
                consulConfig.Address = new Uri(address);
            }));

            return services;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder builder, IHostApplicationLifetime lifetime, IConfiguration configuration)
        {
            var consulClient = builder.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = builder.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();


            // Server ip adresini getiriyoruz.
            var uri = new Uri(configuration["Kestrel:Endpoints:Http:Url"]);
            var registration = new AgentServiceRegistration()
            {
                ID = $"OrderService",
                Name = "OrderService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Order Service", "Order" }
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

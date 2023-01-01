// See https://aka.ms/new-console-template for more information

using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.EventHandlers;
using NotificationService.IntegrationEvents.Events;

internal class Program
{
    private static void Main(string[] args)
    {
        ServiceCollection services = new();
        ConfigureServices(services);

        var sp = services.BuildServiceProvider();

        IEventBus eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subcribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();
        eventBus.Subcribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();

        Console.WriteLine("NotificationService app is running...");
        Console.Read();
    }
    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
        services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
        services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();
        services.AddSingleton<IEventBus>(sp =>
        {
            EventBusConfig config = new()
            {
                ConnectionRetryCount = 5,
                EventNameSuffix = "IntegrationEvent",
                SubscriberClientAppName = "NotificationService",
                EventBusType = EventBusType.RabbitMQ
            };

            return EventBusFactory.Create(config, sp);
        });
    }
}
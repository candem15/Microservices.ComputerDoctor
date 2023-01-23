using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using OrderService.Api.Extensions;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Contexts;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderDbContext>>();

    var dbContextSeeder = new OrderDbContextSeed();

    dbContextSeeder.SeedAsync(context, logger).Wait();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

ConfigureEventBusForSubscription(app);

void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(configure: configure => configure.AddConsole())
        .AddApplicationServices()
        .AddInfrastructureServices(builder.Configuration)
        .ConfigureEventHandlers()
        .AddConsulServiceDiscoveryRegistration(builder.Configuration);

    services.AddSingleton(sp =>
    {
        EventBusConfig config = new EventBusConfig()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            EventNamePrefix = "OrderService",
            EventBusType = EventBusType.RabbitMQ
        };

        return EventBusFactory.Create(config, sp);
    });
}

void ConfigureEventBusForSubscription(IApplicationBuilder app)
{
    var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subcribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}
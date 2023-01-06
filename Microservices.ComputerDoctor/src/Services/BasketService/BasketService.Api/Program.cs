using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureAuth(builder.Configuration);
builder.Services.AddSingleton(x => x.ConfigureRedis(builder.Configuration));
builder.Services.ConfigureConsul(builder.Configuration);
builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new EventBusConfig()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        EventNamePrefix = "BasketService",
        EventBusType = EventBusType.RabbitMQ
    };

    return EventBusFactory.Create(config, sp);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

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

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Lifetime, builder.Configuration);

// Sipariþ oluþturma kuyruðuna abone olup, sipariþin kuyruða ulaþýp ulaþmadýðýndan emin oluyoruz.
app.Services.GetRequiredService<IEventBus>().Subcribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();

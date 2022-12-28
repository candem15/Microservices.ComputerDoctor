using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace EventBus.UnitTest
{

    public class EventBusTests
    {
        private ServiceCollection services;

        public EventBusTests()
        {
            services = new ServiceCollection();
        }

        [Fact]
        public void SubscribeEvent_OnRabbitMQ_ShouldNotThrowException()
        {
            // Arrange
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "ComputerDoctorTopic",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent"
                };
                return EventBusFactory.Create(config, sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            // Act & Assert
            FluentActions
                .Invoking(() =>
                {
                    eventBus.Subcribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
                    eventBus.UnSubcribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
                })
                .Should().NotThrow();
        }

        [Fact]
        public void SendMessage_ToRabbitMQ_ShouldNotThrowException()
        {
            // Arrange
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriberClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "ComputerDoctorTopic",
                    EventBusType = EventBusType.RabbitMQ,
                    EventNameSuffix = "IntegrationEvent"
                };
                return EventBusFactory.Create(config, sp);
            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            // Act & Assert
            FluentActions
                .Invoking(() => eventBus.Publish(new OrderCreatedIntegrationEvent(5)))
                .Should().NotThrow();
        }
    }
}
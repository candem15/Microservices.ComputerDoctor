using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using PaymentService.Api.IntegrationEvents.Events;

namespace PaymentService.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration configuration;
        private readonly IEventBus eventBus;
        private readonly ILogger<OrderStartedIntegrationEventHandler> logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            this.configuration = configuration;
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            // Payment Process

            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId) : new OrderPaymentFailedIntegrationEvent(@event.OrderId,"Unexpected error occurred while payment process!");

            logger.LogInformation($"OrderStartedIntegrationEvent in PaymentService is fired with PaymentSuccess: {paymentSuccessFlag}, OrderId: {@event.OrderId}");

            eventBus.Publish(paymentEvent);

            return Task.CompletedTask;
        }
    }
}
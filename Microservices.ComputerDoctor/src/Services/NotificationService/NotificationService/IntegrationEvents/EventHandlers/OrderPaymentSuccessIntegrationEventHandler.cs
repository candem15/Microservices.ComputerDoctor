using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> logger;

        public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
        {
            this.logger = logger;
        }

        public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
        {
            // Send fail notification to customers.(sms-mail...)

            logger.LogInformation($"Payment success with OrderId: {@event.OrderId}");

            return Task.CompletedTask;
        }
    }
}
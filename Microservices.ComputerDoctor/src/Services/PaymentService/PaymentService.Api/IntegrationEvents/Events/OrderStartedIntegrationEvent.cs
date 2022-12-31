using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public OrderStartedIntegrationEvent()
        {

        }

        public OrderStartedIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}
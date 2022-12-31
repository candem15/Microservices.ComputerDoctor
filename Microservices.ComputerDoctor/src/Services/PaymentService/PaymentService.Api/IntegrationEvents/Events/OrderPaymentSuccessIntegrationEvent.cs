using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
    {
        private int orderId;

        public OrderPaymentSuccessIntegrationEvent(int orderId)
        {
            this.orderId = orderId;
        }
    }
}
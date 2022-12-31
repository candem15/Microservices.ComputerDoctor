using EventBus.Base.Events;

namespace PaymentService.Api.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        private int orderId;
        public string ErrorMessage { get; }

        public OrderPaymentFailedIntegrationEvent(int orderId,string errorMessage)
        {
            this.orderId = orderId;
            ErrorMessage = errorMessage;
        }
    }
}
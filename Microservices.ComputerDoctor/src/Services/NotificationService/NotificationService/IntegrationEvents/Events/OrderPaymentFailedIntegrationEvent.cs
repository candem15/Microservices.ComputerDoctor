using EventBus.Base.Events;

namespace NotificationService.IntegrationEvents.Events
{
    public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
    {
        public int OrderId;
        public string ErrorMessage { get; }

        public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
        {
            OrderId = orderId;
            ErrorMessage = errorMessage;
        }
    }
}
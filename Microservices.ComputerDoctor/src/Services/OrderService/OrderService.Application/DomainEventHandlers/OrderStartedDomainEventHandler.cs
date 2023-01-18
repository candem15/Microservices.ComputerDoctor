using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandlers
{
    public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        private IBuyerRepository _buyerRepository;

        public OrderStartedDomainEventHandler(IBuyerRepository buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            var cardTypeId = (notification.CardTypeId != 0) ? notification.CardTypeId : 1;

            var buyer = await _buyerRepository.GetSingleAsync(i => i.Name == notification.Username, i => i.PaymentMethods);

            bool buyerExisted = buyer != null;

            if (!buyerExisted)
            {
                buyer = new Buyer(notification.Username);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                $"Payment method on {DateTime.UtcNow}",
                notification.CardNumber,
                notification.CardSecurityNumber,
                notification.CardHolderName,
                notification.CardExpiration,
                notification.Order.Id);

            var buyerUpdated = buyerExisted ? _buyerRepository.Update(buyer) : await _buyerRepository.AddAsync(buyer);

            await _buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            
        }
    }
}

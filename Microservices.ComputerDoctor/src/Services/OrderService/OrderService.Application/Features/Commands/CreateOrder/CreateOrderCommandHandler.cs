using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Application.IntegrationEvents;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Features.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private IOrderRepository _orderRepository;
        private IEventBus _eventBus;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var address = new Address(request.ZipCode, request.State, request.Country, request.City, request.Street);

            Order dbOrder = new(request.Username, address, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration, null);

            request.OrderItems.ToList().ForEach(i => dbOrder.AddOrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.PictureUrl, i.Units));

            await _orderRepository.AddAsync(dbOrder);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.Username);

            _eventBus.Publish(orderStartedIntegrationEvent);

            return true;
        }
    }
}
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Contexts;
using System.Linq.Expressions;

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private OrderDbContext _orderDbContext;
        public OrderRepository(OrderDbContext context) : base(context)
        {
            _orderDbContext = context;
        }

        public override async Task<Order> GetByIdAsync(Guid id, params Expression<Func<Order, object>>[] includes)
        {
            var entity = await base.GetByIdAsync(id, includes);

            if (entity == null)
                entity = _orderDbContext.Orders.Local.FirstOrDefault(i => i.Id == id);

            return entity;
        }
        
    }
}

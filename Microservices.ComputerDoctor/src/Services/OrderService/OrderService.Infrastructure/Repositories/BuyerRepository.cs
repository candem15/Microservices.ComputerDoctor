using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Contexts;

namespace OrderService.Infrastructure.Repositories
{
    public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
    {
        public BuyerRepository(OrderDbContext context) : base(context)
        {

        }
    }
}

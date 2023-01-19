using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.EntityConfigurations;
using OrderService.Infrastructure.Extensions;

namespace OrderService.Infrastructure.Contexts
{
    public class OrderDbContext : DbContext, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "ordering";
        private readonly IMediator? mediator;

        public OrderDbContext() : base()
        {

        }

        public OrderDbContext(DbContextOptions<OrderDbContext> options, IMediator mediator) : base(options)
        {
            this.mediator = mediator;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellation = default)
        {
            if (mediator == null)
                return false;

            await mediator.DispatchDomainEventsAsync(this);

            await base.SaveChangesAsync(cancellation);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityConfiguration());

        }
    }
}

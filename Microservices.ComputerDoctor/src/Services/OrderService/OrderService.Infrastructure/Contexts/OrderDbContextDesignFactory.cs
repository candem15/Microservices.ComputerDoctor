using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OrderService.Infrastructure.Contexts
{
    public class OrderDbContextDesignFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        private readonly IConfiguration configuration;

        public OrderDbContextDesignFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public OrderDbContext CreateDbContext(string[] args)
        {
            var connectionString = configuration.GetConnectionString("OrderServiceSqlServer");

            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>()
                .UseSqlServer(connectionString);

            return new OrderDbContext(optionsBuilder.Options, null);
        }
    }
}

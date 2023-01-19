using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Infrastructure.Contexts;
using OrderService.Infrastructure.Repositories;
using System.Reflection;

namespace OrderService.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("OrderServiceSqlServer"));
                opt.EnableSensitiveDataLogging();
            });

            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            var optBuilder = new DbContextOptionsBuilder<OrderDbContext>().UseSqlServer(configuration.GetConnectionString("OrderServiceSqlServer"));

            using var dbContext = new OrderDbContext(optBuilder.Options, null);

            dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();

            return services;
        }
    }
}

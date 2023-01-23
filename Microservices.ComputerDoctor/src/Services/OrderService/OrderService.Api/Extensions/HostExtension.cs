using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace OrderService.Api.Extensions
{
    public static class HostExtension
    {
        public static WebApplication MigrateDbContext<TContext>(this WebApplication host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var service = scope.ServiceProvider;

                var logger = service.GetRequiredService<ILogger<TContext>>();

                var context = service.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context: {DbContextName}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(6),
                            TimeSpan.FromSeconds(9),
                        });

                    retry.Execute(() =>
                    {
                        InvokeSeeder(seeder, context, service);
                    });

                    logger.LogInformation("Migration to database completed!", typeof(TContext).Name);
                }
                catch (Exception)
                {
                    logger.LogInformation("An unexpected error occurred while migrating database on context: {DbContextName}", typeof(TContext).Name);
                }

                return host;
            }
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext? context, IServiceProvider service) where TContext : DbContext
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();

            seeder(context, service);
        }
    }
}

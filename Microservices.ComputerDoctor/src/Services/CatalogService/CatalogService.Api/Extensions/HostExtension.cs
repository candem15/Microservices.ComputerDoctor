using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace CatalogService.Api.Extensions
{
    public static class HostExtension
    {
        public static IHost MigrateDbContext<TContext>(this IHost host, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associate with context: {DbContextName}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                                        .WaitAndRetry(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), 3));

                    retry.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation("Database migrated with context: {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogInformation(ex, "Unexpected error occured while migrating database associated with context: {DbContextName}", typeof(TContext).Name);
                }
            }
            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext? context, IServiceProvider services) where TContext : DbContext
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();
            seeder(context, services);
        }
    }
}
using System.IO.Compression;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace OrderService.Infrastructure.Contexts
{
    public class OrderDbContextSeed
    {
        public async Task SeedAsync(OrderDbContext context, ILogger<OrderDbContext> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = false;
                var contentPath = "Seeding/Setup";

                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData ? GetCardTypesFromFile(contentPath, logger) : GetPredefinedCardTypes());

                        await context.SaveChangesAsync();
                    }

                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData ? GetOrderStatusFromFile(contentPath, logger) : GetPredefinedOrderStatus());

                        await context.SaveChangesAsync();
                    }
                }
            });
        }

        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentPath, ILogger<OrderDbContext> logger)
        {
            string fileName = "OrderStatus.txt";

            if (!File.Exists(fileName))
            {
                return GetPredefinedOrderStatus();
            }

            var fileContent = File.ReadAllLines(fileName);

            int id = 1;
            var list = fileContent.Select(i => new OrderStatus(id++, i)).Where(x => x != null);

            return list;
        }

        private IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return new List<OrderStatus>()
            {
                OrderStatus.Submitted,
                OrderStatus.AwaitingValidation,
                OrderStatus.StockConfirmed,
                OrderStatus.Paid,
                OrderStatus.Shipped,
                OrderStatus.Cancelled
            };
        }

        private IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return new List<CardType>()
            {
                CardType.Amex,
                CardType.Visa,
                CardType.MasterCard,
                CardType.CapitalOne
            };
        }

        private IEnumerable<CardType> GetCardTypesFromFile(string contentPath, ILogger<OrderDbContext> logger)
        {
            string fileName = "CardTypes.txt";

            if (!File.Exists(fileName))
            {
                return GetPredefinedCardTypes();
            }

            var fileContent = File.ReadAllLines(fileName);

            int id = 1;
            var list = fileContent.Select(i => new CardType(id++, i)).Where(x => x != null);

            return list;
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<OrderDbContext> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>()
                            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), retries),
                                onRetry: (exception, timeSpan, retry, ctx) =>
                                {
                                    logger.LogWarning(exception, "[{prefix}] Exeception Type: {excep    tion} occurred!");
                                });
        }
    }
}
using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Domain.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace BasketService.Api.Infrastructure.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger<BasketRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public BasketRepository(ConnectionMultiplexer redis, ILogger<BasketRepository> logger)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
            _logger = logger;
        }

        public async Task<bool> DeleteBasketAsync(string customerId)
        {
            return await _database.KeyDeleteAsync(customerId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var data = await _database.StringGetAsync(customerId);

            if (data.IsNull) return null;

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(x => x.ToString());
        }

        private IServer GetServer()
        {
            return _redis.GetServer(_redis.GetEndPoints().First());
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            var created = await _database.StringSetAsync(customerBasket.BuyerId, JsonConvert.SerializeObject(customerBasket));

            if (!created)
            {
                _logger.LogInformation("An unexpected error occurred while persisting basket!");
                return null;
            }

            _logger.LogInformation("The basket persisted successfully!");

            return await GetBasketAsync(customerBasket.BuyerId);
        }
    }
}

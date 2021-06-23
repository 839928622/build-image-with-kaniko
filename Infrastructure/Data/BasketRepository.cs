using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Data
{
  public  class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _redisDatabase;

        public  BasketRepository(IConnectionMultiplexer redis)
        {
            _redisDatabase = redis.GetDatabase();
        }
        /// <inheritdoc />
        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            var data = await _redisDatabase.StringGetAsync(basketId);
            return data.IsNullOrEmpty ? null :  JsonSerializer.Deserialize<CustomerBasket>(data);
        }

        /// <inheritdoc />
        public async Task<CustomerBasket> UpdateOrCreateBasketAsync(CustomerBasket basket)
        {
          var created=  await _redisDatabase
              .StringSetAsync(basket.Id,JsonSerializer.Serialize(basket),TimeSpan.FromDays(30)); // if that id of basket already exist,then it will be automatically set to new value
          if (!created) return null;
          return await GetBasketAsync(basket.Id);
        }

        /// <inheritdoc />
        public  async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _redisDatabase.KeyDeleteAsync(basketId);
        }
    }
}

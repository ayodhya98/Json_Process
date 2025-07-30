using JsonProcessingApi.Models;
using JsonProcessingApi.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JsonProcessingApi.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<JsonItem> _itemsCollection;

        public MongoDbService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _itemsCollection = database.GetCollection<JsonItem>("JsonItems");
        }

        public async Task<string> CreateAsync(List<JsonItem> items)
        {
            await _itemsCollection.InsertManyAsync(items);
            return "Items created successfully";
        }

        public async Task<List<string>> GetAllTrackingNumbersAsync()
        {
            return await _itemsCollection.Find(_ => true)
                .Project(x => x.U_TrackingNo)
                .ToListAsync();
        }
    }
}

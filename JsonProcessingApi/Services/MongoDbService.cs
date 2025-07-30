using JsonProcessingApi.Models;
using JsonProcessingApi.Services.IServices;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JsonProcessingApi.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoCollection<JsonItem> _itemsCollection;
        private readonly ILogger<MongoDbService> _logger;

        public MongoDbService(
            IOptions<MongoDBSettings> mongoDBSettings,
            ILogger<MongoDbService> logger)
        {
            var settings = mongoDBSettings.Value;

            _logger = logger;

            try
            {
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                _itemsCollection = database.GetCollection<JsonItem>(settings.CollectionName);

                // Create index on tracking number for faster queries
                var indexKeysDefinition = Builders<JsonItem>.IndexKeys.Ascending(x => x.U_TrackingNo);
                var indexOptions = new CreateIndexOptions { Unique = false };
                var indexModel = new CreateIndexModel<JsonItem>(indexKeysDefinition, indexOptions);

                _itemsCollection.Indexes.CreateOne(indexModel);

                _logger.LogInformation("MongoDB service initialized. Collection: {Collection}", settings.CollectionName);
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Failed to initialize MongoDB service");
                throw;
            }
        }

        public async Task CreateAsync(List<JsonItem> items)
        {
            if (items == null || items.Count == 0)
            {
                _logger.LogWarning("Attempted to insert empty or null items list");
                return;
            }

            try
            {
                await _itemsCollection.InsertManyAsync(items);
                _logger.LogInformation("Successfully inserted {Count} items into MongoDB", items.Count);
            }
            catch (MongoBulkWriteException<JsonItem> ex)
            {
                _logger.LogError(ex, "Bulk write error. Inserted: {InsertedCount}", ex.Result.InsertedCount);
                throw;
            }
        }

        public async Task<List<string>> GetTrackingNumbersAsync()
        {
            try
            {
                return await _itemsCollection
                    .Find(FilterDefinition<JsonItem>.Empty)
                    .Project(x => x.U_TrackingNo)
                    .ToListAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error retrieving tracking numbers");
                throw;
            }
        }

        public async Task<JsonItem> GetByTrackingNumberAsync(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                _logger.LogWarning("Empty tracking number provided");
                return null;
            }

            try
            {
                var filter = Builders<JsonItem>.Filter.Eq(x => x.U_TrackingNo, trackingNumber);
                return await _itemsCollection.Find(filter).FirstOrDefaultAsync();
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Error retrieving item by tracking number: {TrackingNumber}", trackingNumber);
                throw;
            }
        }
    }
}
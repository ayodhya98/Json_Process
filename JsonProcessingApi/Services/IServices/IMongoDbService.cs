using JsonProcessingApi.Models;
namespace JsonProcessingApi.Services.IServices;

public interface IMongoDbService
{
    Task CreateAsync(List<JsonItem> items);
    Task<List<string>> GetTrackingNumbersAsync();
    Task<JsonItem> GetByTrackingNumberAsync(string trackingNumber);
}

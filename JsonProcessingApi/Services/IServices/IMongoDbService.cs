using JsonProcessingApi.Models;
namespace JsonProcessingApi.Services.IServices;

public interface IMongoDbService
{
    Task<string> CreateAsync(List<JsonItem> items);
    Task<List<string>> GetAllTrackingNumbersAsync();
}

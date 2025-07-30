namespace JsonProcessingApi.Services.IServices
{
    public interface IFileProcessingService
    {
        Task ProcessFileAsync(Stream fileStream, string fileName);
    }
}

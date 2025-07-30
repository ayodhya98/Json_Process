using JsonProcessingApi.Models;
using JsonProcessingApi.Services.IServices;
using Newtonsoft.Json;

namespace JsonProcessingApi.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IHostEnvironment _hostEnvironment;

        public FileProcessingService(
            IMongoDbService mongoDbService,
            IRabbitMqService rabbitMqService,
            IHostEnvironment hostEnvironment)
        {
            _mongoDbService = mongoDbService;
            _rabbitMqService = rabbitMqService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task ProcessFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                using var streamReader = new StreamReader(fileStream);
                using var jsonReader = new JsonTextReader(streamReader);

                var serializer = new JsonSerializer();
                var items = serializer.Deserialize<List<JsonItem>>(jsonReader);

                await _mongoDbService.CreateAsync(items);
                var trackingNumbers = await _mongoDbService.GetAllTrackingNumbersAsync();

                var outputPath = Path.Combine(_hostEnvironment.ContentRootPath, "TrackingNumbers", $"{Path.GetFileNameWithoutExtension(fileName)}_tracking.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                await File.WriteAllLinesAsync(outputPath, trackingNumbers);

                _rabbitMqService.SendMessage($"File {fileName} processed successfully. {trackingNumbers.Count} tracking numbers extracted.");
            }
            catch (Exception ex)
            {
                _rabbitMqService.SendMessage($"Error processing file {fileName}: {ex.Message}");
                throw;
            }
        }
    }
}

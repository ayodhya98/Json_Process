using JsonProcessingApi.Models;
using JsonProcessingApi.Services.IServices;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JsonProcessingApi.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<FileProcessingService> _logger;

        public FileProcessingService(
            IMongoDbService mongoDbService,
            IRabbitMqService rabbitMqService,
            IHostEnvironment hostEnvironment,
            ILogger<FileProcessingService> logger)
        {
            _mongoDbService = mongoDbService;
            _rabbitMqService = rabbitMqService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        public async Task ProcessFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                _logger.LogInformation("Starting to process file: {FileName}", fileName);
                using var streamReader = new StreamReader(fileStream);
                using var jsonReader = new JsonTextReader(streamReader);

                var serializer = new JsonSerializer();
                var items = serializer.Deserialize<List<JsonItem>>(jsonReader);

                if (items == null || items.Count == 0)
                {
                    _rabbitMqService.SendMessage($"File {fileName} contained no valid items.");
                    return;
                }

                await _mongoDbService.CreateAsync(items);
                _logger.LogInformation("Successfully uploaded {Count} items to MongoDB", items.Count);

                var trackingNumbers = items
                    .Where(x => !string.IsNullOrEmpty(x.U_TrackingNo))
                    .Select(x => x.U_TrackingNo)
                    .Distinct()
                    .ToList();

                var outputPath = Path.Combine(
                    _hostEnvironment.ContentRootPath,
                    "TrackingNumbers",
                    $"{Path.GetFileNameWithoutExtension(fileName)}_tracking.txt");

                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
                await File.WriteAllLinesAsync(outputPath, trackingNumbers);

                var successMessage = $"File {fileName} processed successfully. " +
                                    $"{items.Count} items processed, " +
                                    $"{trackingNumbers.Count} unique tracking numbers extracted.";

                _rabbitMqService.SendMessage(successMessage);
                _logger.LogInformation(successMessage);
            }
            catch (JsonException jsonEx)
            {
                var errorMessage = $"Invalid JSON format in file {fileName}: {jsonEx.Message}";
                _rabbitMqService.SendMessage(errorMessage);
                _logger.LogError(jsonEx, errorMessage);
                throw;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error processing file {fileName}: {ex.Message}";
                _rabbitMqService.SendMessage(errorMessage);
                _logger.LogError(ex, errorMessage);
                throw;
            }
        }
    }
}
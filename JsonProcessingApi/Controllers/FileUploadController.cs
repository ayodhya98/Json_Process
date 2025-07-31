using JsonProcessingApi.Models;
using JsonProcessingApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;


namespace JsonProcessingApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileProcessingService _fileProcessingService;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(
            IFileProcessingService fileProcessingService,
            ILogger<FileUploadController> logger)
        {
            _fileProcessingService = fileProcessingService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadJsonFile([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("No file uploaded.");

            if (Path.GetExtension(model.File.FileName).ToLower() != ".json")
                return BadRequest("Only JSON files are allowed.");

            try
            {
                using var memoryStream = new MemoryStream();
                await model.File.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                _ = ProcessFileInBackground(memoryStream, model.File.FileName);

                return Accepted(new
                {
                    message = "File upload accepted for processing",
                    fileName = model.File.FileName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling file upload");
                return StatusCode(500, "Internal server error");
            }
        }

        private async Task ProcessFileInBackground(Stream fileStream, string fileName)
        {
            try
            {
                await _fileProcessingService.ProcessFileAsync(fileStream, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing file in background");
            }
            finally
            {
                fileStream?.Dispose();
            }
        }
    }
}
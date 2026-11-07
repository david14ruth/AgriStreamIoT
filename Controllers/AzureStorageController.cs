
using AgriStreamIoT.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriStreamIoT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AzureStorageController : ControllerBase
    {
        private readonly AzureStorageService _storageService;

        public AzureStorageController(
            AzureStorageService storageService)
        {
            _storageService = storageService;
        }

        // Initialize Azure Storage
        [HttpPost("initialize")]
        public async Task<IActionResult> Initialize()
        {
            try
            {
                await _storageService.InitializeStorageAsync();

                return Ok(new
                {
                    success = true,
                    message = "Azure Storage initialized successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // Upload image to Azure Blob Storage
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(
            [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new
                    {
                        message = "Please select an image."
                    });
                }

                if (!file.ContentType.StartsWith("image/"))
                {
                    return BadRequest(new
                    {
                        message = "Only image files are allowed."
                    });
                }

                string extension =
                    Path.GetExtension(Path.GetFileName(file.FileName));

                string fileName =
                    $"{Guid.NewGuid()}{extension}";

                using var stream = file.OpenReadStream();

                string uploadedFile =
                    await _storageService.UploadBlobAsync(
                        stream,
                        fileName,
                        file.ContentType);

                return Ok(new
                {
                    success = true,
                    message = "Image uploaded successfully to Azure Blob Storage.",
                    fileName = uploadedFile
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Image upload failed.",
                    error = ex.Message
                });
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BizBio.Infrastructure.Services;

namespace BizBio.API.Controllers;

[Route("api/v1/uploads")]
[ApiController]
[Authorize]
public class UploadsController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;

    public UploadsController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    /// <summary>
    /// Upload a logo image
    /// </summary>
    [HttpPost("logo")]
    public async Task<IActionResult> UploadLogo(IFormFile logo)
    {
        try
        {
            if (logo == null || logo.Length == 0)
                return BadRequest(new { success = false, error = "No file provided" });

            var url = await _fileUploadService.UploadFileAsync(logo, "logos");

            return Ok(new
            {
                success = true,
                data = new
                {
                    url,
                    originalName = logo.FileName,
                    size = logo.Length
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to upload file", details = ex.Message });
        }
    }

    /// <summary>
    /// Upload a menu item image
    /// </summary>
    [HttpPost("menu-image")]
    public async Task<IActionResult> UploadMenuImage(IFormFile image)
    {
        try
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { success = false, error = "No file provided" });

            var url = await _fileUploadService.UploadFileAsync(image, "menu-items");

            return Ok(new
            {
                success = true,
                data = new
                {
                    url,
                    originalName = image.FileName,
                    size = image.Length
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to upload file", details = ex.Message });
        }
    }

    /// <summary>
    /// Upload generic file
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, error = "No file provided" });

            var url = await _fileUploadService.UploadFileAsync(file, "profiles");

            return Ok(new
            {
                success = true,
                data = new
                {
                    url,
                    originalName = file.FileName,
                    size = file.Length
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to upload file", details = ex.Message });
        }
    }

    /// <summary>
    /// Delete an uploaded file
    /// </summary>
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(string fileId)
    {
        try
        {
            var success = await _fileUploadService.DeleteFileAsync(fileId);

            if (!success)
                return NotFound(new { success = false, error = "File not found" });

            return Ok(new { success = true, message = "File deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, error = "Failed to delete file", details = ex.Message });
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BizBio.Infrastructure.Services;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(IFormFile file, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<List<string>> UploadMultipleFilesAsync(List<IFormFile> files, string folder);
}

public class FileUploadService : IFileUploadService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    private readonly IConfiguration _configuration;
    private readonly long _maxFileSize = 10 * 1024 * 1024; // 10MB
    private readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg"
    };

    public FileUploadService(IConfiguration configuration)
    {
        _configuration = configuration;

        // Get upload path from configuration or use default
        _uploadPath = _configuration["FileUpload:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        _baseUrl = _configuration["FileUpload:BaseUrl"] ?? "/uploads";

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }

        // Create subdirectories
        var folders = new[] { "logos", "menu-items", "profiles", "temp" };
        foreach (var folder in folders)
        {
            var folderPath = Path.Combine(_uploadPath, folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        if (file.Length > _maxFileSize)
            throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSize / (1024 * 1024)}MB");

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
            throw new ArgumentException($"File type {extension} is not allowed");

        // Generate unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var folderPath = Path.Combine(_uploadPath, folder);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var filePath = Path.Combine(folderPath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return relative URL
        return $"{_baseUrl}/{folder}/{fileName}";
    }

    public async Task<List<string>> UploadMultipleFilesAsync(List<IFormFile> files, string folder)
    {
        var urls = new List<string>();

        foreach (var file in files)
        {
            try
            {
                var url = await UploadFileAsync(file, folder);
                urls.Add(url);
            }
            catch
            {
                // Skip failed uploads
                continue;
            }
        }

        return urls;
    }

    public Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            // Extract relative path from URL
            var relativePath = filePath.Replace(_baseUrl, "").TrimStart('/');
            var fullPath = Path.Combine(_uploadPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}

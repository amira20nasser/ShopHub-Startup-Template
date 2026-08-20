using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using myshop.BLL.Abstraction;

namespace myshop.BLL.Services;
public class LocalFileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 2 * 1024 * 1024;

    public LocalFileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string?> UploadFileAsync(
        IFormFile? file,
        string folderName)
    {
        if (file == null || file.Length == 0)
            return null;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                $"Invalid file type. Only {string.Join(", ", AllowedExtensions)} files are allowed.");

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException(
                $"File size exceeds the 2 MB limit. Your file is {file.Length / (1024.0 * 1024.0):F2} MB.");

        var rootPath = _webHostEnvironment.WebRootPath;

        var uploadPath = Path.Combine(rootPath, folderName);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var fileName = Guid.NewGuid().ToString();

        var filePath = Path.Combine(
            uploadPath,
            fileName + extension);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create);

        await file.CopyToAsync(fileStream);

        return Path.Combine(folderName, fileName + extension)
            .Replace("\\", "/");
    }

    public void DeleteFile(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var fullPath = Path.Combine(
            _webHostEnvironment.WebRootPath,
            filePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using myshop.BLL.Abstraction;

namespace myshop.BLL.Services;
public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string?> UploadFileAsync(
        IFormFile? file,
        string folderName)
    {
        if (file == null || file.Length == 0)
            return null;

        var rootPath = _webHostEnvironment.WebRootPath;

        var uploadPath = Path.Combine(rootPath, folderName);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        var fileName = Guid.NewGuid().ToString();
        var extension = Path.GetExtension(file.FileName);

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
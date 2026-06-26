using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Api.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string subDirectory, long fileLength)
    {
        if (fileStream == null || fileLength == 0)
            throw new ArgumentException("Debe seleccionar un archivo");

        if (fileLength > MaxFileSize)
            throw new ArgumentException($"La imagen '{fileName}' no puede superar los 5 MB");

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            throw new ArgumentException($"Solo se permiten imágenes JPG, PNG y WebP. Archivo '{fileName}' no válido");

        var uploadsDir = Path.Combine(
            _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            "uploads", subDirectory
        );
        Directory.CreateDirectory(uploadsDir);

        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, uniqueName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return $"/uploads/{subDirectory}/{uniqueName}";
    }

    public Task DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(
            _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            filePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
        );

        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);

        return Task.CompletedTask;
    }
}

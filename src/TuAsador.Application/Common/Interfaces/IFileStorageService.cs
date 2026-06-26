namespace TuAsador.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string subDirectory, long fileLength);
    Task DeleteFileAsync(string filePath);
}

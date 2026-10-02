namespace CambridgeExamSystem.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream content, string fileName, string folder, CancellationToken cancellationToken = default);
    bool Delete(string relativeUrl);
}

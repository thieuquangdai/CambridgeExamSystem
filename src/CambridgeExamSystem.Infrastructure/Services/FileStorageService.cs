using System.Text.RegularExpressions;
using CambridgeExamSystem.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace CambridgeExamSystem.Infrastructure.Services;

public sealed partial class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public const long MaxFileSizeBytes = 50 * 1024 * 1024;
    private const string UploadRoot = "uploads";

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp",
        ".mp3", ".wav", ".ogg", ".m4a",
        ".mp4", ".webm",
        ".pdf"
    };

    public async Task<string> SaveAsync(Stream content, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Định dạng tệp '{extension}' không được hỗ trợ.");
        }

        var safeFolder = SafeSegment().Replace(folder.Trim().ToLowerInvariant(), string.Empty);
        if (string.IsNullOrEmpty(safeFolder))
        {
            safeFolder = "misc";
        }

        var directory = Path.Combine(GetWebRoot(), UploadRoot, safeFolder);
        Directory.CreateDirectory(directory);
        var storedName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(directory, storedName);

        await using (var file = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
        {
            var buffer = new byte[81920];
            long total = 0;
            int read;
            while ((read = await content.ReadAsync(buffer, cancellationToken)) > 0)
            {
                total += read;
                if (total > MaxFileSizeBytes)
                {
                    break;
                }

                await file.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            }

            if (total <= MaxFileSizeBytes)
            {
                return $"/{UploadRoot}/{safeFolder}/{storedName}";
            }
        }

        File.Delete(fullPath);
        throw new InvalidOperationException("Tệp vượt quá dung lượng cho phép (50 MB).");
    }

    public bool Delete(string relativeUrl)
    {
        if (!relativeUrl.StartsWith($"/{UploadRoot}/", StringComparison.Ordinal))
        {
            return false;
        }

        var uploadsRoot = Path.GetFullPath(Path.Combine(GetWebRoot(), UploadRoot));
        var fullPath = Path.GetFullPath(Path.Combine(GetWebRoot(), relativeUrl.TrimStart('/')));
        if (!fullPath.StartsWith(uploadsRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal) || !File.Exists(fullPath))
        {
            return false;
        }

        File.Delete(fullPath);
        return true;
    }

    private string GetWebRoot() =>
        string.IsNullOrEmpty(environment.WebRootPath) ? Path.Combine(environment.ContentRootPath, "wwwroot") : environment.WebRootPath;

    [GeneratedRegex("[^a-z0-9_-]")]
    private static partial Regex SafeSegment();
}

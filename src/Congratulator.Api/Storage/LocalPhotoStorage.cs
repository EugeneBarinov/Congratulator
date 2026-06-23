using Congratulator.Api.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Congratulator.Api.Storage;

/// <summary>Хранит фотографии именинников как файлы в wwwroot/uploads, отдаваемые статикой ASP.NET Core.</summary>
public class LocalPhotoStorage : IPhotoStorage
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp",
    };

    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 МБ

    private readonly string _uploadsRoot;

    public LocalPhotoStorage(IWebHostEnvironment env)
    {
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        _uploadsRoot = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0)
        {
            throw new ArgumentException("Файл пустой.");
        }

        if (file.Length > MaxFileSizeBytes)
        {
            throw new ArgumentException("Файл слишком большой (максимум 5 МБ).");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Недопустимый формат файла. Разрешены: jpg, jpeg, png, gif, webp.");
        }

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var fullPath = Path.Combine(_uploadsRoot, fileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return fileName;
    }

    public void Delete(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        // Защита от выхода за пределы каталога загрузок.
        var safeFileName = Path.GetFileName(fileName);
        var fullPath = Path.Combine(_uploadsRoot, safeFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}

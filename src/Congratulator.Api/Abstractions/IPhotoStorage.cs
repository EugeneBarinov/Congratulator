using Microsoft.AspNetCore.Http;

namespace Congratulator.Api.Abstractions;

/// <summary>
/// Абстракция хранения фотографий. Реализация по умолчанию — локальный диск
/// (Storage/LocalPhotoStorage.cs), но за этим интерфейсом можно скрыть и облачное
/// хранилище без изменений в сервисном слое.
/// </summary>
public interface IPhotoStorage
{
    /// <summary>Сохраняет файл и возвращает сгенерированное имя файла (без пути).</summary>
    Task<string> SaveAsync(IFormFile file, CancellationToken ct = default);

    /// <summary>Удаляет ранее сохранённый файл, если он существует.</summary>
    void Delete(string fileName);
}

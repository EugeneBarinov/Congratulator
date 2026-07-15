using Microsoft.AspNetCore.Http;

namespace Congratulator.Api.Dtos;

/// <summary>Обёртка для IFormFile при загрузке фотографии — нужна Swashbuckle для корректной генерации схемы.</summary>
public class PhotoUploadRequest
{
    public IFormFile Photo { get; set; } = null!;
}

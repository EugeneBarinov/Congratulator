using Congratulator.Api.Abstractions;
using Congratulator.Api.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Congratulator.Api.Controllers;

[ApiController]
[Route("api/people")]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _personService;

    public PeopleController(IPersonService personService)
    {
        _personService = personService;
    }

    /// <summary>Весь список ДР, отсортированный по близости даты.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PersonDto>>> GetAll(CancellationToken ct)
        => Ok(await _personService.GetAllAsync(ct));

    /// <summary>Сегодняшние и ближайшие ДР — главная страница.</summary>
    [HttpGet("today-upcoming")]
    public async Task<ActionResult<IReadOnlyList<PersonDto>>> GetTodayAndUpcoming(CancellationToken ct)
        => Ok(await _personService.GetTodayAndUpcomingAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonDto>> GetById(int id, CancellationToken ct)
    {
        var person = await _personService.GetByIdAsync(id, ct);
        return person is null ? NotFound() : Ok(person);
    }

    [HttpPost]
    public async Task<ActionResult<PersonDto>> Create([FromBody] PersonUpsertDto dto, CancellationToken ct)
    {
        var created = await _personService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonUpsertDto dto, CancellationToken ct)
    {
        var updated = await _personService.UpdateAsync(id, dto, ct);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var deleted = await _personService.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>Загрузка/замена фотографии (multipart/form-data, поле "photo").</summary>
    [HttpPost("{id:int}/photo")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PersonDto>> SetPhoto(int id, [FromForm] PhotoUploadRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await _personService.SetPhotoAsync(id, request.Photo, ct);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}/photo")]
    public async Task<ActionResult<PersonDto>> RemovePhoto(int id, CancellationToken ct)
    {
        var updated = await _personService.RemovePhotoAsync(id, ct);
        return updated is null ? NotFound() : Ok(updated);
    }
}

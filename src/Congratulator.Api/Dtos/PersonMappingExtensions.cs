using Congratulator.Api.Common;
using Congratulator.Api.Models;

namespace Congratulator.Api.Dtos;

public static class PersonMappingExtensions
{
    /// <summary>
    /// Превращает Person в PersonDto, считая производные поля (возраст, дни до ДР и т.п.)
    /// относительно <paramref name="today"/> и порога "ближайших" дней.
    /// </summary>
    public static PersonDto ToDto(this Person person, DateOnly today, int upcomingThresholdDays)
    {
        return new PersonDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName,
            BirthDate = person.BirthDate,
            Relation = person.Relation,
            Notes = person.Notes,
            PhotoUrl = person.PhotoFileName is null ? null : $"/uploads/{person.PhotoFileName}",
            Age = BirthdayCalculator.CalculateAge(person.BirthDate, today),
            DaysUntilNextBirthday = BirthdayCalculator.DaysUntilNextBirthday(person.BirthDate, today),
            IsToday = BirthdayCalculator.IsToday(person.BirthDate, today),
            IsUpcoming = BirthdayCalculator.IsUpcoming(person.BirthDate, today, upcomingThresholdDays),
            NextOccurrence = BirthdayCalculator.NextOccurrence(person.BirthDate, today),
        };
    }
}

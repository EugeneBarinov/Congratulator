namespace Congratulator.Api.Common;

/// <summary>
/// Вся арифметика дней рождения собрана в одном месте: ближайшее наступление даты,
/// число дней до него, попадание в "сегодня"/"ближайшие N дней", расчёт возраста.
/// Класс статический и не имеет побочных эффектов — это чистая, легко тестируемая логика
/// (см. Congratulator.Tests/BirthdayCalculatorTests.cs).
/// </summary>
public static class BirthdayCalculator
{
    /// <summary>
    /// Следующая календарная дата наступления ДР начиная с <paramref name="today"/> (включительно).
    /// Если ДР в этом году уже прошёл — возвращается дата в следующем году.
    /// </summary>
    public static DateOnly NextOccurrence(DateOnly birthDate, DateOnly today)
    {
        var occurrenceThisYear = DateInYear(birthDate, today.Year);
        return occurrenceThisYear >= today ? occurrenceThisYear : DateInYear(birthDate, today.Year + 1);
    }

    /// <summary>
    /// Переносит месяц/день рождения в указанный год.
    /// Для родившихся 29 февраля в невисокосный год датой считается 28 февраля —
    /// это распространённое и предсказуемое соглашение (альтернатива — 1 марта).
    /// </summary>
    private static DateOnly DateInYear(DateOnly birthDate, int year)
    {
        if (birthDate.Month == 2 && birthDate.Day == 29 && !DateTime.IsLeapYear(year))
        {
            return new DateOnly(year, 2, 28);
        }

        return new DateOnly(year, birthDate.Month, birthDate.Day);
    }

    /// <summary>Сколько дней осталось до следующего ДР (0 — сегодня).</summary>
    public static int DaysUntilNextBirthday(DateOnly birthDate, DateOnly today)
        => NextOccurrence(birthDate, today).DayNumber - today.DayNumber;

    public static bool IsToday(DateOnly birthDate, DateOnly today)
        => DaysUntilNextBirthday(birthDate, today) == 0;

    /// <summary>
    /// "Ближайшие" — строго в будущем (не сегодня, у этого своя категория) и в пределах порога.
    /// </summary>
    public static bool IsUpcoming(DateOnly birthDate, DateOnly today, int withinDays)
    {
        var days = DaysUntilNextBirthday(birthDate, today);
        return days > 0 && days <= withinDays;
    }

    /// <summary>Сколько полных лет исполнилось/исполнится на дату <paramref name="asOf"/>.</summary>
    public static int CalculateAge(DateOnly birthDate, DateOnly asOf)
    {
        var age = asOf.Year - birthDate.Year;
        var dayInAsOfYear = Math.Min(birthDate.Day, DateTime.DaysInMonth(asOf.Year, birthDate.Month));
        var anniversaryThisYear = new DateOnly(asOf.Year, birthDate.Month, dayInAsOfYear);

        if (asOf < anniversaryThisYear)
        {
            age--;
        }

        return age;
    }
}

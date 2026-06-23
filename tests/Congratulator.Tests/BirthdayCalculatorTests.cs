using Congratulator.Api.Common;
using Xunit;

namespace Congratulator.Tests;

public class BirthdayCalculatorTests
{
    private static readonly DateOnly Today = new(2026, 6, 23);

    [Fact]
    public void IsToday_ВозвращаетTrue_КогдаДеньИМесяцСовпадают()
    {
        var birthDate = new DateOnly(1995, 6, 23);
        Assert.True(BirthdayCalculator.IsToday(birthDate, Today));
    }

    [Fact]
    public void DaysUntilNextBirthday_СчитаетПравильноеЧислоДней()
    {
        var birthDate = new DateOnly(1990, 6, 26);
        Assert.Equal(3, BirthdayCalculator.DaysUntilNextBirthday(birthDate, Today));
    }

    [Fact]
    public void IsUpcoming_СегодняшнийДеньРожденияНеСчитаетсяБлижайшим()
    {
        var birthDate = new DateOnly(1995, 6, 23);
        Assert.False(BirthdayCalculator.IsUpcoming(birthDate, Today, 7));
    }

    [Theory]
    [InlineData(6, 30, 7, true)]  // ровно на границе порога — включается
    [InlineData(7, 1, 7, false)]  // на день за порогом — не включается
    public void IsUpcoming_ГраницаПорогаСоблюдаетсяВключительно(int month, int day, int threshold, bool expected)
    {
        var birthDate = new DateOnly(1990, month, day);
        Assert.Equal(expected, BirthdayCalculator.IsUpcoming(birthDate, Today, threshold));
    }

    [Fact]
    public void NextOccurrence_ЕслиДеньРожденияУжеПрошёлВЭтомГоду_ПереноситсяНаСледующийГод()
    {
        var birthDate = new DateOnly(1990, 1, 10);
        var next = BirthdayCalculator.NextOccurrence(birthDate, Today);
        Assert.Equal(new DateOnly(2027, 1, 10), next);
    }

    [Fact]
    public void NextOccurrence_29Февраля_ВНевисокосномГодуПереноситсяНа28Февраля()
    {
        var birthDate = new DateOnly(2000, 2, 29);
        var next = BirthdayCalculator.NextOccurrence(birthDate, new DateOnly(2026, 1, 1));
        Assert.Equal(new DateOnly(2026, 2, 28), next);
    }

    [Fact]
    public void NextOccurrence_29Февраля_ВВисокосномГодуОстаётся29Февраля()
    {
        var birthDate = new DateOnly(2000, 2, 29);
        var next = BirthdayCalculator.NextOccurrence(birthDate, new DateOnly(2028, 1, 1));
        Assert.Equal(new DateOnly(2028, 2, 29), next);
    }

    [Fact]
    public void CalculateAge_ВДеньРожденияВозрастУжеУвеличен()
    {
        var birthDate = new DateOnly(1990, 6, 23);
        Assert.Equal(36, BirthdayCalculator.CalculateAge(birthDate, new DateOnly(2026, 6, 23)));
    }

    [Fact]
    public void CalculateAge_ЗаДеньДоДняРожденияВозрастЕщёНеУвеличен()
    {
        var birthDate = new DateOnly(1990, 6, 23);
        Assert.Equal(35, BirthdayCalculator.CalculateAge(birthDate, new DateOnly(2026, 6, 22)));
    }
}

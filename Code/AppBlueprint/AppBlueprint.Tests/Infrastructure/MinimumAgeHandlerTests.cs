using AppBlueprint.Infrastructure.Authentication.Authorization;
using FluentAssertions;

namespace AppBlueprint.Tests.Infrastructure;

internal sealed class MinimumAgeHandlerTests
{
    [Test]
    public async Task CalculateAge_WhenBirthdayHasNotOccurredThisYear_ShouldNotCountCurrentYear()
    {
        // Born Dec 31 2006; evaluated Jan 1 2025 -> still 18, not 19.
        var birth = new DateTime(2006, 12, 31);
        var today = new DateTime(2025, 1, 1);

        int age = MinimumAgeHandler.CalculateAge(birth, today);

        await Assert.That(age).IsEqualTo(18);
    }

    [Test]
    public async Task CalculateAge_OnDayBeforeEighteenthBirthday_ShouldBeSeventeen()
    {
        // The classic age-gate bypass: turns 18 later this year but the naive
        // year-subtraction would already report 18.
        var birth = new DateTime(2007, 6, 13);
        var today = new DateTime(2025, 6, 12);

        int age = MinimumAgeHandler.CalculateAge(birth, today);

        age.Should().Be(17, because: "the 18th birthday is tomorrow, so the user is still 17 today");
    }

    [Test]
    public async Task CalculateAge_OnExactBirthday_ShouldCountThatYear()
    {
        var birth = new DateTime(2007, 6, 12);
        var today = new DateTime(2025, 6, 12);

        int age = MinimumAgeHandler.CalculateAge(birth, today);

        await Assert.That(age).IsEqualTo(18);
    }

    [Test]
    public async Task CalculateAge_AfterBirthdayThisYear_ShouldCountCurrentYear()
    {
        var birth = new DateTime(2007, 1, 1);
        var today = new DateTime(2025, 12, 31);

        int age = MinimumAgeHandler.CalculateAge(birth, today);

        await Assert.That(age).IsEqualTo(18);
    }
}

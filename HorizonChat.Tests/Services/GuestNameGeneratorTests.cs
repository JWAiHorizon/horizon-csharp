using HorizonChat.Services;
using Xunit;

namespace HorizonChat.Tests.Services;

public class GuestNameGeneratorTests
{
    [Fact]
    public void GenerateUniqueName_ShouldReturnNonEmptyString()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateUniqueName();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GenerateUniqueName_ShouldReturnValidFormat()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateUniqueName();

        // Assert
        // Should match either AdjectiveNoun#### or Guest##### format
        var isValidFormat = System.Text.RegularExpressions.Regex.IsMatch(result, @"^[A-Z][a-z]+[A-Z][a-z]+\d{4}$") ||
                           System.Text.RegularExpressions.Regex.IsMatch(result, @"^Guest\d{4,5}$");
        Assert.True(isValidFormat, $"Generated name '{result}' does not match expected format");
    }

    [Fact]
    public void GenerateUniqueName_ShouldGenerateUniqueNames()
    {
        // Arrange
        var generator = new GuestNameGenerator();
        var names = new HashSet<string>();
        const int count = 100;

        // Act
        for (int i = 0; i < count; i++)
        {
            names.Add(generator.GenerateUniqueName());
        }

        // Assert
        // We should have close to 'count' unique names (allowing for some possible collisions in fallback mode)
        Assert.True(names.Count >= count * 0.95, $"Expected at least {count * 0.95} unique names, got {names.Count}");
    }

    [Fact]
    public void GenerateUniqueName_ShouldContainNumberSuffix()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateUniqueName();

        // Assert
        Assert.Matches(@"\d{4,5}$", result);
    }

    [Fact]
    public void GenerateUniqueName_ShouldStartWithCapitalLetter()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateUniqueName();

        // Assert
        Assert.True(char.IsUpper(result[0]), $"Generated name '{result}' should start with a capital letter");
    }

    [Fact]
    public void GenerateSimpleName_ShouldReturnGuestFormat()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateSimpleName();

        // Assert
        Assert.StartsWith("Guest", result);
        Assert.Matches(@"^Guest\d{4}$", result);
    }

    [Fact]
    public void GenerateSimpleName_ShouldHaveFourDigits()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateSimpleName();
        var numberPart = result.Replace("Guest", "");

        // Assert
        Assert.Equal(4, numberPart.Length);
        Assert.True(int.TryParse(numberPart, out var number));
        Assert.InRange(number, 1000, 9999);
    }

    [Theory]
    [InlineData("Guest1234", true)]
    [InlineData("Guest5678", true)]
    [InlineData("Guest12345", true)]
    [InlineData("BraveTiger1234", true)]
    [InlineData("HappyDolphin5678", true)]
    [InlineData("guest1234", false)]
    [InlineData("GUEST1234", false)]
    [InlineData("Guest123", false)]
    [InlineData("Guest", false)]
    [InlineData("", false)]
    [InlineData("  ", false)]
    [InlineData("NormalUser", false)]
    [InlineData("User123", false)]
    public void IsGuestName_ShouldValidateCorrectly(string username, bool expected)
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.IsGuestName(username);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsGuestName_ShouldReturnFalseForNull()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.IsGuestName(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GenerateUniqueName_MultipleInstances_ShouldGenerateDifferentNames()
    {
        // Arrange
        var generator1 = new GuestNameGenerator();
        var generator2 = new GuestNameGenerator();
        var names = new HashSet<string>();

        // Act
        for (int i = 0; i < 50; i++)
        {
            names.Add(generator1.GenerateUniqueName());
            names.Add(generator2.GenerateUniqueName());
        }

        // Assert
        Assert.True(names.Count >= 95, "Multiple generator instances should produce varied names");
    }

    [Fact]
    public void GenerateUniqueName_ShouldNotContainSpaces()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateUniqueName();

        // Assert
        Assert.DoesNotContain(" ", result);
    }

    [Fact]
    public void GenerateUniqueName_ShouldBeLessThan20Characters()
    {
        // Arrange
        var generator = new GuestNameGenerator();

        // Act
        var result = generator.GenerateUniqueName();

        // Assert
        Assert.True(result.Length <= 20, $"Generated name '{result}' is {result.Length} characters, expected <= 20");
    }

    [Fact]
    public void GenerateUniqueName_StressTest_ShouldHandleManyGenerations()
    {
        // Arrange
        var generator = new GuestNameGenerator();
        var names = new List<string>();

        // Act & Assert - should not throw
        for (int i = 0; i < 1000; i++)
        {
            var name = generator.GenerateUniqueName();
            Assert.NotNull(name);
            Assert.NotEmpty(name);
            names.Add(name);
        }

        // Verify we got a reasonable amount of unique names
        var uniqueNames = names.Distinct().Count();
        Assert.True(uniqueNames >= 900, $"Expected at least 900 unique names in 1000 generations, got {uniqueNames}");
    }
}

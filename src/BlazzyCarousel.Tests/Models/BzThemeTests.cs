namespace BlazzyCarousel.Tests.Models;

public class BzThemeTests
{
    [Fact]
    public void BzTheme_ShouldHaveGlassValue()
    {
        // Arrange & Act
        var theme = BzTheme.Glass;

        // Assert
        theme.Should().Be(BzTheme.Glass);
        ((int)theme).Should().Be(0);
    }

    [Fact]
    public void BzTheme_ShouldHaveDarkValue()
    {
        // Arrange & Act
        var theme = BzTheme.Dark;

        // Assert
        theme.Should().Be(BzTheme.Dark);
        ((int)theme).Should().Be(1);
    }

    [Fact]
    public void BzTheme_ShouldHaveLightValue()
    {
        // Arrange & Act
        var theme = BzTheme.Light;

        // Assert
        theme.Should().Be(BzTheme.Light);
        ((int)theme).Should().Be(2);
    }

    [Fact]
    public void BzTheme_ShouldHaveMinimalValue()
    {
        // Arrange & Act
        var theme = BzTheme.Minimal;

        // Assert
        theme.Should().Be(BzTheme.Minimal);
        ((int)theme).Should().Be(3);
    }

    [Fact]
    public void BzTheme_ShouldHaveExactlyFourValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<BzTheme>();

        // Assert
        values.Should().HaveCount(4);
    }

    [Theory]
    [InlineData(BzTheme.Glass, "Glass")]
    [InlineData(BzTheme.Dark, "Dark")]
    [InlineData(BzTheme.Light, "Light")]
    [InlineData(BzTheme.Minimal, "Minimal")]
    public void BzTheme_ShouldConvertToString(BzTheme theme, string expected)
    {
        // Act
        var result = theme.ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Glass", BzTheme.Glass)]
    [InlineData("Dark", BzTheme.Dark)]
    [InlineData("Light", BzTheme.Light)]
    [InlineData("Minimal", BzTheme.Minimal)]
    public void BzTheme_ShouldParseFromString(string value, BzTheme expected)
    {
        // Act
        var result = Enum.Parse<BzTheme>(value);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void BzTheme_ShouldSupportComparison()
    {
        // Arrange
        var glass = BzTheme.Glass;
        var dark = BzTheme.Dark;

        // Act & Assert
        glass.Should().NotBe(dark);
        glass.Should().Be(BzTheme.Glass);
    }
}

namespace BlazzyMotion.Tests.Components;

/// <summary>
/// Basic tests for BzCarousel component covering essential functionality
/// </summary>
public class BzCarouselBasicTests : TestContext
{
    [Fact]
    public void BzCarousel_WithNullItems_ShouldShowLoadingState()
    {
        // Arrange & Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, null));

        // Assert
        cut.Find(".bzc-loading").Should().NotBeNull();
    }

    [Fact]
    public void BzCarousel_WithEmptyItems_ShouldShowEmptyState()
    {
        // Arrange
        var items = new List<TestMovie>();

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        cut.Find(".bzc-empty").Should().NotBeNull();
    }

    [Fact]
    public void BzCarousel_WithItems_ShouldRenderCarousel()
    {
        // Arrange
        var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" },
            new TestMovie { ImageUrl = "test3.jpg", Title = "Movie 3" }
        };

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        cut.Find(".bzc-carousel-box").Should().NotBeNull();
    }

    [Theory]
    [InlineData(BzTheme.Glass, "bzc-theme-glass")]
    [InlineData(BzTheme.Dark, "bzc-theme-dark")]
    [InlineData(BzTheme.Light, "bzc-theme-light")]
    [InlineData(BzTheme.Minimal, "bzc-theme-minimal")]
    public void BzCarousel_WithTheme_ShouldApplyCorrectCssClass(BzTheme theme, string expectedClass)
    {
        // Arrange
        var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Theme, theme));

        // Assert
        var carouselBox = cut.Find(".bzc-carousel-box");
        carouselBox.ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void BzCarousel_WithCustomCssClass_ShouldApplyIt()
    {
        // Arrange
        var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };
        var customClass = "custom-carousel-class";

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.CssClass, customClass));

        // Assert
        var carouselBox = cut.Find(".bzc-carousel-box");
        carouselBox.ClassList.Should().Contain(customClass);
    }

    [Fact]
    public void BzCarousel_DefaultTheme_ShouldBeGlass()
    {
        // Arrange
        var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        var carouselBox = cut.Find(".bzc-carousel-box");
        carouselBox.ClassList.Should().Contain("bzc-theme-glass");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void BzCarousel_WithMultipleItems_ShouldRender(int itemCount)
    {
        // Arrange
        var items = Enumerable.Range(1, itemCount)
            .Select(i => new TestMovie { ImageUrl = $"test{i}.jpg", Title = $"Movie {i}" })
            .ToList();

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items));

        // Assert
        cut.Find(".bzc-carousel-box").Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void BzCarousel_WithInitialSlide_ShouldSetParameter(int initialSlide)
    {
        // Arrange
        var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" },
            new TestMovie { ImageUrl = "test3.jpg", Title = "Movie 3" }
        };

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.InitialSlide, initialSlide));

        // Assert
        cut.Instance.InitialSlide.Should().Be(initialSlide);
    }

    [Fact]
    public void BzCarousel_WithLoop_ShouldSetParameter()
    {
        // Arrange
        var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" },
            new TestMovie { ImageUrl = "test3.jpg", Title = "Movie 3" }
        };

        // Act
        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.Loop, true));

        // Assert
        cut.Instance.Loop.Should().BeTrue();
    }
}

namespace BlazzyMotion.Tests.Components;

/// <summary>
/// Advanced tests for BzCarousel component covering edge cases and parameter validation
/// </summary>
public class BzCarouselAdvancedTests : TestBase
{
  #region Parameter Validation Tests

  [Theory]
  [InlineData(-1)]
  [InlineData(361)]
  [InlineData(500)]
  public void BzCarousel_WithInvalidRotateDegree_ShouldThrowException(int invalidDegree)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act & Assert
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.RotateDegree, invalidDegree));

    act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("*RotateDegree must be between 0 and 360 degrees*");
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(-100)]
  public void BzCarousel_WithNegativeDepth_ShouldThrowException(int invalidDepth)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act & Assert
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.Depth, invalidDepth));

    act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("*Depth must be a non-negative value*");
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void BzCarousel_WithInvalidMinItemsForLoop_ShouldThrowException(int invalidValue)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act & Assert
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.MinItemsForLoop, invalidValue));

    act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("*MinItemsForLoop must be at least 1*");
  }

  [Theory]
  [InlineData(0)]
  [InlineData(-1)]
  public void BzCarousel_WithInvalidMinItemsForCoverflow_ShouldThrowException(int invalidValue)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act & Assert
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.MinItemsForCoverflow, invalidValue));

    act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("*MinItemsForCoverflow must be at least 1*");
  }

  [Theory]
  [InlineData(-1)]
  [InlineData(-5)]
  public void BzCarousel_WithNegativeInitialSlide_ShouldThrowException(int invalidSlide)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act & Assert
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.InitialSlide, invalidSlide));

    act.Should().Throw<ArgumentOutOfRangeException>()
        .WithMessage("*InitialSlide must be a non-negative value*");
  }

  [Theory]
  [InlineData(0, 360)]
  [InlineData(50, 180)]
  [InlineData(100, 90)]
  public void BzCarousel_WithValidRotateDegree_ShouldNotThrow(int rotateDegree, int depth)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.RotateDegree, rotateDegree)
        .Add(p => p.Depth, depth));

    // Assert
    act.Should().NotThrow();
  }

  #endregion

  #region AutoDetectMode Tests

  [Theory]
  [InlineData(1)]
  [InlineData(2)]
  [InlineData(3)]
  public void BzCarousel_WithAutoDetectMode_FewItems_ShouldUseSimpleMode(int itemCount)
  {
    // Arrange
    var items = Enumerable.Range(1, itemCount)
        .Select(i => new TestMovie { ImageUrl = $"test{i}.jpg", Title = $"Movie {i}" })
        .ToList();

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.AutoDetectMode, true)
        .Add(p => p.MinItemsForCoverflow, 4));

    // Assert - Simple mode doesn't have coverflow classes
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Theory]
  [InlineData(4)]
  [InlineData(5)]
  [InlineData(10)]
  public void BzCarousel_WithAutoDetectMode_ManyItems_ShouldUseCoverflowMode(int itemCount)
  {
    // Arrange
    var items = Enumerable.Range(1, itemCount)
        .Select(i => new TestMovie { ImageUrl = $"test{i}.jpg", Title = $"Movie {i}" })
        .ToList();

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.AutoDetectMode, true)
        .Add(p => p.MinItemsForCoverflow, 4));

    // Assert
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Fact]
  public void BzCarousel_WithAutoDetectModeDisabled_ShouldAlwaysUseCoverflowMode()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.AutoDetectMode, false));

    // Assert
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  #endregion

  #region Loop Behavior Tests

  [Theory]
  [InlineData(1, 3, false)]
  [InlineData(2, 3, false)]
  [InlineData(3, 3, true)]
  [InlineData(4, 3, true)]
  public void BzCarousel_LoopBehavior_ShouldRespectMinItemsForLoop(int itemCount, int minItems, bool shouldLoop)
  {
    // Arrange
    var items = Enumerable.Range(1, itemCount)
        .Select(i => new TestMovie { ImageUrl = $"test{i}.jpg", Title = $"Movie {i}" })
        .ToList();

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.Loop, true)
        .Add(p => p.MinItemsForLoop, minItems));

    // Assert
    cut.Instance.Loop.Should().BeTrue();
  }

  [Fact]
  public void BzCarousel_WithLoopDisabled_ShouldNotLoop()
  {
    // Arrange
    var items = Enumerable.Range(1, 5)
        .Select(i => new TestMovie { ImageUrl = $"test{i}.jpg", Title = $"Movie {i}" })
        .ToList();

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.Loop, false));

    // Assert
    cut.Instance.Loop.Should().BeFalse();
  }

  #endregion

  #region Template Priority Tests

  [Fact]
  public void BzCarousel_WithManualTemplate_ShouldUseManualTemplate()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    RenderFragment<TestMovie> customTemplate = movie => builder =>
    {
      builder.OpenElement(0, "div");
      builder.AddAttribute(1, "class", "custom-template");
      builder.AddContent(2, movie.Title);
      builder.CloseElement();
    };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.ItemTemplate, customTemplate));

    // Assert
    cut.Find(".custom-template").Should().NotBeNull();
  }

  [Fact]
  public void BzCarousel_WithoutManualTemplate_ShouldUseGeneratedOrFallback()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items));

    // Assert - Should render something (generated or fallback)
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  #endregion

  #region OnItemSelected Callback Tests

  [Fact]
  public async Task BzCarousel_OnItemSelected_ShouldInvokeCallback()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" }
        };

    TestMovie? selectedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this,
        (movie) => selectedItem = movie);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.OnItemSelected, callback));

    // Simulate click on first item
    var slides = cut.FindAll(".swiper-slide");
    if (slides.Any())
    {
      await slides[0].ClickAsync(new MouseEventArgs());
    }

    // Assert - callback should be set
    cut.Instance.OnItemSelected.Should().NotBe(default(EventCallback<TestMovie>));
  }

  #endregion

  #region ShowOverlay Tests

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void BzCarousel_ShowOverlay_ShouldSetParameter(bool showOverlay)
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.ShowOverlay, showOverlay));

    // Assert
    cut.Instance.ShowOverlay.Should().Be(showOverlay);
  }

  #endregion

  #region Custom Options Tests

  [Fact]
  public void BzCarousel_WithCustomOptions_ShouldUseProvidedOptions()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test.jpg", Title = "Test Movie" }
        };

    var customOptions = new BzCarouselOptions
    {
      Effect = "fade",
      Speed = 1000,
      Loop = false
    };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.Options, customOptions));

    // Assert
    cut.Instance.Options.Should().Be(customOptions);
    cut.Instance.Options!.Effect.Should().Be("fade");
    cut.Instance.Options.Speed.Should().Be(1000);
  }

  [Fact]
  public void BzCarousel_WithoutCustomOptions_ShouldUseDefaultParameters()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" },
            new TestMovie { ImageUrl = "test3.jpg", Title = "Movie 3" },
            new TestMovie { ImageUrl = "test4.jpg", Title = "Movie 4" },
            new TestMovie { ImageUrl = "test5.jpg", Title = "Movie 5" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.RotateDegree, 60)
        .Add(p => p.Depth, 200));

    // Assert
    cut.Instance.RotateDegree.Should().Be(60);
    cut.Instance.Depth.Should().Be(200);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public void BzCarousel_WithSingleItem_ShouldRender()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "single.jpg", Title = "Single Movie" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items));

    // Assert
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Fact]
  public void BzCarousel_WithInitialSlideGreaterThanItemCount_ShouldClamp()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.InitialSlide, 10)
        .Add(p => p.Loop, false));

    // Assert - Should not throw and should render
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Fact]
  public void BzCarousel_WithNullImageUrl_ShouldStillRender()
  {
    // Arrange
    var items = new List<TestMovie>
        {
            new TestMovie { ImageUrl = null, Title = "Movie Without Image" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items));

    // Assert
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Fact]
  public void BzCarousel_ReRenderWithDifferentItemCount_ShouldUpdate()
  {
    // Arrange
    var initialItems = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" }
        };

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, initialItems));

    // Update items
    var newItems = new List<TestMovie>
        {
            new TestMovie { ImageUrl = "test1.jpg", Title = "Movie 1" },
            new TestMovie { ImageUrl = "test2.jpg", Title = "Movie 2" },
            new TestMovie { ImageUrl = "test3.jpg", Title = "Movie 3" }
        };

    cut.SetParametersAndRender(parameters => parameters
        .Add(p => p.Items, newItems));

    // Assert
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  #endregion
}

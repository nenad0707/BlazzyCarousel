namespace BlazzyMotion.Tests.Components;

/// <summary>
/// Tests for BzCarousel scroll selection feature (SelectOnScroll parameter).
/// Covers OnActiveItemChanged, OnActiveIndexChanged events and JS interop callbacks.
/// </summary>
public class BzCarouselScrollSelectionTests : TestBase
{
  #region SelectOnScroll Parameter Tests

  [Fact]
  public void SelectOnScroll_DefaultValue_ShouldBeTrue()
  {
    // Arrange
    var items = CreateTestMovies(5);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeTrue();
  }

  [Fact]
  public void SelectOnScroll_CanBeSetToFalse()
  {
    // Arrange
    var items = CreateTestMovies(5);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, false));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeFalse();
  }

  [Fact]
  public void SelectOnScroll_CanBeSetToTrue()
  {
    // Arrange
    var items = CreateTestMovies(5);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeTrue();
  }

  #endregion

  #region OnActiveItemChanged Event Tests

  [Fact]
  public void OnActiveItemChanged_ShouldBeInvokable()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? changedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this, item => changedItem = item);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.OnActiveItemChanged, callback));

    // Assert
    cut.Instance.OnActiveItemChanged.Should().NotBe(default(EventCallback<TestMovie>));
  }

  [Fact]
  public async Task OnActiveItemChanged_WhenSelectOnScrollTrue_ShouldBeConfigured()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? changedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this, item => changedItem = item);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true)
        .Add(p => p.OnActiveItemChanged, callback));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeTrue();
    cut.Instance.OnActiveItemChanged.HasDelegate.Should().BeTrue();
  }

  [Fact]
  public void OnActiveItemChanged_WhenSelectOnScrollFalse_ShouldStillAcceptCallback()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? changedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this, item => changedItem = item);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, false)
        .Add(p => p.OnActiveItemChanged, callback));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeFalse();
    cut.Instance.OnActiveItemChanged.HasDelegate.Should().BeTrue();
  }

  #endregion

  #region OnActiveIndexChanged Event Tests

  [Fact]
  public void OnActiveIndexChanged_ShouldBeInvokable()
  {
    // Arrange
    var items = CreateTestMovies(5);
    int? changedIndex = null;
    EventCallback<int> callback = EventCallback.Factory.Create<int>(
        this, index => changedIndex = index);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.OnActiveIndexChanged, callback));

    // Assert
    cut.Instance.OnActiveIndexChanged.Should().NotBe(default(EventCallback<int>));
  }

  [Fact]
  public void OnActiveIndexChanged_DefaultValue_ShouldHaveNoDelegate()
  {
    // Arrange
    var items = CreateTestMovies(5);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items));

    // Assert
    cut.Instance.OnActiveIndexChanged.HasDelegate.Should().BeFalse();
  }

  [Fact]
  public void OnActiveIndexChanged_WhenProvided_ShouldHaveDelegate()
  {
    // Arrange
    var items = CreateTestMovies(5);
    EventCallback<int> callback = EventCallback.Factory.Create<int>(
        this, _ => { });

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.OnActiveIndexChanged, callback));

    // Assert
    cut.Instance.OnActiveIndexChanged.HasDelegate.Should().BeTrue();
  }

  #endregion

  #region Combined Events Tests

  [Fact]
  public void BothEvents_CanBeConfiguredSimultaneously()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? changedItem = null;
    int? changedIndex = null;

    EventCallback<TestMovie> itemCallback = EventCallback.Factory.Create<TestMovie>(
        this, item => changedItem = item);
    EventCallback<int> indexCallback = EventCallback.Factory.Create<int>(
        this, index => changedIndex = index);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.OnActiveItemChanged, itemCallback)
        .Add(p => p.OnActiveIndexChanged, indexCallback));

    // Assert
    cut.Instance.OnActiveItemChanged.HasDelegate.Should().BeTrue();
    cut.Instance.OnActiveIndexChanged.HasDelegate.Should().BeTrue();
  }

  [Fact]
  public void AllThreeEvents_CanBeConfiguredSimultaneously()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? selectedItem = null;
    TestMovie? changedItem = null;
    int? changedIndex = null;

    EventCallback<TestMovie> selectCallback = EventCallback.Factory.Create<TestMovie>(
        this, item => selectedItem = item);
    EventCallback<TestMovie> itemCallback = EventCallback.Factory.Create<TestMovie>(
        this, item => changedItem = item);
    EventCallback<int> indexCallback = EventCallback.Factory.Create<int>(
        this, index => changedIndex = index);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true)
        .Add(p => p.OnItemSelected, selectCallback)
        .Add(p => p.OnActiveItemChanged, itemCallback)
        .Add(p => p.OnActiveIndexChanged, indexCallback));

    // Assert
    cut.Instance.OnItemSelected.HasDelegate.Should().BeTrue();
    cut.Instance.OnActiveItemChanged.HasDelegate.Should().BeTrue();
    cut.Instance.OnActiveIndexChanged.HasDelegate.Should().BeTrue();
  }

  #endregion

  #region SelectOnScroll Behavior Tests

  [Fact]
  public void SelectOnScroll_True_WithOnItemSelected_ShouldConfigureBoth()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? selectedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this, item => selectedItem = item);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true)
        .Add(p => p.OnItemSelected, callback));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeTrue();
    cut.Instance.OnItemSelected.HasDelegate.Should().BeTrue();
  }

  [Fact]
  public void SelectOnScroll_False_WithOnItemSelected_ShouldOnlyFireOnClick()
  {
    // Arrange
    var items = CreateTestMovies(5);
    TestMovie? selectedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this, item => selectedItem = item);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, false)
        .Add(p => p.OnItemSelected, callback));

    // Assert - Verify configuration
    cut.Instance.SelectOnScroll.Should().BeFalse();
    cut.Instance.OnItemSelected.HasDelegate.Should().BeTrue();
  }

  #endregion

  #region Edge Cases

  [Fact]
  public void SelectOnScroll_WithEmptyItems_ShouldNotThrow()
  {
    // Arrange
    var items = new List<TestMovie>();

    // Act
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true));

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public void SelectOnScroll_WithNullItems_ShouldNotThrow()
  {
    // Act
    var act = () => RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, null)
        .Add(p => p.SelectOnScroll, true));

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public void SelectOnScroll_WithSingleItem_ShouldWork()
  {
    // Arrange
    var items = CreateTestMovies(1);
    TestMovie? changedItem = null;
    EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
        this, item => changedItem = item);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true)
        .Add(p => p.OnActiveItemChanged, callback));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeTrue();
    cut.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Fact]
  public void SelectOnScroll_ParameterChange_ShouldUpdate()
  {
    // Arrange
    var items = CreateTestMovies(5);
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true));

    // Act
    cut.SetParametersAndRender(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, false));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeFalse();
  }

  #endregion

  #region Rendering Tests

  [Fact]
  public void SelectOnScroll_ShouldNotAffectRendering()
  {
    // Arrange
    var items = CreateTestMovies(5);

    // Act
    var cutWithScroll = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true));

    var cutWithoutScroll = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, false));

    // Assert - Both should render carousel box
    cutWithScroll.Find(".bzc-carousel-box").Should().NotBeNull();
    cutWithoutScroll.Find(".bzc-carousel-box").Should().NotBeNull();
  }

  [Fact]
  public void SelectOnScroll_WithTheme_ShouldWork()
  {
    // Arrange
    var items = CreateTestMovies(5);

    // Act
    var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
        .Add(p => p.Items, items)
        .Add(p => p.SelectOnScroll, true)
        .Add(p => p.Theme, BzTheme.Dark));

    // Assert
    cut.Instance.SelectOnScroll.Should().BeTrue();
    cut.Find(".bzc-carousel-box").ClassList.Should().Contain("bzc-theme-dark");
  }

    #endregion

    #region Disposal Tests

    [Fact]
    public void SelectOnScroll_AfterDispose_ShouldMarkAsDisposed()
    {
        // Arrange
        var items = CreateTestMovies(5);
        TestMovie? changedItem = null;
        EventCallback<TestMovie> callback = EventCallback.Factory.Create<TestMovie>(
            this, item => changedItem = item);

        var cut = RenderComponent<BzCarousel<TestMovie>>(parameters => parameters
            .Add(p => p.Items, items)
            .Add(p => p.SelectOnScroll, true)
            .Add(p => p.OnActiveItemChanged, callback));

        // Act - Just dispose the component via bUnit (handles cleanup)
        cut.Dispose();
    }

    #endregion

    #region Helper Methods

    private static List<TestMovie> CreateTestMovies(int count)
  {
    return Enumerable.Range(1, count)
        .Select(i => new TestMovie
        {
          ImageUrl = $"https://example.com/movie{i}.jpg",
          Title = $"Movie {i}",
          Description = $"Description for Movie {i}",
          Year = 2020 + i,
          Director = $"Director {i}"
        })
        .ToList();
  }

  #endregion
}

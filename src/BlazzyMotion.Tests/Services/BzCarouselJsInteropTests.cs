namespace BlazzyMotion.Tests.Services;

/// <summary>
/// Tests for BzCarouselJsInterop service construction.
/// Note: Async JSInterop behavior is tested indirectly through component tests
/// since bUnit's mock doesn't support Lazy module initialization pattern.
/// </summary>
public class BzCarouselJsInteropTests : TestBase
{
  [Fact]
  public void Constructor_WithValidJsRuntime_ShouldCreateInstance()
  {
    // Arrange & Act
    var service = new BzCarouselJsInterop(JSInterop.JSRuntime);

    // Assert
    service.Should().NotBeNull();
  }

  [Fact]
  public void Service_ShouldImplementIAsyncDisposable()
  {
    // Arrange
    var service = new BzCarouselJsInterop(JSInterop.JSRuntime);

    // Assert
    service.Should().BeAssignableTo<IAsyncDisposable>();
  }
}

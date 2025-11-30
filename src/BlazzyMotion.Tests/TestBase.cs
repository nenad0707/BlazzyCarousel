using Bunit.JSInterop;

namespace BlazzyMotion.Tests;

/// <summary>
/// Base class for all component tests that use bUnit TestContext.
/// Configures JSInterop to prevent JavaScript module loading timeouts.
/// </summary>
public abstract class TestBase : TestContext
{
  protected BunitJSModuleInterop CarouselModule { get; }

  protected TestBase()
  {
    // Configure JSInterop to Loose mode - automatically handles unmocked calls
    JSInterop.Mode = JSRuntimeMode.Loose;

    // Setup the module that BzCarouselJsInterop imports
    // BzCarouselJsInterop uses: jsRuntime.InvokeAsync<IJSObjectReference>("import", path)
    CarouselModule = JSInterop.SetupModule(
      "./_content/BlazzyMotion.Carousel/js/blazzy-carousel.js");

    // Setup all methods the module exposes
    CarouselModule.SetupVoid("initializeCarousel", _ => true);
    CarouselModule.SetupVoid("destroyCarousel", _ => true);
    CarouselModule.SetupVoid("ensureSwiperLoaded", _ => true);
    CarouselModule.Setup<int>("getActiveIndex", _ => true).SetResult(0);
  }
}

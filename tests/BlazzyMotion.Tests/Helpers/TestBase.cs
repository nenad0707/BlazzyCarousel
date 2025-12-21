using Bunit.JSInterop;

namespace BlazzyMotion.Tests;

/// <summary>
/// Base class for all component tests that use bUnit TestContext.
/// Configures JSInterop to prevent JavaScript module loading timeouts.
/// </summary>
public abstract class TestBase : TestContext
{
  protected BunitJSModuleInterop CoreModule { get; }

  protected TestBase()
  {
    // Configure JSInterop to Loose mode - automatically handles unmocked calls
    JSInterop.Mode = JSRuntimeMode.Loose;

    // Setup the module that BzCarouselJsInterop imports
    // Now points to Core module (v1.1.0+)
    CoreModule = JSInterop.SetupModule(
        "./_content/BlazzyMotion.Core/js/blazzy-core.js");

    // Setup all methods the module exposes
    // Updated signature for v1.2.0 - initializeCarousel now accepts dotNetRef
    CoreModule.SetupVoid("initializeCarousel", _ => true);
    CoreModule.SetupVoid("destroyCarousel", _ => true);
    CoreModule.SetupVoid("ensureSwiperLoaded", _ => true);
    CoreModule.Setup<int>("getActiveIndex", _ => true).SetResult(0);
    CoreModule.Setup<int>("getRealIndex", _ => true).SetResult(0);
  }

  // Legacy property for backward compatibility with existing tests
  protected BunitJSModuleInterop CarouselModule => CoreModule;
}

using BlazzyCarousel.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazzyCarousel.Services;

/// <summary>
/// JavaScript interop service for BlazzyCarousel.
/// </summary>
public class BzCarouselJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private bool swiperLoaded = false;
    private ElementReference? _element;

    public BzCarouselJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/BlazzyCarousel/js/blazzy-carousel.js").AsTask());
    }

    /// <summary>
    /// Initialize the carousel with given options.
    /// </summary>
    /// <param name="element">The container element reference</param>
    /// <param name="options">Carousel configuration options</param>
    public async ValueTask InitializeAsync(ElementReference element, BzCarouselOptions options)
    {
        _element = element;
        var module = await moduleTask.Value;

        if (!swiperLoaded)
        {
            await module.InvokeVoidAsync("ensureSwiperLoaded");
            swiperLoaded = true;
        }

        var optionsJson = JsonSerializer.Serialize(options, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });


        await module.InvokeVoidAsync("initializeCarousel", element, optionsJson);
    }

    /// <summary>
    /// Get the currently active slide index.
    /// </summary>
    /// <returns>Active slide index</returns>
    public async ValueTask<int> GetActiveIndexAsync()
    {
        if (moduleTask.IsValueCreated && _element.HasValue)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<int>("getActiveIndex", _element.Value);
        }
        return 0;
    }

    /// <summary>
    /// Destroy the carousel instance.
    /// </summary>
    public async ValueTask DestroyAsync()
    {
        if (moduleTask.IsValueCreated && _element.HasValue)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("destroyCarousel", _element.Value);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DestroyAsync();

        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
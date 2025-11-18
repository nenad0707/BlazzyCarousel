using BlazzyCarousel.Models;
using BlazzyCarousel.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Concurrent;
using System.Reflection;

namespace BlazzyCarousel.Components;

/// <summary>
/// A 3D carousel component with glass morphism design and coverflow effect.
/// </summary>
/// <typeparam name="TItem">The type of items to display in the carousel</typeparam>
public partial class BzCarousel<TItem> : ComponentBase, IAsyncDisposable
{
    #region Parameters

    /// <summary>
    /// The collection of items to display in the carousel.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Template for rendering each carousel item.
    /// If not provided, will attempt to use auto-generated template from [BzImage] attribute.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    /// <summary>
    /// Callback invoked when an item is clicked/selected.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> OnItemSelected { get; set; }

    /// <summary>
    /// Custom loading template.
    /// </summary>
    [Parameter]
    public RenderFragment? LoadingTemplate { get; set; }

    /// <summary>
    /// Custom empty state template.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <summary>
    /// Whether to show the overlay on slides.
    /// </summary>
    [Parameter]
    public bool ShowOverlay { get; set; } = true;

    /// <summary>
    /// Visual theme for the carousel.
    /// </summary>
    [Parameter]
    public BzTheme Theme { get; set; } = BzTheme.Glass;

    /// <summary>
    /// Additional CSS class for customization.
    /// </summary>
    [Parameter]
    public string? CssClass { get; set; }

    /// <summary>
    /// Index of the initially active slide.
    /// </summary>
    [Parameter]
    public int InitialSlide { get; set; } = 0;

    /// <summary>
    /// Whether to enable infinite loop.
    /// </summary>
    [Parameter]
    public bool Loop { get; set; } = true;

    /// <summary>
    /// Minimum number of items required to enable loop mode.
    /// </summary>
    [Parameter]
    public int MinItemsForLoop { get; set; } = 3;

    /// <summary>
    /// Rotation angle for coverflow effect (degrees).
    /// </summary>
    [Parameter]
    public int RotateDegree { get; set; } = 50;

    /// <summary>
    /// Depth of the coverflow effect.
    /// </summary>
    [Parameter]
    public int Depth { get; set; } = 150;

    /// <summary>
    /// Automatically detect and optimize for small item counts.
    /// </summary>
    [Parameter]
    public bool AutoDetectMode { get; set; } = true;

    /// <summary>
    /// Minimum items for coverflow effect. Below this uses simple slider.
    /// </summary>
    [Parameter]
    public int MinItemsForCoverflow { get; set; } = 4;

    /// <summary>
    /// Advanced carousel options. Overrides individual parameters if provided.
    /// </summary>
    [Parameter]
    public BzCarouselOptions? Options { get; set; }

    #endregion

    #region Injected Services

    [Inject]
    private IJSRuntime JS { get; set; } = default!;

    #endregion

    #region Private Fields

    private ElementReference carouselRef;
    private BzCarouselJsInterop? jsInterop;
    private bool initialized = false;

    /// <summary>
    /// Static cache for generated templates (shared across all instances).
    /// Thread-safe via ConcurrentDictionary.
    /// </summary>
    private static readonly ConcurrentDictionary<Type, RenderFragment<object>?>
        _generatedTemplateCache = new();

    /// <summary>
    /// Instance-level cache for effective template.
    /// </summary>
    private RenderFragment<TItem>? _cachedEffectiveTemplate;

    #endregion

    #region Properties

    private int ItemCount => Items?.Count() ?? 0;
    private bool IsLoading => Items == null;
    private bool IsEmpty => Items != null && !Items.Any();

    private bool ShouldEnableLoop => Loop && ItemCount >= MinItemsForLoop;

    private CarouselMode CurrentMode => AutoDetectMode
        ? (ItemCount < MinItemsForCoverflow ? CarouselMode.Simple : CarouselMode.Coverflow)
        : CarouselMode.Coverflow;

    private string ThemeClass => Theme switch
    {
        BzTheme.Glass => "bzc-theme-glass",
        BzTheme.Dark => "bzc-theme-dark",
        BzTheme.Light => "bzc-theme-light",
        BzTheme.Minimal => "bzc-theme-minimal",
        _ => "bzc-theme-glass"
    };

    private int SafeInitialSlide
    {
        get
        {
            if (ItemCount == 0) return 0;
            if (ShouldEnableLoop) return InitialSlide;
            return Math.Min(InitialSlide, ItemCount - 1);
        }
    }

    /// <summary>
    /// Gets the effective template with caching.
    /// Priority: Manual > Generated > Fallback
    /// </summary>
    private RenderFragment<TItem> EffectiveTemplate
    {
        get
        {
            if (_cachedEffectiveTemplate != null)
                return _cachedEffectiveTemplate;

            _cachedEffectiveTemplate = ComputeEffectiveTemplate();
            return _cachedEffectiveTemplate;
        }
    }

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Invalidate cache when ItemTemplate changes.
    /// </summary>
    protected override void OnParametersSet()
    {
        if (ItemTemplate != null)
        {
            _cachedEffectiveTemplate = null;
        }

        base.OnParametersSet();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !IsEmpty && !initialized)
        {
            try
            {
                jsInterop = new BzCarouselJsInterop(JS);

                var options = BuildOptions();

                await jsInterop.InitializeAsync(carouselRef, options);

                initialized = true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[BzCarousel] Error: {ex.Message}");
            }
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Builds Swiper configuration options based on component parameters.
    /// </summary>
    /// <returns>Configured BzCarouselOptions instance</returns>
    private BzCarouselOptions BuildOptions()
    {
        if (Options != null) return Options;

        return CurrentMode switch
        {
            CarouselMode.Simple => new BzCarouselOptions
            {
                Effect = "slide",
                SlidesPerView = Math.Min(ItemCount, 3).ToString(),
                CenteredSlides = true,
                SpaceBetween = 30,
                Loop = false,
                Speed = 300,
                GrabCursor = true
            },

            CarouselMode.Coverflow => new BzCarouselOptions
            {
                Effect = "coverflow",
                SlidesPerView = "auto",
                InitialSlide = SafeInitialSlide,
                CenteredSlides = true,
                Loop = ShouldEnableLoop,
                RotateDegree = RotateDegree,
                Depth = Depth,
                Modifier = ItemCount < 5 ? 1.0 : 1.5,
                Speed = 300,
                GrabCursor = true,
                SlideShadows = true
            },

            _ => throw new InvalidOperationException($"Unknown mode: {CurrentMode}")
        };
    }

    /// <summary>
    /// Handles item click events and invokes the OnItemSelected callback.
    /// Swiper's slideToClickedSlide handles navigation automatically.
    /// </summary>
    /// <param name="item">The clicked item</param>
    private async Task HandleItemClick(TItem item)
    {
        if (OnItemSelected.HasDelegate)
        {
            await OnItemSelected.InvokeAsync(item);
        }
    }

    /// <summary>
    /// Computes effective template using priority system.
    /// Priority: Manual > Generated > Fallback
    /// </summary>
    private RenderFragment<TItem> ComputeEffectiveTemplate()
    {
        // Priority 1: User-provided manual template
        if (ItemTemplate != null)
            return ItemTemplate;

        // Priority 2: Source-generated template
        var generated = TryGetGeneratedTemplateWithCache();
        if (generated != null)
            return generated;

        // Priority 3: Fallback template
        return FallbackTemplate;
    }

    /// <summary>
    /// Attempts to get source-generated template with caching.
    /// Uses reflection to find generated extension method.
    /// </summary>
    private RenderFragment<TItem>? TryGetGeneratedTemplateWithCache()
    {
        var itemType = typeof(TItem);

        if (_generatedTemplateCache.TryGetValue(itemType, out var cached))
        {
            return cached as RenderFragment<TItem>;
        }

        RenderFragment<TItem>? result = null;

        try
        {
            // Build extension class name: MovieBzCarouselExtensions
            var extensionsTypeName = $"{itemType.Namespace}.{itemType.Name}BzCarouselExtensions";

            // Try to find the type in same assembly as TItem
            var extensionsType = itemType.Assembly.GetType(extensionsTypeName);

            if (extensionsType != null)
            {
                // Find the static method
                var method = extensionsType.GetMethod(
                    "GetDefaultBzCarouselTemplate",
                    BindingFlags.Public | BindingFlags.Static);

                if (method != null)
                {
                    // Invoke static method to get RenderFragment
                    result = method.Invoke(null, null) as RenderFragment<TItem>;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BzCarousel] Source Generator template not found for {itemType.Name}: {ex.Message}");
        }

        _generatedTemplateCache.TryAdd(itemType, result as RenderFragment<object>);

        return result;
    }

    /// <summary>
    /// Fallback template when no manual template and no [BzImage] attribute.
    /// </summary>
    private RenderFragment<TItem> FallbackTemplate => item => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "bzc-fallback-item");
        builder.AddContent(2, item?.ToString() ?? "[null]");
        builder.CloseElement();
    };

    #endregion

    #region IAsyncDisposable

    public async ValueTask DisposeAsync()
    {
        if (jsInterop != null)
        {
            await jsInterop.DisposeAsync();
        }
    }

    #endregion

    #region Enums

    private enum CarouselMode
    {
        Simple,
        Coverflow
    }

    #endregion
}
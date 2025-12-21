using BlazzyMotion.Carousel.Models;
using BlazzyMotion.Carousel.Services;
using BlazzyMotion.Core.Abstractions;
using BlazzyMotion.Core.Models;
using BlazzyMotion.Core.Services;
using BlazzyMotion.Core.Templates;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazzyMotion.Carousel.Components;

/// <summary>
/// A 3D carousel component with glassmorphism design and coverflow effect.
/// </summary>
/// <typeparam name="TItem">The type of items to display in the carousel</typeparam>
/// <remarks>
/// <para>
/// BzCarousel provides a beautiful 3D carousel experience with:
/// <list type="bullet">
/// <item>Coverflow 3D effect powered by Swiper.js</item>
/// <item>Glassmorphism visual themes</item>
/// <item>Zero-config setup with [BzImage] attribute</item>
/// <item>Full customization via ItemTemplate</item>
/// </list>
/// </para>
/// <para>
/// <strong>Usage Options:</strong>
/// <list type="number">
/// <item>
/// <strong>Zero Config (recommended):</strong> Decorate your model with [BzImage]
/// <code>
/// public class Movie
/// {
///     [BzImage] public string PosterUrl { get; set; }
///     [BzTitle] public string Title { get; set; }
/// }
/// 
/// &lt;BzCarousel Items="movies" /&gt;
/// </code>
/// </item>
/// <item>
/// <strong>Custom Template:</strong> Provide your own ItemTemplate
/// <code>
/// &lt;BzCarousel Items="movies"&gt;
///     &lt;ItemTemplate&gt;
///         &lt;img src="@context.PosterUrl" alt="@context.Title" /&gt;
///     &lt;/ItemTemplate&gt;
/// &lt;/BzCarousel&gt;
/// </code>
/// </item>
/// </list>
/// </para>
/// </remarks>
public partial class BzCarousel<TItem> : BzComponentBase where TItem : class
{
    #region Parameters

    /// <summary>
    /// The collection of items to display in the carousel.
    /// </summary>
    [Parameter, EditorRequired]
    public IEnumerable<TItem>? Items { get; set; }

    /// <summary>
    /// Template for rendering each carousel item.
    /// If not provided, will use auto-generated template from [BzImage] attribute.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    /// <summary>
    /// Callback invoked when an item is clicked/selected.
    /// When <see cref="SelectOnScroll"/> is true, also fires on scroll/swipe.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> OnItemSelected { get; set; }

    /// <summary>
    /// Callback invoked when the active item changes due to scroll/swipe.
    /// Always fires on scroll regardless of <see cref="SelectOnScroll"/> setting.
    /// </summary>
    /// <remarks>
    /// Use this for lightweight operations like updating a preview panel.
    /// For navigation or API calls, prefer <see cref="OnItemSelected"/> with
    /// <see cref="SelectOnScroll"/> set to false.
    /// </remarks>
    [Parameter]
    public EventCallback<TItem> OnActiveItemChanged { get; set; }

    /// <summary>
    /// Callback invoked when the active slide index changes.
    /// Provides the zero-based index of the newly active slide.
    /// </summary>
    [Parameter]
    public EventCallback<int> OnActiveIndexChanged { get; set; }

    /// <summary>
    /// When true, <see cref="OnItemSelected"/> fires on scroll/swipe in addition to click.
    /// Default is true for zero-config behavior.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Set to false if <see cref="OnItemSelected"/> triggers navigation or API calls,
    /// to prevent unwanted side effects during scrolling.
    /// </para>
    /// <para>
    /// <strong>Example:</strong>
    /// <code>
    /// // For preview updates (keep default true):
    /// &lt;BzCarousel Items="movies" OnItemSelected="@(m => selectedMovie = m)" /&gt;
    /// 
    /// // For navigation (set to false):
    /// &lt;BzCarousel Items="movies" 
    ///              SelectOnScroll="false" 
    ///              OnItemSelected="@(m => Navigation.NavigateTo($"/movie/{m.Id}"))" /&gt;
    /// </code>
    /// </para>
    /// </remarks>
    [Parameter]
    public bool SelectOnScroll { get; set; } = true;

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
    public int MinItemsForLoop { get; set; } = 4;

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

    private ElementReference _carouselRef;
    private BzCarouselJsInterop? _jsInterop;
    private bool _initialized;
    private bool _needsReinit;
    private bool _isReinitializing;
    private int? _lastItemCount;
    private string? _lastOptionsSnapshot;
    private int? _lastKeyParamsHash;
    private DotNetObjectReference<BzCarousel<TItem>>? _dotNetRef;

    /// <summary>
    /// Cached items list for O(1) index access in JS callbacks.
    /// </summary>
    private IReadOnlyList<TItem> _itemsCache = Array.Empty<TItem>();

    /// <summary>
    /// Cached mapped items from BzRegistry.
    /// </summary>
    private IReadOnlyList<BzItem> _mappedItems = Array.Empty<BzItem>();

    /// <summary>
    /// Indicates whether we're using BzRegistry mapping or custom template.
    /// </summary>
    private bool _useRegistryMapping;

    #endregion

    #region Computed Properties

    private int ItemCount => Items?.Count() ?? 0;
    private bool IsLoading => Items == null;
    private bool IsEmpty => Items != null && !Items.Any();
    private bool ShouldEnableLoop => Loop && ItemCount >= MinItemsForLoop;

    private CarouselMode CurrentMode => AutoDetectMode
        ? (ItemCount < MinItemsForCoverflow ? CarouselMode.Simple : CarouselMode.Coverflow)
        : CarouselMode.Coverflow;

    private int SafeInitialSlide
    {
        get
        {
            if (ItemCount == 0) return 0;
            if (ShouldEnableLoop) return InitialSlide;
            return Math.Min(InitialSlide, ItemCount - 1);
        }
    }

    #endregion

    #region Lifecycle Methods

    /// <summary>
    /// Called when parameters are set. Maps items and detects changes.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        ValidateParameters();

        // Cache items for O(1) access in JS callbacks
        _itemsCache = Items != null ? Items.ToList() : Array.Empty<TItem>();

        // Determine rendering strategy
        if (ItemTemplate != null)
        {
            _useRegistryMapping = false;
            _mappedItems = Array.Empty<BzItem>();
        }
        else if (Items != null && BzRegistry.HasMapper<TItem>())
        {
            _useRegistryMapping = true;
            _mappedItems = BzRegistry.ToBzItems(Items);
        }
        else
        {
            _useRegistryMapping = false;
            _mappedItems = Array.Empty<BzItem>();
        }

        await DetectChangesAsync();
        await base.OnParametersSetAsync();
    }

    /// <summary>
    /// Called after render. Initializes or reinitializes Swiper.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (IsDisposed) return;

        if (!IsEmpty && !_initialized)
        {
            _initialized = true;
            try
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                _jsInterop ??= new BzCarouselJsInterop(JS);
                await _jsInterop.InitializeAsync(_carouselRef, BuildOptions(), _dotNetRef);
                _initialized = true;
            }
            catch (Exception ex)
            {
                _initialized = false;
                Console.Error.WriteLine($"[BzCarousel] Initialization error: {ex.Message}");
            }
        }
        else if (_initialized && _needsReinit && !_isReinitializing)
        {
            _isReinitializing = true;
            try
            {
                await _jsInterop!.DestroyAsync();
                _dotNetRef?.Dispose();
                _dotNetRef = DotNetObjectReference.Create(this);
                await _jsInterop.InitializeAsync(_carouselRef, BuildOptions(), _dotNetRef);
                _needsReinit = false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[BzCarousel] Reinitialization error: {ex.Message}");
            }
            finally
            {
                _isReinitializing = false;
            }
        }
    }

    #endregion

    #region JS Invokable Methods

    /// <summary>
    /// Called from JavaScript when the active slide changes.
    /// </summary>
    /// <param name="realIndex">The real index of the active slide (accounts for loop clones)</param>
    [JSInvokable]
    public async Task OnSlideChangeFromJS(int realIndex)
    {
        if (IsDisposed) return;
        if (realIndex < 0 || realIndex >= _itemsCache.Count) return;

        var item = _itemsCache[realIndex];

        // Always fire OnActiveIndexChanged
        if (OnActiveIndexChanged.HasDelegate)
        {
            await OnActiveIndexChanged.InvokeAsync(realIndex);
        }

        // Always fire OnActiveItemChanged
        if (OnActiveItemChanged.HasDelegate)
        {
            await OnActiveItemChanged.InvokeAsync(item);
        }

        // Fire OnItemSelected only if SelectOnScroll is true
        if (SelectOnScroll && OnItemSelected.HasDelegate)
        {
            await OnItemSelected.InvokeAsync(item);
        }

        await InvokeStateHasChangedAsync();
    }

    #endregion

    #region Rendering Methods

    /// <summary>
    /// Gets the RenderFragment for a specific item.
    /// </summary>
    private RenderFragment GetItemContent(TItem item, int index)
    {
        if (ItemTemplate != null)
        {
            return ItemTemplate(item);
        }

        if (_useRegistryMapping && index < _mappedItems.Count)
        {
            var bzItem = _mappedItems[index];
            return BzTemplateFactory.CreateImage()(bzItem);
        }

        return BzTemplateFactory.CreateGenericFallback<TItem>()(item);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Builds Swiper configuration options.
    /// </summary>
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
    /// Handles item click events.
    /// </summary>
    private async Task HandleItemClick(TItem item)
    {
        if (OnItemSelected.HasDelegate)
        {
            await OnItemSelected.InvokeAsync(item);
        }
    }

    /// <summary>
    /// Detects changes that require Swiper reinitialization.
    /// </summary>
    private async Task DetectChangesAsync()
    {
        var currentItemCount = ItemCount;
        var optionsSnapshot = Options != null ? JsonSerializer.Serialize(Options) : null;
        var keyParamsHash = HashCode.Combine(Loop, RotateDegree, Depth, MinItemsForLoop, MinItemsForCoverflow, SelectOnScroll);

        if (currentItemCount == 0)
        {
            await DestroyInternalAsync();
        }

        if (_lastItemCount.HasValue || _lastOptionsSnapshot != null || _lastKeyParamsHash.HasValue)
        {
            var itemCountChanged = _lastItemCount != currentItemCount;
            var optionsChanged = _lastOptionsSnapshot != optionsSnapshot;
            var keyParamsChanged = _lastKeyParamsHash != keyParamsHash;

            if ((itemCountChanged || optionsChanged || keyParamsChanged) && _initialized)
            {
                _needsReinit = true;
            }
        }

        _lastItemCount = currentItemCount;
        _lastOptionsSnapshot = optionsSnapshot;
        _lastKeyParamsHash = keyParamsHash;
    }

    /// <summary>
    /// Destroys the Swiper instance.
    /// </summary>
    private async Task DestroyInternalAsync()
    {
        if (_jsInterop != null)
        {
            await _jsInterop.DestroyAsync();
        }
        _initialized = false;
        _needsReinit = false;
    }

    /// <summary>
    /// Validates component parameters.
    /// </summary>
    private void ValidateParameters()
    {
        if (RotateDegree < 0 || RotateDegree > 360)
            throw new ArgumentOutOfRangeException(nameof(RotateDegree), "RotateDegree must be between 0 and 360 degrees.");

        if (Depth < 0)
            throw new ArgumentOutOfRangeException(nameof(Depth), "Depth must be a non-negative value.");

        if (MinItemsForLoop < 1)
            throw new ArgumentOutOfRangeException(nameof(MinItemsForLoop), "MinItemsForLoop must be at least 1.");

        if (MinItemsForCoverflow < 1)
            throw new ArgumentOutOfRangeException(nameof(MinItemsForCoverflow), "MinItemsForCoverflow must be at least 1.");

        if (InitialSlide < 0)
            throw new ArgumentOutOfRangeException(nameof(InitialSlide), "InitialSlide must be a non-negative value.");
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Disposes component resources.
    /// </summary>
    protected override async ValueTask DisposeAsyncCore()
    {
        _dotNetRef?.Dispose();

        if (_jsInterop != null)
        {
            await _jsInterop.DisposeAsync();
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

namespace BlazzyMotion.Carousel.Models;

/// <summary>
/// Configuration options for the BlazzyCarousel component.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Usage:</strong>
/// <code>
/// var options = new BzCarouselOptions
/// {
///     TouchRatio = 0.8,
///     Threshold = 10,
///     ShortSwipes = false
/// };
/// 
/// &lt;BzCarousel Items="movies" Options="options" /&gt;
/// </code>
/// </para>
/// </remarks>
public class BzCarouselOptions
{
    #region Effect Options

    /// <summary>
    /// Swiper effect type: "slide", "coverflow", "fade", etc.
    /// </summary>
    public string Effect { get; set; } = "coverflow";

    /// <summary>
    /// Number of slides per view. Can be "auto" or a number.
    /// </summary>
    public string SlidesPerView { get; set; } = "auto";

    /// <summary>
    /// Index of the initially active slide.
    /// </summary>
    public int InitialSlide { get; set; } = 0;

    /// <summary>
    /// Whether slides should be centered.
    /// </summary>
    public bool CenteredSlides { get; set; } = true;

    /// <summary>
    /// Enable continuous loop mode.
    /// </summary>
    public bool Loop { get; set; } = true;

    /// <summary>
    /// Space between slides in pixels.
    /// </summary>
    public int SpaceBetween { get; set; } = 0;

    /// <summary>
    /// Transition speed in milliseconds.
    /// </summary>
    public int Speed { get; set; } = 300;

    /// <summary>
    /// Enable grab cursor.
    /// </summary>
    public bool GrabCursor { get; set; } = true;

    #endregion

    #region Coverflow Effect Options

    /// <summary>
    /// Rotation angle for coverflow effect (degrees).
    /// </summary>
    public int RotateDegree { get; set; } = 50;

    /// <summary>
    /// Depth of coverflow effect.
    /// </summary>
    public int Depth { get; set; } = 150;

    /// <summary>
    /// Stretch space between slides in coverflow (pixels).
    /// </summary>
    public int Stretch { get; set; } = 0;

    /// <summary>
    /// Coverflow modifier value.
    /// </summary>
    public double Modifier { get; set; } = 1.5;

    /// <summary>
    /// Enable slide shadows in coverflow.
    /// </summary>
    public bool SlideShadows { get; set; } = true;

    #endregion

    // ═══════════════════════════════════════════════════════════════════
    // TOUCH / MOBILE OPTIONS
    // ═══════════════════════════════════════════════════════════════════
    #region Touch/Mobile Options

    /// <summary>
    /// Touch sensitivity multiplier (0.1 - 2.0).
    /// </summary>
    /// <remarks>
    /// Default: 1.0 (normal sensitivity)
    /// </remarks>
    public double TouchRatio { get; set; } = 1.0;

    /// <summary>
    /// Minimum pixels to trigger swipe.
    /// </summary>
    /// <remarks>
    /// Higher values require more deliberate swipe gesture.
    /// Default: 10 (prevents accidental micro-swipes)
    /// </remarks>
    public int Threshold { get; set; } = 10;

    /// <summary>
    /// Allow quick flick gestures.
    /// </summary>
    /// <remarks>
    /// When false, prevents glitchy behavior from rapid successive swipes.
    /// Default: false
    /// </remarks>
    public bool ShortSwipes { get; set; } = false;

    /// <summary>
    /// Edge resistance ratio (0 - 1).
    /// </summary>
    /// <remarks>
    /// Controls bounce resistance at carousel edges.
    /// Default: 0.85
    /// </remarks>
    public double ResistanceRatio { get; set; } = 0.85;

    /// <summary>
    /// Percentage of slide width to trigger advance (0.1 - 0.9).
    /// </summary>
    /// <remarks>
    /// How far you must swipe to move to next slide.
    /// Default: 0.3 (30% of slide width)
    /// </remarks>
    public double LongSwipesRatio { get; set; } = 0.3;

    #endregion
}

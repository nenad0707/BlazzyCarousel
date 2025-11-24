namespace BlazzyMotion.Carousel.Models;

/// <summary>
/// Configuration options for the BlazzyCarousel component.
/// </summary>
public class BzCarouselOptions
{
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
}
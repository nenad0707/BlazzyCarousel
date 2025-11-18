namespace BlazzyCarousel.Tests.Helpers;

/// <summary>
/// Test model with BzImage attribute for testing carousel functionality
/// </summary>
public class TestMovie
{
    [BzImage]
    public string? ImageUrl { get; set; }

    [BzTitle]
    public string? Title { get; set; }

    [BzDescription]
    public string? Description { get; set; }

    public int Year { get; set; }

    public string? Director { get; set; }
}

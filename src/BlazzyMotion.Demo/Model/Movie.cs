using BlazzyMotion.Carousel.Attributes;

namespace BlazzyMotion.Demo.Model;

public class Movie
{
    public string? Title { get; set; }

    [BzImage]
    public string? ImageUrl { get; set; }
}

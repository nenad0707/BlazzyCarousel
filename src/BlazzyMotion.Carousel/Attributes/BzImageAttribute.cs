namespace BlazzyMotion.Carousel.Attributes;

/// <summary>
/// Marks a property as the image source for carousel items.
/// The Source Generator will automatically create a default ItemTemplate using this property.
/// </summary>
/// <remarks>
/// Apply this attribute to a string property that contains the image URL/path.
/// Only one property per class should have this attribute.
/// </remarks>
/// <example>
/// <code>
/// public class Movie
/// {
///     [BzImage]
///     public string Poster { get; set; } = "";
///     
///     public string Title { get; set; } = "";
/// }
/// 
/// // Usage in component:
/// &lt;BzCarousel Items="movies" /&gt;  // No ItemTemplate needed!
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class BzImageAttribute : Attribute
{
    // Empty - marker attribute only
    // Source Generator will scan for this at compile-time
}
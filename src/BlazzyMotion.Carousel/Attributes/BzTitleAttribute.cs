namespace BlazzyMotion.Carousel.Attributes;

/// <summary>
/// Marks a property as the title/alt text for carousel items.
/// Used for accessibility - generates alt and title attributes on img elements.
/// </summary>
/// <remarks>
/// Optional attribute. If not provided, generated template will use generic alt text.
/// </remarks>
/// <example>
/// <code>
/// public class Movie
/// {
///     [BzImage]
///     public string Poster { get; set; } = "";
///     
///     [BzTitle]  // ← This will be used for alt="..." and title="..."
///     public string Title { get; set; } = "";
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class BzTitleAttribute : Attribute
{
    // Marker attribute for alt/title text
}
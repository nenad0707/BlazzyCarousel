namespace BlazzyCarousel.Attributes;

/// <summary>
/// Marks a property as the description for carousel items.
/// Currently reserved for future use (tooltips, captions, etc.).
/// </summary>
/// <remarks>
/// This attribute is not currently used by the default template generator,
/// but is available for custom implementations and future features.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class BzDescriptionAttribute : Attribute
{
    // Reserved for future use
    // Could be used for: tooltips, captions, aria-describedby, etc.
}
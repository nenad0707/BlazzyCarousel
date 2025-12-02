# BlazzyMotion.Carousel

A modern, high-performance 3D carousel component for Blazor with zero-configuration support through Source Generators.

[![NuGet](https://img.shields.io/nuget/v/BlazzyMotion.Carousel.svg)](https://www.nuget.org/packages/BlazzyMotion.Carousel/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazzyMotion.Carousel.svg)](https://www.nuget.org/packages/BlazzyMotion.Carousel/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)

## Table of Contents

- [Features](#features)
- [Live Demo](#live-demo)
- [Quick Start](#quick-start)
  - [Installation](#installation)
  - [Basic Usage](#basic-usage)
- [How It Works](#how-it-works)
- [API Reference](#api-reference)
  - [Component Parameters](#component-parameters)
- [Attributes](#attributes)
- [Themes](#themes)
- [Advanced Usage](#advanced-usage)
- [Customization](#customization)
- [Responsive Design](#responsive-design)
- [Performance](#performance)
- [Browser Support](#browser-support)
- [Examples](#examples)
- [Validation and Diagnostics](#validation-and-diagnostics)
- [Troubleshooting](#troubleshooting)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Author](#author)
- [Acknowledgments](#acknowledgments)
- [Support](#support)

## Features

- **Zero Configuration**: Use Source Generators to automatically create item templates from your data models
- **3D Coverflow Effect**: Stunning visual presentation powered by Swiper.js
- **Multiple Themes**: Glass, Dark, Light, and Minimal themes included out of the box
- **Adaptive Modes**: Automatically adjusts between coverflow and simple slider based on item count
- **Fully Customizable**: Extensive API for fine-tuning appearance and behavior
- **Type-Safe**: Strongly-typed generic component with full IntelliSense support
- **Responsive Design**: Built-in responsive behavior for desktop, tablet, and mobile devices
- **Performance Optimized**: Template caching and incremental source generation for minimal overhead

## Live Demo

Experience BlazzyMotion.Carousel in action: **[View Live Demo](https://blazzy-motion.github.io/BlazzyMotion/)**

![BlazzyMotion.Carousel Demo](https://raw.githubusercontent.com/Blazzy-Motion/BlazzyMotion/main/docs/images/demo.gif)

## Quick Start

### Installation

Install the package via NuGet:

```bash
dotnet add package BlazzyMotion.Carousel
```

Or via the Package Manager Console:

```powershell
Install-Package BlazzyMotion.Carousel
```

### Basic Usage

#### 1. Define Your Model

Mark your data model with the `[BzImage]` attribute to specify which property contains the image URL:

```csharp
using BlazzyMotion.Carousel.Attributes;

public class Movie
{
    [BzImage]
    public string Poster { get; set; } = "";

    [BzTitle]  // Optional: for accessibility (alt text)
    public string Title { get; set; } = "";
}
```

#### 2. Use the Component

That's it! No need to define an `ItemTemplate`:

```razor
@page "/movies"
@using BlazzyMotion.Carousel.Components

<BzCarousel Items="movies" />

@code {
    private List<Movie> movies = new()
    {
        new Movie { Poster = "/images/movie1.jpg", Title = "Inception" },
        new Movie { Poster = "/images/movie2.jpg", Title = "Interstellar" },
        new Movie { Poster = "/images/movie3.jpg", Title = "The Dark Knight" }
    };
}
```

The Source Generator automatically creates the template for you at compile-time.

## How It Works

### Source Generator Magic

When you mark a property with `[BzImage]`, the BlazzyMotion.Carousel Source Generator automatically creates an extension method during compilation:

```csharp
// Auto-generated at compile-time
public static class MovieBzCarouselExtensions
{
    public static RenderFragment<Movie> GetDefaultBzCarouselTemplate()
    {
        return item => builder =>
        {
            if (item is null || string.IsNullOrWhiteSpace(item.Poster))
                return;

            builder.OpenElement(0, "img");
            builder.AddAttribute(1, "src", item.Poster);
            builder.AddAttribute(2, "alt", item.Title);
            builder.AddAttribute(3, "title", item.Title);
            builder.CloseElement();
        };
    }
}
```

BlazzyMotion.Carousel discovers this method via reflection and uses it automatically. This happens only once per type and is cached for optimal performance.

### Template Priority

The component uses the following priority when selecting a template:

1. **Manual Template** - `ItemTemplate` parameter if provided
2. **Generated Template** - Auto-generated from `[BzImage]` attribute
3. **Fallback Template** - Simple text representation

## API Reference

### Component Parameters

#### Data Parameters

| Parameter        | Type                     | Default      | Description                                                        |
| ---------------- | ------------------------ | ------------ | ------------------------------------------------------------------ |
| `Items`          | `IEnumerable<TItem>`     | **Required** | Collection of items to display in the carousel                     |
| `ItemTemplate`   | `RenderFragment<TItem>?` | `null`       | Custom template for rendering items (overrides generated template) |
| `OnItemSelected` | `EventCallback<TItem>`   | -            | Callback invoked when an item is clicked                           |

#### Appearance Parameters

| Parameter     | Type      | Default | Description                                          |
| ------------- | --------- | ------- | ---------------------------------------------------- |
| `Theme`       | `BzTheme` | `Glass` | Visual theme: `Glass`, `Dark`, `Light`, or `Minimal` |
| `ShowOverlay` | `bool`    | `true`  | Whether to show gradient overlay on slides           |
| `CssClass`    | `string?` | `null`  | Additional CSS classes for customization             |

#### Behavior Parameters

| Parameter              | Type   | Default | Description                                                   |
| ---------------------- | ------ | ------- | ------------------------------------------------------------- |
| `Loop`                 | `bool` | `true`  | Enable infinite loop navigation                               |
| `InitialSlide`         | `int`  | `0`     | Index of the initially active slide                           |
| `MinItemsForLoop`      | `int`  | `4`     | Minimum items required to enable loop mode                    |
| `AutoDetectMode`       | `bool` | `true`  | Automatically switch between coverflow and simple modes       |
| `MinItemsForCoverflow` | `int`  | `4`     | Minimum items for coverflow effect (below uses simple slider) |

#### Effect Parameters

| Parameter      | Type  | Default | Description                                   |
| -------------- | ----- | ------- | --------------------------------------------- |
| `RotateDegree` | `int` | `50`    | Rotation angle for coverflow effect (degrees) |
| `Depth`        | `int` | `150`   | Depth of the coverflow effect                 |

#### Advanced Parameters

| Parameter         | Type                 | Default | Description                                                 |
| ----------------- | -------------------- | ------- | ----------------------------------------------------------- |
| `Options`         | `BzCarouselOptions?` | `null`  | Advanced carousel options (overrides individual parameters) |
| `LoadingTemplate` | `RenderFragment?`    | `null`  | Custom loading state template                               |
| `EmptyTemplate`   | `RenderFragment?`    | `null`  | Custom empty state template                                 |

## Attributes

### BzImageAttribute

Marks a property as the image source for carousel items. The Source Generator creates a default template using this property.

**Requirements:**

- Property must be `public`
- Property must be of type `string`
- Only one property per class should have this attribute

```csharp
[BzImage]
public string ImageUrl { get; set; }
```

### BzTitleAttribute

Marks a property as the title/alt text for carousel items. Used for accessibility attributes on generated img elements.

```csharp
[BzTitle]
public string Name { get; set; }
```

### BzDescriptionAttribute

Reserved for future use (tooltips, captions, etc.). Currently not used by the default template generator.

```csharp
[BzDescription]
public string Description { get; set; }
```

## Themes

BlazzyMotion.Carousel includes four professionally designed themes:

### Glass Theme (Default)

Modern glassmorphism design with blur effect and transparency:

```razor
<BzCarousel Items="items" Theme="BzTheme.Glass" />
```

### Dark Theme

Solid dark background with subtle gradients:

```razor
<BzCarousel Items="items" Theme="BzTheme.Dark" />
```

### Light Theme

Clean light theme with soft shadows:

```razor
<BzCarousel Items="items" Theme="BzTheme.Light" />
```

### Minimal Theme

No background container, pure carousel:

```razor
<BzCarousel Items="items" Theme="BzTheme.Minimal" />
```

## Advanced Usage

### Custom Item Template

If you need full control over item rendering, provide a custom template:

```razor
<BzCarousel Items="products">
    <ItemTemplate Context="product">
        <div class="custom-slide">
            <img src="@product.Image" alt="@product.Name" />
            <h3>@product.Name</h3>
            <p class="price">$@product.Price</p>
        </div>
    </ItemTemplate>
</BzCarousel>
```

### Handling Item Selection

React to user clicks on carousel items:

```razor
<BzCarousel Items="movies" OnItemSelected="HandleMovieClick" />

@code {
    private void HandleMovieClick(Movie movie)
    {
        Console.WriteLine($"Selected: {movie.Title}");
        // Navigate, open modal, etc.
    }
}
```

### Advanced Configuration

Use `BzCarouselOptions` for fine-grained control:

```razor
<BzCarousel Items="items" Options="customOptions" />

@code {
    private BzCarouselOptions customOptions = new()
    {
        Effect = "coverflow",
        SlidesPerView = "auto",
        Loop = true,
        Speed = 500,
        RotateDegree = 45,
        Depth = 200,
        Modifier = 1.5,
        SlideShadows = true
    };
}
```

### Custom Loading State

Provide a custom loading template:

```razor
<BzCarousel Items="asyncItems">
    <LoadingTemplate>
        <div class="custom-loader">
            <span>Loading amazing content...</span>
        </div>
    </LoadingTemplate>
</BzCarousel>
```

### Custom Empty State

Handle empty data gracefully:

```razor
<BzCarousel Items="emptyList">
    <EmptyTemplate>
        <div class="no-data">
            <p>No items available at this time.</p>
        </div>
    </EmptyTemplate>
</BzCarousel>
```

## Customization

### CSS Variables

BlazzyMotion.Carousel uses CSS custom properties for easy customization:

```css
:root {
  /* Dimensions */
  --bzc-slide-width: 180px;
  --bzc-image-max-width: 160px;
  --bzc-swiper-height: 400px;

  /* Effects */
  --bzc-hover-scale: 1.05;
  --bzc-active-scale: 1.1;
  --bzc-transition-duration: 0.3s;

  /* Theme Colors */
  --bzc-glass-bg: rgba(15, 15, 15, 0.6);
  --bzc-glass-border: rgba(255, 255, 255, 0.15);
  --bzc-glass-blur: 12px;
}
```

Override these in your app's CSS:

```css
.my-custom-carousel {
  --bzc-slide-width: 250px;
  --bzc-hover-scale: 1.08;
  --bzc-glass-blur: 20px;
}
```

```razor
<BzCarousel Items="items" CssClass="my-custom-carousel" />
```

## Responsive Design

BlazzyMotion.Carousel automatically adapts to different screen sizes:

- **Desktop** (> 991px): Full-size slides with maximum effects
- **Tablet** (600-991px): Medium-sized slides
- **Mobile** (< 600px): Compact slides optimized for touch

You can override responsive behavior via CSS variables in media queries.

## Performance

### Source Generator Benefits

- **Zero Runtime Overhead**: Templates are generated at compile-time
- **Type Safety**: Full compile-time checking of property names and types
- **IntelliSense Support**: Auto-completion for all generated code

### Caching Strategy

BlazzyMotion.Carousel implements a two-tier caching system:

1. **Static Cache**: Generated templates are cached globally per type
2. **Instance Cache**: Effective template is cached per component instance

This ensures minimal reflection overhead and optimal rendering performance.

## Browser Support

BlazzyMotion.Carousel supports all modern browsers:

- Chrome/Edge (90+)
- Firefox (88+)
- Safari (14+)
- Mobile browsers (iOS Safari, Chrome Mobile)

Requires CSS backdrop-filter support for Glass theme (gracefully degrades on older browsers).

## Examples

### Movie Gallery

```csharp
public class Movie
{
    [BzImage]
    public string Poster { get; set; } = "";

    [BzTitle]
    public string Title { get; set; } = "";

    public int Year { get; set; }
}
```

```razor
<BzCarousel Items="movies"
            Theme="BzTheme.Glass"
            OnItemSelected="ViewMovieDetails" />
```

### Product Showcase

```csharp
public class Product
{
    [BzImage]
    public string ImageUrl { get; set; } = "";

    [BzTitle]
    public string Name { get; set; } = "";

    public decimal Price { get; set; }
}
```

```razor
<BzCarousel Items="products"
            Theme="BzTheme.Light"
            RotateDegree="30"
            Depth="100" />
```

### Team Members

```csharp
public class TeamMember
{
    [BzImage]
    public string Photo { get; set; } = "";

    [BzTitle]
    public string Name { get; set; } = "";

    public string Position { get; set; } = "";
}
```

```razor
<BzCarousel Items="team"
            Theme="BzTheme.Minimal"
            ShowOverlay="false" />
```

## Validation and Diagnostics

The Source Generator provides compile-time validation:

### BZC001: Non-Public Property

```text
Error BZC001: Property 'ImageUrl' with [BzImage] attribute must be public
```

**Fix**: Change property accessibility to `public`.

### BZC002: Non-String Property

```text
Error BZC002: Property 'ImageData' with [BzImage] attribute must be of type 'string'
```

**Fix**: Ensure the property is of type `string` (URL or path).

## Troubleshooting

### Template Not Generated

**Problem**: Component shows fallback template instead of generated one.

**Solution**:

1. Ensure `[BzImage]` attribute is applied to a `public string` property
2. Rebuild the project to trigger Source Generator
3. Check build output for any BZC001 or BZC002 errors

### Carousel Not Visible

**Problem**: Carousel container is present but slides are not visible.

**Solution**:

1. Check that `Items` collection is not null or empty
2. Verify image URLs are valid and accessible
3. Inspect browser console for JavaScript errors

## Roadmap

Planned features for future releases:

- **Navigation Controls**: Previous/Next buttons and pagination indicators
- **Autoplay Mode**: Automatic slide progression with configurable intervals
- **Keyboard Navigation**: Arrow key support for accessibility
- **ARIA Attributes**: Enhanced screen reader support
- **Lazy Loading**: On-demand image loading for large datasets
- **Virtual Scrolling**: Performance optimization for 1000+ items

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

### Building from Source

```bash
git clone https://github.com/Blazzy-Motion/BlazzyMotion.git
cd BlazzyMotion
dotnet build
```

### Running Tests

```bash
dotnet test
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE.txt) file for details.

## Author

- GitHub: [@nenad0707](https://github.com/nenad0707)
- LinkedIn: [Nenad Ristić](https://www.linkedin.com/in/nenad-risti%C4%87-27459958/)

## Acknowledgments

- Built with [Swiper.js](https://swiperjs.com/) for 3D carousel effects
- Inspired by modern UI/UX design principles and glassmorphism trends
- Thanks to the Blazor community for feedback and support

## Support

If you find BlazzyMotion.Carousel useful, please consider:

- Giving it a star on GitHub
- Sharing it with other Blazor developers
- Reporting bugs or suggesting features via GitHub Issues

For questions or support, please open an issue on GitHub.

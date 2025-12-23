# BlazzyMotion.Carousel

A modern, high-performance 3D carousel component for Blazor with zero-configuration support through Source Generators.

[![NuGet](https://img.shields.io/nuget/v/BlazzyMotion.Carousel.svg)](https://www.nuget.org/packages/BlazzyMotion.Carousel/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazzyMotion.Carousel.svg)](https://www.nuget.org/packages/BlazzyMotion.Carousel/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Blazzy-Motion_BlazzyMotion&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Blazzy-Motion_BlazzyMotion)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Blazzy-Motion_BlazzyMotion&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Blazzy-Motion_BlazzyMotion)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Blazzy-Motion_BlazzyMotion&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Blazzy-Motion_BlazzyMotion)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Blazzy-Motion_BlazzyMotion&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Blazzy-Motion_BlazzyMotion)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Blazzy-Motion_BlazzyMotion&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Blazzy-Motion_BlazzyMotion)

## ⚠️ Migration Guide

### Upgrading from v1.0.x to v1.1.x

Version 1.1.0 introduces a modular architecture with `BlazzyMotion.Core` as shared infrastructure. This brings one breaking change:

#### BzTheme Namespace Change

**Before (v1.0.x):**

```csharp
@using BlazzyMotion.Carousel.Models

<BzCarousel Items="movies" Theme="BzTheme.Glass" />
```

**After (v1.1.x):**

```csharp
@using BlazzyMotion.Core.Models

<BzCarousel Items="movies" Theme="BzTheme.Glass" />
```

#### Attributes (No Change Required)

Both namespaces work for backward compatibility:

```csharp
// ✅ Old namespace (still works)
using BlazzyMotion.Carousel.Attributes;

// ✅ New namespace (recommended)
using BlazzyMotion.Core.Attributes;
```

#### Quick Fix

If you see compiler errors after upgrading, simply update your `_Imports.razor`:

```diff
+ @using BlazzyMotion.Core.Models
  @using BlazzyMotion.Carousel.Components
```

---

## Table of Contents

- [BlazzyMotion.Carousel](#blazzymotioncarousel)
  - [⚠️ Migration Guide](#️-migration-guide)
    - [Upgrading from v1.0.x to v1.1.x](#upgrading-from-v10x-to-v11x)
      - [BzTheme Namespace Change](#bztheme-namespace-change)
      - [Attributes (No Change Required)](#attributes-no-change-required)
      - [Quick Fix](#quick-fix)
  - [Table of Contents](#table-of-contents)
  - [Features](#features)
  - [Live Demo](#live-demo)
  - [Quick Start](#quick-start)
    - [Installation](#installation)
    - [Basic Usage](#basic-usage)
      - [1. Define Your Model](#1-define-your-model)
      - [2. Use the Component](#2-use-the-component)
  - [How It Works](#how-it-works)
    - [Source Generator Magic](#source-generator-magic)
    - [Template Priority](#template-priority)
  - [API Reference](#api-reference)
    - [Component Parameters](#component-parameters)
      - [Data Parameters](#data-parameters)
      - [Appearance Parameters](#appearance-parameters)
      - [Behavior Parameters](#behavior-parameters)
      - [Effect Parameters](#effect-parameters)
      - [Advanced Parameters](#advanced-parameters)
  - [Attributes](#attributes)
    - [BzImageAttribute](#bzimageattribute)
    - [BzTitleAttribute](#bztitleattribute)
    - [BzDescriptionAttribute](#bzdescriptionattribute)
  - [Themes](#themes)
    - [Glass Theme (Default)](#glass-theme-default)
    - [Dark Theme](#dark-theme)
    - [Light Theme](#light-theme)
    - [Minimal Theme](#minimal-theme)
  - [Mobile Optimization](#mobile-optimization)
    - [Customizing Touch Behavior](#customizing-touch-behavior)
    - [Touch Options Reference](#touch-options-reference)
  - [Advanced Usage](#advanced-usage)
    - [Custom Item Template](#custom-item-template)
    - [Handling Item Selection](#handling-item-selection)
    - [Preview Panel Pattern](#preview-panel-pattern)
    - [Advanced Configuration](#advanced-configuration)
    - [Custom Loading State](#custom-loading-state)
    - [Custom Empty State](#custom-empty-state)
  - [Customization](#customization)
    - [CSS Variables](#css-variables)
  - [Responsive Design](#responsive-design)
  - [Performance](#performance)
    - [Performance Characteristics](#performance-characteristics)
  - [Browser Support](#browser-support)
  - [Examples](#examples)
    - [Movie Gallery](#movie-gallery)
    - [Product Showcase](#product-showcase)
    - [Team Members](#team-members)
  - [Validation and Diagnostics](#validation-and-diagnostics)
    - [BZC001: Non-Public Property](#bzc001-non-public-property)
    - [BZC002: Non-String Property](#bzc002-non-string-property)
  - [Troubleshooting](#troubleshooting)
    - [Template Not Generated](#template-not-generated)
    - [Carousel Not Visible](#carousel-not-visible)
  - [Roadmap](#roadmap)
  - [Contributing](#contributing)
    - [Building from Source](#building-from-source)
    - [Running Tests](#running-tests)
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

Experience BlazzyMotion.Carousel in action: **[View Live Demo](https://blazzymotion.com/)**

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

When you mark a property with `[BzImage]`, the BlazzyMotion Source Generator automatically creates a registration function during compilation:

```csharp
// Auto-generated at compile-time
internal static class BzMappingRegistration_Movie
{
    [ModuleInitializer]
    internal static void Register()
    {
        BzRegistry.Register<Movie>(item => new BzItem
        {
            ImageUrl = item.Poster,
            Title = item.Title,
            OriginalItem = item
        });
    }
}
```

The `[ModuleInitializer]` attribute ensures this registration runs automatically at application startup, before any of your code executes. BlazzyMotion.Carousel then uses the registered mapper from `BzRegistry` to render your items - with zero reflection and zero configuration.

### Template Priority

The component uses the following priority when selecting a template:

1. **Manual Template** - `ItemTemplate` parameter if provided
2. **Generated Template** - Auto-generated from `[BzImage]` attribute
3. **Fallback Template** - Simple text representation

## API Reference

### Component Parameters

#### Data Parameters

| Parameter              | Type                     | Default      | Description                                                        |
| ---------------------- | ------------------------ | ------------ | ------------------------------------------------------------------ |
| `Items`                | `IEnumerable<TItem>`     | **Required** | Collection of items to display in the carousel                     |
| `ItemTemplate`         | `RenderFragment<TItem>?` | `null`       | Custom template for rendering items (overrides generated template) |
| `OnItemSelected`       | `EventCallback<TItem>`   | -            | Callback invoked when an item is clicked                           |
| `OnActiveItemChanged`  | `EventCallback<TItem>`   | -            | Callback fired on every scroll/swipe, ideal for preview panels     |
| `OnActiveIndexChanged` | `EventCallback<int>`     | -            | Callback fired with zero-based slide index on change               |

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
| `SelectOnScroll`       | `bool` | `true`  | When false, OnItemSelected fires only on click, not on scroll |
| `AutoDetectMode`       | `bool` | `true`  | Automatically switch between coverflow and simple modes       |
| `MinItemsForCoverflow` | `int`  | `4`     | Minimum items for coverflow effect (below uses simple slider) |

#### Effect Parameters

| Parameter      | Type  | Default | Description                                   |
| -------------- | ----- | ------- | --------------------------------------------- |
| `RotateDegree` | `int` | `50`    | Rotation angle for coverflow effect (degrees) |
| `Depth`        | `int` | `150`   | Depth of the coverflow effect                 |

#### Advanced Parameters

| Parameter              | Type                          | Default | Description                                                     |
| ---------------------- | ----------------------------- | ------- | --------------------------------------------------------------- |
| `Options`              | `BzCarouselOptions?`          | `null`  | Advanced carousel options (overrides individual parameters)     |
| `LoadingTemplate`      | `RenderFragment?`             | `null`  | Custom loading state template                                   |
| `EmptyTemplate`        | `RenderFragment?`             | `null`  | Custom empty state template                                     |
| `AdditionalAttributes` | `Dictionary<string, object>?` | `null`  | HTML attributes to splat onto root element (id, aria-_, data-_) |

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

## Mobile Optimization

BlazzyMotion.Carousel includes mobile-optimized touch settings out of the box:

```csharp
// These are the defaults - no configuration needed!
var options = new BzCarouselOptions
{
    TouchRatio = 1.0,        // Normal touch sensitivity
    Threshold = 10,          // Minimum 10px movement to trigger swipe
    ShortSwipes = false,     // Disabled to prevent glitchy behavior
    ResistanceRatio = 0.85,  // Light resistance at edges
    LongSwipesRatio = 0.3    // 30% slide width to advance
};
```

### Customizing Touch Behavior

```razor
<BzCarousel Items="products" Options="@touchOptions" />

@code {
    private BzCarouselOptions touchOptions = new()
    {
        Threshold = 15,        // Require more deliberate swipe
        ShortSwipes = true,    // Enable quick flicks if desired
    };
}
```

### Touch Options Reference

| Option            | Default | Range     | Description                     |
| ----------------- | ------- | --------- | ------------------------------- |
| `TouchRatio`      | `1.0`   | 0.1 - 2.0 | Touch sensitivity multiplier    |
| `Threshold`       | `10`    | 0 - 50    | Minimum pixels to trigger swipe |
| `ShortSwipes`     | `false` | bool      | Allow quick flick gestures      |
| `ResistanceRatio` | `0.85`  | 0 - 1     | Edge bounce resistance          |
| `LongSwipesRatio` | `0.3`   | 0.1 - 0.9 | Slide width % to advance        |

> 💡 **Tip:** If you experience glitchy behavior on mobile, try increasing `Threshold` to 15-20.

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

### Preview Panel Pattern

Use `OnActiveItemChanged` for live preview while browsing, and `SelectOnScroll="false"` when click means navigation:

```razor
<BzCarousel Items="movies"
            SelectOnScroll="false"
            OnItemSelected="NavigateToMovie"
            OnActiveItemChanged="ShowPreview" />

<div class="preview-panel">
    @if (previewMovie != null)
    {
        <h3>@previewMovie.Title</h3>
        <p>@previewMovie.Description</p>
    }
</div>

@code {
    private Movie? previewMovie;

    private void ShowPreview(Movie movie) => previewMovie = movie;

    private void NavigateToMovie(Movie movie)
    {
        Navigation.NavigateTo($"/movie/{movie.Id}");
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

### Performance Characteristics

- **Zero Runtime Overhead**: Mapping functions are generated at compile-time
- **Zero Reflection**: Uses `[ModuleInitializer]` for automatic registration at app startup
- **Type Safety**: Full compile-time checking of property names and types
- **O(1) Lookup**: Dictionary-based mapper lookup per type
- **Compiled Delegates**: Mapping functions are compiled, not interpreted

The entire system is designed for maximum performance with no runtime code generation or reflection.

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

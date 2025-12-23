# BlazzyMotion.Core

Core infrastructure package for BlazzyMotion UI components.

[![NuGet](https://img.shields.io/nuget/v/BlazzyMotion.Core.svg)](https://www.nuget.org/packages/BlazzyMotion.Core/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazzyMotion.Core.svg)](https://www.nuget.org/packages/BlazzyMotion.Core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

## Overview

BlazzyMotion.Core is a shared infrastructure package designed exclusively for the BlazzyMotion component ecosystem. It provides common abstractions, services, and theming used by all BlazzyMotion components (Carousel, Gallery, Masonry, etc.).

**This package is automatically installed as a dependency when you install any BlazzyMotion component. You do not need to install it manually.**

## Installation

```bash
# Install a BlazzyMotion component (Core is included automatically)
dotnet add package BlazzyMotion.Carousel
```

Note: Direct installation is only needed if you are extending the BlazzyMotion ecosystem with your own components that follow the same architecture.

## What's Included

- **Attributes** - `[BzImage]`, `[BzTitle]`, `[BzDescription]` for zero-config setup
- **Base Classes** - `BzComponentBase` for consistent component behavior
- **Models** - `BzItem`, `BzTheme` for data and theming
- **Services** - `BzRegistry`, `BzTemplateFactory` for template mapping and rendering
- **Infrastructure** - `BzJsInteropBase` for JavaScript interop

## Extending the BlazzyMotion Ecosystem

This package is designed for developers who want to create new components within the BlazzyMotion ecosystem using the same architecture and theming system:

```csharp
using BlazzyMotion.Core.Abstractions;
using BlazzyMotion.Core.Models;

public partial class MyCustomComponent<TItem> : BzComponentBase
    where TItem : class
{
    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    [Parameter]
    public BzTheme Theme { get; set; } = BzTheme.Glass;
}
```

## Related Packages

- [BlazzyMotion.Carousel](https://blazzymotion.com/) - 3D Coverflow carousel component

## Documentation

For complete documentation, examples, and live demos, visit:

- [Documentation & Live Demo](https://blazzy-motion.github.io/BlazzyMotion/)
- [GitHub Repository](https://github.com/Blazzy-Motion/BlazzyMotion)

## License

MIT License - free to use in personal and commercial projects.

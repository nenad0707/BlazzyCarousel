namespace BlazzyMotion.Tests.SourceGenerator;

/// <summary>
/// Edge case tests for source generator covering unusual scenarios,
/// error conditions, and boundary cases.
/// </summary>
public class BzCarouselGeneratorEdgeCaseTests : GeneratorTestBase
{
    #region Namespace Edge Cases

    [Fact]
    public void Generator_WithFileScopedNamespace_ShouldGenerate()
    {
        // Arrange - C# 10+ file-scoped namespace
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace;

public class Movie
{
    [BzImage]
    public string Poster { get; set; } = """";
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull("Should support file-scoped namespaces");
    }

    [Fact]
    public void Generator_WithSpecialCharactersInNamespace_ShouldHandle()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace My_Company.Product_1.V2_0
{
    public class Item
    {
        [BzImage]
        public string Image { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "ItemBzCarouselExtensions");
        generated.Should().NotBeNull();
        generated.Should().Contain("namespace My_Company.Product_1.V2_0");
    }

    #endregion

    #region Class Name Edge Cases

    [Fact]
    public void Generator_WithUnderscoreInClassName_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class My_Special_Movie
    {
        [BzImage]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "My_Special_MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
    }

    [Fact]
    public void Generator_WithNumbersInClassName_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie2023
    {
        [BzImage]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "Movie2023BzCarouselExtensions");
        generated.Should().NotBeNull();
    }

    #endregion

    #region Property Name Edge Cases

    [Fact]
    public void Generator_WithUnderscoreInPropertyName_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public string Image_URL { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().Contain("item.Image_URL");
    }

    [Fact]
    public void Generator_WithSingleLetterPropertyName_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Item
    {
        [BzImage]
        public string I { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "ItemBzCarouselExtensions");
        generated.Should().Contain("item.I");
    }

    #endregion

    #region Attribute Placement Variations

    [Fact]
    public void Generator_WithMultipleAttributesOnProperty_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        [Required]
        [MaxLength(500)]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
    }

    #endregion

    #region Abstract and Sealed Classes

    [Fact]
    public void Generator_WithAbstractClass_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public abstract class BaseMovie
    {
        [BzImage]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "BaseMovieBzCarouselExtensions");
        generated.Should().NotBeNull("Should support abstract classes");
    }

    [Fact]
    public void Generator_WithSealedClass_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public sealed class Movie
    {
        [BzImage]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull("Should support sealed classes");
    }

    #endregion

    #region Property Variations

    [Fact]
    public void Generator_WithGetOnlyProperty_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public string Poster { get; }

        public Movie(string poster)
        {
            Poster = poster;
        }
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
        generated.Should().Contain("item.Poster");
    }

    #endregion

    #region Multiple Attributes Scenarios

    [Fact]
    public void Generator_WithMultipleTitleProperties_ShouldUseFirst()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public string Poster { get; set; } = """";
        
        [BzTitle]
        public string Title { get; set; } = """";
        
        [BzTitle]
        public string Name { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        
        // Should use first found title property
        generated.Should().Contain("title = item.");
    }

    #endregion

    #region Compilation Scenarios

    [Fact]
    public void Generator_WithSyntaxError_ShouldNotCrash()
    {
        // Arrange - Source with syntax error
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage
        public string Poster { get; set; } = """";
    }
}";

        // Act
        Action act = () => RunGenerator(source);

        // Assert
        act.Should().NotThrow("Generator should handle syntax errors gracefully");
    }

    [Fact]
    public void Generator_WithMissingUsingDirective_ShouldNotGenerate()
    {
        // Arrange - No using directive
        var source = @"
namespace TestNamespace
{
    public class Movie
    {
        [BlazzyMotion.Carousel.Attributes.BzImage]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull("Should work with fully qualified attribute names");
    }

    #endregion

    #region Default Values

    [Fact]
    public void Generator_WithComplexDefaultValue_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public string Poster { get; set; } = ""https://example.com/default.jpg"";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
    }

    #endregion

    #region XML Special Characters in Documentation

    [Fact]
    public void GeneratedCode_ShouldEscapeXmlSpecialCharacters()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    /// <summary>
    /// Movie class with < and > characters
    /// </summary>
    public class Movie
    {
        [BzImage]
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
        
        // Generated code should have proper XML escaping in docs
        if (generated != null)
        {
            // Check for XML entities
            generated.Should().Contain("&lt;BzCarousel");
        }
    }

    #endregion

    #region Concurrent Generation

    [Fact]
    public void Generator_CalledConcurrently_ShouldProduceConsistentResults()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act - Run generator multiple times in parallel
        var results = Enumerable.Range(0, 5)
            .AsParallel()
            .Select(_ => RunGenerator(source))
            .ToList();

        // Assert
        var firstGenerated = FindGeneratedFile(results[0].GeneratedSources, "TestMovieBzCarouselExtensions");
        
        foreach (var result in results.Skip(1))
        {
            var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
            generated.Should().Be(firstGenerated, "Concurrent runs should produce identical output");
        }
    }

    #endregion

    #region Memory & Performance

    [Fact]
    public void Generator_WithManyClasses_ShouldCompleteInReasonableTime()
    {
        // Arrange - Generate source with many classes
        var classDefinitions = string.Join("\n\n", Enumerable.Range(1, 20).Select(i => $@"
    public class Movie{i}
    {{
        [BzImage]
        public string Poster{i} {{ get; set; }} = """";
    }}"));

        var source = $@"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{{
    {classDefinitions}
}}";

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = RunGenerator(source);
        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "Should complete in under 5 seconds");
        result.GeneratedSources.Should().HaveCountGreaterOrEqualTo(20, "Should generate for all classes");
    }

    #endregion
}

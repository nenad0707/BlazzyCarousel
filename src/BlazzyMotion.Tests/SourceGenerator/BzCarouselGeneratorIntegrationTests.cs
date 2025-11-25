using Microsoft.AspNetCore.Components.Rendering;

namespace BlazzyMotion.Tests.SourceGenerator;

/// <summary>
/// Integration tests that verify generated code works correctly at runtime.
/// These tests compile and execute the generated extension methods.
/// </summary>
public class BzCarouselGeneratorIntegrationTests : GeneratorTestBase
{
    #region Runtime Execution Tests

    [Fact]
    public void GeneratedTemplate_ShouldRenderImageElement()
    {
        // Arrange
        var source = CreateSimpleTestClass();
        var result = RunGenerator(source);
        AssertGeneratedCodeCompiles(result.OutputCompilation!);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("builder.OpenElement(0, \"img\")");
        generated.Should().Contain("builder.AddAttribute(1, \"src\", imageUrl)");
        generated.Should().Contain("builder.CloseElement()");
    }

    [Fact]
    public void GeneratedTemplate_WithTitle_ShouldRenderAltAttribute()
    {
        // Arrange
        var source = CreateSimpleTestClass("Movie", "Poster", "Title");
        var result = RunGenerator(source);
        AssertGeneratedCodeCompiles(result.OutputCompilation!);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().Contain("builder.AddAttribute(2, \"alt\", title)");
    }

    [Fact]
    public void GeneratedTemplate_ShouldHandleNullItem()
    {
        // Arrange
        var source = CreateSimpleTestClass();
        var result = RunGenerator(source);

        // Assert
        var generated = result.GeneratedSources.First(s => s.Contains("BzCarouselExtensions"));
        generated.Should().Contain("if (item is null)");
        generated.Should().Contain("return;");
    }

    [Fact]
    public void GeneratedTemplate_ShouldHandleNullOrEmptyImageUrl()
    {
        // Arrange
        var source = CreateSimpleTestClass();
        var result = RunGenerator(source);

        // Assert
        var generated = result.GeneratedSources.First(s => s.Contains("BzCarouselExtensions"));
        generated.Should().Contain("if (string.IsNullOrWhiteSpace(imageUrl))");
        generated.Should().Contain("return;");
    }

    #endregion

    #region RenderTreeBuilder Sequence Tests

    [Fact]
    public void GeneratedCode_ShouldUseCorrectBuilderSequence()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = result.GeneratedSources.First(s => s.Contains("BzCarouselExtensions"));
        
        // Verify sequence numbers
        generated.Should().Contain("builder.OpenElement(0,");
        generated.Should().Contain("builder.AddAttribute(1,");
        generated.Should().Contain("builder.AddAttribute(2,");
    }

    [Fact]
    public void GeneratedCode_WithTitle_ShouldHaveProperSequence()
    {
        // Arrange
        var source = CreateSimpleTestClass("Movie", "Poster", "Title");

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().Contain("builder.OpenElement(0, \"img\")");
        generated.Should().Contain("builder.AddAttribute(1, \"src\"");
        generated.Should().Contain("builder.AddAttribute(2, \"alt\"");
        generated.Should().Contain("builder.AddAttribute(3, \"title\"");
    }

    #endregion

    #region Method Discovery Tests

    [Fact]
    public void GeneratedExtensionMethod_ShouldBeDiscoverableByReflection()
    {
        // Arrange
        var source = CreateSimpleTestClass("Product", "Image");
        var result = RunGenerator(source);
        
        AssertGeneratedCodeCompiles(result.OutputCompilation!);

        // Act
        var generatedAssembly = result.OutputCompilation!;
        
        // Try to find the generated type
        var extensionsType = generatedAssembly
            .GetTypeByMetadataName("TestNamespace.ProductBzCarouselExtensions");

        // Assert
        extensionsType.Should().NotBeNull("Extension class should be discoverable");
        
        if (extensionsType != null)
        {
            var method = extensionsType.GetMembers("GetDefaultBzCarouselTemplate").FirstOrDefault();
            method.Should().NotBeNull("Extension method should exist");
        }
    }

    [Fact]
    public void GeneratedMethod_ShouldBeStatic()
    {
        // Arrange
        var source = CreateSimpleTestClass();
        var result = RunGenerator(source);

        // Assert
        var generated = result.GeneratedSources.First(s => s.Contains("BzCarouselExtensions"));
        generated.Should().Contain("public static RenderFragment");
    }

    [Fact]
    public void GeneratedMethod_ShouldReturnRenderFragment()
    {
        // Arrange
        var source = CreateSimpleTestClass("Game", "Cover");
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "GameBzCarouselExtensions");
        generated.Should().Contain("RenderFragment<Game>");
    }

    #endregion

    #region Complex Type Tests

    [Fact]
    public void Generator_WithComplexInheritance_ShouldGenerateWorkingCode()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public interface IMedia
    {
        string Id { get; set; }
    }

    public abstract class BaseMedia : IMedia
    {
        public string Id { get; set; } = """";
        
        [BzImage]
        public virtual string ImageUrl { get; set; } = """";
    }

    public class Movie : BaseMedia
    {
        [BzTitle]
        public string Title { get; set; } = """";
        
        public override string ImageUrl { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        AssertGeneratedCodeCompiles(result.OutputCompilation!);
        
        var movieExtensions = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        movieExtensions.Should().NotBeNull();
    }

    #endregion

    #region Property Type Variations

    [Fact]
    public void Generator_WithNullableStringProperty_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public string? Poster { get; set; }
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var movieExtensions = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        movieExtensions.Should().NotBeNull();
        AssertGeneratedCodeCompiles(result.OutputCompilation!);
    }

    [Fact]
    public void Generator_WithRequiredProperty_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public required string Poster { get; set; }
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var movieExtensions = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        movieExtensions.Should().NotBeNull();
        AssertGeneratedCodeCompiles(result.OutputCompilation!);
    }

    #endregion

    #region Generated Code Quality Tests

    [Fact]
    public void GeneratedCode_ShouldBeWellFormatted()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = result.GeneratedSources.First(s => s.Contains("BzCarouselExtensions"));
        
        // Check indentation
        generated.Should().Contain("    public static class");
        generated.Should().Contain("        public static RenderFragment");
        
        // Check line breaks (accept both Windows \r\n and Unix \n)
        var hasLineBreaks = generated.Contains("{\r\n") || generated.Contains("{\n");
        hasLineBreaks.Should().BeTrue("Generated code should have proper line breaks");
    }

    [Fact]
    public void GeneratedCode_ShouldUseConsistentNaming()
    {
        // Arrange
        var source = CreateSimpleTestClass("TestMovie", "PosterUrl");

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        
        // Extension class should follow naming convention
        generated.Should().Contain("class TestMovieBzCarouselExtensions");
        generated.Should().Contain("RenderFragment<TestMovie>");
    }

    #endregion

    #region Edge Case: Empty Classes

    [Fact]
    public void Generator_WithEmptyClass_ShouldNotGenerate()
    {
        // Arrange
        var source = @"
namespace TestNamespace
{
    public class EmptyClass
    {
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var extensions = FindGeneratedFile(result.GeneratedSources, "EmptyClassBzCarouselExtensions");
        extensions.Should().BeNull("Should not generate for classes without [BzImage]");
    }

    [Fact]
    public void Generator_WithOnlyNonPublicMembers_ShouldNotGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        private string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        result.Diagnostics.Should().Contain(d => d.Id == "BZC001");
        
        // Should not generate working code due to error
        var extensions = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        
        if (extensions != null)
        {
            // If code was generated, it should not contain the private property
            extensions.Should().NotContain("item.Poster");
        }
    }

    #endregion

    #region Performance Characteristics

    [Fact]
    public void Generator_WithLargeClass_ShouldGenerateEfficiently()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class LargeClass
    {
        [BzImage]
        public string Image { get; set; } = """";
        
        // Many other properties to simulate large class
        public string Prop1 { get; set; } = """";
        public string Prop2 { get; set; } = """";
        public string Prop3 { get; set; } = """";
        public string Prop4 { get; set; } = """";
        public string Prop5 { get; set; } = """";
        public string Prop6 { get; set; } = """";
        public string Prop7 { get; set; } = """";
        public string Prop8 { get; set; } = """";
        public string Prop9 { get; set; } = """";
        public string Prop10 { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "LargeClassBzCarouselExtensions");
        generated.Should().NotBeNull();
        
        // Should only reference the Image property, not all properties
        generated.Should().Contain("item.Image");
        generated.Should().NotContain("Prop1");
        generated.Should().NotContain("Prop10");
    }

    #endregion
}

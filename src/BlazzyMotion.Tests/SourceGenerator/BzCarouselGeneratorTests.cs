namespace BlazzyMotion.Tests.SourceGenerator;

/// <summary>
/// Main test suite for BzCarouselGenerator covering all core functionality.
/// Tests generation, validation, diagnostics, and attribute detection.
/// </summary>
public class BzCarouselGeneratorTests : GeneratorTestBase
{
    #region Basic Generation Tests

    [Fact]
    public void Generator_WithSimpleClass_ShouldGenerateExtensionClass()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        result.GeneratedSources.Should().NotBeEmpty();
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().NotBeNull();
    }

    [Fact]
    public void Generator_WithSimpleClass_ShouldGenerateCorrectNamespace()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("namespace TestNamespace");
    }

    [Fact]
    public void Generator_WithSimpleClass_ShouldGenerateStaticMethod()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("public static RenderFragment<TestMovie> GetDefaultBzCarouselTemplate()");
    }

    #endregion

    #region Code Content Verification

    [Fact]
    public void GeneratedCode_ShouldContainNullCheck()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("if (item is null)");
        generated.Should().Contain("return;");
    }

    [Fact]
    public void GeneratedCode_ShouldContainImageUrlNullCheck()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("if (string.IsNullOrWhiteSpace(imageUrl))");
    }

    [Fact]
    public void GeneratedCode_ShouldRenderImgElement()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("builder.OpenElement(0, \"img\")");
        generated.Should().Contain("builder.AddAttribute(1, \"src\", imageUrl)");
        generated.Should().Contain("builder.CloseElement()");
    }

    [Fact]
    public void GeneratedCode_ShouldIncludeLazyLoading()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("\"loading\", \"lazy\"");
    }

    #endregion

    #region Attribute Detection Tests

    [Fact]
    public void Generator_WithBzImageAttribute_ShouldDetect()
    {
        // Arrange
        var source = CreateSimpleTestClass("Movie", "Poster");

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
        generated.Should().Contain("item.Poster");
    }

    [Fact]
    public void Generator_WithBzTitleAttribute_ShouldDetect()
    {
        // Arrange
        var source = CreateSimpleTestClass("Movie", "Poster", "Title");

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().Contain("item.Title");
        generated.Should().Contain("builder.AddAttribute(2, \"alt\", title)");
    }

    [Fact]
    public void Generator_WithoutBzImageAttribute_ShouldNotGenerate()
    {
        // Arrange
        var source = @"
namespace TestNamespace
{
    public class Movie
    {
        public string Poster { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().BeNull();
    }

    #endregion

    #region Validation Tests

    [Fact]
    public void Generator_WithNonPublicProperty_ShouldReportDiagnostic()
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
        AssertDiagnosticExists(result.Diagnostics, "BZC001");
    }

    [Fact]
    public void Generator_WithNonStringProperty_ShouldReportDiagnostic()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class Movie
    {
        [BzImage]
        public int PosterId { get; set; }
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        AssertDiagnosticExists(result.Diagnostics, "BZC002");
    }

    [Fact]
    public void Generator_WithValidProperty_ShouldNotReportErrors()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        AssertNoDiagnostics(result.Diagnostics, DiagnosticSeverity.Error);
    }

    #endregion

    #region Inheritance Tests

    [Fact]
    public void Generator_WithInheritedBzImageProperty_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class BaseMedia
    {
        [BzImage]
        public string ImageUrl { get; set; } = """";
    }

    public class Movie : BaseMedia
    {
        public string Title { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var movieExtensions = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        movieExtensions.Should().NotBeNull();
        movieExtensions.Should().Contain("item.ImageUrl");
    }

    [Fact]
    public void Generator_WithOverriddenProperty_ShouldUseChildProperty()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class BaseMedia
    {
        [BzImage]
        public virtual string Image { get; set; } = """";
    }

    public class Movie : BaseMedia
    {
        public override string Image { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var movieExtensions = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        movieExtensions.Should().NotBeNull();
    }

    #endregion

    #region XML Documentation Tests

    [Fact]
    public void GeneratedCode_ShouldIncludeXmlDocumentation()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("/// <summary>");
        generated.Should().Contain("/// <returns>");
        generated.Should().Contain("/// <remarks>");
    }

    [Fact]
    public void GeneratedCode_ShouldDocumentUsage()
    {
        // Arrange
        var source = CreateSimpleTestClass("Product");

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "ProductBzCarouselExtensions");
        // Generator uses XML escaping in documentation comments
        generated.Should().Contain("&lt;BzCarousel");
    }

    #endregion

    #region Multiple Classes Tests

    [Fact]
    public void Generator_WithMultipleClasses_ShouldGenerateForEach()
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
    }

    public class Game
    {
        [BzImage]
        public string Cover { get; set; } = """";
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions").Should().NotBeNull();
        FindGeneratedFile(result.GeneratedSources, "GameBzCarouselExtensions").Should().NotBeNull();
    }

    #endregion

    #region Nested Classes Tests

    [Fact]
    public void Generator_WithNestedClass_ShouldGenerate()
    {
        // Arrange
        var source = @"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{
    public class MediaLibrary
    {
        public class Movie
        {
            [BzImage]
            public string Poster { get; set; } = """";
        }
    }
}";

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "MovieBzCarouselExtensions");
        generated.Should().NotBeNull();
    }

    #endregion

    #region Different Property Names Tests

    [Theory]
    [InlineData("Image")]
    [InlineData("ImageUrl")]
    [InlineData("PosterUrl")]
    [InlineData("CoverImage")]
    [InlineData("Thumbnail")]
    public void Generator_WithDifferentPropertyNames_ShouldGenerate(string propertyName)
    {
        // Arrange
        var source = CreateSimpleTestClass("Item", propertyName);

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "ItemBzCarouselExtensions");
        generated.Should().NotBeNull();
        generated.Should().Contain($"item.{propertyName}");
    }

    #endregion

    #region Compilation Tests

    [Fact]
    public void GeneratedCode_ShouldCompileSuccessfully()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        AssertGeneratedCodeCompiles(result.OutputCompilation!);
    }

    [Fact]
    public void GeneratedCode_WithTitle_ShouldCompileSuccessfully()
    {
        // Arrange
        var source = CreateSimpleTestClass("Movie", "Poster", "Title");

        // Act
        var result = RunGenerator(source);

        // Assert
        AssertGeneratedCodeCompiles(result.OutputCompilation!);
    }

    #endregion

    #region Auto-Generated Header Tests

    [Fact]
    public void GeneratedCode_ShouldHaveAutoGeneratedComment()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("// <auto-generated/>");
        generated.Should().Contain("// Generated by BlazzyCarousel Source Generator");
    }

    [Fact]
    public void GeneratedCode_ShouldHaveNullableEnable()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var generated = FindGeneratedFile(result.GeneratedSources, "TestMovieBzCarouselExtensions");
        generated.Should().Contain("#nullable enable");
    }

    #endregion

    #region Marker File Test

    [Fact]
    public void Generator_ShouldCreateMarkerFile()
    {
        // Arrange
        var source = CreateSimpleTestClass();

        // Act
        var result = RunGenerator(source);

        // Assert
        var marker = FindGeneratedFile(result.GeneratedSources, "GeneratorMarker");
        marker.Should().NotBeNull();
        marker.Should().Contain("class GeneratorMarker");
    }

    #endregion
}

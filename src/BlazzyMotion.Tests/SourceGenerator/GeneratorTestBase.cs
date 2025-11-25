using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace BlazzyMotion.Tests.SourceGenerator;

/// <summary>
/// Base class for Source Generator tests providing helper methods
/// for running generators and asserting results.
/// </summary>
public abstract class GeneratorTestBase
{
    /// <summary>
    /// Result of running a source generator.
    /// </summary>
    protected record GeneratorRunResult(
        ImmutableArray<string> GeneratedSources,
        ImmutableArray<Diagnostic> Diagnostics,
        Compilation? OutputCompilation);

    /// <summary>
    /// Runs the BzCarouselGenerator on the provided source code.
    /// </summary>
    protected GeneratorRunResult RunGenerator(string source)
    {
        // Parse the source code
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        // Create references for compilation
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(BzImageAttribute).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Components.RenderFragment<>).Assembly.Location),
        };

        // Add all referenced assemblies
        var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        references = references.Concat(new[]
        {
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "netstandard.dll")),
            MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Collections.dll")),
        }).ToArray();

        // Create compilation
        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // Create generator instance
        var generator = new BlazzyMotion.SourceGen.SourceGen.BzCarouselGenerator();

        // Create driver and run generator
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics);

        // Get generated sources
        var runResult = driver.GetRunResult();
        var generatedSources = runResult.GeneratedTrees
            .Select(tree => tree.GetText().ToString())
            .ToImmutableArray();

        // Get diagnostics from both compilation and generator
        var allDiagnostics = diagnostics
            .Concat(outputCompilation.GetDiagnostics())
            .Where(d => d.Severity >= DiagnosticSeverity.Warning)
            .ToImmutableArray();

        return new GeneratorRunResult(generatedSources, allDiagnostics, outputCompilation);
    }

    /// <summary>
    /// Finds a generated file by searching for a substring in the filename.
    /// </summary>
    protected string? FindGeneratedFile(ImmutableArray<string> sources, string fileNamePart)
    {
        return sources.FirstOrDefault(s => s.Contains(fileNamePart));
    }

    /// <summary>
    /// Asserts that the compilation has no errors.
    /// </summary>
    protected void AssertGeneratedCodeCompiles(Compilation compilation)
    {
        var errors = compilation.GetDiagnostics()
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToList();

        if (errors.Any())
        {
            var errorMessages = string.Join(Environment.NewLine,
                errors.Select(e => $"{e.Id}: {e.GetMessage()}"));
            
            throw new Exception($"Generated code has compilation errors:{Environment.NewLine}{errorMessages}");
        }
    }

    /// <summary>
    /// Creates a simple test class with BzImage attribute.
    /// </summary>
    protected string CreateSimpleTestClass(string className = "TestMovie", string imageProp = "Poster", string? titleProp = null)
    {
        var titleProperty = titleProp != null
            ? $@"
        [BzTitle]
        public string {titleProp} {{ get; set; }} = """";"
            : "";

        return $@"
using BlazzyMotion.Carousel.Attributes;

namespace TestNamespace
{{
    public class {className}
    {{
        [BzImage]
        public string {imageProp} {{ get; set; }} = """";{titleProperty}
    }}
}}";
    }

    /// <summary>
    /// Asserts that a diagnostic with the given ID exists.
    /// </summary>
    protected void AssertDiagnosticExists(ImmutableArray<Diagnostic> diagnostics, string diagnosticId)
    {
        diagnostics.Should().Contain(d => d.Id == diagnosticId,
            $"Expected diagnostic {diagnosticId} to be reported");
    }

    /// <summary>
    /// Asserts that no diagnostics with the given severity exist.
    /// </summary>
    protected void AssertNoDiagnostics(ImmutableArray<Diagnostic> diagnostics, DiagnosticSeverity severity = DiagnosticSeverity.Error)
    {
        var filtered = diagnostics.Where(d => d.Severity == severity).ToList();
        
        if (filtered.Any())
        {
            var messages = string.Join(Environment.NewLine,
                filtered.Select(d => $"{d.Id}: {d.GetMessage()} at {d.Location}"));
            
            throw new Exception($"Unexpected {severity} diagnostics:{Environment.NewLine}{messages}");
        }
    }
}

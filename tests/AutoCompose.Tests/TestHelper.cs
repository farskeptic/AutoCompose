using AutoCompose.Generator.AutoCompose;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoCompose.Tests
{
    // As per: https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/

    /// <summary>
    /// Used to test source generation using VerifyXUnit.  Reads in a list of sources, and runs source generation against them.
    /// Called from AutoCompose.Tests\Generator\AutoComposeSourceGeneratorTests.cs
    /// </summary>

    public class TestHelper
    {

        public static Task Verify(List<string> sources)
        {
            // Parse the provided string into a C# syntax tree
            var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x)).ToArray();

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: syntaxTrees);


            // Create an instance of our AutoComposeSourceGenerator incremental source generator
            var generator = new AutoComposeSourceGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);

            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver)
                .UseDirectory("Snapshots");
        }
        public static Task Verify(string source, string fileName)
        {
            // Parse the provided string into a C# syntax tree
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Tests",
                syntaxTrees: new[] { syntaxTree });


            // Create an instance of our AutoComposeSourceGenerator incremental source generator
            var generator = new AutoComposeSourceGenerator();

            // The GeneratorDriver is used to run our generator against a compilation
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the source generator!
            driver = driver.RunGenerators(compilation);

            // Use verify to snapshot test the source generator output!
            return Verifier.Verify(driver)
                .UseDirectory("Snapshots").UseParameters(fileName);
        }
    }
}
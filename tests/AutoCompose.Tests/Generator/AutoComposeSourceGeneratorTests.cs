using AutoCompose.Tests;

// As per: https://andrewlock.net/creating-a-source-generator-part-2-testing-an-incremental-generator-with-snapshot-testing/

namespace AutoCompose.Tests.Generator.Common
{
    [UsesVerify] // 👈 Adds hooks for Verify into XUnit

    /// <summary>
    /// Generates tests for each of the files in AutoCompose.Tests/TestCodeSnippets.
    /// Some are NOT used, as they would be illegal code situations that would cause
    /// compile errors.
    /// 
    /// The tests use VerifyXUnit to compare resulting source code results.
    /// </summary>
    public class AutoComposeSourceGeneratorTests
    {
        [Theory]
        [InlineData("C01.Multiple_Classes")]
        [InlineData("C02.Target_Multiple_Types")]

        [InlineData("M01.Method_AlreadyImplemented")]
        [InlineData("M02.Method_FullGeneric")]
        [InlineData("M03.Method_Generic")]

        // C#.NET gives compile error since a protected one doesn't satisfy the interface
        // [InlineData("Method_ImplDetection_WrongAccessModifier")]
        [InlineData("M04.Method_ImplDetection_WrongParameterList")]
        [InlineData("M05.Method_ImplDetection_WrongParameterType")]


        [InlineData("M06.Method_NotImplemented")]
        [InlineData("M07.Method_WithParameters")]
        [InlineData("M08.Multiple_Method_Parameter_Namespaces")]
        [InlineData("P01.Parameter_Multiple_Types")]
        [InlineData("P02.Property_AlreadyImplemented")]
        [InlineData("P03.Property_ImplDetection_WrongAccessModifier")]
        [InlineData("P04.Property_ImplDetection_WrongReturnType")]
        [InlineData("P05.Property_NotImplemented")]

        [InlineData("X01.Decoy_Attribute")] // Note: This test should not generate anything
        [InlineData("X02.Sample")]
        [InlineData("X03.Sample01")]
        [InlineData("A01.Nameof_Target")]
        [InlineData("A02.InternalClassGeneration")]

        // ToDo: C#.NET has trouble with this one - doesn't detect the mismatches
        // [InlineData("Method_ImplDetection_WrongGenericConstraint")]
        // [InlineData("Method_ImplDetection_WrongSecondGenericConstraint")]
        // C#.NET gives compile error since wrong return type doesn't satify the interface
        // [InlineData("Method_ImplDetection_WrongReturnType")]

        public Task SourceCodeGeneration(string fileName)
        {
            // The source code to test
            var source = File.ReadAllText(@$"TestCodeSnippets\{fileName}.txt");

            // Pass the source code to our helper and snapshot test the output
            return TestHelper.Verify(source, fileName);
        }
    }
}
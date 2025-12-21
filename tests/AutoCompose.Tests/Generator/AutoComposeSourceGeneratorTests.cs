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
    /// 
    /// EXPECTED BEHAVIOR:
    /// - AutoCompose only generates code for interface members that are NOT properly implemented.
    /// - A member is "not implemented" if it doesn't exist OR has wrong signature/access modifier.
    /// - See TEST_EXPECTATIONS.md for detailed explanations of each test case.
    /// </summary>
    public class AutoComposeSourceGeneratorTests
    {
        [Theory]
        // Multiple classes in different namespaces - should generate code for each
        [InlineData("C01.Multiple_Classes")] // EXPECTED: Generates Method1 for both classes (2 files)
        // Class implements multiple interfaces - should generate all unimplemented members
        [InlineData("C02.Target_Multiple_Types")] // EXPECTED: Generates all unimplemented members from all interfaces

        // Method tests
        [InlineData("M01.Method_AlreadyImplemented")] // EXPECTED: EMPTY - method already implemented correctly
        [InlineData("M02.Method_FullGeneric")] // EXPECTED: Generates generic method with type parameters
        [InlineData("M03.Method_Generic")] // EXPECTED: Generates generic method

        // C#.NET gives compile error since a protected one doesn't satisfy the interface
        // [InlineData("Method_ImplDetection_WrongAccessModifier")]
        // Method exists but with wrong parameter list - should still generate
        [InlineData("M04.Method_ImplDetection_WrongParameterList")] // EXPECTED: Generates method (wrong params = not implemented)
        // Method exists but with wrong parameter types - should still generate
        [InlineData("M05.Method_ImplDetection_WrongParameterType")] // EXPECTED: Generates method (wrong types = not implemented)

        // Method doesn't exist - should generate
        [InlineData("M06.Method_NotImplemented")] // EXPECTED: Generates public virtual int Method1() { return _sample.Method1(); }
        // Method with parameters doesn't exist - should generate with parameter forwarding
        [InlineData("M07.Method_WithParameters")] // EXPECTED: Generates method with all parameters forwarded
        // Methods with parameters from different namespaces - should generate with proper usings
        [InlineData("M08.Multiple_Method_Parameter_Namespaces")] // EXPECTED: Generates methods with proper namespace handling

        // Property tests
        [InlineData("P01.Parameter_Multiple_Types")] // EXPECTED: Generates all unimplemented properties
        [InlineData("P02.Property_AlreadyImplemented")] // EXPECTED: EMPTY - property already implemented correctly
        // Property exists but wrong access modifier (protected vs public) - should generate
        [InlineData("P03.Property_ImplDetection_WrongAccessModifier")] // EXPECTED: Generates public property (wrong modifier = not implemented)
        // Property exists but wrong return type - should generate
        [InlineData("P04.Property_ImplDetection_WrongReturnType")] // EXPECTED: Generates property (wrong type = not implemented)
        // Property doesn't exist - should generate
        [InlineData("P05.Property_NotImplemented")] // EXPECTED: Generates public virtual int Prop1 { get { return _sample.Prop1; } set { _sample.Prop1 = value; } }

        // Edge cases
        [InlineData("X01.Decoy_Attribute")] // EXPECTED: EMPTY - uses [Decoy] not [AutoCompose], so no generation
        [InlineData("X02.Sample")] // EXPECTED: Generates unimplemented members (check interface for specifics)
        [InlineData("X03.Sample01")] // EXPECTED: Generates unimplemented members (check interface for specifics)
        // Uses nameof() instead of string literal - should work the same
        [InlineData("A01.Nameof_Target")] // EXPECTED: Generates normally (nameof handled like string literal)
        // Internal class - should generate with internal modifier
        [InlineData("A02.InternalClassGeneration")] // EXPECTED: Generates code with internal access modifier

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
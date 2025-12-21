using AutoCompose.Generator.AutoCompose.Extensions;
using AutoCompose.Generator.Common.Fluent;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoCompose.Tests.Generator.AutoCompose.Extensions
{
    public class GeneratorExecutionContextExtensionTests : TestBase
    {
        [Fact]
        public void GetAutoComposeInfo_Succeeds()
        {
            // arrange..
            var className = "MyClass";
            var code = TestFixture.GetSimpleSourceCode(className);
            var api = SyntaxApi.Create(code);
            var autoComposedClass = api.GetClassDeclarationSyntax(className);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            var actual = autoComposedClass.GetAutoComposeInfo(context);
            // assert..
            Assert.Equal("m_sample", actual.MemberName);
            Assert.Equal("ISample", actual.TargetType);
        }

        [Fact]
        public void GetAutoComposeInfo_Throws_WhenNotAnnotated()
        {
            // arrange..
            var className = "MyClass";
            var code = TestFixture.GetSimpleSourceCode(className);
            var api = SyntaxApi.Create(code);
            var otherClass = api.GetClassDeclarationSyntax("OtherClass");
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            // assert..
            var ex = Assert.Throws<ArgumentException>(() => (otherClass.GetAutoComposeInfo(context)));
            Assert.Contains("Class 'OtherClass' must be decorated with an AutoComposeAttribute", ex.Message);
        }

        [Fact]
        public void GetAutoComposeInfo_Throws_WhenBadInvocationUsed()
        {
            // arrange..
            var className = "MyClass";
            var code = TestFixture.GetSimpleSourceCode(className);
            var api = SyntaxApi.Create(code);
            var otherClass = api.GetClassDeclarationSyntax("BadNameOfClass");
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            // assert..
            var ex = Assert.Throws<ArgumentNullException>(() => (otherClass.GetAutoComposeInfo(context)));
            Assert.Contains("Could not find string literal or nameof expression for member name", ex.Message);
        }

        [Fact]
        public void GetAutoComposeInfo_Throws_WhenMissingTypeof()
        {
            // arrange..
            var className = "MyClass";
            var code = TestFixture.GetSimpleSourceCode(className);
            var api = SyntaxApi.Create(code);
            var otherClass = api.GetClassDeclarationSyntax("MissingTypeofClass");
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            // assert..
            var ex = Assert.Throws<ArgumentNullException>(() => (otherClass.GetAutoComposeInfo(context)));
            Assert.Contains("Could not find string literal or nameof expression for member name", ex.Message);
        }

        [Fact]
        public void GetAutoComposeInfo_Succeeds_WithStringTypeSyntax()
        {
            // arrange..
            var className = "StringTypeClass";
            var code = TestFixture.GetSourceCodeWithStringType(className);
            var api = SyntaxApi.Create(code);
            var autoComposedClass = api.GetClassDeclarationSyntax(className);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            var actual = autoComposedClass.GetAutoComposeInfo(context);
            
            // assert..
            Assert.Equal("_sample", actual.MemberName);
            Assert.Equal("ISample", actual.TargetType);
        }

        [Fact]
        public void GetAutoComposeInfo_Succeeds_WithStringTypeAndNameOf()
        {
            // arrange..
            var className = "StringTypeNameOfClass";
            var code = TestFixture.GetSourceCodeWithStringTypeAndNameOf(className);
            var api = SyntaxApi.Create(code);
            var autoComposedClass = api.GetClassDeclarationSyntax(className);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            var actual = autoComposedClass.GetAutoComposeInfo(context);
            
            // assert..
            Assert.Equal("_sample", actual.MemberName);
            Assert.Equal("ISample", actual.TargetType);
        }

        [Fact]
        public void GetAutoComposeInfo_Throws_WhenFirstArgumentIsNotTypeOrString()
        {
            // arrange..
            var className = "InvalidFirstArgClass";
            var code = TestFixture.GetSourceCodeWithInvalidFirstArgument(className);
            var api = SyntaxApi.Create(code);
            var otherClass = api.GetClassDeclarationSyntax(className);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            // assert..
            var ex = Assert.Throws<ArgumentNullException>(() => (otherClass.GetAutoComposeInfo(context)));
            Assert.Contains("First argument must be either typeof(Type) or a string literal", ex.Message);
        }

        [Fact]
        public void GetAutoComposeInfo_Throws_WhenNoArguments()
        {
            // arrange..
            var className = "NoArgsClass";
            var code = TestFixture.GetSourceCodeWithNoArguments(className);
            var api = SyntaxApi.Create(code);
            var otherClass = api.GetClassDeclarationSyntax(className);
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var context = TestFixture.CreateFakeGeneratorExecutionContext(syntaxTree, className);

            // act..
            // assert..
            var ex = Assert.Throws<ArgumentNullException>(() => (otherClass.GetAutoComposeInfo(context)));
            Assert.Contains("No arguments found in attribute", ex.Message);
        }
    }
}

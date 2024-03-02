using AutoCompose.Generator.Common.Extensions;
using Microsoft.CodeAnalysis;

namespace AutoCompose.Tests.Generator.Common.Extensions
{
    public class GeneratorExecutionContextExtensionTests : TestBase
    {
        [Fact]
        public void GetSymbol_Returns_NullForNull()
        {
            // arrange..
            var context = TestFixture.CreateWrapper();
            SyntaxNode? syntaxNode = null;
            // act..
            var actual = context.GetSymbol(syntaxNode);
            // assert..
            Assert.Null(actual);
        }

        [Fact]
        public void GetDependentSymbol_Returns_NullForNull()
        {
            // arrange..
            var context = TestFixture.CreateWrapper();

            List<SyntaxNode> possibleDependencies = new();
            SyntaxNode? syntaxNode = null;
            // act..
            var actual = context.GetDependentSymbol(syntaxNode, possibleDependencies);
            // assert..
            Assert.Null(actual);
        }
    }
}

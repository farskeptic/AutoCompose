using AutoCompose.Generator.AutoCompose.Extensions;
using Microsoft.CodeAnalysis;
using NSubstitute;

namespace AutoCompose.Tests.Generator.AutoCompose.Extensions
{
    public class MethodSymbolExtensionsTests
    {
        [Theory]
        [InlineData(MethodKind.PropertyGet, true)]
        [InlineData(MethodKind.PropertySet, false)]
        public void IsGetter_Succeeds(MethodKind methodKind, bool expectedIsGetter)
        {
            // arrange..
            var methodSymbol = Substitute.For<IMethodSymbol>();
            methodSymbol.MethodKind.Returns(methodKind);
            methodSymbol.DeclaredAccessibility.Returns(Accessibility.Public);
            // act..
            var actual = methodSymbol.IsPublicGetter();
            // assert..
            Assert.Equal(expectedIsGetter, actual);
        }

    }
}

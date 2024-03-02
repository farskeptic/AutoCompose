using AutoCompose.Generator.AutoCompose.Extensions;
using AutoCompose.Generator.Common.Fluent;

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

            // act..
            var actual = autoComposedClass.GetAutoComposeInfo();
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

            // act..
            // assert..
            var ex = Assert.Throws<ArgumentNullException>(() => (otherClass.GetAutoComposeInfo()));
            Assert.Contains("TypeOfExpressionSyntax", ex.Message);
        }

    }
}

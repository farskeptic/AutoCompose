using AutoCompose.Generator.AutoCompose;
using AutoCompose.Generator.Common.Fluent;
using Newtonsoft.Json;

namespace AutoCompose.Tests.Generator.AutoCompose
{
    public class AutoComposeSourceParserTests : TestBase
    {

        [Fact]
        public void AutoComposeSourceParser_Parse_Tracks_NamespaceUsings()
        {
            // arrange..
            var className = "MyClassName";
            var code = TestFixture.GetSimpleSourceCode(className);
            var api = SyntaxApi.Create(code);
            var autoComposedClass = api.GetClassDeclarationSyntax(className);

            var fakeContext = TestFixture.CreateFakeGeneratorExecutionContext(api.GetSyntaxTree(), className);

            // act..
            var model = AutoComposeSourceParser.Parse(fakeContext, autoComposedClass);

            JsonConvert.SerializeObject(model.NamespaceUsings);
            // assert..
            Assert.Contains("MyClassName.InterfaceNamespace", model.NamespaceUsings);
        }

    }
}

using AutoCompose.Generator.AutoCompose;
using AutoCompose.Generator.Common.Fluent;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace AutoCompose.Tests.Generator.AutoCompose
{
    public class AutoComposeSourceParserTests : TestBase
    {
        private readonly ITestOutputHelper _output;

        public AutoComposeSourceParserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AutoComposeSourceParser_Parse_Tracks_NamespaceUsings()
        {
            // arrange..
            var className = "MyClassName";
            // Use a minimal test code that only includes what's needed for this test
            var code = TestFixture.GetMinimalSourceCodeForNamespaceTest(className);
            _output.WriteLine("Generated code:");
            _output.WriteLine(code);
            
            var api = SyntaxApi.Create(code);
            var autoComposedClass = api.GetClassDeclarationSyntax(className);

            var fakeContext = TestFixture.CreateFakeGeneratorExecutionContext(api.GetSyntaxTree(), className);

            // act..
            var model = AutoComposeSourceParser.Parse(fakeContext, autoComposedClass);

            var namespaceUsingsJson = JsonConvert.SerializeObject(model.NamespaceUsings);
            _output.WriteLine($"Namespace usings: {namespaceUsingsJson}");
            
            // assert..
            Assert.Contains("MyClassName.InterfaceNamespace", model.NamespaceUsings);
        }

    }
}

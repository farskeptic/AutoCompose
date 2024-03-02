using AutoCompose.Generator.Common.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NSubstitute;

namespace AutoCompose.Tests
{
    /// <summary>
    /// Used to create data for tests where that data is used multiple times.
    /// </summary>
    public class TestFixture
    {
        public static IGeneratorExecutionContext CreateWrapper()
        {
            return Factory.CreateWrapper(new GeneratorExecutionContext());
        }

        public static string GetSimpleSourceCode(string className)
        {
            return @$"using ExternalNamespace; 

namespace {className}.InterfaceNamespace
{{ 
    public interface ISample 
    {{
        int Val {{ get; set; }}
    }}
}}

namespace {className}.ClassNamespace
{{ 
    [AutoCompose(""m_sample"", typeof(ISample))]
    public class {className} 
    {{
        public void Test(ISample sample) {{ }}
    }}

    public class OtherClass
    {{
        public void Test() {{ }}
    }}
}}
            ";
        }

        public static IGeneratorExecutionContext CreateFakeGeneratorExecutionContext(SyntaxTree syntaxTree, string className)
        {
            // Parse the provided string into a C# syntax tree
            var syntaxTrees = new List<SyntaxTree> { syntaxTree };

            // Create a Roslyn compilation for the syntax tree.
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: "Test",
                syntaxTrees: syntaxTrees);

            var syntaxNodes = syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().ToList();
            var syntaxNode = syntaxNodes.Where(x => x.Identifier.ValueText == className).FirstOrDefault();
            var autoComposedClass = syntaxNode as ClassDeclarationSyntax;


            var context = Substitute.For<IGeneratorExecutionContext>();
            context.Compilation.ReturnsForAnyArgs(compilation);

            return context;

        }


    }
}

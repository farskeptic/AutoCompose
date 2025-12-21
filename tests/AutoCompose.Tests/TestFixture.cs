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
    [AutoCompose(typeof(ISample), ""m_sample"")]
    public partial class {className} 
    {{
        protected readonly ISample m_sample;

        public {className}(ISample sample)
        {{
            m_sample = sample;
        }}

        public void Test(ISample sample) {{ }}
    }}

    public class OtherClass
    {{
        public void Test() {{ }}
    }}

    [AutoCompose(typeof(ISample), nameof(m_sample))]
    public partial class NameOfClass
    {{
        protected readonly ISample m_sample;

        public NameOfClass(ISample sample)
        {{
            m_sample = sample;
        }}
    }}

    [AutoCompose(typeof(ISample), other(m_sample))]
    public partial class BadNameOfClass
    {{
        protected readonly ISample m_sample;

        public BadNameOfClass(ISample sample)
        {{
            m_sample = sample;
        }}
    }}

    [AutoCompose(""_sample"")]
    public partial class MissingTypeofClass
    {{
        protected readonly ISample _sample;

        public MissingTypeofClass(ISample sample)
        {{
            _sample = sample;
        }}
    }}

}}
            ";
        }

        public static string GetMinimalSourceCodeForNamespaceTest(string className)
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
    [AutoCompose(typeof(ISample), ""m_sample"")]
    public partial class {className} 
    {{
        protected readonly ISample m_sample;

        public {className}(ISample sample)
        {{
            m_sample = sample;
        }}

        public void Test(ISample sample) {{ }}
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

        public static string GetSourceCodeWithStringType(string className)
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
    [AutoCompose(""ISample"", ""_sample"")]
    public class {className} 
    {{
        public void Test(ISample sample) {{ }}
    }}
}}
            ";
        }

        public static string GetSourceCodeWithStringTypeAndNameOf(string className)
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
    [AutoCompose(""ISample"", nameof(_sample))]
    public class {className} 
    {{
        protected ISample _sample;
        public void Test(ISample sample) {{ }}
    }}
}}
            ";
        }

        public static string GetSourceCodeWithInvalidFirstArgument(string className)
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
    [AutoCompose(123, ""_sample"")]
    public class {className} 
    {{
        public void Test(ISample sample) {{ }}
    }}
}}
            ";
        }

        public static string GetSourceCodeWithNoArguments(string className)
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
    [AutoCompose()]
    public class {className} 
    {{
        public void Test(ISample sample) {{ }}
    }}
}}
            ";
        }


    }
}

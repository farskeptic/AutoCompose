using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace AutoCompose.Generator.Common.Fluent
{
    /// <summary>
    /// A slightly easier way of working with a piece of code to return specific bits of syntax from it.
    /// Node that when using SourceGeneration, all work must be from the same instance of a syntax tree.
    /// You cannot compare symbols between two syntax trees, even if generated from the same source code.
    /// </summary>
    public class SyntaxApi
    {
        protected string _code;
        protected SyntaxTree _syntaxTree;

        /// <summary>
        /// Creates an instance from a piece of code.
        /// Dependency injection is difficult in SourceGeneration, so we use a factory method
        /// </summary>
        public static SyntaxApi Create(string code)
        {
            return new SyntaxApi(code);
        }

        /// <summary>
        /// Creates an instance from a piece of code.
        /// </summary>
        protected SyntaxApi(string code)
        {
            _code = code;
            _syntaxTree = CSharpSyntaxTree.ParseText(_code);
        }

        /// <summary>
        /// Retrieve the class declaration syntax for a given class name from the code 
        /// supplied in the constructor.
        /// </summary>
        public ClassDeclarationSyntax GetClassDeclarationSyntax(string className)
        {
            return GetSyntaxNode(className) as ClassDeclarationSyntax;
        }

        /// <summary>
        /// Retrieve the SyntaxNode for a given class name from the code 
        /// supplied in the constructor.
        /// </summary>
        public SyntaxNode GetSyntaxNode(string className)
        {
            var syntaxNodes = _syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().ToList();
            var syntaxNode = syntaxNodes.Where(x => x.Identifier.ValueText == className).FirstOrDefault();
            return syntaxNode;
        }

        /// <summary>
        /// Retrieve the SyntaxTree for a given class name from the code 
        /// supplied in the constructor.
        /// </summary>
        public SyntaxTree GetSyntaxTree()
        {
            return _syntaxTree;
        }
    }
}

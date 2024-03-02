using AutoCompose.Generator.Common.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AutoCompose.Generator.Common.Extensions

{
    public static class GeneratorExecutionContextExtension
    {
        /// <summary>
        /// Returns syntax nodes for both classes and interfaces.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identiferValue"></param>
        /// <returns></returns>
        public static List<SyntaxNode> GetSyntaxNodes(this IGeneratorExecutionContext context, string identiferValue)
        {
            var possibleClasses =
                          context.Compilation
                          .SyntaxTrees
                          .SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                          .Where(x => x is ClassDeclarationSyntax)
                          .Cast<ClassDeclarationSyntax>()
                          .Where(x => x.Identifier.ValueText == identiferValue)
                          .ToImmutableList();

            var possibleInterfaces =
                          context.Compilation
                          .SyntaxTrees
                          .SelectMany(syntaxTree => syntaxTree.GetRoot().DescendantNodes())
                          .Where(x => x is InterfaceDeclarationSyntax)
                          .Cast<InterfaceDeclarationSyntax>()
                          .Where(x => x.Identifier.ValueText == identiferValue)
                          .ToImmutableList();

            return possibleClasses.Select(x => x as SyntaxNode).Union(possibleInterfaces.Select(x => x as SyntaxNode)).ToList();
        }

        /// <summary>
        /// Returns the Named Type Symbol for a given syntax node.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="syntaxNode"></param>
        /// <returns></returns>
        public static INamedTypeSymbol GetSymbol(this IGeneratorExecutionContext context, SyntaxNode syntaxNode)
        {
            if (syntaxNode is null)
            {
                return null;
            }
            var root = syntaxNode.SyntaxTree.GetRoot();
            var model = context.Compilation.GetSemanticModel(root.SyntaxTree);
            return (INamedTypeSymbol)model.GetDeclaredSymbol(syntaxNode);
        }

        /// <summary>
        /// Returns the dependent symbol from a list of possible dependencies for a given syntax node.
        /// e.g. when a class has a member that is of a type that has possible matches in multiple namespaces.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="syntaxNode"></param>
        /// <param name="possibleDependencies"></param>
        /// <returns></returns>
        public static INamedTypeSymbol GetDependentSymbol(this IGeneratorExecutionContext context, SyntaxNode syntaxNode, List<SyntaxNode> possibleDependencies)
        {
            if (syntaxNode is null)
            {
                return null;
            }
            var root = syntaxNode.SyntaxTree.GetRoot();

            foreach (var dep in possibleDependencies)
            {
                var depSymbol = context.GetSymbol(dep);

                var possibleNamespaces = root.SyntaxTree.GetRoot().DescendantNodes()
                              .Where(x => x is UsingDirectiveSyntax)
                              .Cast<UsingDirectiveSyntax>()
                              .Where(x => x.Name.ToString() == depSymbol.ContainingNamespace.ToString())
                              .ToImmutableList();

                // we will always use the first definition because we don't support multiple definitions
                if (possibleNamespaces.Any())
                {
                    return depSymbol;
                }

            }

            return context.GetSymbol(possibleDependencies.FirstOrDefault());
        }
    }
}

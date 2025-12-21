using AutoCompose.Generator.Common;
using AutoCompose.Generator.Common.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System;

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
                if (depSymbol is null || depSymbol.TypeKind == TypeKind.Error)
                {
                    continue;
                }

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

            // If no using directive found, return the first valid (non-error) symbol
            var fallbackSymbol = context.GetSymbol(possibleDependencies.FirstOrDefault());
            if (fallbackSymbol != null && fallbackSymbol.TypeKind != TypeKind.Error)
            {
                return fallbackSymbol;
            }

            return null;
        }

        /// <summary>
        /// Gets a type symbol from a typeof expression in an attribute using the semantic model.
        /// This is the most accurate method and works for both project types and external types.
        /// </summary>
        /// <param name="context">The generator execution context</param>
        /// <param name="classSyntax">The class declaration syntax containing the attribute</param>
        /// <param name="typeName">The simple name of the type (e.g. "ISample")</param>
        /// <returns>The type symbol if found, null otherwise</returns>
        public static INamedTypeSymbol GetTypeSymbolFromTypeOfExpression(this IGeneratorExecutionContext context, ClassDeclarationSyntax classSyntax, string typeName)
        {
            if (classSyntax is null || string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            // Get the semantic model for the syntax tree
            var semanticModel = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            
            // Find the AutoCompose attribute
            var attributes = classSyntax.AttributeLists.SelectMany(al => al.Attributes)
                .Where(a => a.Name.ToString() == Constants.AutoCompose || 
                           a.Name.ToString() == Constants.AutoComposeAttribute)
                .ToList();

            foreach (var attribute in attributes)
            {
                if (attribute.ArgumentList?.Arguments == null)
                {
                    continue;
                }

                // Find the typeof expression argument
                var typeofArgument = attribute.ArgumentList.Arguments
                    .FirstOrDefault(arg => arg.Expression is TypeOfExpressionSyntax);

                if (typeofArgument?.Expression is TypeOfExpressionSyntax typeofExpr)
                {
                    // Get the type info from the semantic model
                    var typeInfo = semanticModel.GetTypeInfo(typeofExpr.Type);
                    if (typeInfo.Type is INamedTypeSymbol namedType && namedType.Name == typeName)
                    {
                        return namedType;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a type symbol from the compilation, including types from referenced assemblies (NuGet packages).
        /// First tries to find the type by fully qualified name, then by simple name if a namespace can be inferred.
        /// </summary>
        /// <param name="context">The generator execution context</param>
        /// <param name="typeName">The simple name of the type (e.g. "ISample")</param>
        /// <param name="containingNamespace">Optional namespace hint to help resolve the type</param>
        /// <returns>The type symbol if found, null otherwise</returns>
        public static INamedTypeSymbol GetTypeSymbolFromCompilation(this IGeneratorExecutionContext context, string typeName, string containingNamespace = null)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            // First, try to find by fully qualified name if we have a namespace
            if (!string.IsNullOrEmpty(containingNamespace))
            {
                var fullyQualifiedName = $"{containingNamespace}.{typeName}";
                var symbol = context.Compilation.GetTypeByMetadataName(fullyQualifiedName);
                if (symbol is not null)
                {
                    return symbol;
                }
            }

            // Try to find by simple name in all referenced assemblies
            // This searches through all accessible types in the compilation
            var allTypes = context.Compilation.GlobalNamespace.GetNamespaceMembers()
                .SelectMany(ns => GetAllTypesInNamespace(ns))
                .Where(t => t.Name == typeName)
                .ToList();

            // If we have a namespace hint, prefer types from that namespace
            if (!string.IsNullOrEmpty(containingNamespace) && allTypes.Any())
            {
                var matchingNamespace = allTypes.FirstOrDefault(t => 
                    t.ContainingNamespace.ToDisplayString() == containingNamespace);
                if (matchingNamespace is not null)
                {
                    return matchingNamespace;
                }
            }

            // Return the first match, or null if none found
            return allTypes.FirstOrDefault();
        }

        /// <summary>
        /// Recursively gets all types in a namespace and its sub-namespaces.
        /// </summary>
        private static IEnumerable<INamedTypeSymbol> GetAllTypesInNamespace(INamespaceSymbol namespaceSymbol)
        {
            foreach (var type in namespaceSymbol.GetTypeMembers())
            {
                yield return type;
            }

            foreach (var subNamespace in namespaceSymbol.GetNamespaceMembers())
            {
                foreach (var type in GetAllTypesInNamespace(subNamespace))
                {
                    yield return type;
                }
            }
        }
    }
}

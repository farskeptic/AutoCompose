using AutoCompose.Generator.AutoCompose.Models;
using AutoCompose.Generator.Common;
using AutoCompose.Generator.Common.Extensions;
using AutoCompose.Generator.Common.Wrappers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompose.Generator.AutoCompose.Extensions
{
    public static class GeneratorExecutionContextExtension
    {
        /// <summary>
        /// Retrieves the AutoComposeInfo (e.g. targetType, memberName) for the given 
        /// classDeclarationSyntax.
        /// </summary>
        public static AutoComposeInfo GetAutoComposeInfo(this ClassDeclarationSyntax autoComposedClass, IGeneratorExecutionContext context)
        {
            var className = autoComposedClass.Identifier.ValueText;
            var attributes = autoComposedClass.AttributeLists.SelectMany(y => y.Attributes).ToList();

            if (!attributes.Any(x => x.Name.ToString() == Constants.AutoCompose)) {
                throw new ArgumentException($"Class '{className}' must be decorated with an AutoComposeAttribute. Expected format: [AutoCompose(typeof(TargetType), \"memberName\")]");
            }

            // Get the AutoCompose attribute for better error messages
            var autoComposeAttribute = attributes.FirstOrDefault(x => x.Name.ToString() == Constants.AutoCompose);
            var attributeText = autoComposeAttribute?.ToString() ?? "[AutoCompose(...)]";

            AttributeArgumentSyntax typeofType = null;
            AttributeArgumentSyntax stringType = null;
            string memberName = string.Empty;
            
            // Try to find either typeof expression or string literal for the type (first argument only)
            try
            {
                // Get all arguments - the first one is the type, the second is the member name
                var allArguments = attributes.SelectMany(x => x.ArgumentList?.Arguments ?? Enumerable.Empty<AttributeArgumentSyntax>()).ToList();
                
                if (allArguments.Count == 0)
                {
                    throw new ArgumentNullException(
                        null,
                        $"AutoCompose: No arguments found in attribute for class '{className}'. " +
                        $"Found attribute: {attributeText}. " +
                        $"Please ensure you have provided an attribute like: [AutoCompose(typeof(ISample), \"_sample\")] or [AutoCompose(\"ISample\", \"_sample\")]");
                }
                
                // Check the first argument for the type (either typeof or string)
                var firstArgument = allArguments[0];
                if (firstArgument.Expression is TypeOfExpressionSyntax)
                {
                    typeofType = firstArgument;
                }
                else if (firstArgument.Expression is LiteralExpressionSyntax literalExpr && literalExpr.Kind() == SyntaxKind.StringLiteralExpression)
                {
                    stringType = firstArgument;
                }
                else
                {
                    var argumentType = firstArgument.Expression.GetType().Name;
                    throw new ArgumentNullException(
                        null,
                        $"AutoCompose: First argument must be either typeof(Type) or a string literal for the target type in attribute for class '{className}'. " +
                        $"Found attribute: {attributeText}. " +
                        $"First argument type: {argumentType}. " +
                        $"Please ensure you have provided an attribute like: [AutoCompose(typeof(ISample), \"_sample\")] or [AutoCompose(\"ISample\", \"_sample\")]");
                }
            }
            catch (ArgumentNullException)
            {
                // Re-throw ArgumentNullException as-is (it already has good context)
                throw;
            }
            catch (ArgumentException)
            {
                // Re-throw ArgumentException as-is
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"AutoCompose: Error parsing type argument in attribute for class '{className}'. " +
                    $"Found attribute: {attributeText}. " +
                    $"Please ensure you have provided an attribute like: [AutoCompose(typeof(ISample), \"_sample\")] or [AutoCompose(\"ISample\", \"_sample\")]. " +
                    $"Inner exception: {ex.Message}", ex);
            }

            try
            {
                // Get all arguments to find the member name (second argument)
                var allArguments = attributes.SelectMany(x => x.ArgumentList?.Arguments ?? Enumerable.Empty<AttributeArgumentSyntax>()).ToList();
                
                // The member name is the second argument (index 1)
                if (allArguments.Count > 1)
                {
                    var memberArgument = allArguments[1];
                    
                    if (memberArgument.Expression is LiteralExpressionSyntax memberLiteral)
                    {
                        memberName = memberLiteral.Token.ValueText;
                        if (string.IsNullOrEmpty(memberName))
                        {
                            // Fallback: remove quotes from the string representation
                            memberName = memberLiteral.ToString().Replace("\"", string.Empty);
                        }
                    }
                    else if (memberArgument.Expression is InvocationExpressionSyntax memberNameOf)
                    {
                        // memberNameOf is already an InvocationExpressionSyntax (e.g., nameof(_sample))
                        // Check if it's a nameof expression
                        if (memberNameOf.Expression.ToString() == Constants.NameOf)
                        {
                            var firstArgument = memberNameOf.ArgumentList.Arguments.FirstOrDefault();
                            if (firstArgument is not null)
                            {
                                memberName = firstArgument.Expression.ToString();
                            }
                        }
                    }
                }
                
                if (string.IsNullOrEmpty(memberName))
                {
                    var argumentDetails = string.Join(", ", allArguments.Select((a, i) => $"arg{i}: {a.Expression.GetType().Name} = {a.Expression.ToString()}"));
                    throw new ArgumentNullException(
                        null,
                        $"AutoCompose: Could not find string literal or nameof expression for member name (second argument) in attribute for class '{className}'. " +
                        $"Found attribute: {attributeText}. " +
                        $"Found {allArguments.Count} argument(s): {argumentDetails}. " +
                        $"Please ensure you have provided an attribute as either [AutoCompose(typeof(ISample), \"_sample\")] or [AutoCompose(\"ISample\", \"_sample\")] or [AutoCompose(typeof(ISample), nameof(_sample))]");
                }
            }
            catch (ArgumentNullException)
            {
                // Re-throw ArgumentNullException as-is (it already has good context)
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"AutoCompose: Error parsing member name in attribute for class '{className}'. " +
                    $"Found attribute: {attributeText}. " +
                    $"Please ensure you have provided an attribute as either [AutoCompose(typeof(ISample), \"_sample\")] or [AutoCompose(typeof(ISample), nameof(_sample))]. " +
                    $"Inner exception: {ex.Message}", ex);
            }

            // Try to get the type symbol directly from the semantic model (works for both internal and external types)
            INamedTypeSymbol targetSymbol = null;
            string targetType = null;
            
            if (typeofType is not null && typeofType.Expression is TypeOfExpressionSyntax typeofExpr)
            {
                // Handle typeof(Type) syntax
                try
                {
                    var semanticModel = context.Compilation.GetSemanticModel(autoComposedClass.SyntaxTree);
                    var typeInfo = semanticModel.GetTypeInfo(typeofExpr.Type);
                    
                    if (typeInfo.Type is INamedTypeSymbol namedType)
                    {
                        // Only set targetSymbol if it's a valid, non-error type
                        // If it's an error type, leave targetSymbol as null so fallbacks can run
                        if (namedType.TypeKind != TypeKind.Error)
                        {
                            targetSymbol = namedType;
                            targetType = namedType.Name;
                        }
                    }
                    else if (typeInfo.Type is not null)
                    {
                        // Fallback: use the display string and extract the name
                        var displayString = typeInfo.Type.ToDisplayString();
                        var lastDot = displayString.LastIndexOf('.');
                        targetType = lastDot >= 0 ? displayString.Substring(lastDot + 1) : displayString;
                        // Remove generic parameters if present (e.g., "List`1" -> "List")
                        var backtickIndex = targetType.IndexOf('`');
                        if (backtickIndex >= 0)
                        {
                            targetType = targetType.Substring(0, backtickIndex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // If semantic model fails, fall through to syntax-based extraction
                }
                
                // Fallback: Try syntax-based extraction for simple cases
                if (string.IsNullOrEmpty(targetType) && typeofExpr.ChildNodes().Any())
                {
                    var firstChild = typeofExpr.ChildNodes().First();
                    if (firstChild is IdentifierNameSyntax identifierName)
                    {
                        targetType = identifierName.Identifier.ValueText;
                    }
                    else if (firstChild is QualifiedNameSyntax qualifiedName)
                    {
                        // For qualified names like "System.IO.Abstractions.IFileSystem", get the last part
                        var name = qualifiedName;
                        while (name.Left is QualifiedNameSyntax leftQualified)
                        {
                            name = leftQualified;
                        }
                        // name.Right is always SimpleNameSyntax (the final identifier)
                        targetType = name.Right.Identifier.ValueText;
                    }
                    else
                    {
                        // Try to get the string representation and extract the name
                        var typeString = firstChild.ToString();
                        var lastDot = typeString.LastIndexOf('.');
                        targetType = lastDot >= 0 ? typeString.Substring(lastDot + 1) : typeString;
                    }
                }
            }
            else if (stringType is not null && stringType.Expression is LiteralExpressionSyntax stringLiteral)
            {
                // Handle "TypeName" string syntax
                targetType = stringLiteral.Token.ValueText;
                if (string.IsNullOrEmpty(targetType))
                {
                    // Fallback: remove quotes from the string representation
                    targetType = stringLiteral.ToString().Replace("\"", string.Empty);
                }
            }
            
            if (!string.IsNullOrEmpty(targetType))
            {
                return new AutoComposeInfo(targetType, memberName, targetSymbol);
            }
            
            throw new ArgumentException(
                $"AutoCompose: Error extracting target type name from attribute for class '{className}'. " +
                $"Found attribute: {attributeText}. " +
                $"The type argument may reference a type that cannot be resolved at compile time. " +
                $"Ensure the type is accessible and properly referenced. " +
                $"Use either typeof(Type) or \"TypeName\" as the first argument.");
        }


        /// <summary>
        /// This gets passed an AutoComposeAttribute (e.g.: [AutoCompose(typeof(ISample), "_sample")]
        /// And returns the requested type (e.g. TypeOfExpressionSyntax or LiteralExpressionSyntax)
        /// </summary>
        private static AttributeArgumentSyntax GetAttributeArgumentOfType<T>(List<AttributeSyntax> attributes) where T : SyntaxNode
        {
            return attributes.SelectMany(x => x.ArgumentList?.Arguments ?? Enumerable.Empty<AttributeArgumentSyntax>())
                            .Where(x => x.Expression is T)
                            .FirstOrDefault();
        }
    }
}

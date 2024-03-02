using AutoCompose.Generator.AutoCompose.Models;
using AutoCompose.Generator.Common.Extensions;
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
        public static AutoComposeInfo GetAutoComposeInfo(this ClassDeclarationSyntax autoComposedClass)
        {
            var attributes = autoComposedClass.AttributeLists.SelectMany(y => y.Attributes).ToList();

            var typeofType = GetAttributeArgumentOfType<TypeOfExpressionSyntax>(attributes).GuardNull(nameof(TypeOfExpressionSyntax));

            var composingMember = GetAttributeArgumentOfType<LiteralExpressionSyntax>(attributes).GuardNull();

            var memberName = composingMember.Expression.ToString().Replace("\"", string.Empty);

            if (typeofType.Expression.ChildNodes().Any())
            {
                var targetType = typeofType.Expression.ChildNodes().First().TryGetInferredMemberName();
                if (!string.IsNullOrEmpty(targetType))
                {
                    return new AutoComposeInfo(targetType, memberName);
                }
            }
            throw new ArgumentException("Error retrieving AutoComposeInfo");
        }


        /// <summary>
        /// This gets passed an AutoComposeAttribute (e.g.: [AutoCompose(typeof(ISample), "_sample")]
        /// And returns the requested type (e.g. TypeOfExpressionSyntax or LiteralExpressionSyntax)
        /// </summary>
        private static AttributeArgumentSyntax GetAttributeArgumentOfType<T>(List<AttributeSyntax> attributes)
        {
            return attributes.SelectMany(x => x.ArgumentList.Arguments)
                            .Where(x => x.Expression.GetType() == typeof(T))
                            .FirstOrDefault();
        }
    }
}

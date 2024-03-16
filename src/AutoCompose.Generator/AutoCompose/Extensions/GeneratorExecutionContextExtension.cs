using AutoCompose.Generator.AutoCompose.Models;
using AutoCompose.Generator.Common;
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

            AttributeArgumentSyntax typeofType;
            string memberName = string.Empty;
            try
            {
                typeofType = GetAttributeArgumentOfType<TypeOfExpressionSyntax>(attributes).GuardNull(nameof(TypeOfExpressionSyntax));
            }
            catch (Exception)
            {
                throw new ArgumentException("Error finding typeof expression for AutoComposeAttribute.TargetType.  Please ensure you have provided an attribute like: typeof(ISample)");
            }

            try
            {
                var composingMemberLiteral = GetAttributeArgumentOfType<LiteralExpressionSyntax>(attributes);
                var composingMemberNameOf = GetAttributeArgumentOfType<InvocationExpressionSyntax>(attributes);
                if (composingMemberLiteral is not null)
                {
                    memberName = composingMemberLiteral.Expression.ToString().Replace("\"", string.Empty);
                }
                else if (composingMemberNameOf is not null)
                {
                    var invocationExpression = composingMemberNameOf.Expression.GuardType<InvocationExpressionSyntax>();
                    if (invocationExpression.Expression.ToString() == Constants.NameOf)
                    {
                        memberName = invocationExpression.ArgumentList.Arguments.ToString();
                    }
                }
                if (string.IsNullOrEmpty(memberName))
                {
                    throw new ArgumentException("Error finding string literal or nameof expression for AutoComposeAttribute.MemberName.  Please ensure you have provided an attribute as either \"_sample\" or nameof(_sample)");
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Error finding string literal or nameof expression for AutoComposeAttribute.MemberName.  Please ensure you have provided an attribute as either \"_sample\" or nameof(_sample)");
            }

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

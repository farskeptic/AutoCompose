using AutoCompose.Generator.Common;
using AutoCompose.Generator.Common.Extensions;
using Microsoft.CodeAnalysis;

namespace AutoCompose.Generator.AutoCompose.Extensions
{
    public static class MethodSymbolExtensions
    {
        /// <summary>
        /// Returns either empty string or "return " if the method has a return value.
        /// </summary>
        public static string GetReturnStatement(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.ReturnsVoid ? string.Empty : $"{Constants.Return} ";
        }

        /// <summary>
        /// Returns either "override" or "virtual" as required.
        /// </summary>
        public static string GetOverrideType(this IMethodSymbol methodSymbol, ITypeSymbol composingTypeSymbol)
        {
            return composingTypeSymbol.TypeKind == TypeKind.Class && methodSymbol.IsAbstract ? Constants.Override : Constants.Virtual;
        }

        /// <summary>
        /// Returns the actual return type (e.g. "int", "string", "ISample"), etc.
        /// </summary>
        public static string GetReturnType(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.ReturnType.ToRenderString();
        }

        /// <summary>
        /// Returns the full definition if the method
        /// e.g. int Method1(int nVal, bool bVal, string sVal, ISample sample)
        /// </summary>
        public static string GetMemberInfo(this IMethodSymbol methodSymbol, string composingTypeName)
        {
            var memberString = methodSymbol.ToRenderString();
            var memberInfo = memberString.Replace($"{composingTypeName}.", string.Empty);
            return memberInfo;
        }

        /// <summary>
        /// Returns the full definition of the method without the return value.
        /// e.g. Method1(int nVal, bool bVal, string sVal, ISample sample)
        /// This result goes on to have the variable types (e.g. "bool") removed and
        /// be used to render a call (e.g. m_sample.Method1(nVal, bVal, sVal, sample);
        /// </summary>
        public static string GetCallInfo(this IMethodSymbol methodSymbol, string composingTypeName)
        {
            var memberInfo = GetMemberInfo(methodSymbol, composingTypeName);
            var callInfo = memberInfo.Replace($"{methodSymbol.GetReturnType()} {methodSymbol.Name}", methodSymbol.Name);
            return callInfo;
        }

        /// <summary>
        /// IsPublicGetter
        /// </summary>
        public static bool IsPublicGetter(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.MethodKind == MethodKind.PropertyGet && methodSymbol.DeclaredAccessibility == Accessibility.Public;
        }

        /// <summary>
        /// IsPublicSetter
        /// </summary>
        public static bool IsPublicSetter(this IMethodSymbol methodSymbol)
        {
            return methodSymbol.MethodKind == MethodKind.PropertySet && methodSymbol.DeclaredAccessibility == Accessibility.Public;
        }

    }
}

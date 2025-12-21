using Microsoft.CodeAnalysis;

namespace AutoCompose.Generator.Common.Extensions
{
    public static class SymbolExtensions
    {
        /// <summary>
        /// Returns fully qualified type names like "System.String", "System.Collections.Generic.List<T>", etc.
        /// </summary>
        public static string ToRenderString(this ISymbol symbol)
        {
            // Create a custom format that uses fully qualified names without the global:: prefix
            var format = new SymbolDisplayFormat(
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                memberOptions: SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeType,
                parameterOptions: SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeParamsRefOut,
                miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
            
            return symbol.ToDisplayString(format);
        }

        //public static bool IsInterface(this ITypeSymbol symbol)
        //{
        //    return symbol != null && symbol.TypeKind == TypeKind.Interface;
        //}
    }
}

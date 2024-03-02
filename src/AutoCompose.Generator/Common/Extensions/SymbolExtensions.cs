using Microsoft.CodeAnalysis;

namespace AutoCompose.Generator.Common.Extensions
{
    public static class SymbolExtensions
    {
        /// <summary>
        /// Returns "int", "void", "string", "List<>", etc.
        /// </summary>
        public static string ToRenderString(this ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }

        //public static bool IsInterface(this ITypeSymbol symbol)
        //{
        //    return symbol != null && symbol.TypeKind == TypeKind.Interface;
        //}
    }
}

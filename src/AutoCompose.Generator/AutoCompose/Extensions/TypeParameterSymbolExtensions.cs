using Microsoft.CodeAnalysis;
using System.Linq;

namespace AutoCompose.Generator.AutoCompose.Extensions
{
    public static class TypeParameterSymbolExtensions
    {
        /// <summary>
        /// Determines whether the nullable constraint is on a given type parameter
        /// e.g. where T : ISample? - true
        /// e.g. where T : ISample - false
        /// </summary>
        public static bool HasNullableNotation(this ITypeParameterSymbol parameterSymbol)
        {
            // each constraint type comes in separately, so there's never a list to see if some are annotated
            return parameterSymbol.ConstraintTypes.Any(x => x.NullableAnnotation == NullableAnnotation.Annotated);
        }

        /// <summary>
        /// Determines whether the nullable constraint is on a given reference constraint (e.g. class?)
        /// e.g. where TOut : class? - true
        /// e.g. where TOut : class - false
        /// </summary>
        public static bool HasReferenceTypeConstraintNullableAnnotation(this ITypeParameterSymbol typeParameterSymbol)
        {
            return typeParameterSymbol.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated;
        }
    }
}

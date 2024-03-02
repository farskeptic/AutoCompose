using System;

namespace AutoCompose.Generator.Common.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws an ArgumentException if it's not of the correct type.
        /// </summary>
        public static T GuardType<T>(this object obj)
        {
            if (obj is not T)
            {
                throw new ArgumentException($"Mismatch: {obj?.GetType()} was not a valid {typeof(T).Name}");
            }

            return (T)obj;
        }

        /// <summary>
        /// Throws an ArgumentNullException if it's null
        /// </summary>
        public static T GuardNull<T>(this T obj, string name = null)
        {
            if (obj == null) throw new ArgumentNullException(name);
            return obj;
        }
    }
}

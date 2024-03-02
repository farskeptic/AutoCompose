namespace AutoCompose.Generator.Common.Extensions
{
    // As per: https://medium.com/c-sharp-progarmming/mastering-at-source-generators-18125a5f3fca
    public static class StringExtensions
    {
        /// <summary>
        /// Ensures a string ends with the correct suffix, adding it if required.
        /// </summary>
        public static string EnsureEndsWith(
            this string source,
            string suffix)
        {
            if (source.EndsWith(suffix))
            {
                return source;
            }
            return source + suffix;
        }
    }
}

using Microsoft.CodeAnalysis;

namespace AutoCompose.Generator.Common.Wrappers
{
    /// <summary>
    /// Dependency Injection is difficult in source generation, so we use factory methods
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// GeneratorExecutionContext cannot be easily mocked, so we make a wrapper around it so that
        /// we can test as required.
        /// </summary>
        public static IGeneratorExecutionContext CreateWrapper(GeneratorExecutionContext context)
        {
            return new GeneratorExecutionContextWrapper(context);
        }

    }
}

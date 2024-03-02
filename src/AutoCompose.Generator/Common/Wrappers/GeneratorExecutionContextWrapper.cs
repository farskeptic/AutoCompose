using Microsoft.CodeAnalysis;

namespace AutoCompose.Generator.Common.Wrappers
{
    /// <summary>
    /// GeneratorExecutionContext cannot be easily mocked, so we make a wrapper around it so that
    /// we can test as required.
    /// </summary>
    public class GeneratorExecutionContextWrapper : IGeneratorExecutionContext
    {
        protected readonly GeneratorExecutionContext _generatorExecutionContext;

        public Compilation Compilation => _generatorExecutionContext.Compilation;

        public GeneratorExecutionContextWrapper(GeneratorExecutionContext generatorExecutionContext)
        {
            _generatorExecutionContext = generatorExecutionContext;
        }
    }
}

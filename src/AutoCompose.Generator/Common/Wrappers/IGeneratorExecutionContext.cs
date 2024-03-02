using Microsoft.CodeAnalysis;

namespace AutoCompose.Generator.Common.Wrappers
{
    /// <summary>
    /// GeneratorExecutionContext cannot be easily mocked, so we make a wrapper around it so that
    /// we can test as required.
    /// </summary>
    public interface IGeneratorExecutionContext
    {
        Compilation Compilation { get; }
    }
}

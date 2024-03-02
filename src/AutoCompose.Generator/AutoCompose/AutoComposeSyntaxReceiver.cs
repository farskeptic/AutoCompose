using AutoCompose.Generator.Common;

namespace AutoCompose.Generator.AutoCompose
{
    /// <summary>
    /// AutoComposeAttributeSyntaxReceiver
    /// Registered with the SourceGeneration engine so that all instances of the AutoComposeAttribute
    /// will be available on the context.SyntaxReceiver.
    /// </summary>
    public class AutoComposeAttributeSyntaxReceiver : AttributeSyntaxReceiver
    {
        public AutoComposeAttributeSyntaxReceiver() : base(Constants.AutoComposeAttribute)
        {
        }
    }
}

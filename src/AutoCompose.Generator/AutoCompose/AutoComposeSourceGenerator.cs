using AutoCompose.Generator.Common.Extensions;
using AutoCompose.Generator.Common.Wrappers;
using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Text;

namespace AutoCompose.Generator.AutoCompose
{
    /// <summary>
    /// This generator looks for all instances of the AutoCompose attribute used in the project, and
    /// generates the relevant code to auto-compose it.  
    /// 
    /// e.g. For all classes decorated with the AutCompose attribute...
    ///
    /// [AutoCompose(typeof(ISample), "_sample")]
    /// public partial class MySample : ISample
    /// {
    ///     protected readonly ISample _sample;
    ///
    ///     public MySample(ISample sample)
    ///     {
    ///         _sample = sample;
    ///     }
    ///
    /// ...the relevant code will be generated in a partial class to implement all of the unimplemented ISample methods and properties.
    /// </summary>
    [Generator]
    public class AutoComposeSourceGenerator : ISourceGenerator
    {
        /// <summary>
        /// Actually performs the source code generation.  The process is comprised of:
        /// - for each AutoComposed class
        ///     - parse the symbol into a model
        ///     - render the content into a string
        ///     - save the string to a file
        /// </summary>
        public void Execute(GeneratorExecutionContext context)
        {
            var contextWrapper = Factory.CreateWrapper(context);
            context.SyntaxReceiver.GuardType<AutoComposeAttributeSyntaxReceiver>();
            var syntaxReceiver = context.SyntaxReceiver as AutoComposeAttributeSyntaxReceiver;

            foreach (var autoComposedClass in syntaxReceiver.Classes)
            {
                // parse the symbol
                var model = AutoComposeSourceParser.Parse(contextWrapper, autoComposedClass);

                // render the content
                var renderer = AutoComposeSourceRenderer.Create();
                var content = renderer.Render(model);

                // write the file
                var rawPath = autoComposedClass.SyntaxTree.FilePath;
                var fileName = Path.GetFileNameWithoutExtension(rawPath);
                var genPath = model.ContainingNamespace.Replace(".", "\\");

                context.AddSource($"{genPath}.{fileName}.g.cs", content);
            }
        }

        /// <summary>
        /// Registers the AutoComposeAttribute to be found everywhere, and sent in the
        /// context.SyntaxReceiver passed into the Execute method.
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(GeneratorInitializationContext context)
        {
            // do nothing
            context.RegisterForSyntaxNotifications(() =>
                    new AutoComposeAttributeSyntaxReceiver());
        }
    }
}

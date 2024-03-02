using AutoCompose.Generator.Common;
using AutoCompose.Generator.Common.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoCompose.Generator.AutoCompose
{
    /// <summary>
    /// Renders the source code based on the supplied RenderModel.
    /// </summary>
    public class AutoComposeSourceRenderer
    {
        /// <summary>
        /// Templates are in AutoCompose.Generator/Templates directory
        /// </summary>
        public static Dictionary<string, string> Templates = new();

        /// <summary>
        /// Load the templates on startup
        /// </summary>
        static AutoComposeSourceRenderer()
        {
            LoadTemplates();
        }

        /// <summary>
        /// Templates are in AutoCompose.Generator/Templates directory
        /// </summary>
        private static void LoadTemplates()
        {
            var assembly = typeof(AutoComposeSourceRenderer).Assembly;
            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                using var stream = assembly.GetManifestResourceStream(name);
                using var reader = new StreamReader(stream);
                var content = reader.ReadToEnd();
                Templates.Add(name, content);
            }
        }

        /// <summary>
        /// Dependency Injection is not easy in SourceGeneration, so we use a factory.
        /// </summary>
        public static AutoComposeSourceRenderer Create()
        {
            return new AutoComposeSourceRenderer();
        }

        /// <summary>
        /// Renders the supplied model into a string, using the relevant template.
        /// </summary>
        public string Render(ClassRenderModel model)
        {
            var namespaceUsingStatements = string.Join("\r\n", model.NamespaceUsings.Where(x => x != model.ContainingNamespace).Distinct().OrderBy(x => x).Select(x => $"using {x};"));
            var template = Templates["AutoCompose.Generator.Templates.ClassTemplate.txt"];
            var propertyContent = RenderProperties(model);
            var methodContent = RenderMethods(model);
            var content = template
                .Replace(Constants.Tags.NamespaceUsings, namespaceUsingStatements)
                .Replace(Constants.Tags.ContainingNamespace, model.ContainingNamespace)
                .Replace(Constants.Tags.ContainingClassName, model.ContainingClassName)
                .Replace(Constants.Tags.Properties, propertyContent)
                .Replace(Constants.Tags.Methods, methodContent)
                ;

            return content;
        }

        /// <summary>
        /// Renders the methods of the supplied model into a string, using the relevant template.
        /// </summary>
        public string RenderMethods(ClassRenderModel model)
        {
            var sb = new StringBuilder();

            foreach (var method in model.Methods)
            {
                var template = Templates["AutoCompose.Generator.Templates.MethodTemplate.txt"];
                var content = template
                    .Replace(Constants.Tags.OverrideType, method.OverrideType)
                    .Replace(Constants.Tags.MemberInfo, method.MemberInfo)
                    .Replace(Constants.Tags.ConstraintClauses, method.ConstraintClauses)
                    .Replace(Constants.Tags.ReturnStatement, method.ReturnStatement)
                    .Replace(Constants.Tags.ComposingMemberName, method.ComposingMemberName)
                    .Replace(Constants.Tags.CallInfo, method.CallInfo)
                    ;

                sb.AppendLine(content);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Renders the properties of the supplied model into a string, using the relevant template.
        /// </summary>
        public string RenderProperties(ClassRenderModel model)
        {
            var sb = new StringBuilder();

            foreach (var property in model.Properties)
            {
                var template = Templates["AutoCompose.Generator.Templates.PropertyTemplate.txt"];

                var getStatement = property.HasGetter ? $" get {{ return {property.ComposingMemberName}.{property.Name}; }} " : string.Empty;
                var setStatement = property.HasSetter ? $" set {{ {property.ComposingMemberName}.{property.Name} = value; }}" : string.Empty;

                var content = template
                    .Replace(Constants.Tags.OverrideType, property.OverrideType)
                    .Replace(Constants.Tags.ReturnType, property.ReturnType)
                    .Replace(Constants.Tags.PropertyName, property.Name)
                    .Replace(Constants.Tags.GetStatement, getStatement)
                    .Replace(Constants.Tags.SetStatement, setStatement)
                    ;

                sb.AppendLine(content);
            }

            return sb.ToString();
        }
    }
}

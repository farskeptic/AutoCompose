using System.Collections.Generic;

namespace AutoCompose.Generator.Common.Models
{
    /// <summary>
    /// Created by the Parser, and supplied to the Renderer
    /// </summary>
    public class ClassRenderModel
    {
        public List<string> NamespaceUsings { get; } = new();
        public List<MethodModel> Methods { get; } = new();
        public List<PropertyModel> Properties { get; } = new();
        public string ContainingNamespace { get; set; }
        public string ContainingClassName { get; set; }
        public string AccessModifier { get; set; }
    }
}

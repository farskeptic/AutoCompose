namespace AutoCompose.Generator.Common.Models
{
    /// <summary>
    /// Created by the Parser, and supplied to the Renderer
    /// </summary>
    public class PropertyModel
    {
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public string OverrideType { get; set; }
        public bool HasGetter { get; set; }
        public bool HasSetter { get; set; }
        public string ComposingMemberName { get; set; }
    }
}


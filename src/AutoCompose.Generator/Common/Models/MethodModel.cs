namespace AutoCompose.Generator.Common.Models
{
    /// <summary>
    /// Created by the Parser, and supplied to the Renderer
    /// </summary>
    public class MethodModel
    {
        public string OverrideType { get; set; }
        public string MemberInfo { get; set; }
        public string ConstraintClauses { get; set; }
        public string ReturnStatement { get; set; }
        public string ComposingMemberName { get; set; }
        public string CallInfo { get; set; }
    }
}

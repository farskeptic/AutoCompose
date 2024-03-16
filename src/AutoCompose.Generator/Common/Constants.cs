
namespace AutoCompose.Generator.Common
{
    /// <summary>
    /// Constants used during Parsing and Rendering.
    /// </summary>
    public class Constants
    {
        public const string Attribute = "Attribute";
        public const string AutoCompose = "AutoCompose";
        public const string AutoComposeAttribute = "AutoComposeAttribute";
        public const string Class = "class";
        public const string NewConstraint = "new()";
        public const string NotNull = "notnull";
        public const string NullableClass = "class?";
        public const string Override = "override";
        public const string Return = "return";
        public const string Struct = "struct";
        public const string Unmanaged = "unmanaged";
        public const string Virtual = "virtual";
        public const string NameOf = "nameof";

        public class Tags
        {
            public const string CallInfo = "<<CallInfo>>";
            public const string ComposingMemberName = "<<ComposingMemberName>>";
            public const string ContainingClassName = "<<ContainingClassName>>";
            public const string ContainingNamespace = "<<ContainingNamespace>>";
            public const string ConstraintClauses = "<<ConstraintClauses>>";
            public const string GetStatement = "<<GetStatement>>";
            public const string MemberInfo = "<<MemberInfo>>";
            public const string Members = "<<Members>>";
            public const string Methods = "<<Methods>>";
            public const string NamespaceUsings = "<<NamespaceUsings>>";
            public const string OverrideType = "<<OverrideType>>";
            public const string Properties = "<<Properties>>";
            public const string PropertyName = "<<PropertyName>>";
            public const string ReturnStatement = "<<ReturnStatement>>";
            public const string ReturnType = "<<ReturnType>>";
            public const string SetStatement = "<<SetStatement>>";
            public const string AccessModifier = "<<AccessModifier>>";
        }
    }
}

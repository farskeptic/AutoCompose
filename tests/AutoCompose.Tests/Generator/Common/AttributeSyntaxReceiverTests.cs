using AutoCompose.Generator.Common;
using AutoCompose.Generator.Common.Fluent;

namespace AutoCompose.Tests.Generator.Common
{
    public class AttributeSyntaxReceiverTests : TestBase
    {
        [Theory]
        [InlineData("AutoCompose", 1)]
        [InlineData("Invalid", 0)]
        public void OnVisitSyntaxNode_Succeeds(string attributeName, int classCount)
        {
            // arrange..
            var className = "MyClassName";

            var code = $@"
                public class {attributeName}Attribute : System.Attribute
                {{
                    public int Val {{get; set; }}
                }}

                [{attributeName}Attribute(Val = 55)]
                public class {className} {{ }}
            ";

            var api = SyntaxApi.Create(code);
            var syntaxNode = api.GetSyntaxNode(className);
            var target = new AttributeSyntaxReceiver($"AutoComposeAttribute");
            // act..
            target.OnVisitSyntaxNode(syntaxNode);
            // assert..
            Assert.Equal(classCount, target.Classes.Count);
        }
    }
}

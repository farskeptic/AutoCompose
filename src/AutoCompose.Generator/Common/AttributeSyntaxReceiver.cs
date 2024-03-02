using AutoCompose.Generator.Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace AutoCompose.Generator.Common
{
    /// <summary>
    /// Used to add all instances of classes that are decorated with a given Attribute
    /// e.g.
    ///  [AutoCompose(typeof(ISample), "_sample")]
    ///  public partial class FullGenericSample : ISample
    ///
    /// As per: https://medium.com/c-sharp-progarmming/mastering-at-source-generators-18125a5f3fca
    /// </summary>
    public class AttributeSyntaxReceiver : ISyntaxReceiver
    {
        protected readonly string _attributeName;

        /// <summary>
        /// Constructor
        /// </summary>
        public AttributeSyntaxReceiver(string attributeName)
        {
            _attributeName = attributeName;
        }
        public IList<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

        /// <summary>
        /// Adds to the list each time one is found at a given SyntaxNode
        /// </summary>
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                classDeclarationSyntax.AttributeLists.Any(al => al.Attributes
                        .Any(a => a.Name.ToString().EnsureEndsWith(Constants.Attribute).Equals(_attributeName))))
            {
                Classes.Add(classDeclarationSyntax);
            }
        }
    }
}

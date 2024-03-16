using AutoCompose.Generator.Common.Models;
using AutoCompose.Generator.Common.Wrappers;
using AutoCompose.Generator.AutoCompose.Extensions;
using AutoCompose.Generator.AutoCompose.Models;
using AutoCompose.Generator.Common;
using AutoCompose.Generator.Common.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace AutoCompose.Generator.AutoCompose
{
    /// <summary>
    /// Parses the incoming AutoComposed class (e.g. MySample)
    ///  [AutoCompose(typeof(ISample), "_sample")]
    ///  public partial class MySample : ISample
    ///  {
    ///      protected readonly ISample _sample;
    ///
    ///      public MySample(ISample sample)
    ///      {
    ///          _sample = sample;
    ///      }
    ///  }    
    /// </summary>
    public class AutoComposeSourceParser
    {
        protected readonly AutoComposeInfo _autoComposeInfo;
        protected readonly ClassDeclarationSyntax _autoComposedClass;
        protected ITypeSymbol _autoComposedSymbol;
        protected readonly INamedTypeSymbol _targetSymbol;
        protected readonly ClassRenderModel _renderModel;

        protected AutoComposeSourceParser(AutoComposeInfo autoComposeInfo,
            ClassDeclarationSyntax autoComposedClass,
            INamedTypeSymbol targetSymbol)
        {
            _autoComposeInfo = autoComposeInfo;
            _autoComposedClass = autoComposedClass;
            _targetSymbol = targetSymbol;
            _renderModel = new ClassRenderModel();
        }

        /// <summary>
        /// We can't easily to dependency injection inside a SourceGenerator, so we use a factory.
        /// </summary>
        public static AutoComposeSourceParser Create(AutoComposeInfo autoComposeInfo,
            ClassDeclarationSyntax autoComposedClass,
            INamedTypeSymbol targetSymbol)
        {
            return new AutoComposeSourceParser(autoComposeInfo, autoComposedClass, targetSymbol);
        }

        ///// <summary>
        ///// Note: All interface members are composable
        ///// </summary>
        //public bool CanAutoComposeMember(IMethodSymbol _)
        //{
        //    return _targetSymbol.IsInterface();
        //}

        /// <summary>
        /// Attempts to find if the symbol has already been implemented on the class being autoComposed (e.g. MySample)
        /// </summary>
        public bool IsNotAlreadyImplemented(IMethodSymbol iMember)
        {
            var implementedSymbol = _autoComposedSymbol.FindImplementationForInterfaceMember(iMember);
            return (implementedSymbol is null);
        }

        /// <summary>
        /// This static method takes the context and the autoComposed class, and:
        /// - determines the target symbol (e.g. ISample) to use
        /// - created a parser using the target symbol
        /// - runs the parser to create the model
        /// </summary>
        public static ClassRenderModel Parse(IGeneratorExecutionContext context, ClassDeclarationSyntax autoComposedClass)
        {
            // e.g. "_sample", "ISample"
            var autoComposeInfo = autoComposedClass.GetAutoComposeInfo();

            // e.g. all the definitions of ISample
            var targetNodes = context.GetSyntaxNodes(autoComposeInfo.TargetType).ToList();

            // e.g. the specific definition of ISample
            var targetSymbol = context.GetDependentSymbol(autoComposedClass, targetNodes).GuardNull();

            // create the parser
            var parser = Create(autoComposeInfo.GuardNull(),
                autoComposedClass.GuardNull(),
                targetSymbol.GuardNull());

            // parse to create the model
            return parser.Parse(context);
        }

        /// <summary>
        /// Parses the AutoComposed class (e.g. MySample) and the targetSymbol (e.g. ISample)
        /// and generates a model that contains all of the targetSymbol methods and properties,
        /// minus the ondes that the AutoComposed class has already implemented
        /// </summary>
        protected ClassRenderModel Parse(IGeneratorExecutionContext context)
        {
            // get the model info relevant to the AutoComposed class
            _autoComposedSymbol = context.GetSymbol(_autoComposedClass);
            _renderModel.ContainingClassName = _autoComposedSymbol.Name;
            _renderModel.ContainingNamespace = _autoComposedSymbol.ContainingNamespace.ToDisplayString();
            _renderModel.AccessModifier = _autoComposedSymbol.DeclaredAccessibility.ToString().ToLower();

            // get all of the members (methods and properties) from the targetSymbol (e.g. ISample)
            var targetMembers = _targetSymbol.GetMembers();

            // figure out which ones need to be implemeneted
            var (methods, properties) = GetMembersToImplement(targetMembers);

            // generate the model relevant to properties
            ParseProperties(properties);
            // generate the model relevant to methods
            ParseMethods(context, methods);

            return _renderModel;
        }

        /// <summary>
        /// Parsed the methods, retrieving names, return types, arguments, etc. and adding them to the renderModel.
        /// </summary>
        private void ParseMethods(IGeneratorExecutionContext context, List<IMethodSymbol> membersToImplement)
        {
            var targetSymbolName = _targetSymbol.Name;
            foreach (var member in membersToImplement)
            {
                // all interface members are composable
                //if (!CanAutoComposeMember(member))
                //{
                //    continue;
                //}

                var methodParameters = member.Parameters.ToList();

                // contains variable types in the calling parameter list
                // (e.g. _baseA.Method3(int val1, bool val2, ref string val3, out IBaseA val4);
                var callInfo = member.GetCallInfo(targetSymbolName);

                foreach (var methodParameter in methodParameters)
                {
                    // e.g. for a methodParameter like IBaseA, methodParamTypes would be the definition of that type
                    // since we only support finding a single type (i.e. a single definition of IBaseA in the project
                    // we won't consider multiples
                    var methodParamTypes = context.GetSyntaxNodes(methodParameter.Type.Name).ToList();

                    // e.g. for Method1(IExample example), this would be the INamedTypeSymbol for IExample
                    var methodParamSymbol = context.GetDependentSymbol(_autoComposedClass, methodParamTypes);

                    // track the namespace
                    if (methodParamSymbol is not null)
                    {
                        _renderModel.NamespaceUsings.Add(methodParamSymbol.ContainingNamespace.ToDisplayString());
                    }

                    // generate the call info
                    var type = methodParameter.Type.ToRenderString();

                    // we need to remove the variable types from the call info for each parameter
                    callInfo = callInfo.Replace($"{type} {methodParameter.Name}", methodParameter.Name);
                }

                var methodModel = new MethodModel()
                {
                    OverrideType = member.GetOverrideType(_targetSymbol),
                    MemberInfo = member.GetMemberInfo(targetSymbolName),
                    ConstraintClauses = GenerateGenericConstraintClausesForMember(member),
                    ReturnStatement = member.GetReturnStatement(),
                    ComposingMemberName = _autoComposeInfo.MemberName,
                    CallInfo = callInfo,
                };
                _renderModel.Methods.Add(methodModel);
            }
        }

        /// <summary>
        /// Note: This returns a string as there's no benefit in returning arrays of strings to assemble later
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private string GenerateGenericConstraintClausesForMember(IMethodSymbol member)
        {
            var sb = new StringBuilder();
            var typeArguments = member.TypeArguments.ToList();
            foreach (var typeArgument in typeArguments)
            {
                var symbol = typeArgument as ITypeParameterSymbol;
                var name = symbol?.Name;
                var args = new List<string>();

                // Note: only class and struct can be listed explicitly on abstract
                args.AddRange(symbol.ConstraintTypes.Select(x => symbol.HasNullableNotation() ? $"{x.Name}?" : x.Name).ToList());
                if (symbol.HasReferenceTypeConstraintNullableAnnotation()) args.Add(Constants.NullableClass);
                else if (symbol.HasReferenceTypeConstraint) args.Add(Constants.Class);

                if (symbol.HasUnmanagedTypeConstraint) AddGenericConstraintArg(args, Constants.Unmanaged); // Note: Will automatically apply struct constraint 
                // Note: In order to satisfy the unmanaged constraint, a type must be a struct and all the fields of the type must fall into one of the following categorie
                if (symbol.HasValueTypeConstraint) AddGenericConstraintArg(args, Constants.Struct);
                if (symbol.HasNotNullConstraint) AddGenericConstraintArg(args, Constants.NotNull);
                if (symbol.HasConstructorConstraint) args.Add(Constants.NewConstraint);

                sb.Append($" where {name} : {string.Join(", ", args)}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds the constraint to the list of existing constraints, protecting against duplicates, 
        /// and those things that can't travel together (e.g. unmanaged, struct)
        /// </summary>
        private void AddGenericConstraintArg(List<string> existingConstraints, string constraint)
        {
            if (!existingConstraints.Contains(constraint))
            {
                // unmanaged must be added first, and can have struct combined with it
                if (constraint == Constants.Struct)
                {
                    if (!existingConstraints.Contains(Constants.Unmanaged))
                    {
                        existingConstraints.Add(Constants.Struct);
                    }
                }
                else
                {
                    existingConstraints.Add(constraint);
                }
            }
        }

        /// <summary>
        /// For each property, generates a PropertyModel with the relevant into 
        /// for the property, and adds it to the renderModel
        /// </summary>
        private void ParseProperties(List<IMethodSymbol> membersToImplement)
        {
            var dictProperties = new Dictionary<string, PropertyModel>();
            foreach (var member in membersToImplement)
            {
                var methodReturnType = member.ReturnType.ToRenderString();
                var overrideType = _targetSymbol.TypeKind == TypeKind.Class && member.IsAbstract ? Constants.Override : Constants.Virtual;

                var autoPropertyName = member.AssociatedSymbol.Name;
                var info = dictProperties.ContainsKey(autoPropertyName) ? dictProperties[autoPropertyName] : new PropertyModel()
                {
                    ComposingMemberName = _autoComposeInfo.MemberName,
                    Name = member.AssociatedSymbol.Name,
                    ReturnType = methodReturnType,
                    OverrideType = overrideType
                };

                if (member.IsPublicGetter())
                {
                    info.HasGetter = true;
                }
                if (member.IsPublicSetter())
                {
                    info.HasSetter = true;
                }

                if (!dictProperties.ContainsKey(autoPropertyName))
                {
                    dictProperties.Add(autoPropertyName, info);
                }
            }

            foreach (var property in dictProperties.Values)
            {
                _renderModel.Properties.Add(property);
            }
        }

        /// <summary>
        /// Returns the list of methods and properties that are relevant to implemented, based on a raw list of interface members.
        /// </summary>
        private (List<IMethodSymbol> methods, List<IMethodSymbol> properties) GetMembersToImplement(ImmutableArray<ISymbol> interfaceMembers)
        {
            var members = _autoComposedSymbol.GetMembers();
            var symbolsToImplement = new List<ISymbol>();
            var implementedMembers = new List<string>();
            foreach (var iMember in interfaceMembers)
            {
                if (IsNotAlreadyImplemented(iMember as IMethodSymbol))
                {
                    symbolsToImplement.Add(iMember);
                }
            }

            var missingProperties = symbolsToImplement
                .Where(x => x.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>()
                .ToList();

            var getterSetters = symbolsToImplement
                .Where(x => x.Kind == SymbolKind.Method)
                .Cast<IMethodSymbol>()
                .Where(x => x.MethodKind == MethodKind.PropertyGet ||
                    x.MethodKind == MethodKind.PropertySet)
                .Cast<IMethodSymbol>()
                .ToList();

            var propertiesToImplement = (from gs in getterSetters
                                         join mp in missingProperties
                                             on gs.AssociatedSymbol equals mp
                                         select gs
                               )
                               .ToList();

            var methodsToImplement = symbolsToImplement
                .Where(x => x.Kind == SymbolKind.Method)
                .Cast<IMethodSymbol>()
                .Where(x => x.MethodKind != MethodKind.PropertyGet &&
                    x.MethodKind != MethodKind.PropertySet)
                .ToList();

            _renderModel.NamespaceUsings.AddRange(methodsToImplement.Select(x => x.ContainingNamespace.ToString()).ToList());
            _renderModel.NamespaceUsings.AddRange(propertiesToImplement.Select(x => x.ContainingNamespace.ToString()).ToList());

            return (methodsToImplement, propertiesToImplement);
        }
    }
}

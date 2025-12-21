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
using System;

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
            var autoComposeInfo = autoComposedClass.GetAutoComposeInfo(context);
            if (autoComposeInfo is null)
            {
                throw new InvalidOperationException($"Failed to extract AutoCompose information from class '{autoComposedClass.Identifier.ValueText}'. Ensure the class is decorated with [AutoCompose(typeof(TargetType), \"memberName\")].");
            }

            // Use the symbol if we already have it from GetAutoComposeInfo (avoids redundant lookup)
            INamedTypeSymbol targetSymbol = autoComposeInfo.TargetSymbol;

            // If targetSymbol is an error type, treat it as null to trigger fallbacks
            if (targetSymbol != null && targetSymbol.TypeKind == TypeKind.Error)
            {
                targetSymbol = null;
            }

            // If we don't have the symbol yet, try to get it using the various lookup methods
            if (targetSymbol is null)
            {
                // Try to get the type symbol directly from the typeof expression using semantic model
                // This is the most accurate method and works for both project types and external types
                targetSymbol = context.GetTypeSymbolFromTypeOfExpression(autoComposedClass, autoComposeInfo.TargetType);

                // If not found via semantic model, try syntax tree search (for types defined in the project)
                if (targetSymbol is null || targetSymbol.TypeKind == TypeKind.Error)
                {
                    var targetNodes = context.GetSyntaxNodes(autoComposeInfo.TargetType).ToList();
                    if (targetNodes.Any())
                    {
                        var candidateSymbol = context.GetDependentSymbol(autoComposedClass, targetNodes);
                        // Only use the candidate if it's not an error type
                        if (candidateSymbol != null && candidateSymbol.TypeKind != TypeKind.Error)
                        {
                            targetSymbol = candidateSymbol;
                        }
                    }
                }

                // If still not found, try to get from compilation (for types from NuGet packages/external assemblies)
                if (targetSymbol is null || targetSymbol.TypeKind == TypeKind.Error)
                {
                    // Try to infer namespace from the autoComposed class's namespace or using directives
                    var autoComposedSymbol = context.GetSymbol(autoComposedClass);
                    var namespaceHint = autoComposedSymbol?.ContainingNamespace?.ToDisplayString();
                    
                    var candidateSymbol = context.GetTypeSymbolFromCompilation(autoComposeInfo.TargetType, namespaceHint);
                    // Only use the candidate if it's not an error type
                    if (candidateSymbol != null && candidateSymbol.TypeKind != TypeKind.Error)
                    {
                        targetSymbol = candidateSymbol;
                    }
                }
            }

            // Provide a meaningful error if we still can't find the type
            if (targetSymbol is null)
            {
                var className = autoComposedClass.Identifier.ValueText;
                var targetTypeName = autoComposeInfo.TargetType;
                var location = autoComposedClass.GetLocation();
                var errorMessage = $"AutoCompose: Could not find type '{targetTypeName}' for class '{className}'. " +
                    $"The type may be from a NuGet package or external assembly. " +
                    $"Ensure the type '{targetTypeName}' is accessible and the necessary using directives are present. " +
                    $"If '{targetTypeName}' is from a NuGet package, make sure the package is referenced in your project.";

                throw new InvalidOperationException(errorMessage);
            }

            // Verify the target symbol is an interface or abstract class
            if (targetSymbol.TypeKind != TypeKind.Interface && !(targetSymbol.TypeKind == TypeKind.Class && targetSymbol.IsAbstract))
            {
                var className = autoComposedClass.Identifier.ValueText;
                var targetTypeName = autoComposeInfo.TargetType;
                throw new InvalidOperationException(
                    $"AutoCompose: Type '{targetTypeName}' used in class '{className}' must be an interface or abstract class. " +
                    $"Current type kind: {targetSymbol.TypeKind}.");
            }

            // create the parser
            var parser = Create(autoComposeInfo,
                autoComposedClass,
                targetSymbol);

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
        /// Extracts and adds the namespace from a type symbol to the render model's namespace usings.
        /// Handles nullable types, arrays, and nested generic types.
        /// </summary>
        private void ExtractNamespaceFromParameterType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                return;

            ITypeSymbol typeToCheck = typeSymbol;
            
            // Unwrap nullable types (e.g., int?)
            if (typeToCheck is INamedTypeSymbol namedType && namedType.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            {
                typeToCheck = namedType.TypeArguments.FirstOrDefault();
                if (typeToCheck == null)
                    return;
            }
            
            // Get the underlying type for arrays (e.g., int[])
            if (typeToCheck is IArrayTypeSymbol arrayType)
            {
                typeToCheck = arrayType.ElementType;
            }
            
            // Handle named types (interfaces, classes, structs, etc.)
            if (typeToCheck is INamedTypeSymbol namedTypeSymbol)
            {
                // Use OriginalDefinition to get the actual type definition, not a reference
                // This ensures we get the correct namespace even if the type was referenced via using
                var originalType = namedTypeSymbol.OriginalDefinition ?? namedTypeSymbol;
                
                // Add namespace for the main type
                AddNamespaceFromSymbol(originalType);
                
                // Recursively handle type arguments (e.g., List<MyClass> should add namespaces for both List and MyClass)
                foreach (var typeArg in namedTypeSymbol.TypeArguments)
                {
                    ExtractNamespaceFromParameterType(typeArg);
                }
            }
            else
            {
                // For non-named types, try to extract namespace directly
                AddNamespaceFromSymbol(typeToCheck);
            }
        }

        /// <summary>
        /// Adds the namespace of a type symbol to the render model's namespace usings.
        /// </summary>
        private void AddNamespaceFromSymbol(ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                return;

            var containingNamespace = typeSymbol.ContainingNamespace;
            if (containingNamespace == null)
                return;
            
            // Build the namespace by traversing up the namespace chain
            // This works even when IsGlobalNamespace is incorrectly true
            var namespaceParts = new List<string>();
            var ns = containingNamespace;
            
            while (ns != null)
            {
                // Skip if it's actually the global namespace
                if (ns.IsGlobalNamespace && string.IsNullOrEmpty(ns.Name))
                    break;
                    
                // Get the namespace name - prefer Name property, fall back to ToString()
                var nsName = !string.IsNullOrEmpty(ns.Name) ? ns.Name : null;
                if (string.IsNullOrEmpty(nsName))
                {
                    var nsString = ns.ToString();
                    if (!string.IsNullOrWhiteSpace(nsString) && nsString != "<global namespace>")
                    {
                        // If ToString() gives us the full namespace, use it
                        if (nsString.Contains("."))
                        {
                            // It's already the full namespace, use it
                            var namespaceStringJMagel = nsString;
                            if (namespaceStringJMagel != _renderModel.ContainingNamespace)
                            {
                                _renderModel.NamespaceUsings.Add(namespaceStringJMagel);
                            }
                            return;
                        }
                        nsName = nsString;
                    }
                }
                
                if (!string.IsNullOrEmpty(nsName) && nsName != "<global namespace>")
                {
                    namespaceParts.Insert(0, nsName);
                }
                
                ns = ns.ContainingNamespace;
            }
            
            if (namespaceParts.Count == 0)
                return;
            
            var namespaceString = string.Join(".", namespaceParts);
            
            // Validate and add the namespace (skip if it's the same as the containing namespace)
            if (namespaceString != _renderModel.ContainingNamespace)
            {
                _renderModel.NamespaceUsings.Add(namespaceString);
            }
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
                    // Extract namespace from parameter type for using statements
                    // Get the type from the interface's declaration syntax to ensure we have correct namespace info
                    ITypeSymbol parameterType = null;
                    
                    // Try to get the type from the interface's declaration syntax (where the method is defined)
                    if (_targetSymbol.DeclaringSyntaxReferences.Length > 0)
                    {
                        var syntaxRef = _targetSymbol.DeclaringSyntaxReferences[0];
                        var syntax = syntaxRef.GetSyntax();
                        
                        if (syntax is InterfaceDeclarationSyntax interfaceSyntax)
                        {
                            // Find the method declaration in the interface
                            var methodDecl = interfaceSyntax.Members
                                .OfType<MethodDeclarationSyntax>()
                                .FirstOrDefault(m => m.Identifier.ValueText == member.Name);
                            
                            if (methodDecl != null)
                            {
                                var paramIndex = methodParameters.IndexOf(methodParameter);
                                if (paramIndex >= 0 && paramIndex < methodDecl.ParameterList.Parameters.Count)
                                {
                                    var paramSyntax = methodDecl.ParameterList.Parameters[paramIndex];
                                    if (paramSyntax.Type != null)
                                    {
                                        var semanticModel = context.Compilation.GetSemanticModel(syntaxRef.SyntaxTree);
                                        var typeInfo = semanticModel.GetTypeInfo(paramSyntax.Type);
                                        if (typeInfo.Type != null)
                                        {
                                            parameterType = typeInfo.Type;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    // Fallback to the parameter's type if we couldn't get it from syntax
                    if (parameterType == null)
                    {
                        parameterType = methodParameter.Type;
                    }
                    
                    // Try to get the original definition to ensure correct namespace
                    if (parameterType is INamedTypeSymbol namedType && namedType.OriginalDefinition != null)
                    {
                        parameterType = namedType.OriginalDefinition;
                    }
                    
                    ExtractNamespaceFromParameterType(parameterType);

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

            foreach (var method in methodsToImplement)
            {
                var containingNamespace = method.ContainingNamespace;
                if (containingNamespace is not null && !containingNamespace.IsGlobalNamespace)
                {
                    var namespaceString = containingNamespace.ToString();
                    if (!string.IsNullOrWhiteSpace(namespaceString) && namespaceString != "<global namespace>")
                    {
                        _renderModel.NamespaceUsings.Add(namespaceString);
                    }
                }
            }
            
            foreach (var property in propertiesToImplement)
            {
                var containingNamespace = property.ContainingNamespace;
                if (containingNamespace is not null && !containingNamespace.IsGlobalNamespace)
                {
                    var namespaceString = containingNamespace.ToString();
                    if (!string.IsNullOrWhiteSpace(namespaceString) && namespaceString != "<global namespace>")
                    {
                        _renderModel.NamespaceUsings.Add(namespaceString);
                    }
                }
            }

            return (methodsToImplement, propertiesToImplement);
        }
    }
}

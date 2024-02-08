using System.Collections.Immutable;
using EntityViews.SourceGenerators.Templates;
using EntityViews.SourceGenerators.Templates.Maui;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EntityViews.SourceGenerators.SyntaxNodeHelper;

namespace EntityViews.SourceGenerators;

[Generator]
public class EntityViewsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        ViewModelAnalysis.BaseClasses.Clear();

        context.RegisterPostInitializationOutput(i =>
        {
            i.AddSource("EntityViews.Attributes.g.cs", AttributesTemplate.Build());
            i.AddSource("EntityViews.Validation.g.cs", ValidationTemplate.Build());
        });

        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: IsSyntaxTargetForGeneration,
                transform: GetSemanticTargetForGeneration)
            .Where(static cds => cds is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken t)
    {
        return node is ClassDeclarationSyntax cds && cds.AttributeLists.Count > 0;
    }

    private static ViewModelAnalysis? GetSemanticTargetForGeneration(
        GeneratorSyntaxContext context, CancellationToken t)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

        if (classSymbol is null) return null;

        var classAttributes = classSymbol.GetAttributes();

        foreach (var attributeData in classAttributes)
        {
            if (attributeData.AttributeClass is null) continue;

            var attributeName = attributeData.AttributeClass.ToDisplayString();

            // it seems that source generated attributes are not fully qualified.
            if (attributeName == "EntityViews.Attributes.EntityViewsControlAttribute")
            {
                var inputType = attributeData.ConstructorArguments.FirstOrDefault().Value?.ToString() ?? string.Empty;
                ViewModelAnalysis.BaseClasses[inputType] = classSymbol.ToDisplayString();
            }

            if (attributeName == "EntityViews.Attributes.ViewModelAttribute")
            {
                var getIgnoreExpression = attributeData.NamedArguments
                    .Where(static na => na.Key == "Ignore")
                    .Select(static na => na.Value)
                    .SelectMany(static v => v.Values)
                    .Select(static v => v.Value?.ToString() ?? string.Empty);

                var baseType = attributeData.NamedArguments.First(x => x.Key == "BaseType");
                var viewModelOf = (INamedTypeSymbol)baseType.Value.Value!;

                var forms = attributeData.NamedArguments.FirstOrDefault(x => x.Key == "Form");
                var form = forms.Value.Value is null ? 0 : (int)forms.Value.Value;

                var inputs = classAttributes
                    .Where(static x => x.AttributeClass?.ToDisplayString() == "EntityViews.Attributes.FormInput")
                    .Select(static x =>
                    {
                        string? propertyName = null;
                        string? inputType = null;

                        foreach (var arg in x.NamedArguments)
                        {
                            if (arg.Key == "PropertyName") propertyName = arg.Value.Value?.ToString();
                            if (arg.Key == "InputType") inputType = arg.Value.Value?.ToString();
                            if (propertyName is not null && inputType is not null) break;
                        }

                        return new Tuple<string, string>(propertyName ?? string.Empty, inputType ?? InputTypes.Default);
                    });

                var inputsDictionary = new Dictionary<string, string>();
                foreach (var item in inputs)
                    inputsDictionary[item.Item1] = item.Item2; // override duplicates.

                return new ViewModelAnalysis(
                    classDeclaration,
                    viewModelOf,
                    new HashSet<string>(getIgnoreExpression),
                    form,
                    inputsDictionary);
            }
        }

        // not interested on this one.
        return null;
    }

    private static void Execute(
        Compilation compilation, ImmutableArray<ViewModelAnalysis?> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty) return;

        var generatedBaseTypes = false;

        foreach (var analysis in classes.Distinct())
        {
            if (analysis is null) continue;

            var viewModelOfSymbol = analysis.FindViewModelOfSymbol(compilation) ??
            throw new Exception($"Unable to find target type {analysis.ViewModelOf}.");

            var ignorableByAttribute = viewModelOfSymbol.GetMembers()
                .Where(m => m.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>()
                .Where(p => p.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() is
                    "EntityViews.Attributes.IgnorePropertyAttribute" or "EntityViews.Attributes.IgnoreProperty"))
                .Select(p => p.Name)
                .ToImmutableHashSet();

            var properties = viewModelOfSymbol.GetMembers()
                .Where(m => m.Kind == SymbolKind.Property)
                .Cast<IPropertySymbol>()
                .Where(p =>
                    !ignorableByAttribute.Contains(p.Name) &&
                    !analysis.Ignore.Contains(p.Name));

            var classDeclaration = analysis.ClassDeclaration;

            context.AddSource(
                $"{classDeclaration.Identifier}.g.cs",
                ViewModelTemplate.Build(compilation, classDeclaration, properties));

            if (analysis.Form == 0) continue;

            var rootNamespace =
                compilation.Assembly.GlobalNamespace.GetNamespaceMembers().FirstOrDefault()?.Name
                ?? string.Empty;

            // add more kind of forms here?
            if (analysis.Form == 1)
            {
                if (!generatedBaseTypes)
                {
                    context.AddSource("EntityViews.Input.g.cs", _MauiDefaultInputs.Build());
                    generatedBaseTypes = true;
                }

                foreach (var property in properties)
                {
                    context.AddSource(
                        $"{classDeclaration.Identifier}.{property.Name}.Input.g.cs",
                        MauiFormPropertyTemplate.Build(
                            compilation,
                            classDeclaration.Identifier.ToString(),
                            classDeclaration.GetNameSpace() ?? string.Empty,
                            $"{rootNamespace}.EntityForms.{analysis.ViewModelOf.Name}",
                            property,
                            analysis));
                }
            }
        }
    }
}

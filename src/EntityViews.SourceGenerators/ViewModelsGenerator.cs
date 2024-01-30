using System.Collections.Immutable;
using EntityViews.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static EntityViews.SourceGenerators.SyntaxNodeHelper;

namespace EntityViews.SourceGenerators;

// based on:
// https://andrewlock.net/exploring-dotnet-6-part-9-source-generator-updates-incremental-generators/

[Generator]
public class ViewModelsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
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

        foreach (var attributeData in classSymbol.GetAttributes())
        {
            if (attributeData.AttributeClass is null) continue;

            var nameParts = attributeData.AttributeClass.ToDisplayParts();
            if (nameParts.Length < 5) continue;
            var attributeName = $"{nameParts[0]}.{nameParts[2]}.{nameParts[4]}";

            if (attributeName == "EntityViews.Attributes.ViewModelAttribute")
            {
                var getIgnoreExpression = attributeData.NamedArguments
                    .Where(static na => na.Key == "Ignore")
                    .Select(static na => na.Value)
                    .SelectMany(static v => v.Values)
                    .Select(static v => v.Value?.ToString() ?? string.Empty);

                var baseType = attributeData.NamedArguments.First(x => x.Key == nameof(ViewModelAttribute.BaseType));
                var viewModelOf = (INamedTypeSymbol)baseType.Value.Value!;

                ViewModelAnalysis.TargetTypes[classDeclaration.Identifier.Text] = viewModelOf;

                return new ViewModelAnalysis(
                    classDeclaration,
                    viewModelOf,
                    new HashSet<string>(getIgnoreExpression));
            }
        }

        // not interested on this one.
        return null;
    }

    private static void Execute(
        Compilation compilation, ImmutableArray<ViewModelAnalysis?> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty) return;

        foreach (var @class in classes.Distinct())
        {
            if (@class is null) continue;

            context.AddSource(
                $"{@class.ClassDeclaration.Identifier}.g.cs",
                ViewModelTemplate.Build(compilation, @class));
        }
    }
}

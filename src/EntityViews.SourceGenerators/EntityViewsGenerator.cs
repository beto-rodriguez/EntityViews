﻿using System.Collections.Immutable;
using EntityViews.Attributes;
using EntityViews.SourceGenerators.Templates;
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

                var baseType = attributeData.NamedArguments.First(x => x.Key == "BaseType");
                var viewModelOf = (INamedTypeSymbol)baseType.Value.Value!;

                var forms = attributeData.NamedArguments.FirstOrDefault(x => x.Key == nameof(ViewModelAttribute.Form));
                var form = forms.Value.Value is null ? FormKind.None : (FormKind)forms.Value.Value;

                return new ViewModelAnalysis(
                    classDeclaration,
                    viewModelOf,
                    new HashSet<string>(getIgnoreExpression),
                    form);
            }
        }

        // not interested on this one.
        return null;
    }

    private static void Execute(
        Compilation compilation, ImmutableArray<ViewModelAnalysis?> classes, SourceProductionContext context)
    {
        if (classes.IsDefaultOrEmpty) return;

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

            if (analysis.Form == FormKind.None) continue;

            var rootNamespace =
                compilation.Assembly.GlobalNamespace.GetNamespaceMembers().FirstOrDefault()?.Name
                ?? string.Empty;

            // add more kind of forms here?
            if (analysis.Form == FormKind.MauiMarkup)
            {
                foreach (var property in properties)
                {
                    context.AddSource(
                        $"{classDeclaration.Identifier}.{property.Name}.Input.g.cs",
                        MauiFormTemplate.Build(
                            compilation,
                            classDeclaration.Identifier.ToString(),
                            classDeclaration.GetNameSpace() ?? string.Empty,
                            $"{rootNamespace}.EntityForms.{analysis.ViewModelOf.Name}",
                            property));
                }
            }
        }
    }
}